using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.BankAccounts.Dto
{
    public class BankAccountStatementDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double BeginningBalance { get; set; }
        public List<GetBankTransaction> BankTransaction { get; set; }
    }

    public class GetBankTransaction
    {
        public long Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BankTransactionName { get; set; }
        public double Increase { get; set; }
        public double Reduce { get; set; }
        public string FromBankAccountInfo { get; set; }
        public string ToBankAccountInfo { get; set; }
    }
}
