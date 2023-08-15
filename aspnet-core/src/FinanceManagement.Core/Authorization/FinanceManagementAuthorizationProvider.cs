using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using static FinanceManagement.Authorization.GrantPermissionRoles;
using static FinanceManagement.Authorization.Roles.StaticRoleNames;

namespace FinanceManagement.Authorization
{
    public class FinanceManagementAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Name, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FinanceManagementConsts.LocalizationSourceName);
        }
    }
}
