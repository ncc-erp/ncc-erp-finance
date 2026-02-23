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
using FinanceManagement.Services.Mezon.Dto;
using FinanceManagement.Enums;
using static FinanceManagement.Enums.TypeLogin;
using System.Net.Mail;
using FinanceManagement.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FinanceManagement.IoC;

namespace FinanceManagement.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
		private ILogger<BaseWebService> Logger;
        private readonly IConfiguration _configuration;
        private readonly IWorkScope _workScope;
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
			IConfiguration configuration,
            IWorkScope workScope,
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
			Logger = IocManager.Instance.Resolve<ILogger<BaseWebService>>();
            _configuration = configuration;
            _workScope = workScope;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string tenancyName = null, bool shouldLockout = true)
        {
			Logger.LogInformation("LoginAsyncNoPass");
            var result = await LoginAsyncInternalNoPass(TypeLoginOauth2.Google,token, tenancyName, shouldLockout,null);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant,User>> LoginAsyncNoPassWithMezon(AuthOauth2Mezon input, string tenancyName = null, bool shouldLockout = true)
		{
			Logger.LogInformation("LoginAsyncNoPassWithMezon");
			var result = await LoginAsyncInternalNoPass(TypeLoginOauth2.Mezon, null, tenancyName, shouldLockout, input);
			var user = result.User;
			SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
			return result;
		}
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(TypeLoginOauth2 type, string token, string tenancyName, bool shouldLockout, AuthOauth2Mezon mezonOauthResult)
        {
			Logger.LogInformation("LoginAsyncInternalNoPass");
           
            try
            {
				var emailAddress = "";
				var clientAppId = "";
				var correctAudience = false;
				var correctIssuer = false;
				var correctExpriryTime = false;
                long userMezonId = -1;
                if (type == TypeLoginOauth2.Google)
                {
					if (token.IsNullOrEmpty())
					{
						throw new ArgumentNullException(nameof(token));
					}
					GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
					emailAddress = payload.Email;
					Logger.LogInformation("Payload: " + JsonConvert.SerializeObject(payload));
					// checking
				    clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.ClientAppId);//get clientAppId from setting
					Logger.LogInformation("ClientAppId: " + clientAppId);
				    correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
					correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
					correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;
				}else if(type == TypeLoginOauth2.Mezon)
                {
					emailAddress = mezonOauthResult.sub;
                    userMezonId = mezonOauthResult.user_id;

                    clientAppId = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
					correctAudience = mezonOauthResult.aud.Any(s => s == clientAppId);
					correctIssuer = mezonOauthResult.iss == "https://oauth2.mezon.ai";
                    correctExpriryTime = mezonOauthResult.auth_time > 0;
                }
               

                Tenant tenant = null;

				Logger.LogInformation("correctAudience: " + correctAudience + ", correctIssuer: " + correctIssuer + ", correctExpriryTime: " + correctExpriryTime);
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

                        var user = type == TypeLoginOauth2.Mezon ? GetUserByMezonUserId(userMezonId) : await UserManager.FindByEmailAsync(emailAddress);

                        if (user == null)
                        {
                            var errorMessage = type == TypeLoginOauth2.Mezon ?
                                $"Login fail. Not found MezonUserId {userMezonId} in Finfast. Please contact IT" :
                                $"Login fail. Not found email {emailAddress} in Finfast. Please contact IT";

                            throw new UserFriendlyException(errorMessage);
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

        private User GetUserByMezonUserId(long mezonUserId)
        {            
            if (mezonUserId <= 0)
            {
                throw new UserFriendlyException("MezonUserId null or empty");
            }
            return _workScope.GetAll<User>()
                .Where(x => x.KomuUserId == mezonUserId)
                .FirstOrDefault();
        }
    }
}
