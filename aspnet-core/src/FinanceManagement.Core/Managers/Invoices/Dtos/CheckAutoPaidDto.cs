using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class CheckAutoPaidDto
    {
        public List<MoneyInfoDto> MoneyInfos { get; set; }
        public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; }
        public bool HasCollectionDebt { get; set; }
    }
}
