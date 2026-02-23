using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Report.Dto
{
    public class ReportOutcomingEntryDto
    {
        public GridResult<GetOutcomingEntryDto> ResultPaging { get; set; }
    }
}
