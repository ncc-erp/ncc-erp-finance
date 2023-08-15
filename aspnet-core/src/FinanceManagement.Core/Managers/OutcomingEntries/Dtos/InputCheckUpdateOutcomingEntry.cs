using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class InputCheckUpdateOutcomingEntry
    {
        public long OutcomingEntryId { get; set; }
        public double OldMoney { get; set; }
        public double NewMoney { get; set; }
        public string Note { get; set; }
        public string StatusCode { get; set; }
        public bool HasDetail { get; set; }
        public bool IsFromOutcomingEntry { get; set; }
    }
}
