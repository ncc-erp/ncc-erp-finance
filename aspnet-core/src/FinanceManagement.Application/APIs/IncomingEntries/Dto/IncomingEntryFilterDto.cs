using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntries.Dto
{
    public class IncomingEntryFilterDto
    {
        public long bankTransactionId { get; set; }
        public GridParam param { get; set; }
    }
}
