using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.Accounts.Dto
{
    [AutoMapTo(typeof(Account))]
    public class AccountDto : EntityDto<long>
    {
        [Required]
        public long AccountTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public bool IsActive { get; set; } = true;
        public bool Default { get; set; }
        public AccountTypeEnum Type { get; set; }
    }
    public class NewAccountDto : AccountDto
    {
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public long? BankId { get; set; }
        public long? CurrencyId { get; set; }
    }
}

