using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.Suppliers.Dto
{
    [AutoMapTo(typeof(Supplier))]
    public class SupplierDto : EntityDto<long>
    {
        [Required]
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [ApplySearchAttribute]
        public string ContactPersonName { get; set; }
        [ApplySearchAttribute]
        public string ContactPersonPhone { get; set; }
        [ApplySearchAttribute]
        public string TaxNumber { get; set; }
        public long? OutcomingEntrySupplierId { get; set; }
        public long? OutcomingEntryId { get; set; }
    }
}
