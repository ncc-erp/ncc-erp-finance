using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class UpdateRequestBranchDto
    {
        public long RequestId { get; set; }
        public long BranchId { get; set; }
    }
}
