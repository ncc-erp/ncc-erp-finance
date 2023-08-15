using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.ProjectTools.Dtos
{
    public class CreateInvoiceFromProjectDto
    {
        public string NameInvoice { get; set; }
        public string InvoiceNumber { get; set; }
        public string ClientCode { get; set; }
        public short Month { get; set; }
        public int Year { get; set; }

        /// <summary>
        /// khoản phải thu thực + transfer fee
        /// </summary>
        public double CollectionDebt { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime SendInvoiceDate { get; set; }
        public DateTime Deadline { get; set; }
        public float TransferFee { get; set; }
    }
}
