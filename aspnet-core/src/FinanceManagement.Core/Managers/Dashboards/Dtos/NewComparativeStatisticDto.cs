using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class ResultNewComparativeStatisticDto
    {
        public List<NewComparativeStatisticDto> Statistics { get; set; }
        public double TotalToVND => Statistics.Sum(x => x.CurrentBalananceNumber * x.ExchangeRate);
        public string TotalToVNDFormat => Helpers.FormatMoney(TotalToVND);
        public object ExchangeRates => Statistics.Select(x => new { x.CurrencyName, x.ExchangeRate, x.ExchangeRateFormat }).Distinct().ToList();
    }
    public class NewComparativeStatisticDto
    {
        public long BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public string BankNumber { get; set; }
        public string CurrencyName { get; set; }
        public double BaseBalanceNumber { get; set; }
        public string BaseBalance => Helpers.FormatMoney(BaseBalanceNumber);
        public double IncreaseNumber { get; set; }
        public string Increase => Helpers.FormatMoney(IncreaseNumber);
        public double ReduceNumber { get; set; }
        public string Reduce => Helpers.FormatMoney(ReduceNumber);
        public double CurrentBalananceNumber => BaseBalanceNumber + IncreaseNumber + ReduceNumber;
        public string CurrentBalanace => Helpers.FormatMoney(CurrentBalananceNumber);
        public bool IsActive { get; set; }
        public double ExchangeRate { get; set; }
        public string ExchangeRateFormat => Helpers.FormatMoney(ExchangeRate);
    }
    public class NewComparativeStatisticsIncomingEntryDto
    {
        public string CurrencyName { get; set; }
        public double TotalIncomingNumber { get; set; }
        public string TotalIncoming => Helpers.FormatMoney(TotalIncomingNumber);
        public double TotalBTransactionNubmer { get; set; }
        public string TotalBTransaction => Helpers.FormatMoney(TotalBTransactionNubmer);
        public double DiffMoneyNumber => TotalIncomingNumber - TotalBTransactionNubmer;
        public string DiffMoney => Helpers.FormatMoney(DiffMoneyNumber);
    }
    public class NewComparativeStatisticsOutcomingEntryDto
    {
        public string CurrencyName { get; set; }
        public double TotalOutcomingNumber { get; set; }
        public string TotalOutcoming => Helpers.FormatMoney(TotalOutcomingNumber);
        public double TotalBankTransactionNumber { get; set; }
        public string TotalBankTransaction => Helpers.FormatMoney(TotalBankTransactionNumber);
        public double TotalRefundNumber { get; set; }
        public string TotalRefund => Helpers.FormatMoney(TotalRefundNumber);
        public double DiffMoneyNumber => TotalOutcomingNumber - TotalBankTransactionNumber + TotalRefundNumber;
        public string DiffMoney => Helpers.FormatMoney(DiffMoneyNumber);
    }
    public class ResultComparativeStatisticsOutBankTransactionDto 
    {
        public List<ComparativeStatisticsOutBankTransactionDto> Result { get; set; }
        public IEnumerable<GetTotalCurrencyDto> TotalBankTransactions => Result
            .SelectMany(x => x.TotalBankTransactions)
            .GroupBy(x => new { x.CurrencyId, x.CurrencyName})
            .Select(x => new GetTotalCurrencyDto { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName, Value = x.Sum(s => s.Value) });
        public IEnumerable<GetTotalCurrencyDto> TotalOutcomingEntries => Result
            .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
            .Select(x => new GetTotalCurrencyDto { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName, Value = x.Sum(s => s.ValueMoney) });
        public IEnumerable<GetTotalCurrencyDto> TotalHoanTien => Result
            .SelectMany(x => x.TotalHoanTien)
            .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
            .Select(x => new GetTotalCurrencyDto { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName, Value = x.Sum(s => s.Value) });
    }
    public class ComparativeStatisticsOutBankTransactionDto
    {
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryName { get; set; }
        public string OutcomingEntryStatus { get; set; }
        public string OutcomingEntryStatusName { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double ValueMoney { get; set; }
        public string Money => Helpers.FormatMoney(ValueMoney);
        public string OutcomingEntryTypeName { get; set; }
        public IEnumerable<GetTotalCurrencyDto> TotalBankTransactions => BankTransactions
            .GroupBy(x => new { x.CurrencyId, x.FromCurrencyName })
            .Select(x => new GetTotalCurrencyDto { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.FromCurrencyName, Value = x.Sum(s => s.FromValueNumber)});
        public IEnumerable<GetTotalCurrencyDto> TotalHoanTien => IncomingInfos
            .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
            .Select(x => new GetTotalCurrencyDto { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName, Value = x.Sum(s => s.Value) });
        public IEnumerable<ComparativeStatisticsOutBankTransactionDetailDto> BankTransactions { get; set; }
        public IEnumerable<ComparativeStatisticsRelationInOutDto> IncomingInfos { get; set; }
    }
    public class ComparativeStatisticsOutBankTransactionDetailDto
    {
        public long? BankTransactionId { get; set; }
        public string BankTransactionName { get; set; }
        public long FromBankAccountId { get; set; }
        public string FromBankAccountName { get; set; }
        public long? CurrencyId { get; set; }
        public string FromCurrencyName { get; set; }
        public double FromValueNumber { get; set; }
        public string FromValue => Helpers.FormatMoney(FromValueNumber);
        public long ToBankAccountId { get; set; }
        public string ToCurrencyName { get; set; }
        public string ToBankAccountName { get; set; }
        public double ToValueNumber { get; set; }
        public string ToValue => Helpers.FormatMoney(ToValueNumber);
        public DateTime TimeAt { get; set; }
        public long? BTransactionId { get; set; }
        public string BTransactionName { get; set; }
        public string CurrencyBTransaction { get; set; }
        public double ValueMoneyBTransaction { get; set; }
        public string MoneyBTransaction => Helpers.FormatMoney(ValueMoneyBTransaction);
    }
    public class ComparativeStatisticsRelationInOutDto
    {
        public long IncomingEntryId { get; set; }
        public string Name { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
        public ComparativeStatisticsOutBankTransactionDetailDto BankTransactionInfo { get; set; }
    }
}
