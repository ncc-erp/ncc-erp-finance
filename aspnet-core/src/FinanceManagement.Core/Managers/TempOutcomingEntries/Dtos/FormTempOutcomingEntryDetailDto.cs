using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class FormTempOutcomingEntryDetailDto
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public long OutcomingEntryId { get; set; }
        public long? BranchId { get; set; }
        public long RootTempOutcomingEntryId { get; set; }
    }
    [AutoMapTo(typeof(TempOutcomingEntryDetail))]
    public class CreateTempOutcomingEntryDetailDto : FormTempOutcomingEntryDetailDto { }
    [AutoMapTo(typeof(TempOutcomingEntryDetail))]
    public class UpdateTempOutcomingEntryDetailDto : FormTempOutcomingEntryDetailDto { }
}
