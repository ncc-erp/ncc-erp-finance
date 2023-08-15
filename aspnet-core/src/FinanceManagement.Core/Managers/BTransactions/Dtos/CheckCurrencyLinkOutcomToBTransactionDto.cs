using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class CheckCurrencyLinkOutcomToBTransactionDto
    {
        public bool IsDifferent { get; set; }
        public string CurrencyCode { get; set; }
    }
}
