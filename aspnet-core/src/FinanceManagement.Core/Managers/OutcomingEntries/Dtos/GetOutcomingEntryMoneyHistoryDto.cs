using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class GetOutcomingEntryMoneyHistoryDto : ICustomCreationAudited
    {
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryName { get; set; }
        public string FromValue => Helpers.FormatMoneyVND(FromValueNumber);
        public string ToValue => Helpers.FormatMoneyVND(ToValueNumber);
        public double FromValueNumber { get; set; }
        public double ToValueNumber { get; set; }
        public string Note { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
    }
}
