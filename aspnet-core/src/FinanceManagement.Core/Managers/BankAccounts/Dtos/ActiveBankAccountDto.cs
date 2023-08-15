using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BankAccounts.Dtos
{
    public class ActiveBankAccountDto
    {
        public long BankAccountId { get; set; }
        public double BaseBalance { get; set; }
    }
}
