using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class UpdateNoteInvoiceDto
    {
        public long Id { get; set; }
        public string Note { get; set; }
    }
}
