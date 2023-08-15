using FinanceManagement.Helper;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Periods.Dtos
{
    public class PreviewClosePeriodByBankAccountDto
    {
        public bool IsActive { get; set; }
        public long BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public string BankNumber { get; set; }
        public double? CurrentBalance { get; set; }
        public double? BalanceByBTransaction { get; set; }
        public string DiffMoney => CurrentBalance.HasValue && BalanceByBTransaction.HasValue ? Helpers.FormatMoney(CurrentBalance.Value-BalanceByBTransaction.Value) : string.Empty;
        public bool IsShow => IsActive;
    }
    public class PreviewClosePeriodDto
    {
        public int CountBTransactionDiffDoneStatus { get; set; }
        public int CountOutcomingEntryDiffEndStatus => OutcomingEntryInfos.Count;
        public List<OutcomingEntryInfoDto> OutcomingEntryInfos { get; set; }
        public List<PreviewClosePeriodByBankAccountDto> BankAccountInfos { get; set; }
    }
}
