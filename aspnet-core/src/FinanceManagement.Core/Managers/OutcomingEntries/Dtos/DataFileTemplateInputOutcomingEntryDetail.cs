using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class DataFileTemplateInputOutcomingEntryDetail
    {
        public string Name { get; set; }
        public string BranchName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
    }
}
