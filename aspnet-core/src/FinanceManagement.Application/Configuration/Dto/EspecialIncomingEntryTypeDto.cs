using FinanceManagement.Managers.Settings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Configuration.Dto
{
    public class InputEspecialIncomingEntryTypeDto
    {
        public string DebtIncomingEntryTypeCode{ get; set; }
        public string BalanceIncomingEntryTypeCode { get; set; }
        public string DeviantIncomingEntryTypeCode { get; set; }
    }
    public class OutputEspecialIncomingEntryTypeDto
    {
        public string DebtIncomingEntryTypeCode { get; set; }
        public string DebtIncomingEntryTypeStatus { get; set; }
        public string BalanceIncomingEntryTypeCode { get; set; }
        public string BalanceIncomingEntryTypeStatus { get; set; }
        public string DeviantIncomingEntryTypeCode { get; set; }
        public string DeviantIncomingEntryTypeStatus { get; set; }
        public bool IsAllActive => DebtIncomingEntryTypeStatus == "Active" && BalanceIncomingEntryTypeStatus == "Active" && DeviantIncomingEntryTypeStatus == "Active";
    }
    public class OutcomingSalaryDto
    {
        public string BankAccountId { get; set; }
    }
}
