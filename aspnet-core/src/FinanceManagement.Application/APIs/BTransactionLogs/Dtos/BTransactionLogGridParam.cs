using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.BTransactionLogs.Dtos
{
    public class BTransactionLogGridParam : GridParam
    {
        public FilterDateTimeParam FilterDateTimeParam { get; set; }
    }
}
