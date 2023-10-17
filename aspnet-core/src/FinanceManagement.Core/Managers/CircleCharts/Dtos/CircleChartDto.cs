using Abp.Application.Services.Dto;
using FinanceManagement.Enums;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.CircleCharts.Dtos
{
    public class CircleChartDto : EntityDto<long>
    {
        public string Name { get; set; }
        public bool IsIncome { get; set; }
        public bool IsActive { get; set; }
        public string CircleChartSettingTypeName => IsIncome ? "Loại thu" : "Loại chi";
    }
}
