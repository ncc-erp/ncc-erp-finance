using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class GetBankTransactionDto
    {
        public long BankTransactionId { get; set; }
        public long FromBankAccountId { get; set; }
        public long ToBankAccountId { get; set; }
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public double? Fee { get; set; }
        public string FromAccountTypeCode { get; set; }
        public string ToAccountTypeCode { get; set; }
        public string Currency { get; set; }
        public string FromCurrency { get; set; }
        public long? BTransactionId { get; set; }

        public Boolean IsExchange { get; set; }
    }
}
