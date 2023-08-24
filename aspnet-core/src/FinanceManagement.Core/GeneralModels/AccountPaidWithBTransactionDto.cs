using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class AccountPaidWithBTransactionDto
    {
        public long BTransactionId { get; set; }
        public long AccountId { get; set; }
        public long IncomingEntryTypeId { get; set; }
    }
}
