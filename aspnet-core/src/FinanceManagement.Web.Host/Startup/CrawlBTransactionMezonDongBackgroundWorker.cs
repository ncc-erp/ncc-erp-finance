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
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Host.Startup
{
    public class CrawlBTransactionMezonDongBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<CrawlBTransactionMezonDongBackgroundWorker> _log;
        private FinanceManagementDbContext _context;
        private readonly MMNService _mmnService;
        private static List<string> _dbMezonTransactionHashes;
        private const int TENANT_NULL_ID = -1;
        private readonly IMezonNotification _mezonNotification;
        private const string MEZON_D_CURRENCY_CODE = "MEZOND";
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
            _dbMezonTransactionHashes = new List<string>();
            Timer.Period = CrawlMezonDongConfig.IntervalMilisecond;
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
                var dicPeriods = GetDicTenantIdToActivePeriodId();

                if (!dicMezonBankAccounts.Any())
                {
                    _log.LogInformation("No MezonĐ bank accounts found");
                    return;
                }
                InitDBMezonTransactionHashes();

                //get data using httpclient call to mezon api for multiple wallet addresses
                var dicMezonTransactions = await _mmnService.GetMezonTransactions(dicMezonBankAccounts);


                int successCount = 0;
                int errorCount = 0;

                using (var uow = _context.Database.BeginTransaction())
                {
                    foreach (var kvp in dicMezonTransactions)
                    {
                        var hash = kvp.Key;
                        var mezonTx = kvp.Value;

                        // Xác định các wallet address liên quan đến transaction này
                        var relatedWallets = new List<string>();

                        // Check sender có phải bank account không
                        if (dicMezonBankAccounts.ContainsKey(mezonTx.Sender))
                        {
                            relatedWallets.Add(mezonTx.Sender);
                        }

                        // Check receiver có phải bank account không
                        if (dicMezonBankAccounts.ContainsKey(mezonTx.Receiver))
                        {
                            relatedWallets.Add(mezonTx.Receiver);
                        }

                        if (!relatedWallets.Any())
                        {
                            _log.LogWarning($"Transaction {hash} không liên quan đến bank account nào trong hệ thống");
                            continue;
                        }

                        foreach (var walletAddress in relatedWallets)
                        {
                            var transactionKey = BuildTransactionKey(hash, walletAddress);

                            if (_dbMezonTransactionHashes.Contains(transactionKey))
                            {
                                _log.LogDebug($"Key {transactionKey} already processed, skipping");
                                continue;
                            }

                            AddToDbMezonTransactionHashes(transactionKey);
                            try
                            {
                                var bTransactionLog = new BTransactionLog()
                                {
                                    Message = JsonConvert.SerializeObject(mezonTx),
                                    IsValid = false,
                                    Key = transactionKey
                                };

                                if (mezonTx.Amount == 0)
                                {
                                    bTransactionLog.ErrorMessage = "Transaction amount is zero";
                                    _context.Add(bTransactionLog);
                                    errorCount++;
                                    continue;
                                }

                                double money = mezonTx.Sender.Equals(walletAddress, StringComparison.OrdinalIgnoreCase) ? -mezonTx.Amount
                                             : mezonTx.Receiver.Equals(walletAddress, StringComparison.OrdinalIgnoreCase) ? mezonTx.Amount
                                             : 0;

                                //get datetime of transaction
                                var timeAt = Helpers.ConvertFromUnixTimestamp(mezonTx.Timestamp);
                                bTransactionLog.TimeAt = timeAt;

                                //get information bank account -> {Id, CurrencyId, TenantId} and check exists
                                var bankAccount = GetBankAccountByWalletAddress(dicMezonBankAccounts, walletAddress);
                                if (!bankAccount.IsValid)
                                {
                                    bTransactionLog.ErrorMessage = bankAccount.ErrorMessage;
                                    _context.Add(bTransactionLog);
                                    errorCount++; ;
                                    continue;
                                }

                                var tenantId = bankAccount.Result.TenantId;

                                // Build readable note
                                string senderName = GetBankAccountNameByWallet(dicMezonBankAccounts, mezonTx.Sender);
                                string receiverName = GetBankAccountNameByWallet(dicMezonBankAccounts, mezonTx.Receiver);

                                string note = $"Tài khoản {senderName} đã chuyển đến tài khoản {receiverName} số tiền {mezonTx.Amount} MezonĐ";
                                if (!string.IsNullOrEmpty(mezonTx.TextData))
                                {
                                    note += $" với ND: {mezonTx.TextData}";
                                }

                                var bTransaction = new BTransaction
                                {
                                    BankAccountId = bankAccount.Result.Id,
                                    Money = money,
                                    TimeAt = timeAt,
                                    Note = JsonConvert.SerializeObject(mezonTx),
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
                                    errorCount++;
                                    continue;
                                }

                                _context.Add(bTransaction);
                                _context.SaveChanges();

                                double currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);

                                var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);
                                if (bool.Parse(config))
                                {
                                    string contentNotify = GetContentNotificationMezonDong(
                                        hash: hash,
                                        walletAddress: walletAddress,
                                        bankAccountName: bankAccount.Result.BankAccountName,
                                        money: money,
                                        currencyName: bankAccount.Result.CurrencyName,
                                        timeAt: timeAt.ToString("dd/MM/yyyy HH:mm"),
                                        currentBalance: currentBalanceNumber,
                                        sender: mezonTx.Sender,
                                        receiver: mezonTx.Receiver
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
                                bTransactionLog.TenantId = bankAccount.Result.TenantId;
                                _context.Add(bTransactionLog);

                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                _log.LogError($"Hash: {hash}, Exception: {ex.Message}", ex);
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
        private void InitDBMezonTransactionHashes()
        {
            if (_dbMezonTransactionHashes.Any()) return;

            _dbMezonTransactionHashes = _context.BTransactionLogs
                .Where(x => !x.IsDeleted)
                .Where(x => x.Key != null)
                .Where(x => x.Key.Contains("_"))
                .Select(x => x.Key)
                .ToList();

            _log.LogInformation($"Initialized {_dbMezonTransactionHashes.Count} Mezon transaction hashes from BTransactionLog");
        }
        private string BuildTransactionKey(string hash, string walletAddress)
        {
            return $"{hash}_{walletAddress}";
        }
        private Dictionary<string, MezonBankAccountCrawl> GetDicWalletToBankAccountInfo()
        {
            return _context.BankAccounts
                .Where(x => !x.IsDeleted)
                .Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .Where(x => x.Currency.Code == MEZON_D_CURRENCY_CODE)
                .Where(x => !string.IsNullOrEmpty(x.BankNumber))
                .AsEnumerable()
                .Where(x => x.BankNumber.Length == 44)
                .GroupBy(x => x.BankNumber)
                .Select(g => g.FirstOrDefault())
                .ToDictionary(
                    x => x.BankNumber,
                    x => new MezonBankAccountCrawl
                    {
                        WalletAddress = x.BankNumber,
                        CurrencyId = x.CurrencyId,
                        Id = x.Id,
                        TenantId = x.TenantId,
                        BankAccountName = x.HolderName,
                        CurrencyName = x.Currency.Name
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
                    ErrorMessage = "Can't find bank account for wallet {FormatWalletAddress(walletAddress)}",
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
                .Append($" Từ: `{sender}`\n")
                .Append($" Đến: `{receiver}`\n")
                .Append($" Hash: `{hash}`\n")
                .Append($" Dư hiện tại: **{Helpers.FormatMoney(currentBalance)} {currencyName}**");

            return sb.ToString();
        }

        /// Get bank account name by wallet address (for display)
        private string GetBankAccountNameByWallet(
            Dictionary<string, MezonBankAccountCrawl> dicMezonBankAccounts,
            string walletAddress)
        {
            if (dicMezonBankAccounts.ContainsKey(walletAddress))
            {
                return dicMezonBankAccounts[walletAddress].BankAccountName;
            }

            return walletAddress;
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
