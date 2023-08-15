using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using FinanceManagement.APIs.Banks.Dto;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using FinanceManagement.IoC;

namespace FinanceManagement.APIs.Banks
{
    [AbpAuthorize]
    public class BankAppService : FinanceManagementAppServiceBase
    {
        public BankAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Bank_Create)]
        public async Task<BankDto> Create(BankDto input)
        {
            //Name and code bank are unique
            var nameExist = await WorkScope.GetAll<Bank>().AnyAsync(s => s.Name == input.Name);
            var codeExist = await WorkScope.GetAll<Bank>().AnyAsync(s => s.Code == input.Code);
            if (nameExist)
            {
                throw new UserFriendlyException("Bank name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Bank code already exist");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Bank>(input));
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_Bank_Edit)]
        public async Task<BankDto> Update(BankDto input)
        {
            var bank = await WorkScope.GetAsync<Bank>(input.Id);
            var nameExist = await WorkScope.GetAll<Bank>().AnyAsync(s => s.Name == input.Name && bank.Name != input.Name && bank.Id != input.Id);
            var codeExist = await WorkScope.GetAll<Bank>().AnyAsync(s => s.Code == input.Code && bank.Code != input.Code && bank.Id != input.Id);
            if (nameExist)
            {
                throw new UserFriendlyException("Bank name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Bank code already exist");
            }
            await WorkScope.UpdateAsync(ObjectMapper.Map<BankDto, Bank>(input, bank));

            return input;
        }
        [HttpGet]
        public async Task<List<BankDto>> GetAll()
        {
            return await WorkScope.GetAll<Bank>().Select(s => new BankDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name
            }).ToListAsync();
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Bank)]
        public async Task<GridResult<BankDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<Bank>()
                .Select(s => new BankDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name
                });
            return await query.GetGridResult(query, input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_Bank_Delete)]
        public async Task Delete(long id)
        {
            var bank = await WorkScope.GetAll<Bank>().FirstOrDefaultAsync(s => s.Id == id);
            if (bank == null)
            {
                throw new UserFriendlyException("Bank Id doesn't exist");
            }

            var hasBackAccount = await WorkScope.GetRepo<BankAccount>().GetAllIncluding(x => x.Bank)
                                .AnyAsync(x => x.BankId == id);
            if(hasBackAccount)
            {
                throw new UserFriendlyException("Can not delete Bank when have linked Bank Account");
            }
            await WorkScope.DeleteAsync<Bank>(id);
        }
        [HttpGet]

        public async Task<BankDto> Get(long id)
        {
            return await WorkScope.GetAll<Bank>().Where(s => s.Id == id).Select(s => new BankDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name
            }).FirstOrDefaultAsync();
        }
    }
}
