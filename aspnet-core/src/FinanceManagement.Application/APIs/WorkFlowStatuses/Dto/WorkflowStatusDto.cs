using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkFlowStatus.Dto
{
    [AutoMapTo(typeof(WorkflowStatus))]
    public class WorkflowStatusDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
