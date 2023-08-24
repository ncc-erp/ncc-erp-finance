using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.Authorization.Users;
using FinanceManagement.MultiTenancy;
using Castle.Core.Logging;
using System.Threading.Tasks;
using Abp.Extensions;
using System;
using Google.Apis.Auth;
using Newtonsoft.Json;
using FinanceManagement.Configuration;
using System.Linq;
using Abp.UI;

namespace FinanceManagement.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private ILogger Logger { get; set; }
        public LogInManager(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher,
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory)
            : base(
                  userManager,
                  multiTenancyConfig,
                  tenantRepository,
                  unitOfWorkManager,
                  settingManager,
                  userLoginAttemptRepository,
                  userManagementConfig,
                  iocResolver,
                  passwordHasher,
                  roleManager,
                  claimsPrincipalFactory)
        {
            Logger = NullLogger.Instance;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string secretCode = "", string tenancyName = null, bool shouldLockout = true)
        {
            Logger.Info("LoginAsyncNoPass");
            var result = await LoginAsyncInternalNoPass(token, secretCode, tenancyName, shouldLockout);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(string token, string secretCode, string tenancyName, bool shouldLockout)
        {
            Logger.Info("LoginAsyncInternalNoPass");
            if (token.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(token));
            }
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                var emailAddress = payload.Email;
                Logger.Info("Payload: " + JsonConvert.SerializeObject(payload));
                // checking
                var clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.ClientAppId);//get clientAppId from setting
                Logger.Info("ClientAppId: " + clientAppId);
                var correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                var correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                var correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;

                Tenant tenant = null;

                Logger.Info("correctAudience: " + correctAudience + ", correctIssuer: " + correctIssuer + ", correctExpriryTime: " + correctExpriryTime);
                if (correctAudience && correctIssuer && correctExpriryTime)
                {
                    //Get and check tenant
                    using (UnitOfWorkManager.Current.SetTenantId(null))
                    {
                        if (!MultiTenancyConfig.IsEnabled)
                        {
                            tenant = await GetDefaultTenantAsync();
                        }
                        else if (!string.IsNullOrWhiteSpace(tenancyName))
                        {
                            tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                            if (tenant == null)
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                            }

                            if (!tenant.IsActive)
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                            }
                        }
                    }
                    var tenantId = tenant == null ? (int?)null : tenant.Id;
                    using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await UserManager.InitializeOptionsAsync(tenantId);

                        var user = await UserManager.FindByNameOrEmailAsync(tenantId, emailAddress);
                        if (user == null)
                        {
                            throw new UserFriendlyException(string.Format("Login Fail - Account does not exist"));
                        }

                        if (await UserManager.IsLockedOutAsync(user))
                        {
                            return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                        }
                        if (shouldLockout)
                        {
                            if (await TryLockOutAsync(tenantId, user.Id))
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                            }
                        }

                        await UserManager.ResetAccessFailedCountAsync(user);
                        return await CreateLoginResultAsync(user, tenant);
                    }
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }
            catch (InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }
    }
}
