using Abp.Domain.Entities.Auditing;
using DocumentFormat.OpenXml.ExtendedProperties;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class GetInfoRollbackClientPaidDto
    {
        public BTransactionInforDto BTransactionInfor { get; set; }
        public BankTransactionInforDto BankTransactionInfor { get; set; }
        public List<IncomingEntryInforDto> IncomingEntrieInfors { get; set; }
    }

    public class BTransactionInforDto
    {
        public long BTransactionId { get; set; }
        public long BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public string BankNumber { get; set; }
        public string Note { get; set; }
        public DateTime TimeAt { get; set; }
        public double Money { get; set; }
        public string MoneyFormat => Helpers.FormatMoney(Money);
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public BTransactionStatus Status { get; set; }
    }

    public class BankTransactionInforDto
    {
        public long BankTransactionId { get; set; }
        public string BankTransactionName { get; set; }
        public long FromBankAccountId { get; set; }
        public string FromBankAccountName { get; set; }
        public double FromValue { get; set; }
        public string FromValueFormat => Helpers.FormatMoney(FromValue);
        public string FromCurrencyName { get; set; }
        public long? FromCurrencyId { get; set; }
        public long ToBankAccountId { get; set; }
        public string ToBankAccountName { get; set; }
        public double ToValue { get; set; }
        public string ToValueFormat => Helpers.FormatMoney(ToValue);
        public string ToCurrencyName { get; set; }      
        public long? ToCurrencyId { get; set; }
        public double Fee { get; set; }
    }

    public class IncomingEntryInforDto
    {
        public long? IncomingEntryId { get; set; }
        public string IncomingEntryName { get; set; }
        public double Money { get; set; }
        public string MoneyFormat => Helpers.FormatMoney(Money);
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double? ExchangeRate { get; set; }
        public long? BankTransactionId { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceName { get; set;}
        public short InvoiceDateMonth { get; set; }
        public int InvoiceDateYear { get; set; }
        public string InvoiceCurrencyName { get; set; }
        public NInvoiceStatus? InvoiceStatus { get; set; }
        public string InvoiceStatusName => InvoiceStatus.HasValue ? Helpers.ListInvoiceStatuses.Where(x => x.Value == InvoiceStatus.Value.GetHashCode()).Select(s => s.Name).FirstOrDefault() : string.Empty;
    }
}
