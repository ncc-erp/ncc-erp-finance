using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.APIs.BankTransactions.Dto
{
    [AutoMapTo(typeof(BankTransaction))]
    public class BankTransactionDto: EntityDto<long>
    {
        public string Name { get; set; }
        public long FromBankAccountId { get; set; }
        public string FromBankAccountName { get; set; }
        public long ToBankAccountId { get; set; }
        public string ToBankAccountName { get; set; }
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public double? Fee { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
        public long NumberOfIncomingEntries { get; set; }
        public bool LockedStatus { get; set; }
        public List<InvoiceBankTransactionDto> InvoiceBankTransactions { get; set; }
    }

    public class InvoiceBankTransactionDto : EntityDto<long>
    {
        public long InvoiceId { get; set; }
        public double PaymentAmount { get; set; }
    }
}
