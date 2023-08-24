using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntries.Dto
{
    public class ClientInfo
    {
        public long ClientAccountId { get; set; }
        public string ClientAccountName { get; set; }
        public string ClientAccountCode { get; set; }
    }
}
