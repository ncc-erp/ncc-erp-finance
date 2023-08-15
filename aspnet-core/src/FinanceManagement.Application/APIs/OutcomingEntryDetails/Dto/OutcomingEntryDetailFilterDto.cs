using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails.Dto
{
    public class OutcomingEntryDetailFilterDto
    {
        public long OutcomingEntryId { get; set; }
        public long? BranchId { get; set; }
        public bool? IsNotDone { get; set; }
        public GridParam param { get; set; }

    }
}
