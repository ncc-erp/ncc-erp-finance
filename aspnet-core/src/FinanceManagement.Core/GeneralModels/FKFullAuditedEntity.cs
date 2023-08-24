using Abp.Domain.Entities.Auditing;
using FinanceManagement.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class FKFullAuditedEntity : FullAuditedEntity<long>
    {
        [ForeignKey(nameof(CreatorUserId))]
        public virtual User CreationUser { get; set; }
        [ForeignKey(nameof(LastModifierUserId))]
        public virtual User LastModifiedUser { get; set; }
    }
}
