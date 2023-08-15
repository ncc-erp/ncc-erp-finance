using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class BalanceBankAccountDto
    {
        public long BankAccountId { get; set; }
        public long? CurrencyId { get; set; }
        public long ExplanationId { get; set; }
        public string CurrencyName { get; set; }
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public string BankAccountExplanation { get; set; }
        //public string FromBankAcountTypeCode { get; set; }
        //public string ToBankAcountTypeCode { get; set; }
        public double CurrentBalance { get; set; }
        public double CurrentBalanceToVND { get; set; }
        public double DifferenceBalance { get; set; }
        public double DifferenceBalanceToVND { get; set; }
        public double IncreaseBalance { get; set; }
        public double IncreaseBalanceToVND { get; set; }
        public double ReducedBalance { get; set; }
        public double ReducedBalanceToVND { get; set; }
        //public double TotalOutcoming { get; set; }
        //public double OutComingFeeByTransaction { get; set; }
    }
}
