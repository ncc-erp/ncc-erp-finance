using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class GetTotalValueOutComingEntryDto
    {
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double TotalValue { get; set; }
        public double TotalValueToCurrencyDefault => TotalValue;
        public double TotalValueToVNDRound => Helpers.RoundMoneyToEven(TotalValueToCurrencyDefault);
    }
}
