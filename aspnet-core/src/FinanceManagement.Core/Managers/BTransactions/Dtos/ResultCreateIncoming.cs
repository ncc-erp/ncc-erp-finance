using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ResultCreateIncoming
    {
        public long IncomingEntryId { get; set; }
        public long BankTransactionId { get; set; }
    }
    public class ResultCreateMultiIncoming
    {
        public List<long> IncomingEntryIds { get; set; }
        public long BankTransactionId { get; set; }
    }

}
