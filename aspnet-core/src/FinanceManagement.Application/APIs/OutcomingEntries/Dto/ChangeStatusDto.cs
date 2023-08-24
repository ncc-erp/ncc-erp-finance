using FinanceManagement.Managers.OutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class ChangeStatusDto
    {
        public long OutcomingEntryId { get; set; }
        public long StatusTransitionId { get; set; }
    }
    public class CheckChangeStatusDto
    {
        public long OutcomingEntryId { get; set; }
        public long StatusTransitionId { get; set; }
    }
}
