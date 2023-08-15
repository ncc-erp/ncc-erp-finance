using FinanceManagement.Enums;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.BankAccounts.Dto
{
    public class BankAccountGridParam : GridParam
    {
        public long? Id { get; set; }
        public string BankNumber { get; set; }
        public List<long> BankIds { get; set; }
        public List<long> CurrencyIds { get; set; }
        public List<long> AccountTypeIds { get; set; }
        public List<long> AccountIds { get; set; }
        public bool? IsActive { get; set; }
        public AccountTypeEnum? AccountTypeEnum { get; set; }
        public double? Amount { get; set; }
    }
}
