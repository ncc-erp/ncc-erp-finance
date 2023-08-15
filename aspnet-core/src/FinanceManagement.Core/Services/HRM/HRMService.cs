using Abp.Runtime.Session;
using FinanceManagement.Managers.Dashboards;
using FinanceManagement.MultiTenancy;
using FinanceManagement.Services.ResponseModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.HRM
{
    public class HRMService : BaseWebService
    {
        public HRMService(HttpClient httpClient, TenantManager tenantManager, IAbpSession abpSession) : base(httpClient, tenantManager, abpSession)
        {
        }
        public async Task<DebtStatisticFromHRMDto> GetHRMDebtStatistic()
        {
            return (await GetAsync<AbpResponseResult<DebtStatisticFromHRMDto>>($"api/services/app/Public/GetAllDebtEmployee")).Result;
        }
    }
}
