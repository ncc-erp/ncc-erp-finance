using Abp.Dependency;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.CircleCharts
{
    public interface ICircleChartManager : ITransientDependency
    {
        IQueryable<CircleChartDto> IQGetCircleChart();
        Task<GridResult<CircleChartDto>> GetAllPaging(GridParam input);
        Task<List<CircleChartDto>> GetAll();
        Task<CreateCircleChartDto> Create(CreateCircleChartDto input);
        Task<UpdateCircleChartDto> Update(UpdateCircleChartDto input);
        Task<CircleChartDto> Get(int id);
        Task<long> Delete(long id);
        Task<long> Active(long id);
        Task<long> DeActive(long id);
    }
}
