using Abp.Authorization;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.Authorization.Users;

namespace FinanceManagement.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
