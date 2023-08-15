using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.WorkFlowStatus.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.WorkFlowStatuses
{
    [AbpAuthorize]
    public class WorkflowStatusAppService : FinanceManagementAppServiceBase
    {
        public WorkflowStatusAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_WorkflowStatus_Create)]
        public async Task<WorkflowStatusDto> Create(WorkflowStatusDto input)
        {
            //Name of Workflow is unique
            var nameExist = await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.Name == input.Name);
            var codeExist = await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.Code == input.Code);

            if (nameExist)
            {
                throw new UserFriendlyException("Workflow status name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Workflow status code already exist");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<WorkflowStatus>(input));
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_WorkflowStatus_Edit)]
        public async Task<WorkflowStatusDto> Update(WorkflowStatusDto input)
        {
            var workflowStatus = await WorkScope.GetAsync<WorkflowStatus>(input.Id);
            var nameExist = await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.Name.ToLower() == input.Name.ToLower() && s.Id != input.Id && s.IsDeleted == false);
            var codeExist = await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.Code.ToLower() == input.Code.ToLower() && s.Id != input.Id && s.IsDeleted == false);
            if (nameExist)
            {
                throw new UserFriendlyException("Workflow status name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Workflow status code already exist");
            }
            await WorkScope.UpdateAsync(ObjectMapper.Map<WorkflowStatusDto, WorkflowStatus>(input, workflowStatus));
            return input;
        }
        [HttpGet]
        public async Task<List<GetWorkflowStatusDto>> GetAll()
        {
            return await WorkScope.GetAll<WorkflowStatus>().Select(s => new GetWorkflowStatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code
            }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<GetWorkflowStatusDto>> GetAllForDropDown()
        {
            return await WorkScope.GetAll<WorkflowStatus>().Select(s => new GetWorkflowStatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code
            }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<GetWorkflowStatusDto>> GetAllForDropDownAndNotEqualsEnd()
        {
            var allWorkflowStatus = await WorkScope.GetAll<WorkflowStatus>().Select(s => new GetWorkflowStatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code
            }).ToListAsync();

            allWorkflowStatus.Add(new GetWorkflowStatusDto
            {
                Id = -1,
                Name = "Khác thực thi",
                Code = Constants.WORKFLOW_STATUS_OTHER_END
            });

            return allWorkflowStatus;
        }
        [HttpGet]
        public async Task<List<GetWorkflowStatusDto>> GetTempOutComingEntryStatusOptions()
        {
            var allWorkflowStatus = await WorkScope.GetAll<TempOutcomingEntry>()
                .Select(s => new 
                {
                    Name = s.WorkflowStatus.Name,
                    Code = s.WorkflowStatus.Code,
                    WorkflowStatusId = s.WorkflowStatusId
                })
                .OrderBy(s => s.WorkflowStatusId)
                .Distinct()
                .Select(s => new GetWorkflowStatusDto
                {
                    Name = s.Name,
                    Code = s.Code
                })
                .ToListAsync();

            return allWorkflowStatus;
        }


        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_WorkflowStatus_Delete)]
        public async Task Delete(long id)
        {
            //Check workflowStatus in: WorkflowStatusTransition

            var workflowStatus = await WorkScope.GetAll<WorkflowStatus>().FirstOrDefaultAsync(s => s.Id == id);
            if (workflowStatus == null)
            {
                throw new UserFriendlyException("Workflow status Id doesn't exist");
            }

            var hasInWorkflowStatusTransition = await WorkScope.GetAll<WorkflowStatusTransition>().AnyAsync(s => workflowStatus.Id == s.FromStatusId || workflowStatus.Id == s.ToStatusId);
            if (hasInWorkflowStatusTransition)
            {
                throw new UserFriendlyException($"Không thể xóa trạng thái {workflowStatus.Name} do đã tồn tại trong chuyển tiếp");
            }
            await WorkScope.DeleteAsync<WorkflowStatus>(id);
        }

        public async Task<List<GetWorkflowStatusDto>> GetStatusForOutcomeFilter()
        {
            return await WorkScope.GetAll<WorkflowStatus>()
                .Where(x => x.Code == Constants.WORKFLOW_STATUS_START
                || x.Code == Constants.WORKFLOW_STATUS_PENDINGCEO
                || x.Code == Constants.WORKFLOW_STATUS_APPROVED
                || x.Code == Constants.WORKFLOW_STATUS_REJECTED)
                .Select(x => new GetWorkflowStatusDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }

    }
}
