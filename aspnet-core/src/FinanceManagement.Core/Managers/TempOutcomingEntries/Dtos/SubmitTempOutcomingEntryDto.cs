using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class SubmitTempOutcomingEntryDto
    {
        public long TempOutcomingEntryId { get; set; }
        public long ToStatusId { get; set; }
    }
}
