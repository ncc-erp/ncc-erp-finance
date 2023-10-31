using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class PieChartDto
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
    public class ResultCircleChartDto
    {
        public string ChartName { get; set; }
        public List<ResultCircleChartDetailDto> Details { get; set; } = new List<ResultCircleChartDetailDto>();
    }
    public class ResultCircleChartDetailDto
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
    }
}
