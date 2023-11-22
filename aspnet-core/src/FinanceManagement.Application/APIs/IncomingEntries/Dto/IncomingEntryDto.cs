using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntries.Dto
{
    [AutoMapTo(typeof(IncomingEntry))]
    public class IncomingEntryDto: EntityDto<long>, IGeneralInfoAudited
    {
        public long IncomingEntryTypeId { get; set; }
        public string IncomingEntryTypeName { get; set; }
        public long BankTransactionId { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        public bool Status { get; set; }
        public long AccountId { get; set; } // thuoc ve cong ty
        [ApplySearchAttribute]
        public string AccountName { get; set; }
        public long? BranchId { get; set; }
        public long? CurrencyId { get; set; }
        public string ClientName { get; set; }
        public long ClientAccountId{ get; set; }
        [ApplySearchAttribute]
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Date { get; set; }
        [ApplySearchAttribute]
        public string BranchName { get; set; }
        public double Value { get; set; }
        public double ValueToVND { get; set; }
        public string UpdatedBy => LastModifiedUserId.HasValue ? LastModifiedUser : CreationUser;
        public DateTime UpdatedTime => LastModifiedTime.HasValue ? LastModifiedTime.Value : CreationTime;
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public long? LastModifiedUserId { get; set; }
        public string LastModifiedUser { get; set; }
        public bool RevenueCounted { get; set; }
    }
}
