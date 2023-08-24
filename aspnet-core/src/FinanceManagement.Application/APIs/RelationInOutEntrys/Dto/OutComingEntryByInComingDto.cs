using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.RelationInOutEntrys.Dto
{
    public class OutComingEntryByInComingDto : EntityDto<long>
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
