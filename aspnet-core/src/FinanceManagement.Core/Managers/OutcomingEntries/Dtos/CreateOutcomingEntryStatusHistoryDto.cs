using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    [AutoMapTo(typeof(OutcomingEntryStatusHistory))]
    public class CreateOutcomingEntryStatusHistoryDto
    {
        public long OutcomingEntryId { get; set; }
        public long WorkflowStatusId { get; set; }
        public double Value { get; set; }
        public string CurrencyName { get; set; }
    }
}
