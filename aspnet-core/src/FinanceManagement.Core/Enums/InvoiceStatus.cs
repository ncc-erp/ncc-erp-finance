using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Enums
{
    public enum InvoiceStatus : byte
    {
        New = 0,
        Sent = 1,
        PartialPayment = 2,
        Paid = 3,
        CantPay = 4
    }
}
