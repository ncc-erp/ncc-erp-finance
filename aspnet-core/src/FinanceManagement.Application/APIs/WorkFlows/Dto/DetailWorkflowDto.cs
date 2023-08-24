using Abp.Application.Services.Dto;
using FinanceManagement.APIs.WorkFlowStatus.Dto;
using FinanceManagement.APIs.WorkflowStatusTransitions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkFlows.Dto
{
    public class DetailWorkflowDto : EntityDto<long>
    {
        public string Name { get; set; }
        public List<OutcomingEntryTypeWorkflow> OutcomingEntryTypes { get; set; }
        public List<GetWorkflowStatusTransitionDto> Transitions { get; set; }

        //out
    }
    public class OutcomingEntryTypeWorkflow : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
