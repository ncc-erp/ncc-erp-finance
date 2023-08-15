using Abp.Domain.Entities.Auditing;
using FinanceManagement.Anotations;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class GetAllBTransactionDto : ILastModifiedAudited, ICustomCreationAudited
    {
        public long BTransactionId { get; set; }
        public long BankAccountId { get; set; }
        [ApplySearch]
        public string BankAccountNumber { get; set; }
        [ApplySearch]
        public string BankAccountName { get; set; }
        public string FromAccountName { get; set; }
        public string ToAccountName { get; set; }

        public bool IsShowFromAccountName => MoneyNumber > 0;
        public string StrFromTo => BankTransactionId.HasValue ? (MoneyNumber > 0 ? "From" : "To") : "";
        public string CreatorUserFirstName => IsCrawl ? "Crawl" : CreationUser.Split(' ').FirstOrDefault();
        public string FromToAccountName => IsShowFromAccountName ? FromAccountName : ToAccountName;
        public string Money => (CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(MoneyNumber) : Helpers.FormatMoney(MoneyNumber));
        public double MoneyNumber { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string MoneyCurrency => Money + " " + CurrencyName;
        public DateTime TimeAt { get; set; }
        public BTransactionStatus BTransactionStatus { get; set; }
        public string BTransactionStatusName => Helpers.GetEnumName(BTransactionStatus);
        public bool IsCrawl { get; set; }
        [ApplySearch]
        public string Note { get; set; }
        public long? BankTransactionId { get; set; }
        public string BankTransactionName { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public long? LastModifiedUserId { get; set; }
        public string LastModifiedUser { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
    }
}
