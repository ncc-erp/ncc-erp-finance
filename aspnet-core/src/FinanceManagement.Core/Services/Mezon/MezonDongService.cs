using Abp.Runtime.Session;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.MultiTenancy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public class MezonDongService : BaseWebService
    {
        private readonly IOptions<CrawlBTransactionMezonDongConfig> _options;
        private readonly ILogger<MezonDongService> _log;

        public MezonDongService(
            HttpClient httpClient,
            IOptions<CrawlBTransactionMezonDongConfig> options,
            TenantManager tenantManage,
            IAbpSession session,
            ILogger<MezonDongService> log) : base(httpClient, tenantManage, session)
        {
            _options = options;
            _log = log;
        }

        public async Task<MezonDongTransactionResponse> GetTransactionsByWallet(
            string walletAddress,
            int page = 0,
            int limit = 50)
        {
            string url = $"/indexer-api/1337/transactions" +
                         $"?page={page}&limit={limit}&sort_by=transaction_timestamp&sort_order=desc" +
                         $"&wallet_address={walletAddress}";

            _log.LogInformation($"Calling Mezon API for wallet {walletAddress}: {url}");

            return await this.GetAsync<MezonDongTransactionResponse>(url);
        }

        public async Task<Dictionary<string, string>> GetMezonTransactions(
            Dictionary<string, MezonBankAccountCrawl> dicWalletAccounts,
            List<string> existingHashes = null,
            long cutoffTimestamp = 0)
        {
            var result = new Dictionary<string, string>();

            if (dicWalletAccounts == null || !dicWalletAccounts.Any())
            {
                _log.LogWarning("dicWalletAccounts is null or empty");
                return result;
            }

            if (cutoffTimestamp == 0)
            {
                cutoffTimestamp = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds();
            }

            _log.LogInformation($"Crawling transactions with cutoff timestamp: {cutoffTimestamp}");

            foreach (var kvp in dicWalletAccounts)
            {
                var walletAddress = kvp.Key;
                var bankAccount = kvp.Value;

                try
                {
                    _log.LogInformation($"Processing wallet: {bankAccount.BankAccountName} ({walletAddress})");

                    int limit = 1000;

                    var apiResponse = await GetTransactionsByWallet(walletAddress, 0, limit);

                    if (apiResponse?.Data != null && apiResponse.Data.Any())
                    {
                        int newTxCount = 0;
                        int oldTxCount = 0;
                        int existingTxCount = 0;

                        foreach (var tx in apiResponse.Data)
                        {
                            
                            if (tx.Timestamp < cutoffTimestamp)
                            {
                                oldTxCount++;
                                continue;
                            }

                            
                            if (existingHashes != null && existingHashes.Contains(tx.Hash))
                            {
                                existingTxCount++;
                                continue; // BỎ QUA, đã có rồi
                            }

                            
                            if (result.ContainsKey(tx.Hash))
                            {
                                continue;
                            }

                            
                            double actualAmount = tx.Amount / 1_000_000.0;
                            double money = 0;

                                // Determine money direction
                                if (tx.Sender.Equals(walletAddress, StringComparison.OrdinalIgnoreCase))
                                {
                                    money = -Math.Abs(actualAmount); 
                                }
                                else if (tx.Receiver.Equals(walletAddress, StringComparison.OrdinalIgnoreCase))
                                {
                                    money = Math.Abs(actualAmount); 
                                }
                                
                                string senderName = GetBankAccountNameByWallet(dicWalletAccounts, tx.Sender);
                                string receiverName = GetBankAccountNameByWallet(dicWalletAccounts, tx.Receiver);

                                
                                string noteText = !string.IsNullOrEmpty(tx.TextData) ? tx.TextData : "";

                                
                                string message = $"Tài khoản {senderName} đã chuyển đến tài khoản {receiverName} số tiền {actualAmount}";

                                if (!string.IsNullOrEmpty(noteText))
                                {
                                    message += $" với ND: {noteText}";
                                }

                                
                                    message += $"|#Meta:Money={money};Timestamp={tx.Timestamp};Wallet={walletAddress}";

                            result[tx.Hash] = message;
                            newTxCount++;
                        }
                        _log.LogInformation($"Wallet {bankAccount.BankAccountName}: New={newTxCount}, Existing={existingTxCount}, Old={oldTxCount}");
                    }
                    else
                    {
                        _log.LogInformation($"No transactions found for wallet {walletAddress}");
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
        
        private string GetBankAccountNameByWallet(
            Dictionary<string, MezonBankAccountCrawl> dicMezonBankAccounts,
            string walletAddress)
        {
            if (dicMezonBankAccounts.ContainsKey(walletAddress))
            {
                return dicMezonBankAccounts[walletAddress].BankAccountName;
            }

            
            return FormatWalletAddress(walletAddress);
        }

        private string FormatWalletAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length <= 12)
                return address;

            return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 6)}";
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
            public string AmountStr { get; set; }  
            [JsonIgnore]
            public double Amount => double.Parse(AmountStr);

            [JsonProperty("transaction_timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("block_number")]
            public long BlockNumber { get; set; }
            [JsonProperty("text_data")]
            public string TextData { get; set; }  
        }
    }
}
