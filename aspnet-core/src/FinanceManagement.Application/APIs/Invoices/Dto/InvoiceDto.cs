using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Invoices.Dto
{
    [AutoMapTo(typeof(Invoice))]
    public class InvoiceDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Project { get; set; }
        public DateTime TimeAt { get; set; }
        public string AccountCode { get; set; }
        public double TotalPrice { get; set; }
        public InvoiceStatus Status { get; set; }
        public InvoiceCreatedBy CreatedBy { get; set; }
        public string Note { get; set; }
    }
}
