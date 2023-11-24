using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Enums
{
    public enum ExpenseType : long
    {
        REAL_EXPENSE = 0,
        NON_EXPENSE = 1
    }

    public enum StatusFilter : long
    {
        ALL = 0,
        ACTIVE = 1,
        INACTIVE = 2
    }

    public enum RevenueExpenseType : long
    {
        ALL_REVENUE_EXPENSE = 0,
        REAL_REVENUE_EXPENSE = 1,
        NON_REVENUE_EXPENSE = 2
    }
}
