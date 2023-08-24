using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.RelationInOutEntrys.Dto
{
    public class InComingEntryByOutComingDto : EntityDto<long>
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public double Value { get; set; }
        public string IncomingEntryTypeName { get; set; }
        public bool RevenueCounted { get; set; }
        public bool IsRefund { get; set; }
        public long RelationInOutId { get; set; }
    }
}
