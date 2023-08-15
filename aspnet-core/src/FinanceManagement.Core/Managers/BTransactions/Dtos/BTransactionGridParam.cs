using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class BTransactionGridParam : GridParam
    {
        public FilterMoneyParam FilterMoneyParam { get; set; }
        public FilterDateTimeParam FilterDateTimeParam { get; set; }
    }
    public class FilterMoneyParam
    {
        public ExpressionEnum Type { get; set; }
        public double? FromValue { get; set; }
        public double? ToValue { get; set; }
    }

    public enum ExpressionEnum
    {
        NO_FILTER = 0,
        LESS_OR_EQUAL = 1,
        LARGER_OR_EQUAL = 2,
        EQUAL = 3,
        FT = 4
    }

    public class FilterDateTimeParam
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
