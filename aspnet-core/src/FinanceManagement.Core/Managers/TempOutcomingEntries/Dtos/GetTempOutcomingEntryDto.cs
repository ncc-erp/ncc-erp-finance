using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class GetTempOutcomingEntryDto : ILastModifiedTimeAudited
    {
        public long Id { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public string Name { get; set; }
        public string Requester { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
        public long WorkflowStatusId { get; set; }
        public string WorkflowStatusName { get; set; }
        public string WorkflowStatusCode { get; set; }
        public List<ActionDto> Action { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public long? SupplierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ApproveTime { get; set; }
        public DateTime? ExecuteTime { get; set; }
        public OutcomingEntryFileStatus IsAcceptFile { get; set; }
        public string PaymentCode { get; set; }
        public long? CreatorUserId { get; set; }
        public int? RequestInBankTransaction { get; set; }
        public Boolean Accreditation { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public long RootOutcomingEntryId { get; set; }
        public string Reason { get; set; }
    }
    public class RequestChangeOutcomingEntryInfoDto
    {
        public GetOutcomingEntryDto RootOutcomingEntry { get; set; }
        public GetTempOutcomingEntryDto TempOutcomingEntry { get; set; }
    }
}