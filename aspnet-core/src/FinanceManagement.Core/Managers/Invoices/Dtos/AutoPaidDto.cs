using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class AutoPaidDto
    {
        public long AccountId { get; set; }
        public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; }
    }
}
