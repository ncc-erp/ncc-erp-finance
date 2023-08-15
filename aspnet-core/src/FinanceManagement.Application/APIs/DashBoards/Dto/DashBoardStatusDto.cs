using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class DashBoardStatusDto
    {
        public long TotalPending { get; set; }
        public long TotalPendingCFO { get; set; }
        public long TotalTransfered { get; set; }
        public long TotalEnd { get; set; }
        public long TotalApproved { get; set; }
        public long TotalRequestChangePending { get; set; }
    }
}
