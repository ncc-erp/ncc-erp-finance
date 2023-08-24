using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using FinanceManagement.Enums;
using FinanceManagement.Entities;
using FinanceManagement.Anotations;
using Microsoft.AspNetCore.Http;

namespace FinanceManagement.APIs.RevenueManageds.Dto
{
    [AutoMapTo(typeof(RevenueManaged))]
    public class RevenueManagedDto : EntityDto<long>
    {
        [ApplySearchAttribute]
        public string NameInvoice { get; set; }
        public long? AccountId { get; set; }
        [ApplySearchAttribute]
        public string AccountTypeCode { get; set; }
        [ApplySearchAttribute]
        public string AccountName { get; set; }
        public short Month { get; set; }
        public double CollectionDebt { get; set; }
        public double? DebtReceived { get; set; }
        public long UnitId { get; set; }
        public string CurrencyCode { get; set; }
        public RevenueManagedStatus Status { get; set; }
        public DateTime SendInvoiceDate { get; set; }
        public DateTime Deadline { get; set; }
        public RemindStatus? RemindStatus { get; set; }
        [ApplySearchAttribute]
        public string Note { get; set; }
        public IEnumerable<string> PathFiles { get; set; }

        public double RemainDebt => this.CollectionDebt - (this.DebtReceived.HasValue ? this.DebtReceived.Value : 0);
    }
    public class RevenueManagedFiles
    {
        public long Id { get; set; }
        public List<IFormFile> Files { get; set; } 
        public List<string> FileNames { get; set; }
    }
    
    public class RevenueManagedReadFile
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
    }
}
