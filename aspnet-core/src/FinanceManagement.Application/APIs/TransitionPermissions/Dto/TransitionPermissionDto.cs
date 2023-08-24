using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.TransitionPermissions.Dto
{
    [AutoMapTo(typeof(WorkflowStatusTransitionPermission))]
    public class TransitionPermissionDto : EntityDto<long>
    {
        public long TransitionId { get; set; }
        public long RoleId { get; set; }

    }
}
