using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Entities
{
    public class Currency : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// max internal transfer fee
        /// </summary>
        public float MaxITF { get; set; }
        public long? DefaultBankAccountId { get; set; }
        /// <summary>
        /// Default Bank gửi và Bank nhận khi bán ngoại tệ
        /// </summary>
        public long? DefaultBankAccountIdWhenSell { get; set; }

        /// <summary>
        /// default bank nhận khi mua ngoại tệ
        /// </summary>
        public long? DefaultToBankAccountIdWhenBuy { get; set; }

        /// <summary>
        /// default bank gửi khi mua ngoại tệ
        /// </summary>
        public long? DefaultFromBankAccountIdWhenBuy { get; set; }
        public bool IsCurrencyDefault { get; set; }

        public virtual ICollection<CurrencyConvert> CurrencyConverts { get; set; }
    }
}
