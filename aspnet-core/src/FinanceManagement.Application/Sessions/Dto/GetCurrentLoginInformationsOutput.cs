using System;

namespace FinanceManagement.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public ApplicationInfoDto Application { get; set; }

        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }
        public int? PeriodId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string DefaultCurrencyCode { get; set; }
        public long DefaultCurrencyId { get; set; }
        public bool IsEnableMultiCurrency { get; set; }
    }
}
