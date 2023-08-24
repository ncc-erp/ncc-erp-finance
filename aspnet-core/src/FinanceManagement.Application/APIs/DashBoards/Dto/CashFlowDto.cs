using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class CashFlowDto
    {
        public string Month { get; set; }
        public double TotalIncomingByMonth { get; set; }
        public Dictionary<string, double> IncomingByMonths { get; set; }
        public double TotalOutcomingByMonth { get; set; }
        public Dictionary<string, double> OutcomingByMonths { get; set; }
    }
}
