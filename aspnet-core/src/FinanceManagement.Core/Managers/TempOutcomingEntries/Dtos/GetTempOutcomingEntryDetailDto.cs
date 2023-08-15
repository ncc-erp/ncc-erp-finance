using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FinanceManagement.GeneralModels;

namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class GetTempOutcomingEntryDetailDto : ILastModifiedTimeAudited
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? AccountId { get; set; }
        public string UserCode { get; set; }
        public string AccountName { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public long? BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public long? RootOutcomingEntryDetailId { get; set; }
        public long RootTempOutcomingEntryId { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
    public class RequestChangeOutcomingEntryDetailInfoDto
    {
        public ActionTypeEnum ActionType { get; set; }
        public string ActionTypeName => Helpers.GetEnumName(ActionType);
        public double Total
        {
            get
            {
                if(ActionType != ActionTypeEnum.DELETE)
                    return TempOutcomingEntryDetailDto.Total;
                return 0;
            }
        }
        public GetOutcomingEntryDetailDto OutcomingEntryDetail { get; set; }
        public GetTempOutcomingEntryDetailDto TempOutcomingEntryDetailDto { get; set; }
    }
    public class GetRequestChangeOutcomingEntryDetailDto
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public List<RequestChangeOutcomingEntryDetailInfoDto> RequestChangeDetails { get; set; }
        public double TotalMoneyNumber => RequestChangeDetails.Sum(s => s.Total);
        public string TotalMoney => Helpers.FormatMoneyVND(TotalMoneyNumber);
    }
}
