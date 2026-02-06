using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.Report.Dto
{
    [AutoMapTo(typeof(OutcomingEntryType))]
    public class ReportOutcomingEntryTypeDto : OutputCategoryEntryType
    {
        public string Code { get; set; }
        public string PathName { get; set; }
        public long Level { get; set; }
        public long WorkflowId { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Trường dữ liệu không được bỏ trống.")]
        public ExpenseType? ExpenseType { get; set; }
        public IEnumerable<GetTotalCurrencyDto> TotalCurrencies { get; set; }
    }
}
