using Abp.Authorization;
using Abp.UI;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleChartDetails;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.CircleChartDetails
{
    [AbpAuthorize]
    public class CircleChartDetailAppService : FinanceManagementAppServiceBase
    {
        private readonly ICircleChartDetailManager _circleChartDetailManager;
        public CircleChartDetailAppService(IWorkScope ws, ICircleChartDetailManager circleChartDetailManager) : base(ws)
        {
            _circleChartDetailManager = circleChartDetailManager;
        }

        [HttpGet]
        public async Task<CircleChartInfoDto> GetCircleChartDetailsByChartId(long circleChartId)
        {
            return await _circleChartDetailManager.GetCircleChartDetailsByChartId(circleChartId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Create)]
        public async Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input)
        {
            return await _circleChartDetailManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Edit)]
        public async Task<UpdateCircleChartDetailDto> Update(UpdateCircleChartDetailDto input)
        {
            return await _circleChartDetailManager.Update(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Edit)]
        public async Task<UpdateCircleChartInOutcomeTypeIdsDto> UpdateInOutcomeTypeIds(UpdateCircleChartInOutcomeTypeIdsDto input)
        {
            return await _circleChartDetailManager.UpdateInOutcomeTypeIds(input);
        }


        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _circleChartDetailManager.Delete(id);
        }
    }
}
