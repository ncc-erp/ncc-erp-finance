using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.APIs.WorkFlows.Dto
{
    [AutoMapTo(typeof(Workflow))]
    public class WorkflowDto : EntityDto<long>
    {
        [Required]
        [ApplySearchAttribute]
        public string Name { get; set; }
    }
}
