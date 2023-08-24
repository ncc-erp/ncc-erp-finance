using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class GetWorkflowChangeStatusInfoDto
    {
        public string ToStatusCode { get; set; }
        public long ToTransitionStatusId { get; set; }

        public string FromStatusCode { get; set; }
        public string ToTransitionName { get; set; }


    }
}
