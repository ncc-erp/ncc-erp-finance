using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class PercentEntryTypeDto
    {
        public Dictionary<string, double> IncomingEntry { get; set; }
        public double TotalIncomingEntry { get; set; }
        public Dictionary<string, double> OutcomingEntry { get; set; }
        public double TotalOutcomingEntry { get; set; }
    }
}
