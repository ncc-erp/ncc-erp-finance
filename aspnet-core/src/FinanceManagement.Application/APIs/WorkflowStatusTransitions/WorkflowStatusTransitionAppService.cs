using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.WorkflowStatusTransitions.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.APIs.TransitionPermissions.Dto;
using FinanceManagement.IoC;

namespace FinanceManagement.APIs.WorkflowStatusTransitions
{
    [AbpAuthorize]
    public class WorkflowStatusTransitionAppService : FinanceManagementAppServiceBase
    {
        public WorkflowStatusTransitionAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_Workflow_WorkflowDetail_Edit)]
        public async Task<WorkflowStatusTransitionDto> Create(WorkflowStatusTransitionDto input)
        {
            if(input.ToStatusId == 0 || input.FromStatusId == 0)
            {
                throw new UserFriendlyException("ToStatus and FromStatus cannot null !");
            }

            var isExist = await WorkScope.GetAll<WorkflowStatusTransition>().AnyAsync(x => (x.FromStatusId == input.FromStatusId && x.ToStatusId == input.ToStatusId && x.WorkflowId == (long)input.workflowId) && x.Id != input.Id);
            if (isExist)
                throw new UserFriendlyException("WorkflowStatusTransition already exist !");


            if (input.ToStatusId == input.FromStatusId)
            {
                throw new UserFriendlyException("ToStatus and FromStatus cannot match !");
            }

            input.Id = await WorkScope.InsertAndGetIdAsync(new WorkflowStatusTransition
            {
                Name = input.Name,
                FromStatusId = input.FromStatusId,
                ToStatusId = input.ToStatusId,
                WorkflowId = input.workflowId
            });
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_Workflow_WorkflowDetail_Edit)]

        public async Task<WorkflowStatusTransitionDto> Update(WorkflowStatusTransitionDto input)
        {
            if (input.ToStatusId == input.FromStatusId)
            {
                throw new UserFriendlyException("ToStatus and FromStatus cannot match !");
            }

            var isExist = await WorkScope.GetAll<WorkflowStatusTransition>().AnyAsync(x => (x.FromStatusId == input.FromStatusId && x.ToStatusId == input.ToStatusId && x.WorkflowId == (long)input.workflowId) && x.Id != input.Id);
            if (isExist)
                throw new UserFriendlyException("WorkflowStatusTransition already exist !");

            var transition = await WorkScope.GetAsync<WorkflowStatusTransition>(input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map<WorkflowStatusTransitionDto, WorkflowStatusTransition>(input, transition));
            return input;
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_Workflow_WorkflowDetail_Edit)]
        public async Task Delete(long id)
        {
            var transaction = await WorkScope.GetAll<WorkflowStatusTransition>()
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (transaction == default)
            {
                throw new UserFriendlyException("WorkflowStatusTransition Id doesn't exist");
            }

            var permissionList = await WorkScope.GetAll<WorkflowStatusTransitionPermission>()
                .Where(s => s.TransitionId == id)
                .ToListAsync();

            foreach (var permission in permissionList)
            {
                permission.IsDeleted = true;
            }

            transaction.IsDeleted = true;

            await CurrentUnitOfWork.SaveChangesAsync();
        }
        [HttpGet]
      //  [AbpAuthorize(PermissionNames.Admin_WorkflowStatusTransition_ViewDetail)]
        public async Task<DetailStatusTransitionDto> GetRoleByTransition(long id)
        {
            var transition = await WorkScope.GetAsync<WorkflowStatusTransition>(id);
            var fromStatus = await WorkScope.GetAsync<WorkflowStatus>(transition.FromStatusId);
            var toStatus = await WorkScope.GetAsync<WorkflowStatus>(transition.ToStatusId);
            var permissions = await WorkScope.GetAll<WorkflowStatusTransitionPermission>().Where(s => s.TransitionId == id)
                .Select(s => new GetRoleTransitionDto
                {
                    Id = s.Id,
                    RoleDisplayName = s.Role.DisplayName,
                    RoleName = s.Role.Name,
                    RoleId = s.RoleId
                }).ToListAsync();
            return new DetailStatusTransitionDto
            {
                Id = transition.Id,
                FromSatatusCode = fromStatus.Code,
                FromSatatusName = fromStatus.Name,
                ToSatatusCode = toStatus.Code,
                ToSatatusName = toStatus.Name,
                Permission = permissions
            };
        }
       

    }
}
