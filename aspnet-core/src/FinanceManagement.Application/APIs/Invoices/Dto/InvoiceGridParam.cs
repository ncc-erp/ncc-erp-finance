using FinanceManagement.Enums;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Invoices.Dto
{
    public class InvoiceGridParam : GridParam
    {
        public bool? IsDoneDebt { get; set; }
        public FilterDateTimeParam FilterDateTimeParam { get; set; }
        public List<long> AccountIds { get; set; } = new List<long>();
        public List<NInvoiceStatus?> Statuses { get; set; } = new List<NInvoiceStatus?>();
    }
}
