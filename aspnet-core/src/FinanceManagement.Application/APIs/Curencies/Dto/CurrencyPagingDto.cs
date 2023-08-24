using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Curencies.Dto
{
    public class CurrencyPaging1Dto
    {
        public int TotalCount { get; set; }
        public IEnumerable<CurrencyConvertDto> Items { get; set; }
    }

    public class CurrencyPagingDto: Entity<long>
    {
        public string Code { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        public float MaxITF { get; set; }
        /// <summary>
        /// Default Bank gửi khi tạo ghi nhận thu
        /// </summary>
        public long? DefaultBankAccountId { get; set; }
        public string DefaultBankAccountName{ get; set; }
        /// <summary>
        /// Default Bank gửi và Bank nhận khi bán ngoại tệ
        /// </summary>
        public long? DefaultBankAccountIdWhenSell { get; set; }
        public string DefaultBankAccountNameWhenSell { get; set; }
        /// <summary>
        /// default bank nhận khi mua ngoại tệ
        /// </summary>
        public long? DefaultToBankAccountIdWhenBuy { get; set; }
        public string DefaultToBankAccountNameWhenBuy { get; set; }

        /// <summary>
        /// default bank gửi khi mua ngoại tệ
        /// </summary>
        public long? DefaultFromBankAccountIdWhenBuy { get; set; }
        public string DefaultFromBankAccountNameWhenBuy { get; set; }
        public bool IsCurrencyDefault { get; set; }
    }

    public class CurrencyDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class BankAccountDto
    {
        public long? Id { get; set; }
        public string BankName { get; set; }
    }

    [AutoMapTo(typeof(Currency))]
    public class CreateCurrency : EntityDto<long>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public float MaxITF { get; set; }
        public bool IsCurrencyDefault { get; set; }

    }

    [AutoMapTo(typeof(Currency))]
    public class EditCurrency : EntityDto<long>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public float MaxITF { get; set; }
        public long? DefaultBankAccountId { get; set; }
        public long? DefaultBankAccountIdWhenSell { get; set; }
        public long? DefaultToBankAccountIdWhenBuy { get; set; }
        public long? DefaultFromBankAccountIdWhenBuy { get; set; }
        public bool IsCurrencyDefault { get; set; }
    }
    public class CurrencyByIdDto: EditCurrency
    {
    }
}
