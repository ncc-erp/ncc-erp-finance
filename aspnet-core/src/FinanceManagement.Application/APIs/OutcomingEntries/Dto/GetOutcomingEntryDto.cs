using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.GetOutcomingEntries.Dto
{
    public class GetInformationExport : GetOutcomingEntryDto
    {
        public string Receiver { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
