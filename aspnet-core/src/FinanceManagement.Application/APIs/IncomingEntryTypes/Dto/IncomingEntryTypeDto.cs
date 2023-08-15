using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryTypes.Dto
{
    [AutoMapTo(typeof(IncomingEntryType))]
    public class IncomingEntryTypeDto : OutputCategoryEntryType
    {
        [Required]
        public string Code { get; set; }
        public string PathId { get; set; }
        public string PathName { get; set; }
        public long Level { get; set; }
        public bool RevenueCounted { get; set; }
        public bool IsActive { get; set; }
        public bool IsClientPaid { get; set; }
        public bool IsClientPrePaid { get; set; }
    }
}

