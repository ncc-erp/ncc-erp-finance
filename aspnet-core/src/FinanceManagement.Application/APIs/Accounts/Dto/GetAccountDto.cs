using Abp.Application.Services.Dto;
using FinanceManagement.Anotations;
using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Accounts.Dto
{
    public class GetAccountDto : EntityDto<long>
    {
        public long AccountTypeId { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountTypeCode { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string Code { get; set; }
        public bool Default { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<GetBankAccountDto> Banks { get; set; }
        public AccountTypeEnum Type { get; set; }
        public string TypeName => Helpers.GetEnumName(Type);
    }
    public class GetBankAccountDto : EntityDto<long>
    {
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public string CurrencyName { get; set; }
        public string BankCode { get; set; }
        public string CurrencyCode { get; set; }
        public bool Status { get; set; }
        public string StatusName => Status ? "Active" : "InActive";

    }
}
