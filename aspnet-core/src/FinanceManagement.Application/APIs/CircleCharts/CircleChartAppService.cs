using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleCharts;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.CircleCharts
{
    [AbpAuthorize]
    public class CircleChartAppService : FinanceManagementAppServiceBase
    {
        private readonly ICircleChartManager _circleChartManager;
        public CircleChartAppService(IWorkScope ws, ICircleChartManager circleChartSettingManager) : base(ws)
        {
            _circleChartManager = circleChartSettingManager;
        }

        [HttpGet]
        public async Task<List<CircleChartDto>> GetAll()
        {
            return await _circleChartManager.GetAll();
        }

        [HttpGet]
        public async Task<List<CircleChartDto>> GetAllActive()
        {
            return await _circleChartManager.GetAllActive();
        }

        [HttpPost]
        public async Task<GridResult<CircleChartDto>> GetAllPaging(GridParam input)
        {
            return await _circleChartManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_Create)]
        public async Task<CreateCircleChartDto> Create(CreateCircleChartDto input)
        {
            return await _circleChartManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_Edit)]
        public async Task<UpdateCircleChartDto> Update(UpdateCircleChartDto input)
        {
            return await _circleChartManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _circleChartManager.Delete(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_ActiveDeactive)]
        public async Task<long> Active(long id)
        {
            return await _circleChartManager.Active(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_ActiveDeactive)]
        public async Task<long> DeActive(long id)
        {
            return await _circleChartManager.DeActive(id);
        }
    }
}
