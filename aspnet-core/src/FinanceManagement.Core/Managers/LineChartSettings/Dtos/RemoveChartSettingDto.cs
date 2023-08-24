using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.LineChartSettings.Dtos
{
    public class RemoveChartSettingDto
    {
        public long ChartSettingId { get; set; }
        public long ReferenceId { get; set; }
    }
}
