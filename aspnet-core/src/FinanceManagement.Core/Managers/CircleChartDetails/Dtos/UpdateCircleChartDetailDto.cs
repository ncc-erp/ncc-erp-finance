using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using System.Text;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;

namespace FinanceManagement.Managers.CircleChartDetails.Dtos
{
    [AutoMapTo(typeof(CircleChartDetail))]
    public class UpdateCircleChartDetailDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long? BranchId { get; set; }
        public string Color { get; set; }
        public List<long> ClientIds { get; set; }
        public RevenueExpenseType? RevenueExpenseType { get; set; }
    }

    [AutoMapTo(typeof(CircleChartDetail))]
    public class UpdateCircleChartInOutcomeTypeIdsDto : EntityDto<long>
    {
        public List<long> InOutcomeTypeIds { get; set; }
    }
}
