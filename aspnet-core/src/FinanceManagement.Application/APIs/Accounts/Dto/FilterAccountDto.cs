using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Accounts.Dto
{
    public class FilterAccountDto : GridParam
    {
        public bool? BankAccountStatus { get; set; }
    }
}
