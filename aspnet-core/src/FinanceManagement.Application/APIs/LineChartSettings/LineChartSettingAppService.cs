using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.LineChartSettings;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.LineChartSettings
{
    [AbpAuthorize]
    public class LineChartSettingAppService : FinanceManagementAppServiceBase
    {
        ILineChartSettingManager _lineChartSettingManager;
        public LineChartSettingAppService(IWorkScope ws, ILineChartSettingManager lineChartSettingManager) : base(ws)
        {
            _lineChartSettingManager = lineChartSettingManager;
        }

        [HttpGet]
        public async Task<List<LineChartSettingDto>> GetAll()
        {
            return await _lineChartSettingManager.GetAll();
        }

        [HttpPost]
        public async Task<GridResult<LineChartSettingDto>> GetAllPaging(GridParam input)
        {
            return await _lineChartSettingManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_Create)]
        public async Task<CreateLineChartSettingDto> Create(CreateLineChartSettingDto input)
        {
            return await _lineChartSettingManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_Edit)]
        public async Task<UpdateLineChartSettingDto> Update(UpdateLineChartSettingDto input)
        {
            return await _lineChartSettingManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _lineChartSettingManager.Delete(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_ActiveDeactive)]
        public async Task<long> Active(long id)
        {
            return await _lineChartSettingManager.Active(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_Edit)]
        public async Task<AddReferenceToLinechartDto> AddReferenceToLineChart(AddReferenceToLinechartDto input)
        {
            return await _lineChartSettingManager.AddReferenceToLineChart(input);
        }


        [HttpPut]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_ActiveDeactive)]
        public async Task<long> DeActive(long id)
        {
            return await _lineChartSettingManager.DeActive(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_LineChartSetting_Edit)]
        public async Task<RemoveChartSettingDto> RemoveLineChartReference(RemoveChartSettingDto input)
        {
            return await _lineChartSettingManager.RemoveLineChartReference(input);
        }

    }
}
