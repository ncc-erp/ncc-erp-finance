using FinanceManagement.Enums;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using Newtonsoft.Json;
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
        public long Id { get; set; }
        public string ChartName { get; set; }
        public bool IsIncome { get; set; }
        public List<ResultCircleChartDetailDto> Details { get; set; } = new List<ResultCircleChartDetailDto>();
    }
    public class ResultCircleChartDetailDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
        
    }
}
