using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkflowStatusTransitions.Dto
{
    public class GetWorkflowStatusTransitionDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long FromStatusId { get; set; }
        public string FromStatusName { get; set; }
        public string FromStatusCode { get; set; }
        public long ToStatusId { get; set; }
        public string ToStatusName { get; set; }
        public string ToStatusCode { get; set; }
        public List<TransitionRoleDto> Roles { get; set; }

    }

    public class TransitionRoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
