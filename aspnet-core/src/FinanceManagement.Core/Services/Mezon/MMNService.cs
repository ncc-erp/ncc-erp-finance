using Abp.Runtime.Session;
using FinanceManagement.MultiTenancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public class MMNService : BaseWebService
    {

        public MMNService(
            HttpClient httpClient,
            TenantManager tenantManage,
            IAbpSession session
            ) : base(httpClient, tenantManage, session)
        {
        }

        public async Task<List<MMNTransaction>> GetMMNTransactionByWalletAddress(string walletAddress)
        {
            int limit = CrawlMezonDongConfig.Limit;
            string url = $"/indexer-api/1337/transactions" +
                         $"?page=0&limit={limit}&sort_by=transaction_timestamp&sort_order=desc" +
                         $"&wallet_address={walletAddress}";

            var response = await GetAsync<MMNTransactionResponse>(url);
            if (response == null)
            {
                return new List<MMNTransaction> { };
            }
            return response.Data;
        }

        public async Task<List<MMNTransaction>> GetMMNTransactions(List<string> addresses)
        {
            var hashes = new List<string>();
            var results = new List<MMNTransaction>();

            foreach (var address in addresses)
            {
                var mmnTransactions = await GetMMNTransactionByWalletAddress(address);

                var insertList = mmnTransactions.Where(s => !hashes.Contains(s.Hash)).ToList();

                if (insertList.Any())
                {
                    results.AddRange(insertList);
                    hashes.AddRange(insertList.Select(s => s.Hash));

                }
            }
            return results;
        }

        public class MMNTransactionResponse
        {
            [JsonProperty("data")]
            public List<MMNTransaction> Data { get; set; }
        }

        public class MMNTransaction
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

            public string ToString() => JsonConvert.SerializeObject(this);
            public string BuilKey(bool isSender) => $"{Hash}_{(isSender?this.Sender:this.Receiver)}";
        }
    }
}
