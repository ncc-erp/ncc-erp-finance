using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Notifications.Mezon;
using FinanceManagement.Notifications.Mezon.Dto;
using FinanceManagement.Services.Firebase;
using FinanceManagement.Services.Mezon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Host.Startup
{
    public class CrawlBTransactionMezonDongBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<CrawlBTransactionMezonDongBackgroundWorker> _log;
        private FinanceManagementDbContext _context;
        private const int TENANT_NULL_ID = -1;
        private readonly IMezonNotification _mezonNotification;
        private static List<string> _dbMezonTransactionHashes;
        private readonly IOptions<CrawlBTransactionMezonDongConfig> _mezonDongOptions;
        private readonly MezonDongService _mezonDongService;
        public IMySettingManager MySettingManager { get; set; }

        public CrawlBTransactionMezonDongBackgroundWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            ILogger<CrawlBTransactionMezonDongBackgroundWorker> log,
            IMezonNotification mezonNotification,
            IOptions<CrawlBTransactionMezonDongConfig> mezonDongOptions,
            IHttpClientFactory httpClientFactory,
            MezonDongService mezonDongService
        ) : base(timer)
        {
            _context = iocResolver.Resolve<FinanceManagementDbContext>();
            _log = log;
            _mezonDongService = mezonDongService;
            _dbMezonTransactionHashes = new List<string>();
            _mezonDongOptions = mezonDongOptions;
            Timer.Period = _mezonDongOptions.Value.IntervalMilisecond;
            _mezonNotification = mezonNotification;
        }
        protected override void DoWork()
        {
            _log.LogInformation($"CrawlBTransactionMezonDongBackgroundWorker.DoWork() start");

            if (_mezonDongOptions.Value.EnableCrawlBTransactionMezonDong)
            {
                CrawlBTransactionMezonD().Wait();
            }
        }
        private async Task CrawlBTransactionMezonD()
        {
            Console.WriteLine(">>> CrawlBTransactionMezonD() START <<<");
            InitDBMezonTransactionHashes();

            var dicMezonBankAccounts = GetDicWalletToBankAccountInfo();
            if (dicMezonBankAccounts == null || !dicMezonBankAccounts.Any())
            {
                Console.WriteLine("dicMezonBankAccounts is NULL or EMPTY");
                return;
            }
            var dicPeriods = GetDicTenantIdToActivePeriodId();
            if (dicPeriods == null || !dicPeriods.Any())
            {
                Console.WriteLine("dicPeriods is NULL or EMPTY");
                return;
            }

            //get data using httpclient call to mezon api for multiple wallet addresses
            var dicMezonTransactions = await _mezonDongService.GetMezonTransactions(dicMezonBankAccounts);
            if (dicMezonTransactions == null || !dicMezonTransactions.Any())
            {
                Console.WriteLine("dicMezonTransactions is NULL or EMPTY");
                return;
            }

            Console.WriteLine("Passed all null checks, starting processing...");

            //get hash not exists in List to insert to DB
            var insertHashes = dicMezonTransactions.Keys.Except(_dbMezonTransactionHashes);

            using (var uow = _context.Database.BeginTransaction())
            {
                foreach (var hash in insertHashes)
                {
                    AddToDbMezonTransactionHashes(hash);
                    try
                    {
                        var mezonTx = dicMezonTransactions[hash];
                        _log.LogInformation($"Hash: {hash} and Value: {dicMezonTransactions[hash]}");

                        var bTransactionLog = new BTransactionLog()
                        {
                            Message = dicMezonTransactions[hash],
                            IsValid = false,
                            Key = hash
                        };

                        // Extract transaction info from message
                        var crawlResult = CrawlMezonTransactionHelper.ExtractMezonTransaction(dicMezonTransactions[hash]);

                        // Validate extracted data
                        if (crawlResult.TransactionAmount == 0 || string.IsNullOrEmpty(crawlResult.WalletAddress))
                        {
                            bTransactionLog.ErrorMessage = "Can't extract TransactionAmount or WalletAddress";
                            _context.Add(bTransactionLog);
                            _log.LogInformation(JsonConvert.SerializeObject(bTransactionLog));
                            continue;
                        }

                        //get datetime of transaction
                        var timeAt = Helpers.ConvertFromUnixTimestamp(crawlResult.Timestamp);
                        bTransactionLog.TimeAt = timeAt;

                        //get information bank account -> {Id, CurrencyId, TenantId} and check exists
                        var bankAccount = GetBankAccountByWalletAddress(dicMezonBankAccounts, crawlResult.WalletAddress);
                        if (!bankAccount.IsValid)
                        {
                            bTransactionLog.ErrorMessage = bankAccount.ErrorMessage;
                            _context.Add(bTransactionLog);
                            _log.LogInformation(JsonConvert.SerializeObject(bTransactionLog));
                            continue;
                        }

                        var tenantId = bankAccount.Result.TenantId;
                        var bTransaction = new BTransaction
                        {
                            BankAccountId = bankAccount.Result.Id,
                            Money = crawlResult.TransactionAmount,
                            TimeAt = timeAt,
                            Note = dicMezonTransactions[hash],
                            IsCrawl = true,
                            TenantId = tenantId,
                            Status = BTransactionStatus.DONE
                        };

                        if (!tenantId.HasValue && dicPeriods.ContainsKey(TENANT_NULL_ID))
                        {
                            bTransaction.PeriodId = dicPeriods[TENANT_NULL_ID];
                        }
                        else if (tenantId.HasValue && dicPeriods.ContainsKey(tenantId.Value))
                        {
                            bTransaction.PeriodId = dicPeriods[tenantId.Value];
                        }
                        else
                        {
                            bTransactionLog.ErrorMessage = $"No active period found for tenant {tenantId}";
                            _context.Add(bTransactionLog);
                            _log.LogWarning($"Hash: {hash}, {bTransactionLog.ErrorMessage}");
                            continue;
                        }

                        _context.Add(bTransaction);
                        _context.SaveChanges();

                        double currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);

                        var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);
                        if (bool.Parse(config))
                        {
                            string contentNotify = GetContentNotificationMezonDong(
                                hash: crawlResult.Hash,
                                walletAddress: crawlResult.WalletAddress,
                                bankAccountName: bankAccount.Result.BankAccountName,
                                money: crawlResult.TransactionAmount,
                                currencyName: bankAccount.Result.CurrencyName,
                                timeAt: timeAt.ToString("dd/MM/yyyy HH:mm"),
                                currentBalance: currentBalanceNumber,
                                sender: crawlResult.Sender,
                                receiver: crawlResult.Receiver
                            );
                            var mezonMessage = new MezonMessage
                            {
                                t = contentNotify,
                                mentions = new List<Mentions>()
                            };
                            _mezonNotification.NotifyWithMezonMessage(mezonMessage, tenantId);
                        }
                        // Đánh dấu log là hợp lệ và lưu thông tin
                        bTransactionLog.BTransactionId = bTransaction.Id;
                        bTransactionLog.IsValid = true;
                        bTransactionLog.TenantId = bankAccount.Result.TenantId;
                        _context.Add(bTransactionLog);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError($"Hash: {hash}, Exception: {ex.Message}", ex);
                    }
                }
                _context.SaveChanges();
                uow.Commit();
            }
        }
        private void InitDBMezonTransactionHashes()
        {
            if (_dbMezonTransactionHashes.Any()) return;

            _dbMezonTransactionHashes = _context.BTransactionLogs
                .Where(x => !x.IsDeleted)
                .Where(x => x.Key != null)
                .Where(x => x.Key.Length == 66)
                .Select(x => x.Key)
                .ToList();

            _log.LogInformation($"Initialized {_dbMezonTransactionHashes.Count} Mezon transaction hashes from BTransactionLog");
        }

        private Dictionary<string, MezonBankAccountCrawl> GetDicWalletToBankAccountInfo()
        {
            return _context.BankAccounts
                .Where(x => !x.IsDeleted)
                .Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .Where(x => x.Currency.Name == "MezonD")
                .Where(x => !string.IsNullOrEmpty(x.BankNumber))
                .AsEnumerable()
                .GroupBy(x => x.BankNumber)
                .Select(x => new
                {
                    MezonId = x.Key,
                    Info = x.Select(s => new
                    {
                        s.BankNumber,
                        s.CurrencyId,
                        s.Id,
                        s.TenantId,
                        s.HolderName,
                        CurrencyName = s.Currency.Name
                    }).FirstOrDefault()
                })
                .ToDictionary(x => x.MezonId, x => new MezonBankAccountCrawl
                {
                    WalletAddress = x.Info.BankNumber,
                    CurrencyId = x.Info.CurrencyId,
                    Id = x.Info.Id,
                    TenantId = x.Info.TenantId,
                    BankAccountName = x.Info.HolderName,
                    CurrencyName = x.Info.CurrencyName
                });
        }
        private void AddToDbMezonTransactionHashes(string hash)
        {
            try
            {
                _dbMezonTransactionHashes.Add(hash);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
        private ResultCheckMezonBankAccount GetBankAccountByWalletAddress(Dictionary<string, MezonBankAccountCrawl> dicMezonBankAccounts, string walletAddress)
        {
            if (!dicMezonBankAccounts.ContainsKey(walletAddress))
                return new ResultCheckMezonBankAccount
                {
                    ErrorMessage = "Can't Found Mezon Id",
                    IsValid = false
                };

            return new ResultCheckMezonBankAccount
            {
                IsValid = true,
                Result = dicMezonBankAccounts[walletAddress]
            };
        }
        private string GetContentNotificationMezonDong(
            string hash,
            string walletAddress,
            string bankAccountName,
            double money,
            string currencyName,
            string timeAt,
            double currentBalance,
            string sender,
            string receiver
        )
        {
            var direction = money > 0 ? "NHẬN VÀO" : "CHUYỂN ĐI";
            var sb = new StringBuilder()
                .Append($" **{direction} - BĐSD TK MezonĐ**\n")
                .Append($" Tài khoản: **{bankAccountName}** ({walletAddress})\n")
                .Append($" Số tiền: **{(money > 0 ? "+" : "")}{Helpers.FormatMoney(money)} {currencyName}**\n")
                .Append($" Thời gian: {timeAt}\n")
                .Append($" Từ: `{FormatWalletAddress(sender)}`\n")
                .Append($" Đến: `{FormatWalletAddress(receiver)}`\n")
                .Append($" Hash: `{hash}`\n")
                .Append($" Dư hiện tại: **{Helpers.FormatMoney(currentBalance)} {currencyName}**");

            return sb.ToString();
        }

        private string FormatWalletAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length <= 12)
                return address;

            return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 6)}";
        }
        private double GetCurrentBalanance(long periodId, long bankAccountId)
        {
            var duDauKy = _context.PeriodBankAccounts
                .Where(x => !x.IsDeleted)
                .Where(x => x.IsActive)
                .Where(x => x.PeriodId == periodId)
                .Where(s => s.BankAccountId == bankAccountId)
                .Select(x => x.BaseBalance)
                .FirstOrDefault();

            var bienDongTangGiam = _context.BTransactions
                .Where(x => !x.IsDeleted)
                .Where(x => x.PeriodId == periodId)
                .Where(x => x.BankAccountId == bankAccountId)
                .Sum(x => x.Money);
            var duHienTai = duDauKy + bienDongTangGiam;
            return duHienTai;
        }
        private Dictionary<int, int> GetDicTenantIdToActivePeriodId()
        {
            return _context.Periods
                .Where(x => x.IsActive)
                .Select(x => new { x.Id, TenantId = x.TenantId.HasValue ? x.TenantId.Value : TENANT_NULL_ID })
                .ToDictionary(x => x.TenantId, x => x.Id);
        }
    }
}
