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
        public long CircleChartId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Color { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public RevenueExpenseType? RevenueExpenseType { get; set; }
        public string ClientIds { get; set; }
        public string InOutcomeTypeIds { get; set; }
        /// <summary>
        /// DeserializeObject from Json string to list<long>
        /// </summary>
        public List<long> ListClientIds => (string.IsNullOrWhiteSpace(ClientIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(ClientIds);
        /// <summary>
        /// DeserializeObject from Json string to list<long>
        /// </summary>
        public List<long> ListInOutcomeTypeIds => (string.IsNullOrWhiteSpace(InOutcomeTypeIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(InOutcomeTypeIds);
        public CircleChartDetailInfoDto CircleChartDetailInfo { get; set;}
    }
}
