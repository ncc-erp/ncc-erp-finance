using FinanceManagement.Enums;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.RevenueManageds.Dto
{
    public class RevenueGridParams : GridParam
    {
        public bool? IsDoneDebt { get; set; }
        public FilterDateTimeParam FilterDateTimeParam { get; set; }
        public List<long> AccountIds { get; set; } = new List<long>();
    }
}
