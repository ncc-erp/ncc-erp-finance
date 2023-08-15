using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.APIs.RelationInOutEntrys.Dto
{
    [AutoMapTo(typeof(RelationInOutEntry))]
    public class RelationInOutEntryDto : EntityDto<long>
    {
        public long OutcomingEntryId { get; set; }
        public long IncomingEntryId { get; set; }
        public bool IsRefund { get; set; }
    }
    public class SetRefundRelationInOutDto : EntityDto<long>
    {
        public bool IsRefund { get; set; }
    }
}
