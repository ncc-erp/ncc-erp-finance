using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryTypes.Dto
{
    public class InputFilterOutcomingEntryTypeDto
    {
        public bool? IsActive { get; set; }
        public ExpenseType? ExpenseType { get; set; }
        public string SearchText { get; set; }
        public bool IsGetAll()
        {
            return !IsActive.HasValue && !ExpenseType.HasValue && string.IsNullOrEmpty(SearchText);
        }
        public bool IsGetAllNodeUpperAndLower()
        {
            if (((IsActive.HasValue && !IsActive.Value) || (ExpenseType.HasValue && ExpenseType.Value == Enums.ExpenseType.REAL_EXPENSE)) && string.IsNullOrEmpty(SearchText))
            {
                return false;
            }
            return true;
        }
    }
}
