using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class ChangeAllDetailOutcomingEntryDto
    {
        public long OutcomingEntryId { get; set; }
        public string Note { get; set; }
        public List<ChangeDetailOutcomingEntryDto> OutcomingEntryDetails { get; set; }
    }

    [AutoMapTo(typeof(OutcomingEntryDetail))]
    public class ChangeDetailOutcomingEntryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total => UnitPrice * Quantity;
        public long OutcomingEntryId { get; set; }
        public long? BranchId { get; set; }
    }
}
