using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class ResultGetOutcomingEntryDto
    {
        public GridResult<GetOutcomingEntryDto> ResultPaging { get; set; }
        public IEnumerable<GetTotalCurrencyDto> TotalCurrencies { get; set; }
    }
    [AutoMapTo(typeof(OutcomingEntry))]
    [AutoMapFrom(typeof(GetTempOutcomingEntryDto))]
    public class GetOutcomingEntryDto : IGeneralInfoAudited
    {
        //[ApplySearch]
        public long Id { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public ExpenseType? ExpenseType { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string Requester { get; set; }
        public long? BranchId { get; set; }
        [ApplySearchAttribute]
        public string BranchName { get; set; }
        public long AccountId { get; set; }
        [ApplySearchAttribute]
        public string AccountName { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        [ApplySearchAttribute]
        public string WorkflowStatusName { get; set; }
        public string WorkflowStatusCode { get; set; }
        public List<ActionDto> Action { get; set; }
        [ApplySearchAttribute]
        public string OutcomingEntryTypeCode { get; set; }
        [ApplySearchAttribute]
        public string OutcomingEntryTypeName { get; set; }
        public long? SupplierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ApproveTime { get; set; }
        public DateTime? ExecuteTime { get; set; }
        public OutcomingEntryFileStatus IsAcceptFile { get; set; }
        [ApplySearchAttribute]
        public string PaymentCode { get; set; }
        public long? CreatorUserId { get; set; }
        public int? RequestInBankTransaction { get; set; }
        public Boolean Accreditation { get; set; }
        public string UpdatedBy => LastModifiedUserId.HasValue ? LastModifiedUser : CreationUser;
        public DateTime UpdatedTime => LastModifiedTime.HasValue ? LastModifiedTime.Value : CreationTime;
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public long? LastModifiedUserId { get; set; }
        public string LastModifiedUser { get; set; }
        public DateTime? ReportDate { get; set; }
        public IEnumerable<GetOutcomingEntryStatusHistoryDto> StatusHistories { get; set; }
        public bool IsShowButtonRequestChange { get; set; }
        public bool IsShowButtonViewRequestChange { get; set; }
        public bool IsShowButtonApproveRequestChange { get; set; }
        public bool IsShowButtonRejectRequestChange { get; set; }
        public bool IsShowButtonSendRequestChange { get; set; }
        public long? TempOutcomingEntryId { get; set; }
    }
    public class GetTotalCurrencyDto
    {
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
    }
}
