using FinanceManagement.Managers.CircleChartDetails.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.CircleCharts.Dtos
{
    public class CircleChartInfoDto : CircleChartDto
    {
        public List<CircleChartDetailInfoDto> Details { get; set; }
        /// <summary>
        /// list of all Id from detail
        /// </summary>
        [JsonIgnore]
        public List<long> AllClientIds => this.Details.SelectMany(s => s.ListClientIds).Distinct().ToList();
        /// <summary>
        /// list of all Id from detail
        /// </summary>
        [JsonIgnore]
        public List<long> AllInOutcomeTypeIds => this.Details.SelectMany(s => s.ListInOutcomeTypeIds).Distinct().ToList();
    }
}
