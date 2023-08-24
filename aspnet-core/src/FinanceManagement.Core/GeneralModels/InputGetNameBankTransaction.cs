using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class InputGetNameBankTransaction
    {
        public string BankNumber { get; set; }
        public double Money { get; set; }
        public string CurrencyName { get; set; }
        public DateTime TimeAt { get; set; }
    }
}
