using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Invoices.Dtos
{
    public class CheckSetDoneInvoiceDto
    {
        private double TotalPaid;
        private double CollectionDebt;
        public double TotalDebt => CollectionDebt - TotalPaid;
        public double MaxITF { get; set; }
        public bool IsAllowedSetDone => TotalDebt <= MaxITF ? true : false;
        public CheckSetDoneInvoiceDto(double? TotalPaid, double CollectionDebt)
        {
            this.TotalPaid = TotalPaid ?? 0;
            this.CollectionDebt = CollectionDebt;   
        }
    }
}
