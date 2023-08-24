using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CurrenciesConvert;
using FinanceManagement.Managers.CurrenciesConvert.Dto;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.CurrenciesConvert
{
    [AbpAuthorize]
    public class CurrencyConvertAppService : FinanceManagementAppServiceBase
    {
        public readonly CurrencyConvertManager _currencyconvertmanager;

        public CurrencyConvertAppService(IWorkScope workScope,
            CurrencyConvertManager currencyconvertmanager
            ) : base(workScope)
        {
            _currencyconvertmanager = currencyconvertmanager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert)]
        public async Task<GridResult<CurrenciesConvertDto>> GetAllPaging(InputToFilterCurrencyConvert input)
        {
            return await _currencyconvertmanager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert_Create)]
        public async Task<CreateCurrencyConvertDto> Create(CreateCurrencyConvertDto input)
        {
            return await _currencyconvertmanager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert_Edit)]
        public async Task<UpdateCurrencyConvertDto> Update(UpdateCurrencyConvertDto input)
        {
            return await _currencyconvertmanager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _currencyconvertmanager.Delete(id);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert)]
        public List<int> GetMonthOfCurrencyConvert()
        {
            return _currencyconvertmanager.GetMonthOfCurrencyConvert();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_CurrencyConvert)]
        public List<int> GetYearOfCurrencyConvert()
        {
            return _currencyconvertmanager.GetYearOfCurrencyConvert();
        }
    }
}
