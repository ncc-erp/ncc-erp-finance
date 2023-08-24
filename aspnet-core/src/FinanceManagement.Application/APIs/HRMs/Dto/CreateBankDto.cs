using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.HRMs.Dto
{
    [AutoMapTo(typeof(Account))]
    public class CreateAccountAndBankAccountDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public string HolderName { get; set; }
        public string BankNumber { get; set; }
    }
}
