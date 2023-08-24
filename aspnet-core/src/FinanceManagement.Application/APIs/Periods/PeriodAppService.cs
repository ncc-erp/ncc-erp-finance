using Abp.Authorization;
using Abp.UI;
using FinanceManagement.Authorization;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Managers.Periods.Dtos;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FinanceManagement.APIs.Periods
{
    [AbpAuthorize]
    public class PeriodAppService : FinanceManagementAppServiceBase
    {
        private readonly IPeriodManager _periodManager;
        public PeriodAppService(IWorkScope workScope, IPeriodManager periodManager) : base(workScope)
        {
            _periodManager = periodManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Period)]
        public async Task<GridResult<GetPeriodDto>> GetAllPaging(GridParam gridParams)
        {
            var query = _periodManager
                .IQGetPeriod()
                .OrderByDescending(x => x.StartDate);
            return await query.GetGridResult(query, gridParams);
        }

        [HttpGet]
        public async Task<List<GetPeriodDto>> GetAll()
        {
            return await _periodManager
                .IQGetPeriod()
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();
        }
        [HttpGet]
        public async Task<GetPeriodDto> GetFirstPeriod()
        {
            return await _periodManager
                .IQGetPeriod()
                .OrderBy(x => x.StartDate)
                .FirstOrDefaultAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Period_CloseAndCreate)]
        public async Task CloseAndCreatePeriod(CloseAndCreatePeriodDto input)
        {
            var isTheFirstTime = await _periodManager.IsTheFirstRecord();
            if (isTheFirstTime)
                throw new UserFriendlyException("Gọi nhầm Api. Chưa tồn tại kì kế toán nào!");
            await _periodManager.ClosePeriod();
            await MySettingManager.SetApplyToMultiCurrencyOutcome("true");
            await _periodManager.Create(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Period_Create)]
        public async Task CreateTheFirstTime(CreatePeriodForTheFirstTime input)
        {
            var isTheFirstTime = await _periodManager.IsTheFirstRecord();
            if (!isTheFirstTime)
                throw new UserFriendlyException("Gọi nhầm Api. Đã tồn tại ít nhất một kì kế toán!");

            var period = await _periodManager.CreatePeriod(new FormPeriod
            {
                Name = input.Name,
                IsActive = false,
                StartDate = input.StartDate,
            });

            await _periodManager.AssignPeriodIdForTheFirstTime(period, input.PeriodBankAccounts);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_Period_Edit)]
        public async Task Update(UpdatePeriodDto input)
        {
            await _periodManager.Update(input);
        }
        [HttpGet]
        public async Task<GetPeriodHaveDetail> Get(int id)
        {
            return await _periodManager.Get(id);
        }
        [HttpGet]
        public async Task<bool> IsTheFirstRecord()
        {
            return await _periodManager.IsTheFirstRecord();
        }
        [HttpGet]
        public async Task<PreviewClosePeriodDto> PreviewBeforeWhenClosePeriod()
        {
            return await _periodManager.PreviewBeforeWhenClosePeriod();
        }
        [HttpPost]
        public async Task<PreviewClosePeriodByBankAccountDto> CheckDiffRealBalanceAndBTransaction(PreviewClosePeriodByBankAccountDto input)
        {
            return await _periodManager.CheckDiffRealBalanceAndBTransaction(input);
        }
    }
}
