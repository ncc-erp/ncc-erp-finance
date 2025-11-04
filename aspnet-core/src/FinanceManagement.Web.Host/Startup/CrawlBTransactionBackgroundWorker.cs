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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SimpleBase;
using FinanceManagement.Services.Mezon;

namespace FinanceManagement.Web.Host.Startup
{
    public class CrawlBTransactionBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<CrawlBTransactionBackgroundWorker> _log;
        private FinanceManagementDbContext _context;
        private readonly FirebaseService _firebaseService;
        private static List<string> _dbTransactionKeys;
        private readonly IOptions<FirebaseConfig> _firesbaseOptions;
        private const int TENANT_NULL_ID = -1;
        private readonly IMezonNotification _mezonNotification;
        public IMySettingManager MySettingManager { get; set; }

        public CrawlBTransactionBackgroundWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            ILogger<CrawlBTransactionBackgroundWorker> log,
            FirebaseService firebaseService,
            IOptions<FirebaseConfig> options,
            IMezonNotification mezonNotification
        ) : base(timer)
        {
            _context = iocResolver.Resolve<FinanceManagementDbContext>();
            _log = log;
            _firebaseService = firebaseService;
            _dbTransactionKeys = new List<string>();
            _firesbaseOptions = options;
            Timer.Period = _firesbaseOptions.Value.IntervalMilisecond;
            _mezonNotification = mezonNotification;
        }
        protected override void DoWork()
        {
            _log.LogInformation($"CrawlBTransactionBackgroundWorker.DoWork() start");
            CrawlBTransaction().Wait();

        }
        private async Task CrawlBTransaction()
        {
            //using HashSet to save key exists
            InitDBTransactionKeys();

            var dicBankTransactions = GetDicBankNumberToBankAccountInfo();
            var dicPeriods = GetDicTenantIdToActivePeriodId();

            //get data using httpclient call to firebase
            var dicFireBaseTransaction = await _firebaseService.GetBTransactions<Dictionary<string, string>>();

            //get key not exists in HashSet to insert to DB
            var insertKeys = dicFireBaseTransaction.Keys.Except(_dbTransactionKeys);
           
            using (var uow = _context.Database.BeginTransaction())
            {
                foreach (var key in insertKeys)
                {
                    AddToDbTransactionKeys(key);
                    try
                    {
                        _log.LogInformation($"Key: {key} and Value: {dicFireBaseTransaction[key]}");

                        var bTransactionLog = new BTransactionLog()
                        {
                            Message = dicFireBaseTransaction[key],
                            IsValid = false,
                            Key = key
                        };

                        //get datetime of transaction
                        var convertTimestamp = Helpers.ConvertTimestampToLong(key);
                        if (!convertTimestamp.IsValid)
                        {
                            bTransactionLog.ErrorMessage = convertTimestamp.ErrorMessage;
                            _context.Add(bTransactionLog);

                            continue;
                        }
                        var timeAt = Helpers.ConvertFromUnixTimestamp(convertTimestamp.Result);
                        bTransactionLog.TimeAt = timeAt;
                        var crawlResult = CrawlBTransactionHelper.ExtractBTransaction(dicFireBaseTransaction[key]);
                        if (crawlResult.TransactionAmount == 0 || string.IsNullOrEmpty(crawlResult.AccountNumber))
                        {
                            bTransactionLog.ErrorMessage = "Can't extract TransactionAmount or AccountNumber";
                            _context.Add(bTransactionLog);
                            _log.LogInformation(JsonConvert.SerializeObject(bTransactionLog));
                            continue;
                        }

                        //get information bank account -> {Id, CurrencyId, TenantId} and check exists
                        var bankAccount = GetBankAccountByBankNumber(dicBankTransactions, crawlResult.AccountNumber);
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
                            Note = dicFireBaseTransaction[key],
                            IsCrawl = true,
                            TenantId = tenantId
                        };
                        if (!tenantId.HasValue && dicPeriods.ContainsKey(TENANT_NULL_ID))
                        {
                            bTransaction.PeriodId = dicPeriods[TENANT_NULL_ID];
                        }
                        else if (tenantId.HasValue && dicPeriods.ContainsKey(tenantId.Value))
                        {
                            bTransaction.PeriodId = dicPeriods[tenantId.Value];
                        }

                        _context.Add(bTransaction);
                        _context.SaveChanges();

                        double currentBalanceNumber = -1;

                        currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);                   

                        var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);

                        if (bool.Parse(config))
                        {
                            string contentNotify = GetContentNotificationCrawlBTransaction(
                                message: dicFireBaseTransaction[key],
                                bankNumber: crawlResult.AccountNumber,
                                bankAccountName: bankAccount.Result.BankAccountName,
                                money: crawlResult.TransactionAmount,
                                currencyName: bankAccount.Result.CurrencyName,
                                timeAt.ToString("dd/MM/yyyy HH:mm"),
                                duTheoMessage: crawlResult.Balance,
                                duSo: currentBalanceNumber
                            );
                            var mezonMessage = new MezonMessage
                            {
                                t = contentNotify,
                                mentions = new List<Mentions>()
                            };

                            //_komuNotification.NotifyWithMessage(contentNotify, tenantId);
                            _mezonNotification.NotifyWithMezonMessage(mezonMessage, tenantId);
                        }

                        bTransactionLog.BTransactionId = bTransaction.Id;
                        bTransactionLog.IsValid = true;
                        bTransactionLog.TenantId = bankAccount.Result.TenantId;
                        _context.Add(bTransactionLog);
                        
                    }
                    catch (Exception ex)
                    {
                        _log.LogError($"Key: {key}, Exception: " + ex.InnerException);
                    }
                }
                _context.SaveChanges();
                uow.Commit();
            }
        }
        private void InitDBTransactionKeys()
        {
            if (_dbTransactionKeys.Any()) return;   
            _dbTransactionKeys = _context.BTransactionLogs
                .Where(s => !s.IsDeleted)
                .Select(s => s.Key)
                .ToList();
              
        }
        private Dictionary<int, int> GetDicTenantIdToActivePeriodId()
        {
            return _context.Periods
                .Where(x => x.IsActive)
                .Select(x => new { x.Id, TenantId = x.TenantId.HasValue ? x.TenantId.Value : TENANT_NULL_ID })
                .ToDictionary(x => x.TenantId, x => x.Id);
        }
        private Dictionary<string, BankAccountCrawl> GetDicBankNumberToBankAccountInfo()
        {
            return _context.BankAccounts
                .Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .Where(x => x.BankNumber.Length >= 5)
                .AsEnumerable()
                .GroupBy(x => x.BankNumber)
                .Select(x => new
                {
                    x.Key,
                    Info = x.Select(s => new { s.BankNumber, s.CurrencyId, s.Id, s.TenantId, s.HolderName, CurrencyName = s.Currency.Name }).FirstOrDefault()
                })
                .ToDictionary(x => x.Key, x => new BankAccountCrawl
                {
                    CurrencyId = x.Info.CurrencyId,
                    Id = x.Info.Id,
                    TenantId = x.Info.TenantId,
                    BankAccountName = x.Info.HolderName,
                    CurrencyName = x.Info.CurrencyName
                });
        }
        private void AddToDbTransactionKeys(string key)
        {
            try
            {
                _dbTransactionKeys.Add(key);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
        private ResultCheckBankAccount GetBankAccountByBankNumber(Dictionary<string, BankAccountCrawl> dicBankAccounts, string bankNumber)
        {
            if (!dicBankAccounts.ContainsKey(bankNumber))
                return new ResultCheckBankAccount
                {
                    ErrorMessage = "Can't Found Bank Number",
                    IsValid = false
                };

            return new ResultCheckBankAccount
            {
                IsValid = true,
                Result = dicBankAccounts[bankNumber]
            };
        }

        private string GetContentNotificationCrawlBTransaction(
            string message,
            string bankNumber = "",
            string bankAccountName = "",
            double money = 0,
            string currencyName = "",
            string timeAt = "",
            double duTheoMessage = 0,
            double duSo = 0
        )
        {
            var sb = new StringBuilder()
                        .Append($"BĐSD TK: {bankAccountName} ({bankNumber}) {(money > 0 ? "+" : "")}{Helpers.FormatMoney(money)}** {currencyName} lúc {timeAt} ." )
                        .Append($"\r\n{message}");
            if (duSo >= 0)
                sb.Append($"\nDư sổ(A): {Helpers.FormatMoney(duSo)} {(duSo == duTheoMessage ? "" : "KHÁC")} dư theo BĐSD(B): {Helpers.FormatMoney(duTheoMessage)} => Chênh lệch(B-A): {Helpers.FormatMoney(duTheoMessage - duSo)}");

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

    }
}
