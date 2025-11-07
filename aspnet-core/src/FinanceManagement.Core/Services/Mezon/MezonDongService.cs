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
        private readonly ILogger<MezonDongService> _log;

        public MezonDongService(
            HttpClient httpClient,
            TenantManager tenantManage,
            IAbpSession session,
            ILogger<MezonDongService> log) : base(httpClient, tenantManage, session)
        {
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

            return await this.GetAsync<MezonDongTransactionResponse>(url);
        }

        public async Task<Dictionary<string, MezonDongTransaction>> GetMezonTransactions(
            Dictionary<string, MezonBankAccountCrawl> dicWalletAccounts)
        {
            var result = new Dictionary<string, MezonDongTransaction>();

            if (dicWalletAccounts == null || !dicWalletAccounts.Any())
            {
                _log.LogWarning("dicWalletAccounts is null or empty");
                return result;
            }

            _log.LogInformation($"Crawling transactions for {dicWalletAccounts.Count} wallet(s)");

            foreach (var kvp in dicWalletAccounts)
            {
                var walletAddress = kvp.Key;
                var bankAccount = kvp.Value;

                var apiResponse = await GetTransactionsByWallet(walletAddress, page: 0, limit: 1000);

                foreach (var tx in apiResponse.Data)
                {
                    if (!result.ContainsKey(tx.Hash))
                    {
                        result[tx.Hash] = tx;
                    }
                }
                    
            }
            _log.LogInformation($"Retrieved {result.Count} total Mezon transactions");
            return result;
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
            [JsonProperty("text_data")]
            public string TextData { get; set; }  // ← Note field
        }
    }
}
