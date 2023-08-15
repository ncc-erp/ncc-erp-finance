using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.APIs.OutcomingEntryDetails.Dto;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.HRMs.Dto
{
    [AutoMapTo(typeof(OutcomingEntry))]
    public class CreateOutcomingEntryHRMDto : EntityDto<long>
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        public long AccountId { get; set; }
        public long? BranchId { get; set; }
        public long? CurrencyId { get; set; }
        public long? SupplierId { get; set; }
        public List<OutcomingEntryDetailDto> Detail;
    }
}
