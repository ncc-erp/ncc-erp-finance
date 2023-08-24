using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryTypes.Dto
{
    [AutoMapTo(typeof(IncomingEntryType))]
    public class UpdateIncomingEntryTypeDto : EntityDto<long>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public bool RevenueCounted { get; set; }
        public bool IsActive { get; set; }
        public bool IsClientPaid { get; set; }
        public bool IsClientPrePaid { get; set; }
    }
}

