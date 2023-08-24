using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntrySuppliers.Dto
{
    [AutoMapTo(typeof(OutcomingEntrySupplier))]
    public class OutcomingEntrySupplierDto : EntityDto<long>
    {
        public long OutcomingEntryId { get; set; }
        public long SupplierId { get; set; }
    }
}
