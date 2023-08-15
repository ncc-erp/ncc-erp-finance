using Abp.Runtime.Session;
using FinanceManagement.GeneralModels;
using FinanceManagement.MultiTenancy;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Firebase
{
    public class FirebaseService : BaseWebService
    {
        private readonly IOptions<FirebaseConfig> _options;
        public FirebaseService(HttpClient httpClient, IOptions<FirebaseConfig> options, TenantManager tenantManage, IAbpSession session) : base(httpClient, tenantManage, session) 
        {
            _options = options;
        }

        public async Task<T> GetCrawlTransactions<T>()
        {
            string url = $"/messages/.json?auth={_options.Value.SecretKey}";
            return await this.GetAsync<T>(url);
        }
    }
}
