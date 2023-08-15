using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class OutcomingEntryType : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PathName { get; set; }
        public long Level { get; set; }
        public long? ParentId { get; set; }
        public long WorkflowId { get; set; }
        public ExpenseType? ExpenseType { get; set; }
        public bool IsActive { get; set; } 
    }
}
