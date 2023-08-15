using Abp.Domain.Repositories;
using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using FinanceManagement.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.LineChartSettings
{
    public class LineChartSettingManager : DomainManager, ILineChartSettingManager
    {
        public LineChartSettingManager(IWorkScope ws) : base(ws)
        {
        }

        public async Task<long> Active(long id)
        {
            var entity = _ws.Get<LineChart>(id);
            entity.IsActive = true;

            await _ws.UpdateAsync(entity);

            return id;
        }

        public async Task<AddReferenceToLinechartDto> AddReferenceToLineChart(AddReferenceToLinechartDto input)
        {
            var entity = new LineChartSetting
            {
                LinechartId = input.LinechartId,
                ReferenceId = input.ReferenceId
            };
            await _ws.InsertAsync(entity);
            return input;
        }

        public async Task<CreateLineChartSettingDto> Create(CreateLineChartSettingDto input)
        {
            var entity = ObjectMapper.Map<LineChart>(input);
            entity.IsActive = true;

            await _ws.InsertAsync(entity);

            return input;
        }

        public async Task<long> DeActive(long id)
        {
            var entity = _ws.Get<LineChart>(id);
            entity.IsActive = false;

            await _ws.UpdateAsync(entity);

            return id;
        }

        public async Task<long> Delete(long id)
        {
            var currentLineChart = await _ws.GetAll<LineChart>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (currentLineChart == default)
            {
                throw new UserFriendlyException($"Can't find setting with id {id}");
            }

            var curentSetting = await _ws.GetAll<LineChartSetting>()
                .Where(x => x.LinechartId == id)
                .ToListAsync();

            foreach (var item in curentSetting)
            {
                item.IsDeleted = true;
            }

            currentLineChart.IsDeleted = true;

            await CurrentUnitOfWork.SaveChangesAsync();

            return id;
        }

        public async Task<LineChartSettingDto> Get(int id)
        {
            return await IQGetLineChartSetting()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LineChartSettingDto>> GetAll()
        {
            return await IQGetLineChartSetting().ToListAsync();
        }

        public Task<GridResult<LineChartSettingDto>> GetAllPaging(GridParam input)
        {
            var query = IQGetLineChartSetting();

            var dicIncome = _ws.GetAll<IncomingEntryType>()
                .ToDictionary(x => x.Id, x => x.Name);
            var dicOutcome = _ws.GetAll<OutcomingEntryType>()
                .ToDictionary(x => x.Id, x => x.Name);

            var settings = _ws.GetAll<LineChartSetting>()
                .ToList()
                .GroupBy(x => x.LinechartId)
                .Select(x => new
                {
                    x.Key,
                    references = x.Select(s => new ReferenceInfoDto
                    {
                        Id = s.ReferenceId,
                        Name = s.LineChart.Type == LineChartSettingType.Income
                        ? (dicIncome.ContainsKey(s.ReferenceId) ? dicIncome[s.ReferenceId] : null)
                        : (dicOutcome.ContainsKey(s.ReferenceId) ? dicOutcome[s.ReferenceId] : null)

                    }).ToList()
                })
                .ToDictionary(x => x.Key, x => x.references);

            var result = query.GetGridResult(query, input);

            foreach (var item in result.Result.Items)
            {
                if (settings.ContainsKey(item.Id))
                {
                    item.ListReference = settings[item.Id].ToList();
                }
            }

            return result;
        }


        public IQueryable<LineChartSettingDto> IQGetLineChartSetting()
        {
            return _ws.GetAll<LineChart>()
                .Select(x => new LineChartSettingDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Color = x.Color,
                    Type = x.Type,
                });
        }

        public async Task<UpdateLineChartSettingDto> Update(UpdateLineChartSettingDto input)
        {
            var existLinechart = _ws.GetAll<LineChart>()
                .Where(x => x.Id == input.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Type
                })
                .FirstOrDefault();

            if (input.Type != existLinechart.Type)
            {
                var currentSettings = _ws.GetAll<LineChartSetting>()
                    .Where(x => x.LinechartId == existLinechart.Id)
                    .ToList();

                if (currentSettings.Count > 0)
                {
                    throw new UserFriendlyException("Chart đã được gắn reference, hãy xóa reference trước khi edit type");
                }
            }

            await _ws.UpdateAsync(ObjectMapper.Map<LineChart>(input));

            return input;
        }

        public async Task<RemoveChartSettingDto> RemoveLineChartReference(RemoveChartSettingDto input)
        {
            var chartSetting = _ws.GetAll<LineChartSetting>()
                .Where(x => x.LinechartId == input.ChartSettingId)
                .Where(x => x.ReferenceId == input.ReferenceId)
                .FirstOrDefault();

            if (chartSetting == default)
            {
                throw new UserFriendlyException($"Can't find setting");
            }

            await _ws.DeleteAsync(chartSetting);

            return input;
        }
    }
}
