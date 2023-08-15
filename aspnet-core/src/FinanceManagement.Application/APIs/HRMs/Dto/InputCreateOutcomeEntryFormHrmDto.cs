using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.HRMs.Dto
{
    public class InputCreateOutcomeEntryFormHrmDto
    {
        public string Name { get; set; }
        public List<InputDetailDto> Details { get; set; }
    }
    public class InputDetailDto
    {
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public string BranchCode { get; set; }
    }
}
