using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class OverviewOutcomingEntryStatisticDto
    {
        public long StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public int Count { get; set; }
        public int Index { get; set; }
    }
}
