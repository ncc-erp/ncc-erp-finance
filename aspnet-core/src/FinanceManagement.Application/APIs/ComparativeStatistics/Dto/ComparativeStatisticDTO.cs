using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.ComparativeStatistics.Dto
{
    [AutoMapTo(typeof(ComparativeStatistic))]
    public class ComparativeStatisticDTO : EntityDto<long>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DifferentExplanation { get; set; }
    }
}
