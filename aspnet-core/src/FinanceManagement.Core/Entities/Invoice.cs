using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using FinanceManagement.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Entities
{
    public class Invoice : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string NameInvoice { get; set; }
        public long AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
        public short Month { get; set; }
        public double CollectionDebt { get; set; }
        public long CurrencyId { get; set; }
        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }
        public NInvoiceStatus Status { get; set; }
        public DateTime Deadline { get; set; }

        public string Note { get; set; }

        #region new version
        [MaxLength(20)]
        public string InvoiceNumber { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// national transfer fee
        /// </summary>
        public double NTF { get; set; }
        /// <summary>
        /// internal transfer fee
        /// </summary>
        public double ITF { get; set; }
        public virtual ICollection<IncomingEntry> IncomingEntries { get; set; }
        #endregion
    }
}
