using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    [AutoMapTo(typeof(Invoice))]
    public class CreateInvoiceDto
    {
        public string NameInvoice { get; set; }
        public long AccountId { get; set; }
        public short Month { get; set; }
        public double CollectionDebt { get; set; }
        public long CurrencyId { get; set; }
        public DateTime Deadline { get; set; }
        public string Note { get; set; }
        public int Year { get; set; }
        public double NTF { get; set; }
        public double ITF { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
