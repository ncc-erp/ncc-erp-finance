using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class BTransactionLoggerDto
    {
        public string ErrorMessage { get; set; }
        public bool IsValid { get; set; }
        public string Status => IsValid ? "Success" : "Fail";
    }
}
