using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    [AutoMapTo(typeof(OutcomingEntryDetail))]
    [AutoMapFrom(typeof(GetTempOutcomingEntryDetailDto))]
    public class GetOutcomingEntryDetailDto : EntityDto<long>
    {
        public int? TenantId { get; set; }
        [ApplySearchAttribute]
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
        public bool IsNotDone { get; set; }
    }
    public class ResultGetOutcomingEntryDetailDto
    {
        public GridResult<GetOutcomingEntryDetailDto> Paging { get; set; }
        public double TotalMoney { get; set; }
    }
}
