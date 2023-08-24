using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.LineChartSettings.Dtos
{
    [AutoMapTo(typeof(LineChart))]
    public class UpdateLineChartSettingDto : LineChartSettingDto
    {
    }
}
