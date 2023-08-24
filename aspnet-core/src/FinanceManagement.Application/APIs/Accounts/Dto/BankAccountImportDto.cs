using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Accounts.Dto
{
    public class BankAccountImportDto
    {
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string HolderName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankCode { get; set; }        
    }
}
