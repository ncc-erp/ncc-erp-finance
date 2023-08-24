using Abp.Runtime.Session;
using FinanceManagement.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Project
{
    public class ProjectService : BaseWebService
    {
        public ProjectService(HttpClient httpClient, TenantManager tenantManager, IAbpSession abpSession) : base(httpClient, tenantManager, abpSession)
        {
            AddAbpTenantNameHeaders();
        }

        public async Task<object> DownloadFileTimeSheetProject(long fileId)
        {
            return await GetAsync<object>($"api/services/app/TimesheetProject/DownloadFileTimesheetProject?timesheetProjectId={fileId}");
        }
    }
}
