using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class PaymentInvoiceMappingDto
{
    public long BTransactionId { get; set; }

    public long AccountId { get; set; }

    /// <summary>
    /// Có tạo Bonus ghi nhận thu riêng không
    /// </summary>
    public bool IsCreateBonus { get; set; } = false;

    public long? IncomingEntryTypeId { get; set; }

    public string IncomingEntryName { get; set; }

    public double? IncomingEntryValue { get; set; }

    /// <summary>
    /// Các tỷ giá cần quy đổi (nếu giao dịch liên quan nhiều currency)
    /// </summary>
    public List<CurrencyNeedConvertDto> CurrencyNeedConverts { get; set; }

    /// <summary>
    /// Mapping InvoiceId và số tiền người dùng muốn thanh toán
    /// </summary>
    public List<InvoiceMappingDto> InvoiceMappings { get; set; }

    /// <summary>
    /// Số tiền muốn ghi nhận là khách hàng trả trước
    /// </summary>
    public double PrepaidValue { get; set; } = 0;
}

public class InvoiceMappingDto
{
    public long InvoiceId { get; set; }

    public double Value { get; set; }
}
}
