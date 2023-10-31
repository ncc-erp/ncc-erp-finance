using Abp.Dependency;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.CircleChartDetails
{
    public interface ICircleChartDetailManager : ITransientDependency
    {
        Task<CircleChartInfoDto> GetCircleChartDetailsByChartId(long circleChartId);
        Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input);
        Task<UpdateCircleChartDetailDto> Update(UpdateCircleChartDetailDto input);
        Task<UpdateCircleChartInOutcomeTypeIdsDto> UpdateInOutcomeTypeIds(UpdateCircleChartInOutcomeTypeIdsDto input);
        Task<long> Delete(long id);
    }
}
