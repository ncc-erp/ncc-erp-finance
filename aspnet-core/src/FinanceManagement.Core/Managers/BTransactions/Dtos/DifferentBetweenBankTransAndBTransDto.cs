using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class DifferentBetweenBankTransAndBTransDto
    {
        public string BankTransactionValue => BankTransactionValueNumber.HasValue ? (FromCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(BankTransactionValueNumber.Value) : Helpers.FormatMoney(BankTransactionValueNumber.Value)) : string.Empty;
        public string BTransactionValue => BTransactionValueNumber.HasValue ? (FromCurrencyName == FinanceManagementConsts.VND_CURRENCY_NAME ? Helpers.FormatMoneyVND(BTransactionValueNumber.Value) : Helpers.FormatMoney(BTransactionValueNumber.Value)): string.Empty;
        public double? BTransactionValueNumber { get; set; }
        public double? BankTransactionValueNumber { get; set; }
        public string FromCurrencyName { get; set; }
        public string ToCurrencyName { get; set; }
        public DateTime? BankTransactionTimeAt { get; set; }
        public DateTime? BTransactionTimeAt { get; set; }
        public double ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
        public bool IsDifferentValue { get; set; }
        public bool IsDifferentCurrency { get; set; }
    }
}
