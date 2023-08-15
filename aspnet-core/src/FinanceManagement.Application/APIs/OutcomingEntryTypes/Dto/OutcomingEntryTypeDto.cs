using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryTypes.Dto
{
    [AutoMapTo(typeof(OutcomingEntryType))]
    public class OutcomingEntryTypeDto : OutputCategoryEntryType
    {
        [Required]
        public string Code { get; set; }
        public string PathName { get; set; }
        public long Level { get; set; }
        public long WorkflowId { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Trường dữ liệu không được bỏ trống.")]
        public ExpenseType? ExpenseType { get; set; }
    }
}

