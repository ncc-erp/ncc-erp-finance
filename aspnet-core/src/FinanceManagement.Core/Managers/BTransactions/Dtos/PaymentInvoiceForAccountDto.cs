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
    public class InvoicePaymentMappingDto
    {
    public long InvoiceId { get; set; }
    public decimal Value { get; set; }
    }

    public class PaymentInvoiceMappingDto
    {
    public long BTransactionId { get; set; }
    public long AccountId { get; set; }
    public bool IsCreateBonus { get; set; }
    public decimal? IncomingEntryValue { get; set; } // optional nếu là bonus
    public decimal CustomerAdvanceValue { get; set; } // khách trả trước
    public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; }
    public List<InvoicePaymentMappingDto> InvoiceMappings { get; set; }
    }
}
