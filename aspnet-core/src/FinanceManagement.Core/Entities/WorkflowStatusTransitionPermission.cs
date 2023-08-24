using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class WorkflowStatusTransitionPermission : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long TransitionId { get; set; }
        [ForeignKey(nameof(TransitionId))]
        public virtual WorkflowStatusTransition WorkflowStatusTransition { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
