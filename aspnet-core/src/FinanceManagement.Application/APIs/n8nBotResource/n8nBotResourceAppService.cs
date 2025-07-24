using Abp.Application.Services;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Dashboards;
using FinanceManagement.Managers.Dashboards.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FinanceManagement.APIs.N8nBotResource
{
    public class N8nBotResourceAppService : FinanceManagementAppServiceBase
    {
        private readonly IDashboardManager _dashboardManager;

        public N8nBotResourceAppService(
            IWorkScope workScope,
            IDashboardManager dashboardManager
        ) : base(workScope)
        {
            _dashboardManager = dashboardManager;
        }
        [HttpGet]
        [NccAuth]
        public async Task<ResultChartDto> GetNewChartXSecret(DateTime startDate, DateTime endDate, bool isByPeriod)
        {
            if (isByPeriod)
            {
            return await _dashboardManager.GetDataNewChart(startDate, endDate);
            }
            else
            {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _dashboardManager.GetDataNewChart(startDate, endDate);
            }
            }
        }

        [HttpGet]
        [NccAuth]
        public async Task<List<BaoCaoChungDto>> GetBaoCaoChungThangNay()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

        
            var dicCurrencyConvert = GetAndCheckDictionaryCurrencyConvertByYearMonth(startDate, endDate);

            var result = await _dashboardManager.GetDataBaoCaoChung(
                startDate,
                endDate,
                dicCurrencyConvert,
                0,      // branchId = 0: lấy tất cả chi nhánh
                null    // expenseType = null: không lọc theo loại chi
            );


            return result;
        }

        private Dictionary<CurrencyYearMonthDto, double> GetAndCheckDictionaryCurrencyConvertByYearMonth(DateTime startDate, DateTime endDate)
        {
            var dicCurrencyConvert = _dashboardManager.GetDictionaryCurrencyConvertByYearMonth(startDate, endDate);
            _dashboardManager.CheckDictionaryCurrencyConvertByYearMonth(dicCurrencyConvert, startDate, endDate);
            return dicCurrencyConvert;
        }
    }
}
