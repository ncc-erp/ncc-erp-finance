using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class SetDoneOutcomingEntryDto
    {
        public long OutcomingEntryId { get; set; }
        public DateTime ExcutedTime { get; set; }
    }
}
