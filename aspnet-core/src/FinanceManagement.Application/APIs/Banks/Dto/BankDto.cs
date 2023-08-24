using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.Banks.Dto
{
    [AutoMapTo(typeof(Bank))]
    public class BankDto : EntityDto<long>
    {
        [Required]
        [ApplySearchAttribute]
        public string Name { get; set; }
        [Required]
        [ApplySearchAttribute]
        public string Code { get; set; }
    }
}

