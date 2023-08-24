using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    [AutoMapTo(typeof(RelationInOutEntry))]
    public class CreateRelationInOutDto
    {
        public long IncomingEntryId { get; set; }
        public long OutcomingEntryId { get; set; }
    }
}
