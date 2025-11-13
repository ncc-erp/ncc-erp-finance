using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Enums;
using FinanceManagement.Helper;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Notifications.Mezon;
using FinanceManagement.Notifications.Mezon.Dto;
using FinanceManagement.Services.Mezon;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static FinanceManagement.Services.Mezon.MMNService;

namespace FinanceManagement.Web.Host.Startup
{
    public class CrawlBTransactionMezonDongBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<CrawlBTransactionMezonDongBackgroundWorker> _log;
        private FinanceManagementDbContext _context;
        private readonly MMNService _mmnService;
        private static List<string> _dbMezonBTransactionList;
        private const int TENANT_NULL_ID = -1;
        private readonly IMezonNotification _mezonNotification;
        public IMySettingManager MySettingManager { get; set; }

        public CrawlBTransactionMezonDongBackgroundWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            ILogger<CrawlBTransactionMezonDongBackgroundWorker> log,
            IMezonNotification mezonNotification,
            IHttpClientFactory httpClientFactory,
            MMNService mmnService
        ) : base(timer)
        {
            _context = iocResolver.Resolve<FinanceManagementDbContext>();
            _log = log;
            _mmnService = mmnService;
            _dbMezonBTransactionList = new List<string>();
            Timer.Period = CrawlMezonDongConfig.IntervalSecond * 1000;
            _mezonNotification = mezonNotification;
        }
        protected override void DoWork()
        {
            _log.LogInformation($"CrawlBTransactionMezonDongBackgroundWorker.DoWork() start");

            CrawlBTransactionMezonD().Wait();

        }
        private async Task CrawlBTransactionMezonD()
        {
            try
            {
                var dicMezonBankAccounts = GetDicWalletToBankAccountInfo();
                if (!dicMezonBankAccounts.Any())
                {
                    _log.LogInformation("No MezonĐ bank accounts found");
                    return;
                }

                var walletAddresses = dicMezonBankAccounts.Keys.ToList();
                var dicPeriods = GetDicTenantIdToActivePeriodId();

                InitDBMezonBTransactionList();

                //get data using httpclient call to mezon api for multiple wallet addresses
                var mmnTransactions = await _mmnService.GetMMNTransactions(walletAddresses);


                int successCount = 0;
                int errorCount = 0;

                using (var uow = _context.Database.BeginTransaction())
                {
                    foreach (var tx in mmnTransactions)
                    {
                        string hash = tx.Hash;

                        bool isSenderInDb = dicMezonBankAccounts.ContainsKey(tx.Sender);
                        bool isReceiverInDb = dicMezonBankAccounts.ContainsKey(tx.Receiver);

                        if (!isSenderInDb && !isReceiverInDb)
                            continue;

                        // 1) Lưu chiều gửi
                        if (isSenderInDb)
                        {
                            string key = $"{hash}_{tx.Sender}";

                            if (!_dbMezonBTransactionList.Contains(key))
                            {
                                _dbMezonBTransactionList.Add(key);
                                bool ok = await InsertTransaction(tx, tx.Sender, true, dicMezonBankAccounts, dicPeriods, key);
                                if (ok) successCount++; else errorCount++;
                            }
                        }

                        // 2) Lưu chiều nhận
                        if (isReceiverInDb)
                        {
                            string key = $"{hash}_{tx.Receiver}";

                            if (!_dbMezonBTransactionList.Contains(key))
                            {
                                _dbMezonBTransactionList.Add(key);
                                bool ok = await InsertTransaction(tx, tx.Receiver, false, dicMezonBankAccounts, dicPeriods, key);
                                if (ok) successCount++; else errorCount++;
                            }
                        }
                    }

                    _context.SaveChanges();
                    uow.Commit();

                    _log.LogInformation($"Crawl completed. Success: {successCount}, Errors: {errorCount}");

                }
            }
            catch (Exception ex)
            {
                _log.LogError($"CrawlBTransactionMezonD() ERROR: {ex.Message}", ex);
                throw;
            }
        }
        private async Task<bool> InsertTransaction(MMNTransaction tx,
            string walletAddress,
            bool isSender,
            Dictionary<string, BankAccountCrawl> dicMezonBankAccounts,
            Dictionary<int, int> dicPeriods,
            string transactionKey)
        {
            try
            {
                var bTransactionLog = new BTransactionLog()
                {
                    Message = JsonConvert.SerializeObject(tx),
                    IsValid = false,
                    Key = transactionKey
                };

                double money = isSender ? -tx.Amount : tx.Amount;

                //get datetime of transaction
                var timeAt = Helpers.ConvertFromUnixTimestamp(tx.Timestamp);
                bTransactionLog.TimeAt = timeAt;

                var bankAccount = dicMezonBankAccounts[walletAddress];

                var tenantId = bankAccount.TenantId;

                var bTransaction = new BTransaction
                {
                    BankAccountId = bankAccount.Id,
                    Money = money,
                    TimeAt = timeAt,
                    Note = JsonConvert.SerializeObject(tx),
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
                }

                _context.Add(bTransaction);
                _context.SaveChanges();

                double currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);

                var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);
                if (bool.Parse(config))
                {
                    string contentNotify = GetContentNotificationMezonDong(
                        hash: tx.Hash,
                        bankAccountName: bankAccount.BankAccountName,
                        money: money,
                        currencyName: bankAccount.CurrencyName,
                        timeAt: timeAt.ToString("dd/MM/yyyy HH:mm"),
                        duSo: currentBalanceNumber
                    );
                    var mezonMessage = new MezonMessage
                    {
                        t = contentNotify,
                        mentions = new List<Mentions>()
                    };
                    _mezonNotification.NotifyWithMezonMessage(mezonMessage, tenantId);
                }

                bTransactionLog.BTransactionId = bTransaction.Id;
                bTransactionLog.IsValid = true;
                bTransactionLog.TenantId = bankAccount.TenantId;
                _context.Add(bTransactionLog);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError($"InsertTransaction ERROR key={transactionKey}: {ex}");
                return false;
            }
        }
        private void InitDBMezonBTransactionList()
        {
            if (_dbMezonBTransactionList.Any()) return;

            _dbMezonBTransactionList = _context.BTransactionLogs
                .Where(x => !x.IsDeleted)
                .Where(x => x.Key != null)
                .Where(x => x.Key.Contains("_"))
                .Select(x => x.Key)
                .ToList();

            _log.LogInformation($"Initialized {_dbMezonBTransactionList.Count} Mezon transaction hashes from BTransactionLog");
        }
        private Dictionary<string, BankAccountCrawl> GetDicWalletToBankAccountInfo()
        {
            return _context.BankAccounts
                .Where(x => !x.IsDeleted)
                .Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .Where(x => x.Currency.Code == CurrencyCode.MEZOND)
                .Where(x => !string.IsNullOrEmpty(x.BankNumber))
                .AsEnumerable()
                .Where(x => x.BankNumber.Length == 44)
                .GroupBy(x => x.BankNumber)
                .ToDictionary(
                    g => g.Key,
                    g => new BankAccountCrawl
                    {
                        WalletAddress = g.Key,
                        CurrencyId = g.First().CurrencyId,
                        Id = g.First().Id,
                        TenantId = g.First().TenantId,
                        BankAccountName = g.First().HolderName,
                        CurrencyName = g.First().Currency.Name
                    });
        }
        private string GetContentNotificationMezonDong(
            string hash,
            string bankAccountName,
            double money,
            string currencyName,
            string timeAt,
            double duSo
        )
        {
            var sb = new StringBuilder();

            // 1) Dòng chính
            sb.Append($"BĐSD TK: {bankAccountName} {(money > 0 ? "+" : "")}{Helpers.FormatMoney(money)} {currencyName} lúc {timeAt}");

            // 2) URL giao dịch
            sb.Append($"\nhttps://dev-mmn.nccsoft.vn/transactions/{hash}");

            sb.Append($"\nDư sổ(A): {Helpers.FormatMoney(duSo)}");

            return sb.ToString();
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
