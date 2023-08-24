using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries
{
    public class CloneOutcomeDto
    {
        public long OutcomeEntryId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public long? CurrencyId { get; set; }
    }
}
