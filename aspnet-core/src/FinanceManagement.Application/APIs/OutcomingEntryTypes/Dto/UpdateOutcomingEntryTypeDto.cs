using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryTypes.Dto
{
    [AutoMapTo(typeof(OutcomingEntryType))]
    public class UpdateOutcomingEntryTypeDto : EntityDto<long>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public long WorkflowId { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Trường dữ liệu không được bỏ trống.")]
        public ExpenseType? ExpenseType { get; set; }
    }
}

