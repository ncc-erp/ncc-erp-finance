using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    [AutoMapFrom(typeof(Invoice))]
    public class GetInvoiceByIdDto
    {
        public long Id { get; set; }
        public string NameInvoice { get; set; }
        public long? AccountId { get; set; }
        public short Month { get; set; }
        public int Year { get; set; }
        public double CollectionDebt { get; set; }
        public long CurrencyId { get; set; }
        public DateTime Deadline { get; set; }
        public NInvoiceStatus Status { get; set; }
        public string Note { get; set; }
        public double NTF { get; set; }
        public double ITF{ get; set; }
        public double? InvoiceTotal => CollectionDebt + NTF;
    }
}
