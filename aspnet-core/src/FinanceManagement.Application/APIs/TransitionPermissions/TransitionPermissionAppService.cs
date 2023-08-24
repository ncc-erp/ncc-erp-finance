using Abp.Authorization;
using FinanceManagement.APIs.TransitionPermissions.Dto;
using FinanceManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Authorization;
using FinanceManagement.IoC;

namespace FinanceManagement.APIs.TransitionPermissions
{
    [AbpAuthorize]
    public class TransitionPermissionAppService : FinanceManagementAppServiceBase
    {
        public TransitionPermissionAppService(IWorkScope workScope) : base(workScope)
        {
        }

        //create transition permission, validate transitionID && RoleId in a line
        [HttpPost]
        //[AbpAuthorize(PermissionNames.Admin_TransitionPermission_Create)]
        public async Task<TransitionPermissionDto> Create(TransitionPermissionDto input)
        {
            var isExist = await WorkScope.GetAll<WorkflowStatusTransitionPermission>().AnyAsync(x => x.TransitionId == input.TransitionId && x.RoleId == input.RoleId);
            if (isExist)
                throw new Abp.UI.UserFriendlyException("WorkFlowStatusTransitionPermission already exists");

            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<WorkflowStatusTransitionPermission>(input));
            return input;
        }
        [HttpDelete]
       // [AbpAuthorize(PermissionNames.Admin_TransitionPermission_Delete)]

        public async Task Delete(long id)
        {
            var permission = await WorkScope.GetAll<WorkflowStatusTransitionPermission>().FirstOrDefaultAsync(s => s.Id == id);
            if (permission == null)
            {
                throw new Abp.UI.UserFriendlyException("Transition Permission Id doesn't exist");
            }
            await WorkScope.DeleteAsync<WorkflowStatusTransitionPermission>(id);
        }

    }
}
