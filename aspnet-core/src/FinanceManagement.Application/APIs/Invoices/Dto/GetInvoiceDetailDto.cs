using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Invoices.Dto
{
    public class GetInvoiceDetailDto : EntityDto<long>
    {
        public long InvoiceId { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
    }
}
