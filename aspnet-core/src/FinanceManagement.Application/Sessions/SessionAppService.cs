using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Sessions.Dto;

namespace FinanceManagement.Sessions
{
    public class SessionAppService : FinanceManagementAppServiceBase, ISessionAppService
    {
        private readonly IPeriodManager _periodManager;
        public SessionAppService(IWorkScope workScope, IPeriodManager periodManager) : base(workScope)
        {
            _periodManager = periodManager;
        }

        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }
            
            var currentPeriod = await _periodManager.GetCurrentPeriod();

            if(currentPeriod != null)
            {
                output.PeriodId = currentPeriod.Id;
                output.PeriodStartDate = currentPeriod.StartDate;
                output.PeriodEndDate = currentPeriod.EndDate;
            }

            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault != null)
            {
                output.DefaultCurrencyCode = currencyDefault.Code;
                output.DefaultCurrencyId = currencyDefault.Id;
            }

            output.IsEnableMultiCurrency = await IsAllowOutcomingEntryByMutipleCurrency();

            return output;
        }
    }
}
