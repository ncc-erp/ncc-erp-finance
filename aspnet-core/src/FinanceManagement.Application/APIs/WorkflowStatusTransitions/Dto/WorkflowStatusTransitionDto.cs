using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkflowStatusTransitions.Dto
{
    [AutoMapTo(typeof(WorkflowStatusTransition))]
    public class WorkflowStatusTransitionDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public long workflowId { get; set; }
        // public int RoleId { get; set; }
    }
}
