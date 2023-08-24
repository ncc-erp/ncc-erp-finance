using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class IncomingEntry : FullAuditedEntity<long>, IMayHaveTenant, IMustHavePeriod
    {
        public int? TenantId { get; set; }
        public int PeriodId { get; set; }
        public long IncomingEntryTypeId { get; set; }
        [ForeignKey(nameof(IncomingEntryTypeId))]
        public virtual IncomingEntryType IncomingEntryType { get; set; }
        public long? BankTransactionId { get; set; }
        [ForeignKey(nameof(BankTransactionId))]
        public virtual BankTransaction BankTransaction { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public long? AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
        public long? BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
        public double Value { get; set; }
        public long? CurrencyId { get; set; }
        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }

        #region new version
        public long? ToCurrencyId { get; set; }
        [ForeignKey(nameof(ToCurrencyId))]
        public virtual Currency ToCurrency { get; set; }
        public double? ExchangeRate { get; set; }

        public long? BTransactionId { get; set; }
        [ForeignKey(nameof(BTransactionId))]
        public virtual BTransaction BTransactions { get; set; }
        public long? InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice Invoices{ get; set; }

        public virtual ICollection<RelationInOutEntry> RelationInOutEntries { get; set; }
        #endregion
    }
}
