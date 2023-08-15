using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class InputContentSendNotifyKomu
    {
        public string TitleLink { get; set; }
        public string OutcomingEntryName { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public string BranchName { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreationTime { get; set; }
    }
    public class InputSubContentSendNotifyKomu
    {
        public long OutcomingEntryId { get; set; }
        public string StatusCode { get; set; }
        public string Verifier { get; set; }
        public double Money { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class InputContentSalarySendNotifyKomu 
    {
        public long OutcomingEntryId { get; set; }
        public string StatusCode { get; set; }
        public string Money { get; set; }
        public string OutcomingEntryName { get; set; }
    }
}
