using Abp.Application.Services.Dto;
using FinanceManagement.Anotations;
using FinanceManagement.APIs.BankTransactions.Dto;
using FinanceManagement.Enums;
using System.Collections.Generic;

namespace FinanceManagement.APIs.BankAccounts.Dto
{
    public class DetailBankAccountDto : EntityDto<long>
    {
        [ApplySearchAttribute]
        public string HolderName { get; set; }
        [ApplySearchAttribute]
        public string BankNumber { get; set; }
        public long? BankId { get; set; }
        public string BankName { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long AccountTypeId { get; set; }
        public string AccountTypeCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveInPeriod { get; set; }
        public double Amount => BaseBalance + Increase - Reduce;
        public double BaseBalance { get; set; }
        public bool LockedStatus { get; set; }
        public AccountTypeEnum AccountTypeEnum { get; set; }
        public double Increase { get; set; }
        public double Reduce { get; set; }
    }
}
