using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class BankAccountOption
    {
        public long BankAccountId { get; set; }
        public string BankAccountHolderName { get; set; }
        public string BankAccountNumber { get; set; }
        public long? BankAccountCurrencyId { get; set; }
        public string BankAccountCurrencyName { get; set; }
        public string BankAccountCurrencyCode { get; set; }
        public string BankAccountTypeCode { get; set; }
        public long? AccountId { get; set; }
        public string AccountName { get; set; }
        public long AccountTypeId { get; set; }
        public string AccountTypeName { get; set; }
        public AccountTypeEnum AccountTypeEnum { get; set; }
        public string AccountTypeEnumName => ((AccountTypeEnum)AccountTypeEnum).ToString();
        public string BankAccountName => BankAccountHolderName;
        public long Value => BankAccountId;
        public string Name => string.Format(FinanceManagementConsts.BANK_ACCOUNT_OPTION_NAME, BankAccountHolderName, BankAccountCurrencyName, BankAccountNumber, AccountTypeEnumName);
    }
    public class FilterBankAccount
    {
        public long? CurrencyId { get; set; }
        public string CurrencyNameOrCode { get; set; }
        public bool? IsActive { get; set; }
        public bool IsAccountTypeNotCompany { get; set; } = true;
        public AccountTypeEnum OrderByType { get; set; } = AccountTypeEnum.COMPANY;

    }
}
