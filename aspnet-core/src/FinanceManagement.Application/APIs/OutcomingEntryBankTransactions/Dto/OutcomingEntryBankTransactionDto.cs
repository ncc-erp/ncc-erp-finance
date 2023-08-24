using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryBankTransactions.Dto
{
    [AutoMapTo(typeof(OutcomingEntryBankTransaction))]
    public class OutcomingEntryBankTransactionDto: EntityDto<long>
    {
        public long BankTransactionId { get; set; }
        public long OutcomingEntryId { get; set; }
    }
}
