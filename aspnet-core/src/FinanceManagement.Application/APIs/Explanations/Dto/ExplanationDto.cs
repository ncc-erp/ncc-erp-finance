using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Explanations.Dto
{
    [AutoMapTo(typeof(Explanation))]
    public class ExplanationDto : EntityDto<long>
    {
        public string BankAccountExplanation { get; set; }
        public ExplanationType Type { get; set; }
        public long? BankAccountId { get; set; }
        public long ComparativeStatisticId { get; set; }
    }
}
