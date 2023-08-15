using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using FinanceManagement.APIs.AccountTypes.Dto;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using FinanceManagement.IoC;

namespace FinanceManagement.APIs.AccountTypes
{
    [AbpAuthorize]
    public class AccountTypeAppService : FinanceManagementAppServiceBase
    {
        public AccountTypeAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_AccountType_Create)]
        public async Task<AccountTypeDto> Create(AccountTypeDto input)
        {
            //Name and code accountype are unique
            var nameExist = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Name == input.Name);
            var codeExist = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Code == input.Code);
            if (nameExist)
            {
                throw new UserFriendlyException("Account Type name already exists");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Account Type code already exists");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<AccountType>(input));
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_AccountType_Edit)]
        public async Task<AccountTypeDto> Update(AccountTypeDto input)
        {
            var nameExist = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            var codeExist = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Code == input.Code && s.Id != input.Id);
            if (nameExist)
            {
                throw new UserFriendlyException("Account Type name already exists");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Account Type code already exists");
            }

            var AccountType = await WorkScope.GetAsync<AccountType>(input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map<AccountTypeDto, AccountType>(input, AccountType));
            return input;
        }
        [HttpGet]
        public async Task<List<AccountTypeDto>> GetAll()
        {
            return await WorkScope.GetAll<AccountType>().Select(s => new AccountTypeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<AccountTypeDto>> GetAllForDropDown()
        {
            return await WorkScope.GetAll<AccountType>().Select(s => new AccountTypeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name
            }).ToListAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_AccountType)]
        public async Task<GridResult<AccountTypeDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<AccountType>()
                .Select(s => new AccountTypeDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name
                });
            return await query.GetGridResult(query, input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_AccountType_Delete)]
        public async Task Delete(long id)
        {
            var accountType = await WorkScope.GetAll<AccountType>().FirstOrDefaultAsync(s => s.Id == id);
            var hasAccount = await WorkScope.GetAll<Account>().AnyAsync(m => m.AccountTypeId == id);
            if (accountType == null)
            {
                throw new UserFriendlyException("Account Type doesn't exist");
            }
            if (hasAccount)
            {
                throw new UserFriendlyException("Can't delete account type when you have linked account");
            }
            await WorkScope.DeleteAsync<AccountType>(id);
        }
        [HttpGet]

        public async Task<AccountTypeDto> Get(long id)
        {
            return await WorkScope.GetAll<AccountType>().Where(s => s.Id == id).Select(s => new AccountTypeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name
            }).FirstOrDefaultAsync();
        }
    }
}


