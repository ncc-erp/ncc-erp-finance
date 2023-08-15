using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.Branches.Dto;
using FinanceManagement.APIs.Curencies.Dto;
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

namespace FinanceManagement.APIs.Branches
{
    [AbpAuthorize]
    public class BranchAppService : FinanceManagementAppServiceBase
    {
        public BranchAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Branch_Create)]
        public async Task<BranchDto> Create(BranchDto input)
        {
            if(input.Default == true)
            {
                var hasDefaultBranch = await WorkScope.GetAll<Branch>().AnyAsync(b => b.Default == true);
                if(hasDefaultBranch)
                {
                    throw new UserFriendlyException("Default branch already exists");
                }
            }
            var isBranch = await WorkScope.GetAll<Branch>().AnyAsync(b => b.Code == input.Code || b.Name == input.Name);
            if (isBranch)
            {
                throw new UserFriendlyException("Code and name branch already exists.");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Branch>(input));
            CurrentUnitOfWork.SaveChanges();
            return input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_Branch_Edit)]
        public async Task<BranchDto> Update(BranchDto input)
        {
            var branches = WorkScope.GetAll<Branch>();
            var currentBranch = await branches.FirstOrDefaultAsync(b => b.Id == input.Id);

            if(currentBranch == null)
            {
                throw new UserFriendlyException("Branch doesn't exist");
            }

            if(currentBranch.Default != true && input.Default == true)
            {
                var hasDefaultBranch = await branches.AnyAsync(b => b.Id != input.Id && b.Default == true);
                if (hasDefaultBranch)
                {
                    throw new UserFriendlyException("Default branch already exists");
                }
            }
            var isBranch = await branches.AnyAsync(b => (b.Code == input.Code || b.Name == input.Name) && b.Id != input.Id);
            if (isBranch)
            {
                throw new UserFriendlyException("Code and name branch already exists.");
            }

            await WorkScope.UpdateAsync(ObjectMapper.Map<BranchDto, Branch>(input, currentBranch));
            return input;
        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Branch_View)]
        public async Task<GridResult<BranchDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<Branch>().Select(s => new BranchDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Default = s.Default,
            });
            return await query.GetGridResult(query, input);
        }
        [HttpGet]

        public async Task<List<BranchDto>> GetAll()
        {
            return await WorkScope.GetAll<Branch>().Select(s => new BranchDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Default = s.Default,
            }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<BranchDto>> GetAllForDropdown()
        {
            return await WorkScope.GetAll<Branch>().Select(s => new BranchDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Default = s.Default,
            }).ToListAsync();
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_Branch_Delete)]
        public async Task Delete(long id)
        {
            var branch = await WorkScope.GetAll<Branch>().FirstOrDefaultAsync(s => s.Id == id);
            if (branch == null)
            {
                throw new UserFriendlyException("Branch doesn't exist");
            }

            var hasIncomingEntries = await WorkScope.GetAll<IncomingEntry>().AnyAsync(ie => ie.BranchId == id);
            if(hasIncomingEntries) {
                throw new UserFriendlyException("Can not delete Branch when you have linked Incoming entries");
            }
            await WorkScope.DeleteAsync<Branch>(id);
        }

        [HttpGet]

        public async Task<BranchDto> Get(long id)
        {
            return await WorkScope.GetAll<Branch>().Where(s => s.Id == id).Select(s => new BranchDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Default = s.Default,
            }).FirstOrDefaultAsync();
        }
    }
}
