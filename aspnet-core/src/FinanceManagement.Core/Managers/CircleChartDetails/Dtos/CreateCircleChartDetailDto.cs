using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Managers.CircleChartDetails.Dtos
{
    [AutoMapTo(typeof(Entities.NewEntities.CircleChartDetail))]
    public class CreateCircleChartDetailDto
    {
        public long CircleChartId { get; set; }
        public string Name { get; set; }
        public long? BranchId { get; set; }
        public string Color { get; set; }
    }
}
