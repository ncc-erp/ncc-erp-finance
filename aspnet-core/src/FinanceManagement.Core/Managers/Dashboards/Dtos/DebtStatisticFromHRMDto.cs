using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Dashboards
{
    public class DebtStatisticFromHRMDto
    {
        public List<EmployeeDebtDto> ListDebtEmployees { get; set; }
        public double TotalLoan { get; set; }
        public int EmployeeCount { get; set; }
    }
    public class EmployeeDebtDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public double InterestRate { get; set; }
        public string Note { get; set; }
    }
}
