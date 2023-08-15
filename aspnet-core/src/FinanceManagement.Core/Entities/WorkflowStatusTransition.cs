using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class WorkflowStatusTransition : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public long? WorkflowId { get; set; }
        [ForeignKey(nameof(WorkflowId))]
        public virtual Workflow Workflow { get; set; }
        public virtual ICollection<WorkflowStatusTransitionPermission> WorkflowStatusTransitionPermissions { get; set; }
    }
}
