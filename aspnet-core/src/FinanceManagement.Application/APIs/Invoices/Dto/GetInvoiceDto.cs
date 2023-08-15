using Abp.Application.Services.Dto;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Invoices.Dto
{
    public class GetInvoiceDto : EntityDto<long>
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string ClientName { get; set; }
        [ApplySearchAttribute]
        public string Project { get; set; }
        public DateTime TimeAt { get; set; }
        [ApplySearchAttribute]
        public string AccountCode { get; set; }
        public double TotalPrice { get; set; }
        public InvoiceStatus Status { get; set; }
        public InvoiceCreatedBy CreatedBy { get; set; }
        public string Note { get; set; }
    }
}
