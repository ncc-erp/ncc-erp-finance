
using Abp.Authorization;
using Abp;
using FinanceManagement.Authorization.Users;
using FinanceManagement.IoC;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.Extension;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.APIs.Auditlog.Dto;
using FinanceManagement.GeneralModels;
using Abp.Auditing;
using FinanceManagement.Authorization;
using System.Reflection;
using Abp.Linq.Extensions;

namespace FinanceManagement.APIs.Auditlog
{
    public class AuditLogAppService : FinanceManagementAppServiceBase
    {
        public AuditLogAppService(IWorkScope workScope) : base(workScope)
        { }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_Auditlog_View)]
        public async Task<GridResult<GetAuditLogDto>> GetAllPagging(AuditLogGridParamDTO input)
        {
            var usersInfo = WorkScope.GetAll<User>().Select(s => new
            {
                s.Id,
                s.EmailAddress
            });
            var query = WorkScope.GetAll<AuditLog>()
                .Select(s => new GetAuditLogDto
                {
                    ExecutionDuration = s.ExecutionDuration,
                    ExecutionTime = s.ExecutionTime,
                    MethodName = s.MethodName,
                    Parameters = s.Parameters,
                    ServiceName = s.ServiceName,
                    UserId = s.UserId.HasValue ? s.UserId.Value : default,
                    UserIdString = s.UserId.ToString(),
                    EmailAddress = usersInfo.Where(x => s.UserId.Value == x.Id).Select(x => x.EmailAddress).FirstOrDefault()
                })
                .Where(s => s.ServiceName != "FinanceManagement.APIs.Auditlog.AuditLogAppService")
                .WhereIf(input.MethodName.HasValue(), s => s.MethodName == input.MethodName)
                .WhereIf(input.ServiceName.HasValue(), s => s.ServiceName == input.ServiceName);

            return await query.GetGridResult(query, input);
        }
        
        [HttpGet]
        public async Task<List<GetAllEmailAddressInAuditLogDto>> GetAllEmailAddressInAuditLog()
        {
            var userIdInAuditLog = await WorkScope.GetAll<AuditLog>().Where(s => s.UserId != null)
                .Select(s => s.UserId).Distinct().ToListAsync();

            var emailAddressByUserId = WorkScope.GetAll<User>().Where(s => userIdInAuditLog.Contains(s.Id)).Select(s => new GetAllEmailAddressInAuditLogDto
            {
                EmailAddress = s.EmailAddress,
                UserId = s.Id
            }).ToListAsync();

            return await emailAddressByUserId;
        }
        [HttpGet]
        [AbpAuthorize]
        public async Task<List<String>> GetAllMethodName()
        {
            var methodNameAuditLog = await WorkScope.GetAll<AuditLog>()
                .Select(s => s.MethodName)
                .Distinct()
                .OrderBy(methodName => methodName)
                .ToListAsync();

            return methodNameAuditLog;
        }
        [HttpGet]
        [AbpAuthorize]
        public async Task<List<String>> GetAllServiceName()
        {
            var serviceNameAuditLog = await WorkScope.GetAll<AuditLog>()
                .Where(s => s.ServiceName != "FinanceManagement.APIs.Auditlog.AuditLogAppService")
                .Select(s => s.ServiceName)
                .Distinct()
                .OrderBy(serviceName => serviceName)
                .ToListAsync();

            return serviceNameAuditLog;
        }
    }
}
