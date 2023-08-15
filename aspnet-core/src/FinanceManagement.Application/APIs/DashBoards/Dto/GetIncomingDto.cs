using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class GetIncomingDto
    {
        public long IncomingEntryId { get; set; }
        public long FromBankAccountId { get; set; }
        public double Value { get; set; }
        public double ValueToVND => Value * (ExchangeRate != default ? ExchangeRate : 1);
        public string IncomingEntryTypeName { get; set; }
        public string IncomingEntryTypeCode { get; set; }
        public string PathName { get; set; }
        public long BankTransactionId { get; set; }
        public double ExchangeRate { get; set; }
    }
}
