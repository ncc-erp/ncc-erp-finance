using Abp.Domain.Entities.Auditing;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class GetOutcomingEntryStatusHistoryDto : ICustomCreationAudited
    {
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryName { get; set; }
        public long WorkflowStatusId { get; set; }
        public string WorkflowStatusName { get; set; }
        public string WorkflowStatusCode { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
        public long? TempId { get; set; }
        public double ValueNumber { get; set; }
        public string Value => Helpers.FormatMoneyVND(ValueNumber);
        public string CurrencyName { get; set; }
        public bool IsRoot => !TempId.HasValue ? true : false;
    }
}
