using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BankTransactions.Dtos
{
    public class ExportDataStatisticFileDto
    {
        public long BankTransactionId { get; set; }
        public string BankTransactionName { get; set; }
        public string FromBankAccountName { get; set; }
        public string ToBankAccountName { get; set; }
        public double FromValue { get; set; }
        public string FromCurrency { get; set; }
        public double ToValue { get; set; }
        public string ToCurrency { get; set; }
        public double Fee { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
        public IEnumerable<ExportDataStatisticOutcomingEntryDto> OutcomingEntries { get; set; }
        public IEnumerable<ExportDataStatisticIncomingEntryDto> IncomingEntries { get; set; }

    }
    public class ExportDataStatisticOutcomingEntryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string CurrencyName { get; set; }
        public string Status { get; set; }
        public string OutcomingEntryType { get; set; }
        public string IsChiPhi => !ExpenseType.HasValue || (ExpenseType.HasValue && ExpenseType.Value == Enums.ExpenseType.NON_EXPENSE) ? "FALSE" : "TRUE";
        public DateTime? ReportDate { get; set; }
        public DateTime CreateTime { get; set; }
        public ExpenseType? ExpenseType { get; set; }
    }
    public class ExportDataStatisticIncomingEntryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string CurrencyName { get; set; }
        public string IncomingEntryType { get; set; }
        public bool RevenueCounted { get; set; }
        public string IsTinhDoanhThu => RevenueCounted ? "TRUE" : "FALSE";
        public bool HoanTien { get; set; }
        public string IsHoanTien => HoanTien ? "TRUE" : "FALSE";
        public string ClientName { get; set; }
    }
}
