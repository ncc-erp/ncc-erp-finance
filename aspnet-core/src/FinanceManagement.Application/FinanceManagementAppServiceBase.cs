using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using FinanceManagement.Authorization.Users;
using FinanceManagement.MultiTenancy;
using FinanceManagement.IoC;
using Abp.Dependency;
using Abp.ObjectMapping;
using FinanceManagement.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Managers.Settings;

namespace FinanceManagement
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class FinanceManagementAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }
        protected IWorkScope WorkScope { get; set; }
        public IMySettingManager MySettingManager { get; set; }

        protected FinanceManagementAppServiceBase(IWorkScope workScope)
        {
            LocalizationSourceName = FinanceManagementConsts.LocalizationSourceName;
            WorkScope = workScope;
        }
        protected async Task<bool> IsAllowOutcomingEntryByMutipleCurrency()
        {
            var config = await MySettingManager.GetApplyToMultiCurrencyOutcome();
            if (bool.TryParse(config, out var result))
            {
                return result;
            }
            return false;
        }
        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        protected virtual async Task<Currency> GetCurrencyDefaultAsync()
        {
            return await WorkScope.GetAll<Currency>()
                .Where(x => x.IsCurrencyDefault)
                .FirstOrDefaultAsync();
        }
    }
}
