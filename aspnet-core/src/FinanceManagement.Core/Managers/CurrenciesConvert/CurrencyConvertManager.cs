using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CurrenciesConvert.Dto;
using FinanceManagement.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.CurrenciesConvert
{
    public class CurrencyConvertManager : DomainManager
    {
        public CurrencyConvertManager(IWorkScope ws) : base(ws)
        {

        }

        public async Task<GridResult<CurrenciesConvertDto>> GetAllPaging(InputToFilterCurrencyConvert input)
        {
            var query = _ws.GetAll<CurrencyConvert>()
                .Select(s => new CurrenciesConvertDto
                {
                   Id = s.Id,
                   CurrencyName = s.Currency.Name,
                   CurrencyId = s.CurrencyId,
                   Value = s.Value,
                   DateAt = s.DateAt,
                });
            if(input.Month != 0)
            {
                query = query.Where(x => x.DateAt.Date.Month == input.Month);
            }
            if (input.Year != 0)
            {
                query = query.Where(x => x.DateAt.Date.Year == input.Year);
            }
            return await query.GetGridResult(query, input);
        }
        public async Task<CreateCurrencyConvertDto> Create(CreateCurrencyConvertDto input)
        {
            ValidCreate(input);

            var query = _ws.GetAll<CurrencyConvert>()
                .Where(x=> x.CurrencyId == input.CurrencyId);

            var listMonths = new List<int>();
            var listYears = new List<int>();

            if (query != default)
            {
                listYears = query.Select(x => x.DateAt.Year).ToList();

                if (!listYears.Contains(input.Year))
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        await CreateByMonth(input, i);
                    }
                    return input;
                }
                listMonths = query.Where(x=> x.DateAt.Year == input.Year).Select(x => x.DateAt.Month).ToList();

                for (int i = 1; i <= 12; i++)
                {
                    if (!listMonths.Contains(i))
                    {
                        await CreateByMonth(input, i);
                    }
                }
                return input;
            }

            for (int i = 1; i <= 12; i++)
            {
                await CreateByMonth(input, i);
            }
            return input;
        }
        public void ValidCreate(CreateCurrencyConvertDto input)
        {
            var listCCs = _ws.GetAll<CurrencyConvert>()
                .Where(x => x.CurrencyId == input.CurrencyId)
                .Where(x => x.DateAt.Date.Year == input.Year)
                .ToList();

            if(listCCs != null && listCCs.Count() == 12)
            {
                throw new UserFriendlyException($"Tỉ giá này trong cả năm {input.Year} đã được tạo");
            }
        }
        private async Task CreateByMonth(CreateCurrencyConvertDto input , int month)
        {
            var enity = new CurrencyConvert();
            enity.CurrencyId = input.CurrencyId;
            enity.Value = input.Value;
            enity.DateAt = new DateTime(input.Year, month, 15);
            await _ws.InsertAndGetIdAsync(ObjectMapper.Map<CurrencyConvert>(enity));
        }

        public async Task<UpdateCurrencyConvertDto> Update(UpdateCurrencyConvertDto input)
        {
            ValidCurrencyConvert(input.Id);
            var currencyConvert = _ws.GetAll<CurrencyConvert>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();
            currencyConvert.Value = input.Value;
            await _ws.UpdateAsync(currencyConvert);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            ValidCurrencyConvert(id);
            await _ws.DeleteAsync<CurrencyConvert>(id);
            return id;

        }

        private void ValidCurrencyConvert(long id)
        {
            var currencyConvert = _ws.GetAll<CurrencyConvert>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (currencyConvert == default)
            {
                throw new UserFriendlyException($"Can not found currency convert with Id = {id}");
            }
        }


        public List<int> GetMonthOfCurrencyConvert()
        {
            var listMonths = _ws.GetAll<CurrencyConvert>()
                .Select(x => x.DateAt.Month)
                .Distinct()
                .OrderBy(x=> x)
                .ToList();
            return listMonths;
        }

        public List<int> GetYearOfCurrencyConvert()
        {
            var listMonths = _ws.GetAll<CurrencyConvert>()
                .Select(x => x.DateAt.Year)
                .Distinct()
                .OrderBy(x=> x)
                .ToList();
            return listMonths;
        }
    }
}
