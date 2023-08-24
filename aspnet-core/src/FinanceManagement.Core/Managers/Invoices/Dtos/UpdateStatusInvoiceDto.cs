using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class UpdateStatusInvoiceDto
    {
        public long Id { get; set; }
        public NInvoiceStatus Status { get; set; }
    }
}
