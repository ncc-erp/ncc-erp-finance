using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.CircleCharts.Dtos
{
    [AutoMapTo(typeof(Entities.NewEntities.CircleChart))]
    public class CreateCircleChartDto
    {
        public string Name { get; set; }
        public bool IsIncome { get; set; }
    }
}
