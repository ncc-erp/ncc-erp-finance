using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntryTypes.Dto
{
    public class InputFilterIncomingEntryTypeDto
    {
        public bool? IsActive { get; set; }
        public bool? RevenueCounted { get; set; }
        public string SearchText { get; set; }
        public bool IsGetAll()
        {
            return !IsActive.HasValue && !RevenueCounted.HasValue && string.IsNullOrEmpty(SearchText);
        }
        public bool IsGetAllNodeUpperAndLower()
        {
            if(((IsActive.HasValue && !IsActive.Value) || (RevenueCounted.HasValue && RevenueCounted.Value)) && string.IsNullOrEmpty(SearchText)) 
            {
                return false;
            }
            return true;
        }
    }
}
