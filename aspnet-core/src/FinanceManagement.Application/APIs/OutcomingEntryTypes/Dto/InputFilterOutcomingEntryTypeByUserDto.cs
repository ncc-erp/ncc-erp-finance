using DocumentFormat.OpenXml.Wordprocessing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryTypes.Dto
{
    public class InputFilterOutcomingEntryTypeByUserDto
    {
        public long UserId { get; set; }
        public bool? IsGranted { get; set; }    
        public string SearchText { get; set; }
        public bool IsGetAll()
        {
            return (!IsGranted.HasValue || IsGranted.Value) && string.IsNullOrEmpty(SearchText);
        }
        public bool IsGetAllNodeUpperAndLower()
        {
            if (((IsGranted.HasValue && !IsGranted.Value)) && string.IsNullOrEmpty(SearchText))
            {
                return false;
            }
            return true;
        }
    }
}
