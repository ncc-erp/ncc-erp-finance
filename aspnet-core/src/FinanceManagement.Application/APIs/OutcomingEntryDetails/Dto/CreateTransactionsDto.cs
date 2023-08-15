using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails.Dto
{
    public class CreateTransactionsDto: EntityDto<long>
    {
        public string Name { get; set; }
        public long FromBankAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
    }
}
