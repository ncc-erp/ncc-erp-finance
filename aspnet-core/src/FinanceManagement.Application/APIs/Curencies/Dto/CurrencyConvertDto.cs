using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.Curencies.Dto
{
    [AutoMapTo(typeof(Currency))]
    public class CurrencyConvertDto : EntityDto<long>
    {
        [Required]
        [ApplySearchAttribute]
        public string Name { get; set; }
        [Required]
        [ApplySearchAttribute]
        public string Code { get; set; }
        [Required]
        public double Value { get; set; }
        public float MaxITF { get; set; }
        public long? DefaultBankAccountId { get; set; }
        public long? DefaultBankAccountIdWhenSell { get; set; }
        public long? DefaultToBankAccountIdWhenBuy { get; set; }
        public long? DefaultFromBankAccountIdWhenBuy { get; set; }

    }
}
