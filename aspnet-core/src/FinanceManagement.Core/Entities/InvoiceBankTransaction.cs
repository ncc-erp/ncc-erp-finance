
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Entities
{
    public class InvoiceBankTransaction : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice Invoice { get; set; }
        public long BankTransactionId { get; set; }
        [ForeignKey(nameof(BankTransactionId))]
        public virtual BankTransaction BankTransaction { get; set; }
        public double PaymentAmount { get; set; }
    }
}
