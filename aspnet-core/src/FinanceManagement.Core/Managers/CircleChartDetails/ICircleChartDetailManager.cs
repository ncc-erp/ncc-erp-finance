using Abp.Dependency;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.CircleChartDetails
{
    public interface ICircleChartDetailManager : ITransientDependency
    {
        IQueryable<CircleChartDetailDto> IQGetCircleChartDetails(bool isIncome);
        Task<List<CircleChartDetailDto>> GetAllCircleChartDetails(bool isIncome);
        Task<List<CircleChartDetailDto>> GetCircleChartDetailsById(long chartId);
        Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input);
        Task<UpdateCircleChartDetailDto> Update(UpdateCircleChartDetailDto input);
        Task<long> Delete(long id);
        Task<long> Active(long id);
        Task<long> DeActive(long id);
    }
}
