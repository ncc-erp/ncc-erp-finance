using Abp.Runtime.Session;
using FinanceManagement.GeneralModels;
using FinanceManagement.MultiTenancy;
using FinanceManagement.Uitls;
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

        public async Task<T> GetBTransactions<T>()
        {

            string url = $"/messages/.json?auth={_options.Value.SecretKey}&orderBy=%22$key%22&startAt=%22{DateTimeUtils.GetLast30DaysUnixTimeSeconds()}%22";
            return await this.GetAsync<T>(url);
        }
    }
}
