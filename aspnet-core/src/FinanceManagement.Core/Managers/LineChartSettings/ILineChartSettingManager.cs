using Abp.Dependency;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.LineChartSettings
{
    public interface ILineChartSettingManager : ITransientDependency
    {
        IQueryable<LineChartSettingDto> IQGetLineChartSetting();
        Task<GridResult<LineChartSettingDto>> GetAllPaging(GridParam input);
        Task<List<LineChartSettingDto>> GetAll();
        Task<CreateLineChartSettingDto> Create(CreateLineChartSettingDto input);
        Task<UpdateLineChartSettingDto> Update(UpdateLineChartSettingDto input);
        Task<AddReferenceToLinechartDto> AddReferenceToLineChart(AddReferenceToLinechartDto input);
        Task<RemoveChartSettingDto> RemoveLineChartReference(RemoveChartSettingDto input);
        Task<LineChartSettingDto> Get(int id);
        Task<long> Delete(long id);
        Task<long> Active(long id);
        Task<long> DeActive(long id);
    }
}
