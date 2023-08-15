using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    [AutoMapTo(typeof(OutcomingEntry))]
    public class OutcomingEntryDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        public long AccountId { get; set; }
        public long? BranchId { get; set; }
        public long? CurrencyId { get; set; }
        public long? SupplierId { get; set; }
        public Boolean Accreditation { get; set; }
        public OutcomingEntryFileStatus isAcceptFile { get; set; }
        public string PaymentCode { get; set; }
    }
}
