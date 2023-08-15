using FinanceManagement.Enums;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class GetAllPagingOutComingEntryDto : GridParam
    {
        public double? Id { get; set; }
        public double? Money { get; set; }
        public List<long> Requesters { get; set; }
        public List<long> Branchs { get; set; }
        public long? OutComingEntryType { get; set; }
        public string OutComingStatusCode { get; set; }
        public string TempStatusCode { get; set; }
        public bool Accreditation { get; set; }
        public long? OutcomingEntryTypeId { get; set; }
        public ExpenseType? ExpenseType { get; set; }
        public OutcomingEntryFilterDateTime FilterDateTimeParam { get; set; }
    }

    public class OutcomingEntryFilterDateTime : FilterDateTimeParam
    {
        public OutcomingEntryFilterDateTimeType DateTimeType { get; set; }
    }
    public enum OutcomingEntryFilterDateTimeType
    {
        NO_FILTER = 0,
        REPORT_DATE = 1,
    }
}
