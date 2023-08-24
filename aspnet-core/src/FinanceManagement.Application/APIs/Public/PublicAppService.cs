using Abp.Dependency;
using FinanceManagement.APIs.Public.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;

namespace FinanceManagement.APIs.Public
{
    public class PublicAppService : FinanceManagementAppServiceBase
    {
        protected IHttpContextAccessor _httpContextAccessor { get; set; }

        public PublicAppService(IWorkScope workScope) : base(workScope)
        {
            _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
        }

        [HttpGet]
        //[NccAuth]
        public GetResultConnectDto CheckConnect()
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.SecretKey);
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            var securityCodeHeader = header["X-Secret-Key"];
            var result = new GetResultConnectDto();
            if (secretCode != securityCodeHeader)
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);
                return result;
            }
            result.IsConnected = true;
            result.Message = "Connected";
            return result;
        }
        [HttpGet]
        public async Task UpdateIncomingEntry()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var query = from i in WorkScope.GetAll<IncomingEntry>()
                            join b in WorkScope.GetAll<BTransaction>() on i.BTransactionId equals b.Id
                            select new { i, b.FromAccountId };
                var result = await query.ToListAsync();
                foreach (var item in result)
                {
                    item.i.AccountId = item.FromAccountId;
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
