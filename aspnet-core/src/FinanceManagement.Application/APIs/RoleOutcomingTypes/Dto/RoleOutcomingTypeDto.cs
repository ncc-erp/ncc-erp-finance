using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.RoleOutcomingTypes.Dto
{
    [AutoMapTo(typeof(UserOutcomingType))]
    public class UserOutcomingTypeDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public long OutcomingEntryTypeId { get; set; }
    }
}
