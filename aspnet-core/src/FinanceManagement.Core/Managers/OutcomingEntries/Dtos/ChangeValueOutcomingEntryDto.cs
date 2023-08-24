using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class ChangeValueOutcomingEntryDto
    {
        public long OutcomingEntryId { get; set; }
        public string Note { get; set; }
        public double NewValue { get; set; }
    }
}
