using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Roles.Dto
{
    public class GetUserNotInRoleDto
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string EmailAddress { get; set; }
    }
    public class GetUserInRoleDto: GetUserNotInRoleDto
    {
       public long Id { get; set; }
    }

    public class AddUserIntoRole
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class DeleteUserFromRole : AddUserIntoRole
    {

    }

}
