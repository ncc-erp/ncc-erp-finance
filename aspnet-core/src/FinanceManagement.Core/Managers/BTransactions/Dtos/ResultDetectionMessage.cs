using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ResultDetectionMoney
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public double Result { get; set; }
    }
    public class ResultDetectionBankAccount
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; }
    }
    public class ResultConvertTimestamp
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public long Result { get; set; }
    }
    public class ResultCheckBankAccount
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public BankAccountCrawl Result { get; set; }
    }
    public class BankAccountCrawl
    {
        public int? TenantId { get; set; }
        public long Id { get; set; }
        public long? CurrencyId { get; set; }
        public string BankAccountName { get; set; }
        public string CurrencyName { get; set; }
    }
    public class ResultConvertDateTime
    {
        public bool IsValid { get; set; }
        public DateTime? Result { get; set; }
    }
    public class ResultConvertMoney
    {
        public bool IsValid { get; set; }
        public double? Result { get; set; }
    }
}
