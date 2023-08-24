using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class FilterTypeOptions
    {
        public TypeFilterTypeOptions Type { get; set; }
        public bool IsShowAll { get; set; }
        public long? UserId { get; set; }

    }
    public enum TypeFilterTypeOptions
    {
        OUTCOMING_ENTRY_TYPE = 0,
        INCOMING_ENTRY_TYPE = 1,
    }
}
