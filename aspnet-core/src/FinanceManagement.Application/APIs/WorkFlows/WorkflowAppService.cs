using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.WorkFlows.Dto;
using FinanceManagement.APIs.WorkFlowStatus.Dto;
using FinanceManagement.APIs.WorkflowStatusTransitions.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.WorkFlows
{
    [AbpAuthorize]
    public class WorkflowAppService : FinanceManagementAppServiceBase
    {
        public WorkflowAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_Workflow_Create)]
        public async Task<WorkflowDto> Create(WorkflowDto input)
        {
            //Name of Workflow is unique
            var nameExist = await WorkScope.GetAll<Workflow>().AnyAsync(s => s.Name == input.Name);
            if (nameExist)
            {
                throw new UserFriendlyException("Tên Workflow đã tồn tại");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Workflow>(input));
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_Workflow_WorkflowDetail_Edit)]
        public async Task<WorkflowDto> Update(WorkflowDto input)
        {
            var workflow = await WorkScope.GetAsync<Workflow>(input.Id);
            var nameExist = await WorkScope.GetAll<Workflow>().AnyAsync(s => s.Name == input.Name && workflow.Name != input.Name);
            if (nameExist)
            {
                throw new UserFriendlyException("Tên Workflow đã tồn tại");
            }
            await WorkScope.UpdateAsync(ObjectMapper.Map<WorkflowDto, Workflow>(input, workflow));
            return input;
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Admin_Workflow)]
        public async Task<List<WorkflowDto>> GetAll()
        {
            return await WorkScope.GetAll<Workflow>().Select(s => new WorkflowDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<WorkflowDto>> GetAllDropDown()
        {
            return await WorkScope.GetAll<Workflow>().Select(s => new WorkflowDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToListAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_Workflow)]
        public async Task<GridResult<WorkflowDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<Workflow>()
                .Select(s => new WorkflowDto
                {
                    Id = s.Id,
                    Name = s.Name
                });
            return await query.GetGridResult(query, input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_Workflow_Delete)]
        public async Task Delete(long id)
        {
            //Check workflow in: OutcomingEntryTypes va WorkflowStatus 

            var workflow = await WorkScope.GetAll<Workflow>().FirstOrDefaultAsync(s => s.Id == id);
            if (workflow == null)
                throw new UserFriendlyException($"Không tồn tại WorkflowId: {id}");

            var outcomingEntryType = await WorkScope.GetAll<OutcomingEntryType>().Where(s => s.WorkflowId == id).FirstOrDefaultAsync();
            if (outcomingEntryType != default)
                throw new UserFriendlyException($"Không thể xóa {workflow.Name} vì Workflow đã gắn với loại chi {outcomingEntryType.Name}");

            var workflowStatusTransition = await WorkScope.GetAll<WorkflowStatusTransition>().Where(s => s.WorkflowId == id).ToListAsync();

            await WorkScope.SoftDeleteRangeAsync<WorkflowStatusTransition>(workflowStatusTransition);

            await WorkScope.DeleteAsync<Workflow>(id);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Admin_Workflow_ViewDetail)]
        public async Task<DetailWorkflowDto> Get(long id)
        {
            var hasWorkflow = await WorkScope.GetAll<Workflow>().AnyAsync(s => s.Id == id);
            if (!hasWorkflow)
            {
                throw new UserFriendlyException($"Không tồn tại WorkflowId: {id}");
            }

            var transitions = await WorkScope.GetAll<WorkflowStatus>().ToListAsync();
            var transitionIds = transitions.Select(s => s.Id);
            //join into transition vs role

            var transationsPermissions = (from t in WorkScope.GetAll<WorkflowStatusTransition>().Where(s => s.WorkflowId == id)
                                          join w in WorkScope.GetAll<WorkflowStatusTransitionPermission>()
                                          on t.Id equals w.TransitionId into ww
                                          from w1 in ww.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = t.Id,
                                              Name = t.Name,
                                              FromStatusId = t.FromStatusId,
                                              ToStatusId = t.ToStatusId,
                                              RoleId = w1.RoleId,
                                              RoleName = w1.Role.Name
                                          }).AsEnumerable()
                                         .GroupBy(s => new { s.Id, s.FromStatusId, s.ToStatusId, s.Name })
                                         .Select(s => new GetWorkflowStatusTransitionDto
                                         {
                                             Id = s.Key.Id,
                                             Name = s.Key.Name,
                                             FromStatusId = s.Key.FromStatusId,
                                             ToStatusId = s.Key.ToStatusId,
                                             Roles = s.Select(k => new TransitionRoleDto
                                             {
                                                 RoleId = k.RoleId,
                                                 RoleName = k.RoleName
                                             }).ToList()
                                         }).ToList();

            foreach (var t in transationsPermissions)
            {
                var fromStatus = transitions.FirstOrDefault(s => s.Id == t.FromStatusId);
                var toStatus = transitions.FirstOrDefault(s => s.Id == t.ToStatusId);
                t.FromStatusCode = fromStatus?.Code;
                t.FromStatusName = fromStatus?.Name;
                t.ToStatusCode = toStatus?.Code;
                t.ToStatusName = toStatus?.Name;
                if (t.Roles.FirstOrDefault().RoleId == 0)
                {
                    t.Roles.RemoveAll(s => s.RoleId == 0);
                }
            }

            var query = (from w in WorkScope.GetAll<Workflow>().Where(x => x.Id == id)
                        join wst in WorkScope.GetAll<WorkflowStatusTransition>() on w.Id equals wst.WorkflowId into lst
                        from s in lst.DefaultIfEmpty()
                        select new DetailWorkflowDto
                        {
                            Id = w.Id,
                            Name = w.Name,
                        }).FirstOrDefault();
            if (transationsPermissions.Count != 0)
            {
                query.Transitions = transationsPermissions;
            }
            return query;
        }
    }
}
