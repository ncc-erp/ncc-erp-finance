using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryReport.Dto
{
    [AutoMapTo(typeof(IncomingEntryType))]
    public class IncomingEntryTypeReportDto : OutputCategoryEntryType
    {
        [Required]
        public string Code { get; set; }
        public long Level { get; set; }
        public bool RevenueCounted { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<GetTotalIncomingCurrencyDto> TotalCurrencies { get; set; }
    }
}
