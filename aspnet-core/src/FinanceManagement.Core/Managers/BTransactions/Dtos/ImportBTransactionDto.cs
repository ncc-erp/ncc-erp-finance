using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ImportBTransactionDto
    {
        public IFormFile BTransactionFile { get; set; }
        public long BankAccountId { get; set; }
    }
}
