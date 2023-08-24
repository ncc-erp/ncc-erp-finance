using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class OverviewBTransactionStatisticDto
    {
        public int Quantity { get; set; }
        public BTransactionStatus Status { get; set; }
        public string StatusName => Helpers.GetEnumName(Status);
    }
}
