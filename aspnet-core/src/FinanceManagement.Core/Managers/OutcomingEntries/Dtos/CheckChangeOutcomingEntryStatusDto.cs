using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class CheckChangeOutcomingEntryStatusDto
    {
        public bool IsAllowed { get; set; }
        public bool? IsChangeGeneral { get; set; }
        public string Message { get; set; }
    }
}
