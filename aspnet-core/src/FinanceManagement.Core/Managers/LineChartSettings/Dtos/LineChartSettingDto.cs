using Abp.Application.Services.Dto;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.LineChartSettings.Dtos
{
    public class LineChartSettingDto : EntityDto<long>
    {
        public string Name { get; set; }
        public LineChartSettingType Type { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; }
        public List<ReferenceInfoDto> ListReference { get; set; }
        public string LineChartSettingTypeName => Type == LineChartSettingType.Income ? "Loại thu" : "Loại chi";
    }
    public class ReferenceInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
