using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Commons.Dtos
{
    public class InputFilterEntryTypeDto
    {
        public StatusFilter Status { get; set; }
        public RevenueExpenseType RevenueExpenseType { get; set; }
        public string SearchText { get; set; }
        public bool IsGetAll()
        {
            return (Status == StatusFilter.ALL) && (RevenueExpenseType == RevenueExpenseType.ALL_REVENUE_EXPENSE) && string.IsNullOrEmpty(SearchText);
        }
        public bool IsGetAllNodeUpperAndLower()
        {
            if ((Status == StatusFilter.INACTIVE || RevenueExpenseType == RevenueExpenseType.REAL_REVENUE_EXPENSE) && string.IsNullOrEmpty(SearchText))
            {
                return false;
            }
            return true;
        }

    }
}
