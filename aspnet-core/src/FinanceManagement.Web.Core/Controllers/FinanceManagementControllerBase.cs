using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace FinanceManagement.Controllers
{
    public abstract class FinanceManagementControllerBase: AbpController
    {
        protected FinanceManagementControllerBase()
        {
            LocalizationSourceName = FinanceManagementConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
