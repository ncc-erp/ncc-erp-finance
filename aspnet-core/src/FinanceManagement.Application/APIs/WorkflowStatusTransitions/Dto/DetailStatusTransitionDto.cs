using Abp.Application.Services.Dto;
using FinanceManagement.APIs.TransitionPermissions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkflowStatusTransitions.Dto
{
    public class DetailStatusTransitionDto : EntityDto<long>
    {
        public string FromSatatusName { get; set; }
        public string FromSatatusCode { get; set; }
        public string ToSatatusName { get; set; }
        public string ToSatatusCode { get; set; }
        public List<GetRoleTransitionDto> Permission { get; set; }
    }
}
