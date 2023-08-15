using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class OverviewInvoiceStatisticDto
    {
        public int QuantityInvoiceDebt { get; set; }
        public List<InvoiceCurrencyStatisticDto> InvoiceCurrencies { get; set; }
    }
    public class InvoiceCurrencyStatisticDto
    {
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
    }
    public class InvoiceCalculationDebt
    {
        public long CurrencyId { get; set; }
        public double CollectionDebt { get; set; }
        public double Paid { get; set; }
        public double Debt => CollectionDebt - Paid;
        public NInvoiceStatus Status { get; set; }
    }
}
