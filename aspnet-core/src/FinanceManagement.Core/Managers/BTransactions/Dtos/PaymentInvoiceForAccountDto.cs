using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class PaymentInvoiceForAccountDto
    {
        public long BTransactionId { get; set; }
        public long AccountId { get; set; }
        public long? IncomingEntryTypeId { get; set; }
        public double? IncomingEntryValue { get; set; }
        public string IncomingEntryName { get; set; }
        public bool IsCreateBonus { get; set; }
        public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; } = new List<CurrencyNeedConvertDto>();
    }
}
