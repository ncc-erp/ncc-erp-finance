using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.CircleCharts.Dtos
{
    [AutoMapTo(typeof(CircleChart))]
    public class UpdateCircleChartDto : CircleChartDto
    {
    }
}
