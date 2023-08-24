using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class ChartDto
    {
        public string Month { get; set; }
        public double totalOutomingSalary { get; set; }
        public double totalIncomingClient { get; set; }
    }
}
