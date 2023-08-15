using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.WorkFlowStatus.Dto
{
    public class GetWorkflowStatusDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

}
