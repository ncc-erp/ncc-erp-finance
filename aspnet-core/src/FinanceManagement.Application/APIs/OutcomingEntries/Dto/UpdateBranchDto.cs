using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class UpdateRequestBranchDto
    {
        public long RequestId { get; set; }
        public long BranchId { get; set; }
    }

    [AutoMapTo(typeof(OutcomingEntryDetail))]
    public class UpdateRequestDetailBranchDto
    {
        public long RequestDetailId { get; set; }

        public long BranchId { get; set; }
    }
}
