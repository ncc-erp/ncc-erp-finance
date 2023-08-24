using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.BankTransactions.Dto
{
    public class GetAllPagingBankTransactionDto : GridParam
    {
        public double? Id { get; set; }
        public double? FromMoney { get; set; }
        public double? ToMoney { get; set; }
        public List<long> FromBankAccounts { get; set; }
        public List<long> ToBankAccounts { get; set; }
        public double? Fee { get; set; }
        public List<long> FromCurrencyIds { get; set; }
        public List<long> ToCurrencyIds { get; set; }
        public BankTransactionFilterDateTime FilterDateTime { get; set; }
    }
    public class BankTransactionFilterDateTime : FilterDateTimeParam
    {
        public BankTransactionFilterDateTimeType DateTimeType { get; set; }
    }
    public enum BankTransactionFilterDateTimeType
    {
        NO_FILTER = 0,
        TRANSACTION_TIME = 1,
        CREATE_TIME = 2,
    }
}
