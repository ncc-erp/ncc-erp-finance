using Abp.Dependency;
using FinanceManagement.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Helper;
using System.Text.RegularExpressions;
using FinanceManagement.Configuration;
using FinanceManagement.Managers.BTransactions.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using FinanceManagement.Services.Firebase;
using FinanceManagement.GeneralModels;
using FinanceManagement.Enums;
using FinanceManagement.Notifications.Komu;
using FinanceManagement.Entities;
using FinanceManagement.Managers.Settings;

namespace FinanceManagement.Web.Host.Startup
{
    public class CrawlBTransactionBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<CrawlBTransactionBackgroundWorker> _log;
        private FinanceManagementDbContext _context;
        private readonly FirebaseService _firebaseService;
        private static HashSet<string> _hashSetKey;
        private readonly IOptions<FirebaseConfig> _firesbaseOptions;
        private readonly IOptions<KomuNotificationConfig> _komuNotificationOptions;
        private const int TENANT_NULL_ID = -1;
        private readonly IKomuNotification _komuNotification;
        public IMySettingManager MySettingManager { get; set; }

        public CrawlBTransactionBackgroundWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            ILogger<CrawlBTransactionBackgroundWorker> log,
            FirebaseService firebaseService,
            IOptions<FirebaseConfig> options,
            IOptions<KomuNotificationConfig> komuNotificationOptions,
            IKomuNotification komuNotification
        ) : base(timer)
        {
            _context = iocResolver.Resolve<FinanceManagementDbContext>();
            _log = log;
            _firebaseService = firebaseService;
            _hashSetKey = new HashSet<string>();
            _firesbaseOptions = options;
            Timer.Period = _firesbaseOptions.Value.IntervalMilisecond;
            _komuNotificationOptions = komuNotificationOptions;
            _komuNotification = komuNotification;
        }
        protected override void DoWork()
        {
            _log.LogCritical($"Start Firebase Background Service");
            CrawlBTransaction().Wait();
        }
        private async Task CrawlBTransaction()
        {
            //using HashSet to save key exists
            InitHashSetKey();

            var dicBankTransactions = GetDicBankNumberToBankAccountInfo();
            var dicPeriods = GetDicTenantIdToActivePeriodId();

            //get data using httpclient call to firebase
            var dics = await _firebaseService.GetCrawlTransactions<Dictionary<string, string>>();

            //get key not exists in HashSet to insert to DB
            var insertKeys = dics.Keys.Except(_hashSetKey);

            var regexMoneyDetection = new Regex(SettingManager.GetSettingValueForApplication(AppSettingNames.RegexMoneyDetection));
            var regexSTKDetection = new Regex(SettingManager.GetSettingValueForApplication(AppSettingNames.RegexSTKDetection));
            var regexRemainMoneyDetection = new Regex(SettingManager.GetSettingValueForApplication(AppSettingNames.RegexRemainMoneyDetection));

            using (var uow = _context.Database.BeginTransaction())
            {
                foreach (var key in insertKeys)
                {
                    AddToHashSet(key);
                    try
                    {
                        _log.LogCritical($"Key: {key} and Value: {dics[key]}");

                        var logger = new BTransactionLog()
                        {
                            Message = dics[key],
                            IsValid = false,
                            Key = key
                        };

                        //get datetime of transaction
                        var convertTimestamp = Helpers.ConvertTimestampToLong(key);
                        if (!convertTimestamp.IsValid)
                        {
                            logger.ErrorMessage = convertTimestamp.ErrorMessage;
                            _context.Add(logger);

                            continue;
                        }
                        var timeAt = Helpers.ConvertFromUnixTimestamp(convertTimestamp.Result);
                        logger.TimeAt = timeAt;

                        var messageClean = Helpers.RemoveNewLine(dics[key]);

                        var moneyDetection = Helpers.DetectionMoney(regexMoneyDetection, messageClean);
                        var bankNumberDetection = Helpers.DetectionBankNumber(regexSTKDetection, messageClean);
                        var remainMoneyDetection = Helpers.DetectionMoney(regexRemainMoneyDetection, messageClean);

                        //check money of transaction
                        if (!moneyDetection.IsValid)
                        {
                            logger.ErrorMessage = moneyDetection.ErrorMessage;
                            _context.Add(logger);
                            continue;
                        }

                        //check bank number of transaction
                        if (!bankNumberDetection.IsValid)
                        {
                            logger.ErrorMessage = bankNumberDetection.ErrorMessage;
                            _context.Add(logger);
                            continue;
                        }

                        //get information bank account -> {Id, CurrencyId, TenantId} and check exists
                        var bankAccount = GetBankAccountByBankNumber(dicBankTransactions, bankNumberDetection.Result);
                        if (!bankAccount.IsValid)
                        {
                            logger.ErrorMessage = bankAccount.ErrorMessage;
                            _context.Add(logger);
                            continue;
                        }

                        var tenantId = bankAccount.Result.TenantId;
                        var bTransaction = new BTransaction
                        {
                            BankAccountId = bankAccount.Result.Id,
                            Money = moneyDetection.Result,
                            TimeAt = timeAt,
                            Note = dics[key],
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

                        //check remain money detection
                        if (remainMoneyDetection.IsValid)
                        {
                            currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);
                        }
                        else
                        {
                            logger.ErrorMessage = remainMoneyDetection.ErrorMessage;
                        }

                        var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);

                        if (bool.Parse(config))
                        {
                            string contentNotify = GetContentNotificationCrawlBTransaction(
                                message: dics[key],
                                bankNumber: bankNumberDetection.Result,
                                bankAccountName: bankAccount.Result.BankAccountName,
                                money: moneyDetection.Result,
                                currencyName: bankAccount.Result.CurrencyName,
                                timeAt.ToString("dd/MM/yyyy HH:mm"),
                                duTheoMessage: remainMoneyDetection.Result,
                                duSo: currentBalanceNumber
                            );

                            _komuNotification.NotifyWithMessage(contentNotify, tenantId);
                        }

                        logger.BTransactionId = bTransaction.Id;
                        logger.IsValid = true;
                        logger.TenantId = bankAccount.Result.TenantId;
                        _context.Add(logger);
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
        private void InitHashSetKey()
        {
            if (_hashSetKey.Any()) return;
            _hashSetKey = _context.BTransactionLogs.Select(s => s.Key).ToHashSet();
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
        private void AddToHashSet(string key)
        {
            try
            {
                _hashSetKey.Add(key);
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
                        .AppendLine($"**BĐSD** TK: {bankAccountName} ({bankNumber}) **{(money > 0 ? "+" : "")}{Helpers.FormatMoney(money)}** {currencyName} lúc {timeAt}")
                        .AppendLine($"```{message}");
            if (duSo >= 0)
                sb.AppendLine($"\nDư sổ(A): {Helpers.FormatMoney(duSo)} {(duSo == duTheoMessage ? "" : "KHÁC")} dư theo BĐSD(B): {Helpers.FormatMoney(duTheoMessage)} => Chênh lệch(B-A): {Helpers.FormatMoney(duTheoMessage - duSo)}```");
            else
                sb.AppendLine("```");
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
