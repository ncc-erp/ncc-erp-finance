using Abp.Authorization;
using Abp.UI;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleChartDetails;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
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
        public async Task<List<CircleChartDetailDto>> GetAllCircleChartDetails(bool isIncome)
        {
            return await _circleChartDetailManager.GetAllCircleChartDetails(isIncome);
        }

        [HttpGet]
        public async Task<List<CircleChartDetailDto>> GetCircleChartDetailsById(long chartId)
        {
            return await _circleChartDetailManager.GetCircleChartDetailsById(chartId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_View)]
        public async Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input)
        {
            var circleChart = WorkScope.Get<CircleChart>(input.CircleChartId);
            if (circleChart == null)
                throw new UserFriendlyException("Không tồn tại circleChart với id = " + circleChart.Id);
            return await _circleChartDetailManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Edit)]
        public async Task<UpdateCircleChartDetailDto> Edit(UpdateCircleChartDetailDto input)
        {
            _ = IsClientExisted(input.ClientIds);
            return await _circleChartDetailManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _circleChartDetailManager.Delete(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_ActiveDeactive)]
        public async Task<long> Active(long id)
        {
            return await _circleChartDetailManager.Active(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_CircleChart_CircleChartDetail_ActiveDeactive)]
        public async Task<long> DeActive(long id)
        {
            return await _circleChartDetailManager.DeActive(id);
        }

        public bool IsClientExisted(List<long> clientIds)
        {
            foreach (var clientId in clientIds)
            {
                var client = WorkScope.Get<Account>(clientId);
                if (client == null)
                    throw new UserFriendlyException("Không tồn tại account có Id = " + clientId);
                if (client.AccountType.Code != Constants.ACCOUNT_TYPE_CLIENT)
                    throw new UserFriendlyException($"Account có id = '{clientId}' không thuộc loại CLIENT ");
            }
            return true;
        }
    }
}
