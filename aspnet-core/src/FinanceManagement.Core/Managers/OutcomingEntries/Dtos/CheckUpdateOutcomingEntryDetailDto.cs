using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class ResultCheckUpdateOutcomingEntryDetailDto
    {
        public string Message { get; set; }
        public bool IsUpdate { get; set; }
        public string HistoryNote { get; set; }
    }
}
