using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class OutcomingEntryInfoDto
    {
        public long Id { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        public string WorkflowStatusName { get; set; }
        public string WorkflowStatusCode { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
    }
}
