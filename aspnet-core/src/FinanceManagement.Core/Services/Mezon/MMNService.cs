using Abp.Dependency;
using Abp.Runtime.Session;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.MultiTenancy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public class MMNService : BaseWebService
    {
        private readonly ILogger<MMNService> _log;

        public MMNService(
            HttpClient httpClient,
            TenantManager tenantManage,
            IAbpSession session,
            ILogger<MMNService> log) : base(httpClient, tenantManage, session)
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

            return await GetAsync<MezonDongTransactionResponse>(url);
        }

        public async Task<Dictionary<string, MezonDongTransaction>> GetMezonTransactions(
            Dictionary<string, MezonBankAccountCrawl> dicWalletAccounts, int limit = 0)
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

                int currentPage = 0;
                bool hasMore = true;
                while (hasMore)
                {
                    var apiResponse = await GetTransactionsByWallet(walletAddress, currentPage, limit == 0 ? 100 : limit);

                    foreach (var tx in apiResponse.Data)
                    {
                        if (!result.ContainsKey(tx.Hash))
                        {
                            result[tx.Hash] = tx;
                        }
                    }

                    // Nếu limit = 0 thì tự crawl sang trang tiếp theo
                    if (limit == 0)
                    {
                        currentPage++;
                        hasMore = apiResponse.Data.Count > 0;
                    }
                    else
                    {
                        // Nếu limit > 0 thì chỉ crawl đúng 1 lần (theo limit)
                        hasMore = false;
                    }
                }

            }
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
            public double Amount
            {
                get
                {
                    if (double.TryParse(AmountStr, out double value))
                    {
                        return value / 1_000_000.0; // xử lý chia 1_000_000 ngay trong model
                    }
                    return 0;
                }
            }

            [JsonProperty("transaction_timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("block_number")]
            public long BlockNumber { get; set; }
            [JsonProperty("text_data")]
            public string TextData { get; set; }  // ← Note field
        }
    }
}
