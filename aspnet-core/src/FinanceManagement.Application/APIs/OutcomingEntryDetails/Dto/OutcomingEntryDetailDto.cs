using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails.Dto
{
    [AutoMapTo(typeof(OutcomingEntryDetail))]
    public class OutcomingEntryDetailDto : EntityDto<long> 
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public long? AccountId { get; set; }
        public string UserCode { get; set; }
        public string AccountName { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public long? BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }
}
