using Abp.AutoMapper;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.LineChartSettings.Dtos
{
    [AutoMapTo(typeof(Entities.NewEntities.LineChart))]
    public class CreateLineChartSettingDto
    {
        public string Name { get; set; }
        public Enums.LineChartSettingType Type { get; set; }
        public string Color { get; set; }
    }
}
