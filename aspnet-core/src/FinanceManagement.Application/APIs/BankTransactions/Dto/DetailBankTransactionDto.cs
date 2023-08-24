using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.BankTransactions.Dto
{
    public class DetailBankTransactionDto: EntityDto<long>, IBTransactionInfo, IGeneralInfoAudited
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        public long? FromBankAccountId { get; set; }
        [ApplySearchAttribute]
        public string FromBankAccountName { get; set; }
        public string FromBankAccountCurrency { get; set; }
        public long? FromBankAccountCurrencyId { get; set; }
        public string FromBankAccountTypeCode { get; set; }
        public string FromBankAccountNumber { get; set; }
        public AccountTypeEnum FromBankAccountTypeEnum { get; set; }
        public long? ToBankAccountId { get; set; }
        [ApplySearchAttribute]
        public string ToBankAccountName { get; set; }
        public string ToBankAccountCurrency { get; set; }
        public long? ToBankAccountCurrencyId { get; set; }
        public string ToBankAccountTypeCode { get; set; }
        public string ToBankAccountNumber { get; set; }
        public AccountTypeEnum ToBankAccountTypeEnum { get; set; }
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public double Fee { get; set; }
        public DateTime TransactionDate { get; set; }
        [ApplySearchAttribute]
        public string Note { get; set; }
        public long NumberOfIncomingEntries { get; set; }
        public DateTime CreateDate { get; set; }
        public bool LockedStatus { get; set; }
        public bool IsWarning => BTransactionId.HasValue ? false : true;
        public long? BTransactionId { get; set; }
        public DateTime? BTransactionTimeAt { get; set; }
        public string BTransactionMoney => BTransactionMoneyNumber.HasValue ? (BTransactionCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(BTransactionMoneyNumber.Value) : Helpers.FormatMoney(BTransactionMoneyNumber.Value)) : String.Empty;
        public double? BTransactionMoneyNumber { get; set; }
        public string BTransactionBankNumber { get; set; }
        public long? BTransactionCurrencyId { get; set; }
        public string BTransactionCurrencyName { get; set; }
        public string UpdatedBy => LastModifiedUserId.HasValue ? LastModifiedUser : CreationUser;
        public DateTime UpdatedTime => LastModifiedTime.HasValue ? LastModifiedTime.Value : CreationTime;
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public long? LastModifiedUserId { get; set; }
        public string LastModifiedUser { get; set; }
    }
}
