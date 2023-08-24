using Abp.Authorization;
using FinanceManagement.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Abp.UI;
using FinanceManagement.APIs.Curencies.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using System.Collections.Generic;
using FinanceManagement.IoC;
using System.Linq.Dynamic.Core;

namespace FinanceManagement.APIs.Curencies
{
    [AbpAuthorize]
    public class CurrencyAppService : FinanceManagementAppServiceBase
    {
        public CurrencyAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Currency_Create)]
        public async Task<CreateCurrency> Create(CreateCurrency input)
        {
            //Name and code currency are unique
            var nameExist = await WorkScope.GetAll<Currency>().AnyAsync(s => s.Name == input.Name);
            var codeExist = await WorkScope.GetAll<Currency>().AnyAsync(s => s.Code == input.Code);
            if (nameExist)
            {
                throw new UserFriendlyException("Currency name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Currency code already exist");
            }
            if(input.IsCurrencyDefault)
            {
                //TODO::inactive old default currency
                await SetInactiveCurrencyDefault();
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Entities.Currency>(input));
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_Currency_Edit)]
        public async Task<EditCurrency> Update(EditCurrency input)
        {
            var currency = await WorkScope.GetAsync<Currency>(input.Id);
            var nameExist = await WorkScope.GetAll<Currency>().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            var codeExist = await WorkScope.GetAll<Currency>().AnyAsync(s => s.Code == input.Code && s.Id != input.Id);
            if (nameExist)
            {
                throw new UserFriendlyException("Currency name already exist");
            }
            if (codeExist)
            {
                throw new UserFriendlyException("Currency code already exist");
            }
            if(input.IsCurrencyDefault != currency.IsCurrencyDefault && input.IsCurrencyDefault)
            {
                await SetInactiveCurrencyDefault();
            }
            await WorkScope.UpdateAsync(ObjectMapper.Map<EditCurrency, Currency>(input, currency));
            return input;
        }
        private async Task SetInactiveCurrencyDefault()
        {
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault == null)
                return;
            currencyDefault.IsCurrencyDefault = false;
            CurrentUnitOfWork.SaveChanges();
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Currency)]
        public async Task<GridResult<CurrencyPagingDto>> GetAllPaging(GridParam input)
        {
            var dictBA = WorkScope.GetAll<BankAccount>()
                .Select(x => new BankAccountDto
                {
                    Id = x.Id,
                    BankName = x.HolderName
                }).ToDictionary(x => x.Id, x => new { x.BankName });

            var query = WorkScope.GetAll<Currency>()
                .Select(c => new CurrencyPagingDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    MaxITF = c.MaxITF,
                    DefaultBankAccountId = c.DefaultBankAccountId,
                    DefaultBankAccountIdWhenSell = c.DefaultBankAccountIdWhenSell,
                    DefaultFromBankAccountIdWhenBuy = c.DefaultFromBankAccountIdWhenBuy,
                    DefaultToBankAccountIdWhenBuy = c.DefaultToBankAccountIdWhenBuy,
                    IsCurrencyDefault = c.IsCurrencyDefault,
                });
            var result = await query.GetGridResult(query, input);

            foreach (var dto in result.Items)
            {
                dto.DefaultBankAccountName = dto.DefaultBankAccountId != null && dictBA.ContainsKey(dto.DefaultBankAccountId) ? dictBA[dto.DefaultBankAccountId].BankName : "";
                dto.DefaultBankAccountNameWhenSell = dto.DefaultBankAccountIdWhenSell != null && dictBA.ContainsKey(dto.DefaultBankAccountIdWhenSell) ? dictBA[dto.DefaultBankAccountIdWhenSell].BankName : "";
                dto.DefaultFromBankAccountNameWhenBuy = dto.DefaultFromBankAccountIdWhenBuy != null && dictBA.ContainsKey(dto.DefaultFromBankAccountIdWhenBuy) ? dictBA[dto.DefaultFromBankAccountIdWhenBuy].BankName : "";
                dto.DefaultToBankAccountNameWhenBuy = dto.DefaultToBankAccountIdWhenBuy != null && dictBA.ContainsKey(dto.DefaultToBankAccountIdWhenBuy) ? dictBA[dto.DefaultToBankAccountIdWhenBuy].BankName : "";
            }
            return result;
        }

        [HttpGet]
        public List<CurrencyConvertDto> GetAllForDropdown()
        {
            var list = WorkScope.GetAll<CurrencyConvert>().Include(s => s.Currency)
                .AsEnumerable<CurrencyConvert>()
                .GroupBy(x => x.Currency.Id)
                .Select(s => new CurrencyConvertDto
                {
                    Id = s.Key,
                    Code = s.OrderByDescending(x => x.DateAt).FirstOrDefault().Currency.Code,
                    Name = s.OrderByDescending(x => x.DateAt).FirstOrDefault().Currency.Name,
                    Value = s.OrderByDescending(x => x.DateAt).FirstOrDefault().Value
                }).ToList();
            return list;
        }
        public async Task<List<CurrencyConvertDto>> GetAll()
        {
            return await WorkScope.GetAll<CurrencyConvert>().Include(s => s.Currency)
                .Select(s => new CurrencyConvertDto
                {
                    Id = s.CurrencyId,
                    Code = s.Currency.Code,
                    Name = s.Currency.Name,
                    Value = s.Value
                }).ToListAsync();
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_Currency_Delete)]
        public async Task Delete(long id)
        {
            var currency = await WorkScope.GetAll<Currency>().FirstOrDefaultAsync(s => s.Id == id);
            if (currency == null)
            {
                throw new UserFriendlyException("Currency isn't exits");
            }

            var hasBankAccount = await WorkScope.GetAll<BankAccount>().AnyAsync(b => b.CurrencyId == id);
            if (hasBankAccount)
            {
                throw new UserFriendlyException("Can't delete Currency when you have a linked Bank Account");
            }

            var hasInvoice = await WorkScope.GetAll<Invoice>().AnyAsync(b => b.CurrencyId == id);
            if (hasInvoice)
                throw new UserFriendlyException("Không thể xóa loại tiền khi đã link tới Invoice");

            var hasOutcomingEntry = await WorkScope.GetAll<OutcomingEntry>().AnyAsync(b => b.CurrencyId == id);
            if (hasOutcomingEntry)
                throw new UserFriendlyException("Không thể xóa loại tiền khi đã link tới Request chi");

            var currencyConvert = WorkScope.GetAll<CurrencyConvert>().Where(s => s.CurrencyId == id).ToList();
            if (currencyConvert != null && currencyConvert.Count() > 0)
            {

                foreach (var cc in currencyConvert)
                {
                    await WorkScope.DeleteAsync<CurrencyConvert>(cc.Id);
                }
            }
            await WorkScope.DeleteAsync<Currency>(id);
        }
        [HttpGet]

        public async Task<CurrencyByIdDto> Get(long id)
        {
            return await WorkScope.GetAll<Currency>().Where(s => s.Id == id).Select(s => new CurrencyByIdDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                MaxITF = s.MaxITF,
                DefaultBankAccountId = s.DefaultBankAccountId,
                DefaultBankAccountIdWhenSell = s.DefaultBankAccountIdWhenSell,
                DefaultFromBankAccountIdWhenBuy = s.DefaultFromBankAccountIdWhenBuy,
                DefaultToBankAccountIdWhenBuy = s.DefaultToBankAccountIdWhenBuy,
                IsCurrencyDefault = s.IsCurrencyDefault
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<List<CurrencyDto>> GetAllCurrencyForDropdown()
        {
            return await WorkScope.GetAll<Currency>()
                .Select(s => new CurrencyDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                }).ToListAsync();
        }
        [HttpPost]
        public async Task SetDefaultToBankAccountIdWhenBuy(EditCurrency input)
        {
            var currency = WorkScope.Get<Currency>(input.Id);
            currency.DefaultToBankAccountIdWhenBuy = input.DefaultToBankAccountIdWhenBuy;
            await WorkScope.UpdateAsync(currency);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        [HttpPost]
        public async Task SetDefaultFromBankAccountIdWhenBuy(EditCurrency input)
        {
            var currency = WorkScope.Get<Currency>(input.Id);
            currency.DefaultFromBankAccountIdWhenBuy = input.DefaultFromBankAccountIdWhenBuy;
            await WorkScope.UpdateAsync(currency);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        [HttpPost]
        public async Task SetDefaultCurrency(EditCurrency input)
        {
            var currencyDefaultOld = await WorkScope.GetAll<Currency>()
                .Where(s => s.IsCurrencyDefault == true)
                .ToListAsync();
            foreach (var item in currencyDefaultOld)
            {
                item.IsCurrencyDefault = false;
            }
            var currency = WorkScope.Get<Currency>(input.Id);
            currency.IsCurrencyDefault = input.IsCurrencyDefault;
            await WorkScope.UpdateAsync(currency);
        }
        [HttpPost]
        public async Task SetDefaultBankAccount(EditCurrency input)
        {
            var currency = WorkScope.Get<Currency>(input.Id);
            currency.DefaultBankAccountId = input.DefaultBankAccountId;
            await WorkScope.UpdateAsync(currency);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
