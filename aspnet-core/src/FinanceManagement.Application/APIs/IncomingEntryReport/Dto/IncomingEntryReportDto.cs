using FinanceManagement.APIs.IncomingEntries.Dto;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryReport.Dto
{
    public class IncomingEntryReportDto
    {
        public GridResult<IncomingEntryDto> ResultPagging { get; set; }
    }
}
