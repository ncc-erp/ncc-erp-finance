using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using FinanceManagement.Configuration;
using FinanceManagement.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Authorization
{
    /// <summary>
    /// NccAuth apply concept authentication request from another tool in ERP System 
    /// - The first will get secretcode from request header and check with secretcode of application from AppsettingProvider
    /// - Next, using Multi-tenancy through tenancy-name 
    /// </summary>
    public class NccAuthAttribute : ActionFilterAttribute
    {
        private ILogger _logger;
        private IAbpSession _session;
        public NccAuthAttribute()
        {
            _logger = NullLogger.Instance;
            _session = NullAbpSession.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var header = context.HttpContext.Request.Headers;
            //get secret code from setting manager
            var _settingManager = context.HttpContext.RequestServices.GetService(typeof(ISettingManager)) as ISettingManager;
            var secretCode = _settingManager.GetSettingValue(AppSettingNames.SecretKey);
            var securityCodeHeader = header["X-Secret-Key"].ToString();
            if (secretCode != securityCodeHeader) 
            {
                throw new UserFriendlyException($"SecretCode does not match! FinfastCode: {secretCode.Substring(secretCode.Length - 3)} != {securityCodeHeader}");
            }
            //convention name for multi-tenancy
            var _tenantManager = context.HttpContext.RequestServices.GetService(typeof(TenantManager)) as TenantManager;
            var tenantNameHader = header["Abp-TenantName"];
            if (string.IsNullOrEmpty(tenantNameHader))
            {
                return;
            }
            var tenant = _tenantManager.FindByTenancyName(tenantNameHader);
            if(tenant == null)
            {
                _logger.Error($"Not found Tenancy Name: {tenantNameHader}");
                return;
            }
            //set session tenant to use through request -> apply concept CurrentUnitOfWork.SetTenantId(_session.TenantId)
            _session.Use(tenant.Id, null);
        }
    }
}
