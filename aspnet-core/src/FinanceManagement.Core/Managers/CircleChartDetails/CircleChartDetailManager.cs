using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Managers.LineChartSettings.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace FinanceManagement.Managers.CircleChartDetails
{
    public class CircleChartDetailManager : DomainManager, ICircleChartDetailManager
    {
        public CircleChartDetailManager(IWorkScope ws) : base(ws)
        {
        }


        public IQueryable<CircleChartDetailDto> IQGetCircleChartDetails(bool isIncome)
        {
            var clientDictionary = GetClientFromCircleChartDetail();

            var inOutcomeDictionary = GetInOutcomeFromCircleChartDetail(isIncome);

            var query = _ws.GetAll<CircleChartDetail>()
                .Select(x => new CircleChartDetailDto
                {
                    Id = x.Id,
                    CircleChartId = x.CircleChartId,
                    Name = x.Name,
                    BranchId = x.Branch != null ? x.BranchId : null,
                    BranchName = x.Branch != null ? x.Branch.Name : null,
                    Color = x.Color,
                    IsActive = x.IsActive,
                    Clients = clientDictionary.ContainsKey(x.Id) ? clientDictionary[x.Id] : null,
                    InOutcomes = inOutcomeDictionary.ContainsKey(x.Id) ? inOutcomeDictionary[x.Id] : null,
                });
            return query;
        }

        public async Task<List<CircleChartDetailDto>> GetAllCircleChartDetails(bool isIncome)
        {
            return await IQGetCircleChartDetails(isIncome).ToListAsync();
        }

        public async Task<List<CircleChartDetailDto>> GetCircleChartDetailsById(long chartId)
        {
            var circleChart = _ws.Get<CircleChart>(chartId);
            if (circleChart == null) 
                throw new UserFriendlyException("Không tồn tại biểu đồ tròn với id = " + chartId);
            return await IQGetCircleChartDetails(circleChart.IsIncome).Where(s => s.CircleChartId == chartId).ToListAsync();
        }

        public async Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input)
        {
            var entity = ObjectMapper.Map<CircleChartDetail>(input);
            entity.IsActive = true;
            await _ws.InsertAsync(entity);
            return input;
        }

        public async Task<UpdateCircleChartDetailDto> Update(UpdateCircleChartDetailDto input)
        {
            var entity = await _ws.GetAsync<CircleChartDetail>(input.Id);
            if (entity == null)
                throw new UserFriendlyException("Không tồn tại circleChartDetail với id = " + entity.Id);
            MapAndUpdateProperties(input, entity);
            await _ws.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            var currentCircleChartDetail = await _ws.GetAll<CircleChartDetail>()
               .Where(x => x.Id == id)
               .FirstOrDefaultAsync();

            if (currentCircleChartDetail == default)
            {
                throw new UserFriendlyException($"Can't find circle chart detail with id {id}");
            }

            currentCircleChartDetail.IsDeleted = true;

            await CurrentUnitOfWork.SaveChangesAsync();

            return id;
        }

        public async Task<long> Active(long id)
        {
            var entity = _ws.Get<CircleChartDetail>(id);
            entity.IsActive = true;

            await _ws.UpdateAsync(entity);

            return id;
        }

        public async Task<long> DeActive(long id)
        {
            var entity = _ws.Get<CircleChartDetail>(id);
            entity.IsActive = false;

            await _ws.UpdateAsync(entity);

            return id;
        }


        public Dictionary<long,List<ClientDto>> GetClientFromCircleChartDetail()
        {
            var dicClientIds = _ws.GetAll<CircleChartDetail>()
                .ToDictionary(x => x.Id,
                            x => x.ClientIds?.Split(',').Select(long.Parse).ToList());
            
            var allClientIds = dicClientIds.Values
                .Where(ids => ids != null)
                .SelectMany(ids => ids)
                .Distinct().ToList();

            var allClients = GetAccountByListId(allClientIds).ToList();

            return dicClientIds
                .ToDictionary(
                    s => s.Key,
                    s => allClients?.Where(client => s.Value != null && s.Value.Contains(client.ClientId)).ToList());
        }

        public Dictionary<long, List<InOutcomingDto>> GetInOutcomeFromCircleChartDetail(bool isIncome)
        {
            var dicInOutComeIds = _ws.GetAll<CircleChartDetail>()
                .ToDictionary(x => x.Id,
                            x => x.InOutcomingIds?.Split(',').Select(long.Parse).ToList());

            var allInOutComeIds = dicInOutComeIds.Values
                .Where(ids => ids != null)
                .SelectMany(ids => ids)
                .Distinct().ToList();

            var allInOutCome = GetInOutcomeByListId(allInOutComeIds, isIncome).ToList();

            return dicInOutComeIds
                .ToDictionary(
                    s => s.Key,
                    s => allInOutCome?.Where(InOutCome => s.Value != null && s.Value.Contains(InOutCome.InOutcomingId)).ToList());
        }

        public IQueryable<ClientDto> GetAccountByListId(List<long> clientId)
        {
            return _ws.GetAll<Account>()
                .Where(s => clientId.Contains(s.Id))
                .Select(s => new ClientDto
                {
                    ClientId = s.Id,
                    ClientName = s.Name,
                });
        }

        public IQueryable<InOutcomingDto> GetInOutcomeByListId(List<long> clientId, bool isIncome)
        {
            if (isIncome)
            {
                return _ws.GetAll<IncomingEntryType>()
                    .Where(s => clientId.Contains(s.Id))
                    .Select(s => new InOutcomingDto
                    {
                        InOutcomingId = s.Id,
                        InOutcomingName = s.Name,
                    });
            }
            return _ws.GetAll<OutcomingEntryType>()
                    .Where(s => clientId.Contains(s.Id))
                    .Select(s => new InOutcomingDto
                    {
                        InOutcomingId = s.Id,
                        InOutcomingName = s.Name,
                    });
        }

        private void MapAndUpdateProperties(UpdateCircleChartDetailDto input, CircleChartDetail entity)
        {
            ObjectMapper.Map(input, entity);

            if (input.ClientIds.IsNullOrEmpty())
            {
                entity.ClientIds = null;
            }
            else
            {
                entity.ClientIds = string.Join(",", input.ClientIds);
            }

            var checkInOutComingInput = entity.CircleChart.IsIncome ? IsIncomingTypeExisted(input.InOutcomingIds)
                                                                    : IsOutcomingTypeExisted(input.InOutcomingIds);
            if (checkInOutComingInput)
            {
                if (input.InOutcomingIds.IsNullOrEmpty())
                {
                    entity.InOutcomingIds = null;
                }
                else
                {
                    entity.InOutcomingIds = string.Join(",", input.InOutcomingIds);
                }
            }
        }

        public bool IsIncomingTypeExisted(List<long> incomingTypeIds)
        {
            if (incomingTypeIds == null) return true;
            foreach (var incomingTypeId in incomingTypeIds)
            {
                var incomingType = _ws.Get<IncomingEntryType>(incomingTypeId);
                if (incomingType == null)
                    throw new UserFriendlyException("Không tồn tại IncomingEntryType có Id = " + incomingTypeId);
            }
            return true;
        }

        public bool IsOutcomingTypeExisted(List<long> outcomingTypeIds)
        {
            if (outcomingTypeIds == null) return true;
            foreach (var outcomingTypeId in outcomingTypeIds)
            {
                var outcomingType = _ws.Get<OutcomingEntryType>(outcomingTypeId);
                if (outcomingType == null)
                    throw new UserFriendlyException("Không tồn tại OutcomingEntryType có Id = " + outcomingTypeId);
            }
            return true;
        }
    }
}
