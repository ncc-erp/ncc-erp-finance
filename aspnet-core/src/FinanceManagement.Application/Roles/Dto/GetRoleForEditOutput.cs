using System.Collections.Generic;
using static FinanceManagement.Authorization.GrantPermissionRoles;

namespace FinanceManagement.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<SystemPermission> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}