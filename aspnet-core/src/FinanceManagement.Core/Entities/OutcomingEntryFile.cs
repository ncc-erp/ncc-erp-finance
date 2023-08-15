using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Entities
{
    public class OutcomingEntryFile : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long OutcomingEntryId { get; set; }
        [ForeignKey(nameof(OutcomingEntryId))]
        public virtual OutcomingEntry OutcomingEntry { get; set; }
        public string FilePath { get; set; }

    }
}
