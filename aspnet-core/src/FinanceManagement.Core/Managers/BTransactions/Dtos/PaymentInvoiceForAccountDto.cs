using System;
using System.Collections.Generic;
using System.Linq;
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
    public double Value { get; set; }
    }

    public class PaymentInvoiceMappingDto
    {
    public long BTransactionId { get; set; }
    public long AccountId { get; set; }
    public bool IsCreateBonus { get; set; }
    public long? IncomingEntryTypeId { get; set; }
    public string IncomingEntryName { get; set; }
        /// <summary>
        /// Khách hàng Bonus
        /// </summary>
    public double IncomingEntryValue { get; set; } // optional nếu là bonus
        /// <summary>
        /// Khách hàng trả trước
        /// </summary>
    public double CustomerAdvanceValue { get; set; } // khách trả trước

    public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; } = new List<CurrencyNeedConvertDto>();
    public List<InvoicePaymentMappingDto> InvoiceMappings { get; set; } = new List<InvoicePaymentMappingDto>();
    public double TotalMappingValue => InvoiceMappings.Sum(x => x.Value) + CustomerAdvanceValue + IncomingEntryValue;
    }
}
