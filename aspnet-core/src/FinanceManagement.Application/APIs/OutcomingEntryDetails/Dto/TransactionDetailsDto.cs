using Abp.Application.Services.Dto;
using FinanceManagement.Anotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails.Dto
{
    public class TransactionDetailsDto: EntityDto<long>
    {
        public long TransactionId { get; set; }
        public long OutcomingEntryId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AccountName { get; set; }
        public string FromBank { get; set; }
        public string FromBankCurrencyCode { get; set; }
        public string ToBank { get; set; }
        public string ToBankCurrencyCode { get; set; }
        public double FromValue { get; set; }
        public double Fee { get; set; }
        public double ToValue { get; set; }
        [ApplySearch]
        public string Name { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string  CreateDate { get; set; }
        public string BTransactionInfo { get; set; }
        public long? BTransactionId { get; set; }
    }
}
