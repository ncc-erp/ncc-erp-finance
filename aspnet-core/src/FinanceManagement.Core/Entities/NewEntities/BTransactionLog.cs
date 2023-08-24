using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class BTransactionLog : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [MaxLength(3000)]
        public string Message { get; set; }
        public DateTime TimeAt { get; set; }
        [MaxLength(2000)]
        public string ErrorMessage { get; set; }
        public long? BTransactionId { get; set; }
        [MaxLength(200)]
        public string Key { get; set; }
        public bool IsValid { get; set; }
    }
}
