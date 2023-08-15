using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class UserOutcomingType : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        [ForeignKey(nameof(OutcomingEntryTypeId))]
        public virtual OutcomingEntryType OutcomingEntryType { get; set; }

    }
}
