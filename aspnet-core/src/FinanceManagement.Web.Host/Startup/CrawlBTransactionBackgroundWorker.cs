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
        private static List<string> _dbMezonTransactionHashes;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<CrawlBTransactionMezonDongConfig> _mezonDongOptions;
        public IMySettingManager MySettingManager { get; set; }

        public CrawlBTransactionBackgroundWorker(
            AbpTimer timer,
            IIocResolver iocResolver,
            ILogger<CrawlBTransactionBackgroundWorker> log,
            FirebaseService firebaseService,
            IOptions<FirebaseConfig> options,
            IMezonNotification mezonNotification,
            IOptions<CrawlBTransactionMezonDongConfig> mezonDongOptions,
            IHttpClientFactory httpClientFactory

        ) : base(timer)
        {
            _context = iocResolver.Resolve<FinanceManagementDbContext>();
            _log = log;
            _firebaseService = firebaseService;
            _dbTransactionKeys = new List<string>();
            _firesbaseOptions = options;
            Timer.Period = _firesbaseOptions.Value.IntervalMilisecond;
            _mezonNotification = mezonNotification;
            _mezonDongOptions = mezonDongOptions;
            _dbMezonTransactionHashes = new List<string>();
            _httpClientFactory = httpClientFactory;
        }
        protected override void DoWork()
        {
            _log.LogInformation($"CrawlBTransactionBackgroundWorker.DoWork() start");
            //CrawlBTransaction().Wait();

            if (_mezonDongOptions.Value.EnableCrawlBTransactionMezonDong)
            {
                CrawlBTransactionMezonD().Wait();
            }
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
            var dicMezonTransactions = await GetMezonTransactionsForAllAccounts(dicMezonBankAccounts);
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
                        _log.LogInformation($"Hash: {hash} and Transaction: {JsonConvert.SerializeObject(mezonTx)}");

                        //get datetime of transaction
                        var timeAt = Helpers.ConvertFromUnixTimestamp(mezonTx.Timestamp);

                        //determine wallet address and money direction
                        var walletAddress = mezonTx.WalletAddress;
                        double money = 0;
                        double actualAmount = mezonTx.Amount / 1_000_000.0;

                        if (mezonTx.Sender.Equals(walletAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            money = -Math.Abs(actualAmount); // Tiền ra
                        }
                        else if (mezonTx.Receiver.Equals(walletAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            money = Math.Abs(actualAmount); // Tiền vào
                        }
                        else
                        {
                            _log.LogWarning($"Transaction {hash} does not match wallet {walletAddress}");
                            continue;
                        }
                        //get information bank account -> {Id, CurrencyId, TenantId} and check exists
                        var bankAccount = GetBankAccountByWalletAddress(dicMezonBankAccounts, mezonTx.WalletAddress);
                        if (!bankAccount.IsValid)
                        {
                            _log.LogWarning($"Hash: {hash}, Error: {bankAccount.ErrorMessage}");
                            continue;
                        }

                        var tenantId = bankAccount.Result.TenantId;
                        var bTransaction = new BTransaction
                        {
                            BankAccountId = bankAccount.Result.Id,
                            Money = money,
                            TimeAt = timeAt,
                            Note = BuildMezonTransactionNote(mezonTx),
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
                            _log.LogWarning($"No active period found for tenant {tenantId}");
                            continue;
                        }

                        _context.Add(bTransaction);
                        _context.SaveChanges();

                        double currentBalanceNumber = GetCurrentBalanance(bTransaction.PeriodId, bTransaction.BankAccountId);

                        var config = await MySettingManager.GetEnableCrawlBTransactionNoti(tenantId);
                        if (bool.Parse(config))
                        {
                            string contentNotify = GetContentNotificationMezonDong(
                                hash: mezonTx.Hash,
                                walletAddress: mezonTx.WalletAddress,
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

            _dbMezonTransactionHashes = _context.BTransactions
                .Where(x => !x.IsDeleted)
                .Where(x => x.IsCrawl)
                .Where(x => x.Note != null && x.Note.StartsWith("MezonĐ|Hash:"))
                .Select(x => x.Note)
                .ToList()
                .Select(note => ExtractHashFromNote(note))
                .Where(hash => !string.IsNullOrEmpty(hash))
                .ToList();

            _log.LogInformation($"Initialized {_dbMezonTransactionHashes.Count} Mezon transaction hashes");
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
        private async Task<Dictionary<string, MezonTransactionData>> GetMezonTransactionsForAllAccounts(
            Dictionary<string, MezonBankAccountCrawl> dicWalletAccounts)
        {
            var result = new Dictionary<string, MezonTransactionData>();
            var client = _httpClientFactory.CreateClient();

            foreach (var kvp in dicWalletAccounts)
            {
                var walletAddress = kvp.Key;
                var bankAccount = kvp.Value;

                try
                {
                    var url = $"{_mezonDongOptions.Value.BaseAddress}/indexer-api/1337/transactions" +
                              $"?page=0&limit=50&sort_by=transaction_timestamp&sort_order=desc" +
                              $"&wallet_address={walletAddress}";

                    _log.LogInformation($"Calling Mezon API for {bankAccount.BankAccountName}: {url}");

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _log.LogError($"API call failed for {walletAddress} with status: {response.StatusCode}");
                        continue;
                    }

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<MezonDongTransactionResponse>(jsonContent);

                    if (apiResponse?.Data != null && apiResponse.Data.Any())
                    {
                        foreach (var tx in apiResponse.Data)
                        {
                            if (!result.ContainsKey(tx.Hash))
                            {
                                result[tx.Hash] = new MezonTransactionData
                                {
                                    Hash = tx.Hash,
                                    Sender = tx.Sender,
                                    Receiver = tx.Receiver,
                                    Amount = tx.Amount,
                                    Timestamp = tx.Timestamp,
                                    BlockNumber = tx.BlockNumber,
                                    WalletAddress = walletAddress
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error calling API for {walletAddress}: {ex.Message}", ex);
                }
            }

            _log.LogInformation($"Retrieved {result.Count} total Mezon transactions");
            return result;
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
        private string ExtractHashFromNote(string note)
        {
            if (string.IsNullOrEmpty(note)) return null;

            // Note format: "MezonĐ|Hash: {hash} | From: ..."
            var parts = note.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return null;

            var hashPart = parts[1].Trim();
            if (hashPart.StartsWith("Hash:"))
            {
                return hashPart.Substring(5).Trim();
            }

            return null;
        }

        private string BuildMezonTransactionNote(MezonTransactionData tx)
        {
            return $"MezonĐ|Hash: {tx.Hash} | From: {tx.Sender} | To: {tx.Receiver} | Amount: {tx.Amount} | Block: {tx.BlockNumber}";
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
        public class MezonTransactionData
        {
            public string Hash { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public double Amount { get; set; }
            public long Timestamp { get; set; }
            public long BlockNumber { get; set; }
            public string WalletAddress { get; set; }
        }
        public class MezonDongTransactionResponse
        {
            [JsonProperty("data")]
            public List<MezonDongTransaction> Data { get; set; }
        }

        public class MezonDongTransaction
        {
            [JsonProperty("hash")]
            public string Hash { get; set; }

            [JsonProperty("from_address")]
            public string Sender { get; set; }

            [JsonProperty("to_address")]
            public string Receiver { get; set; }

            [JsonProperty("value")]
            public string AmountStr { get; set; }  // API trả về string
            [JsonIgnore]
            public double Amount => double.Parse(AmountStr);

            [JsonProperty("transaction_timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("block_number")]
            public long BlockNumber { get; set; }
        }
    }
}
