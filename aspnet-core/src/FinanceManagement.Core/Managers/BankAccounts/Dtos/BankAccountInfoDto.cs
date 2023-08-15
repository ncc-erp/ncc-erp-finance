using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BankAccounts.Dtos
{
    public class BankAccountInfoDto
    {
        public long Id { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string HolderName { get; set; }
        public double BaseBalance { get; set; }
        public AccountTypeEnum AccountTypeEnum { get; set; }
    }
}
