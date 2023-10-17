using Abp.Domain.Repositories;
using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.CircleCharts
{
    public class CircleChartManager : DomainManager, ICircleChartManager
    {
        public CircleChartManager(IWorkScope ws) : base(ws)
        {
        }

        public IQueryable<CircleChartDto> IQGetCircleChart()
        {
            return _ws.GetAll<CircleChart>()
                .Select(x => new CircleChartDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    IsIncome = x.IsIncome,
                });
        }

        public async Task<List<CircleChartDto>> GetAll()
        {
            return await IQGetCircleChart().ToListAsync();
        }

        public async Task<GridResult<CircleChartDto>> GetAllPaging(GridParam input)
        {
            var query = IQGetCircleChart();
            return await query.GetGridResult(query, input);
        }

        public async Task<CreateCircleChartDto> Create(CreateCircleChartDto input)
        {
            var entity = ObjectMapper.Map<CircleChart>(input);
            entity.IsActive = true;

            await _ws.InsertAsync(entity);

            return input;
        }

        public async Task<UpdateCircleChartDto> Update(UpdateCircleChartDto input)
        {
            var existLinechart = _ws.GetAll<CircleChart>()
                .Where(x => x.Id == input.Id)
                .Select(x => new
                {
                    x.Id,
                    x.IsIncome
                })
                .FirstOrDefault();

            if (input.IsIncome != existLinechart.IsIncome)
            {
                var currentSettings = _ws.GetAll<CircleChartDetail>()
                    .Where(x => x.CircleChartId == existLinechart.Id)
                    .ToList();

                if (currentSettings.Count > 0)
                {
                    throw new UserFriendlyException("Chart đã được gắn reference, hãy xóa reference trước khi edit type");
                }
            }

            await _ws.UpdateAsync(ObjectMapper.Map<CircleChart>(input));

            return input;
        }

        public async Task<CircleChartDto> Get(int id)
        {
            return await IQGetCircleChart()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<long> Delete(long id)
        {
            var currentCircleChart = await _ws.GetAll<CircleChart>()
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();

            if (currentCircleChart == default)
            {
                throw new UserFriendlyException($"Can't find circle chart with id {id}");
            }

            var currentDetail = await _ws.GetAll<CircleChartDetail>()
                .Where(x => x.CircleChartId == id)
                .ToListAsync();

            foreach (var item in currentDetail)
            {
                item.IsDeleted = true;
            }

            currentCircleChart.IsDeleted = true;

            await CurrentUnitOfWork.SaveChangesAsync();

            return id;
        }

        public async Task<long> Active(long id)
        {
            var entity = _ws.Get<CircleChart>(id);
            entity.IsActive = true;

            await _ws.UpdateAsync(entity);

            return id;
        }

        public async Task<long> DeActive(long id)
        {
            var entity = _ws.Get<CircleChart>(id);
            entity.IsActive = false;

            await _ws.UpdateAsync(entity);

            return id;
        }
    }
}
