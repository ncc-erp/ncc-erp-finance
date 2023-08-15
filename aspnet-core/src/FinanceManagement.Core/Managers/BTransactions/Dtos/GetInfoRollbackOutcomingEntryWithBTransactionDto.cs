using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class GetInfoRollbackOutcomingEntryWithBTransactionDto
    {
        public GetInfoRollbackBTransactionDto BTransactionInfo { get; set; }
        public GetInfoRollbackBankTransactionDto BankTransactionInfo { get; set; }
        public IEnumerable<GetInfoRollbackOutcomingEntryDto> RollbackOutcomingEntryInfos { get; set; }
    }
    public class GetInfoRollbackBTransactionDto
    {
        public long BTransactionId { get; set; }
        public string BankAccountName { get; set; }
        public string BankNumber { get; set; }
        public string Note { get; set; }
        public DateTime TimeAt { get; set; }
        public double Money { get; set; }
        public string MoneyFormat => Helpers.FormatMoney(Money);
        public string CurrencyName { get; set; }
    }
    public class GetInfoRollbackBankTransactionDto
    {
        public long BankTransactionId { get; set; }
        public string BankTransactionName { get; set; }
        public long FromBankAccountId { get; set; }
        public string FromBankAccountName { get; set; }
        public double FromValue { get; set; }
        public string FromValueFormat => Helpers.FormatMoney(FromValue);
        public string FromCurrencyName { get; set; }
        public long ToBankAccountId { get; set; }
        public string ToBankAccountName { get; set; }
        public double ToValue { get; set; }
        public string ToValueFormat => Helpers.FormatMoney(ToValue);
        public string ToCurrencyName { get; set; }
    }
    public class GetInfoRollbackOutcomingEntryDto
    {
        public long OutcomingEntryId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
        public string WorkflowStatus { get; set; }
        public string OutcomingEntryTypeName { get; set; }
        public string WorkflowStatusCode { get; set; }
    }
}
