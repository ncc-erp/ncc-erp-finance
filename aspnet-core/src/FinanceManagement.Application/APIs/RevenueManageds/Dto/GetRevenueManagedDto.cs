using Abp.Application.Services.Dto;
using FinanceManagement.Anotations;
using FinanceManagement.Enums;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.RevenueManageds.Dto
{
    public class GetRevenueManagedDto
    {
        public GridResult<RevenueManagedDto> RevenueManagedDtos { get; set; }
        public List<RemainingDebt> RemainingDebts { get; set; }
    }
    public class RemainingDebt
    {
        public double CollectionDebt { get; set; }
        public double? DebtReceived { get; set; }
        public string CurrencyCode { get; set; }

        public double RemainDebt => this.CollectionDebt - (this.DebtReceived.HasValue ? this.DebtReceived.Value : 0);
        //public double? Quantity => this.CollectionDebt - this.DebtReceived;
    }
}
