using Abp.Application.Services.Dto;
using FinanceManagement.Enums;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace FinanceManagement.Managers.CircleChartDetails.Dtos
{
    public class CircleChartDetailInfoDto : EntityDto<long>
    {
        public long CircleChartId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public BranchInfoDto Branch { get; set; }
        public List<ClientInfoDto> Clients { get; set; }
        public List<InOutcomeTypeDto> InOutcomeTypes { get; set; }
        [JsonIgnore]
        public string ClientIds { get; set; }
        [JsonIgnore]
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
    }
    public class ClientInfoDto
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
    }
    public class InOutcomeTypeDto
    {
        public long InOutcomeTypeId { get; set; }
        public string InOutcomeTypeName { get; set; }
    }

    public class BranchInfoDto
    {
        public long BranchId { get; set;}
        public string BranchName { get; set; }
    }
}