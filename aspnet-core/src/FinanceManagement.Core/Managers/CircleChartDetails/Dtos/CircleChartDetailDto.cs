using Abp.Application.Services.Dto;
using FinanceManagement.Enums;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Managers.CircleChartDetails.Dtos
{
    public class CircleChartDetailDto : EntityDto<long>
    {
        public long CircleChartId { get; set; }
        public string Name { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
        public List<ClientDto> Clients { get; set; }
        public List<InOutcomingDto> InOutcomes { get; set; }

    }
    public class ClientDto
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
    }
    public class InOutcomingDto
    {
        public long InOutcomingId { get; set; }
        public string InOutcomingName { get; set; }
    }
}