using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.TransitionPermissions.Dto
{
    public class GetRoleTransitionDto : EntityDto<long>
    {
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public long RoleId { get; set; }
    }
}
