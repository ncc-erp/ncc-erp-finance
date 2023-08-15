using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class GetOutcomingDto
    {
        public long OutcomingEntryId { get; set; }
        public long? ParentId { get; set; }
        public double Value { get; set; }
        public double ExchangeRate { get; set; }
        public double ValueToVND => Value * ( ExchangeRate != default ? ExchangeRate : 1 );
        public string OutComingEntryTypeCode { get; set; }
        public string PathName { get; set; }
        public long? TransactionId { get; set; }
        public bool IsActive { get; set; }
    }
}
