using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class DeleteOutcomingEntryDetailDto
    {
        public long Id { get; set; }
        public string HistoryNote { get; set; }
    }
}
