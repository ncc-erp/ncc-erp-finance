using FinanceManagement.Anotations;
using FinanceManagement.Enums;
using FinanceManagement.Helper;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class AllPropInvoiceAndIncomByAccountDto
    {
        public long AccountId { get; set; }
        [ApplySearch]
        public string AccountName { get; set; }
        public long? IncomingEntryId { get; set; }
        [ApplySearch]
        public string IncomingName { get; set; }
        public double Money { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string TransactionInfo { get; set; }
        public long? InvoiceId { get; set; }
        [ApplySearch]
        public string InvoiceName { get; set; }
        [ApplySearch]
        public string InvoiceNumber { get; set; }
        public double CollectionDebt { get; set; }
        public DateTime? Deadline { get; set; }
        public string Note { get; set; }
        public NInvoiceStatus? Status { get; set; }
        public string StatusName => Status.HasValue ? Helpers.GetEnumName(Status.Value) : null;
        public long InvoiceCurrencyId { get; set; }
        public string InvoiceCurrencyName { get; set; }
        public double ExchangeRate { get; set; }
        public double NTF { get; set; }
        public double ITF { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InvoiceCreationTime { get; set; }
        public string InvoiceCreatedBy { get; set; }
        public double BTransactionMoney { get; set; }
        public long BTransactionId { get; set; }
        public DateTime? BTransactionTimeAt { get; set; }
        public string BTransactionNote { get; set; }
        public string BTransactionAccountName { get; set; }
        public string BTransactionAccountNumber { get; set; }
        public long? BankTransactionId { get; set; }
    }
    public class OverviewInvoiceDto
    {
        public GridResult<InvoiceAndIncomByAccountDto> Pagings { get; set; }
        public List<StatisticInvoiceDto> Statistics { get; set; }
    }
    public class StatisticInvoiceDto
    {
        public string CollectionDebt => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(CollectionDebtNumber) : Helpers.FormatMoney(CollectionDebtNumber);
        public double CollectionDebtNumber { get; set; }
        public string Paid => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(PaidNumber) : Helpers.FormatMoney(PaidNumber);
        public double PaidNumber { get; set; }
        public string Debt => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(DebtNumber) : Helpers.FormatMoney(DebtNumber);
        public double DebtNumber => CollectionDebtNumber - PaidNumber;
        public string CurrencyName { get; set; }
        public long CurrencyId { get; set; }
    }
    public class InvoiceAndIncomByAccountDto
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public bool IsAllowAutoPaid => Invoices.Any(s => s.IsEspecial);
        public IEnumerable<InvoiceByAccountDto> Invoices { get; set; }
        public IEnumerable<MoneyInfoDto> TotalCollectionDebt => Invoices.Where(x => x.InvoiceId.HasValue)
                                                                        .GroupBy(x => new { x.InvoiceCurrencyId, x.InvoiceCurrencyName })
                                                                        .Select(x => new MoneyInfoDto
                                                                        {
                                                                            CurrencyId = x.Key.InvoiceCurrencyId,
                                                                            CurrencyName = x.Key.InvoiceCurrencyName,
                                                                            TotalMoneyNumber = x.Sum(s => s.CollectionDebtNumber)
                                                                        });

        public IEnumerable<MoneyInfoDto> TotalPaid
        {
            get
            {
                var totalPaidHasInvoice = Invoices.Where(s => s.InvoiceId.HasValue)
                                                    .GroupBy(x => new { x.InvoiceCurrencyId, x.InvoiceCurrencyName })
                                                    .Select(x => new MoneyInfoDto
                                                    {
                                                        CurrencyId = x.Key.InvoiceCurrencyId,
                                                        CurrencyName = x.Key.InvoiceCurrencyName,
                                                        TotalMoneyNumber = x.Sum(s => s.PaidValueNumber)
                                                    });
                var listTotalPaid = new List<IncomingByAccountDto>();
                Invoices.Where(s => !s.InvoiceId.HasValue)
                        .Select(s => s.Incomings)
                        .ToList()
                        .ForEach(item => listTotalPaid.AddRange(item));
                var totalPaidHasNotInvoice = listTotalPaid.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                                                            .Select(x => new MoneyInfoDto
                                                            {
                                                                CurrencyId = x.Key.CurrencyId,
                                                                CurrencyName = x.Key.CurrencyName,
                                                                TotalMoneyNumber = x.Sum(s => s.MoneyNumber)
                                                            });
                var result = totalPaidHasInvoice.Concat(totalPaidHasNotInvoice);
                return result.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                            .Select(x => new MoneyInfoDto
                            {
                                CurrencyId = x.Key.CurrencyId,
                                CurrencyName = x.Key.CurrencyName,
                                TotalMoneyNumber = x.Sum(s => s.TotalMoneyNumber)
                            });
            }
        }
        public IEnumerable<MoneyInfoDto> TotalDebt
        {
            get
            {
                var totalDebtHasInvoice = Invoices.Where(s => s.InvoiceId.HasValue)
                                                    .GroupBy(x => new { x.InvoiceCurrencyId, x.InvoiceCurrencyName })
                                                    .Select(x => new MoneyInfoDto
                                                    {
                                                        CurrencyId = x.Key.InvoiceCurrencyId,
                                                        CurrencyName = x.Key.InvoiceCurrencyName,
                                                        TotalMoneyNumber = x.Sum(s => s.DebtOfRevenueNumber)
                                                    });
                var listTotalDebt = new List<IncomingByAccountDto>();
                Invoices.Where(s => !s.InvoiceId.HasValue)
                        .Select(s => s.Incomings)
                        .ToList()
                        .ForEach(item => listTotalDebt.AddRange(item));
                var totalDebtHasNotInvoice = listTotalDebt.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                                                            .Select(x => new MoneyInfoDto
                                                            {
                                                                CurrencyId = x.Key.CurrencyId,
                                                                CurrencyName = x.Key.CurrencyName,
                                                                TotalMoneyNumber = -x.Sum(s => s.MoneyNumber)
                                                            });
                var result = totalDebtHasInvoice.Concat(totalDebtHasNotInvoice);
                return result.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                            .Select(x => new MoneyInfoDto
                            {
                                CurrencyId = x.Key.CurrencyId,
                                CurrencyName = x.Key.CurrencyName,
                                TotalMoneyNumber = x.Sum(s => s.TotalMoneyNumber)
                            });
            }
        }
    }
    public class IncomingByAccountDto
    {
        public long? IncomingEntryId { get; set; }
        public long? RevenueId { get; set; }
        public string IncomingName { get; set; }
        public string Money => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(MoneyNumber) : Helpers.FormatMoney(MoneyNumber);
        public double MoneyNumber { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double ExchangeRate { get; set; }
        public double BTransactionMoney { get; set; }
        public long BTransactionId { get; set; }
        public DateTime? BTransactionTimeAt { get; set; }
        public string BTransactionNote { get; set; }
        public string TransactionInfo { get; set; }
        public double ToMoneyNumber => MoneyNumber * ExchangeRate;
        public string ToMoney => CurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(ToMoneyNumber) : Helpers.FormatMoney(ToMoneyNumber);
        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }
        public bool IsShowBTransactionNote { get; set; }
        public string BTransactionAccountName { get; set; }
        public string BTransactionAccountNumber { get; set; }
        public bool IsReverseExchangeRate => ExchangeRate < 1;
        public string InvoiceCurrencyName { get; set; }
        public string FromCurrencyName => !IsReverseExchangeRate ? CurrencyName : InvoiceCurrencyName;
        public string ToCurrencyName => !IsReverseExchangeRate ? InvoiceCurrencyName : CurrencyName;
        public double ExchangeRateDisplay => !IsReverseExchangeRate ? ExchangeRate : Math.Round(1 / ExchangeRate, 2);
        public long? BankTransactionId { get; set; }
    }
    public class InvoiceByAccountDto
    {
        public string PaidValue => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(PaidValueNumber) : Helpers.FormatMoney(PaidValueNumber);
        public double PaidValueNumber => Incomings.Sum(s => s.MoneyNumber * s.ExchangeRate);
        public string DebtOfRevenue => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(DebtOfRevenueNumber) : Helpers.FormatMoney(DebtOfRevenueNumber);
        public double DebtOfRevenueNumber => CollectionDebtNumber - PaidValueNumber;
        public string InvoiceTotal => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(InvoiceTotalNumber) : Helpers.FormatMoney(InvoiceTotalNumber);
        public double InvoiceTotalNumber => CollectionDebtNumber + NTFNumber + ITFNubmer;
        public IEnumerable<IncomingByAccountDto> Incomings { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceNumber { get; set; }
        public string CollectionDebt => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(CollectionDebtNumber) : Helpers.FormatMoney(CollectionDebtNumber);
        public double CollectionDebtNumber { get; set; }
        public DateTime? Deadline { get; set; }
        public string Note { get; set; }
        public NInvoiceStatus? Status { get; set; }
        public string StatusName => Status.HasValue ? Helpers.ListInvoiceStatuses.Where(x => x.Value == Status.Value.GetHashCode()).Select(s => s.Name).FirstOrDefault() : string.Empty;
        public long InvoiceCurrencyId { get; set; }
        public string InvoiceCurrencyName { get; set; }
        public string NTF => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(NTFNumber) : Helpers.FormatMoney(NTFNumber);
        public double NTFNumber { get; set; }
        public string ITF => InvoiceCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(ITFNubmer) : Helpers.FormatMoney(ITFNubmer);
        public double ITFNubmer { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime InvoiceCreationTime { get; set; }
        public string InvoiceCreatedBy { get; set; }
        public bool IsProjectTool => string.IsNullOrEmpty(InvoiceCreatedBy) ? true : false;
        public bool IsEspecial => !InvoiceId.HasValue ? true : false;
    }
}
