using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Managers.Dashboards.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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


        public async Task<CircleChartInfoDto> GetCircleChartDetailsByChartId(long circleChartId)
        {
            var circleChartDetailInfoDto = await _ws.GetAll<CircleChart>()
                .Where(s => s.Id == circleChartId)
                .Select(s => new CircleChartInfoDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    IsIncome = s.IsIncome,
                    Details = s.CircleChartDetails.Select(x => new CircleChartDetailInfoDto
                    {
                        Id = x.Id,
                        CircleChartId = s.Id,
                        Name = x.Name,
                        Color = x.Color,
                        ClientIds = x.ClientIds,
                        InOutcomeTypeIds = x.InOutcomeTypeIds,
                        RevenueExpenseType = x.RevenueExpenseType,
                        BranchId = x.BranchId,
                        Branch = new BranchInfoDto
                        {
                            BranchId = x.Branch.Id,
                            BranchName = x.Branch.Name
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();

            circleChartDetailInfoDto.Details.ForEach(s =>
            {
                s.Clients = CreateClientInfoDto(circleChartDetailInfoDto, s.ListClientIds);
                s.InOutcomeTypes = CreateInOutcomeTypeDto(circleChartDetailInfoDto, s.ListInOutcomeTypeIds);
            });

            return circleChartDetailInfoDto;
        }

        public async Task<CircleChartDetailInfoDto> GetCircleChartDetailInfoById(long id)
        {
            return await _ws.GetAll<CircleChartDetail>()
                .Where(s => s.Id == id)
                .Select(s => new CircleChartDetailInfoDto
                {
                    Id = s.Id,
                    CircleChartId = s.CircleChartId, 
                    Name = s.Name,
                    Color = s.Color,
                    RevenueExpenseType = s.RevenueExpenseType,
                    BranchId = s.BranchId,
                    ClientIds = s.ClientIds,
                    InOutcomeTypeIds = s.InOutcomeTypeIds,
                    Branch = new BranchInfoDto
                    {
                        BranchId = s.Branch.Id,
                        BranchName = s.Branch.Name
                    }
                })
                .FirstOrDefaultAsync();
        }

        public List<InOutcomeTypeDto> CreateInOutcomeTypeDto(CircleChartInfoDto circleChartDetailInfoDtos, List<long> ListInOutcomeTypeIds) {
            var resultList = new List<InOutcomeTypeDto>();
            if (circleChartDetailInfoDtos.IsIncome)
            {
                var dicInOutcomeTypeIdToInfo = _ws.GetAll<IncomingEntryType>().Where(s => circleChartDetailInfoDtos.AllInOutcomeTypeIds.Contains(s.Id))
                    .Select(s => new InOutcomeTypeDto { InOutcomeTypeId = s.Id, InOutcomeTypeName = s.Name })
                    .AsNoTracking()
                    .ToDictionary(s => s.InOutcomeTypeId);

                foreach (var inOutcomeTypeId in ListInOutcomeTypeIds)
                {
                    if (dicInOutcomeTypeIdToInfo.ContainsKey(inOutcomeTypeId))
                    {
                        resultList.Add(dicInOutcomeTypeIdToInfo[inOutcomeTypeId]);
                    }
                }
            } else
            {
                var dicInOutcomeTypeIdToInfo = _ws.GetAll<OutcomingEntryType>().Where(s => circleChartDetailInfoDtos.AllInOutcomeTypeIds.Contains(s.Id))
                    .Select(s => new InOutcomeTypeDto { InOutcomeTypeId = s.Id, InOutcomeTypeName = s.Name})
                    .AsNoTracking()
                    .ToDictionary(s => s.InOutcomeTypeId);

                foreach (var clientId in ListInOutcomeTypeIds) {
                    if (dicInOutcomeTypeIdToInfo.ContainsKey(clientId)){
                        resultList.Add(dicInOutcomeTypeIdToInfo[clientId]);
                    }
                }
            }
            return resultList;
        }

        public List<ClientInfoDto> CreateClientInfoDto(CircleChartInfoDto circleChartDetailInfoDto, List<long> ListClientIds)
        {

            var dicClientIdToInfo = _ws.GetAll<Account>().Where(s => circleChartDetailInfoDto.AllClientIds.Contains(s.Id))
                .Select(s => new ClientInfoDto { ClientId = s.Id, ClientName = s.Name + " [" + s.Code + "]" })
                .AsNoTracking()
                .ToDictionary(s => s.ClientId);

            var resultList = new List<ClientInfoDto>();

            foreach (var clientId in ListClientIds)
            {
                if (dicClientIdToInfo.ContainsKey(clientId))
                {
                    resultList.Add(dicClientIdToInfo[clientId]);
                }
            }
            return resultList;
        }


        public async Task<CreateCircleChartDetailDto> Create(CreateCircleChartDetailDto input)
        {
            var entity = ObjectMapper.Map<CircleChartDetail>(input);
            await _ws.InsertAsync(entity);
            return input;
        }

        public async Task<UpdateCircleChartDetailDto> Update(UpdateCircleChartDetailDto input)
        {
            var entity = await _ws.GetAsync<CircleChartDetail>(input.Id);
            if (entity == null)
                throw new UserFriendlyException("Không tồn tại circleChartDetail với id = " + entity.Id);

            ObjectMapper.Map(input, entity);
            entity.ClientIds = (input.ClientIds.IsNullOrEmpty())
                ? null
                : JsonConvert.SerializeObject(input.ClientIds);

            await _ws.UpdateAsync(entity);
            return input;
        }

        public async Task<UpdateCircleChartInOutcomeTypeIdsDto> UpdateInOutcomeTypeIds(UpdateCircleChartInOutcomeTypeIdsDto input)
        {
            var entity = await _ws.GetAsync<CircleChartDetail>(input.Id);
            if (entity == null)
                throw new UserFriendlyException("Không tồn tại circleChartDetail với id = " + entity.Id);

            ObjectMapper.Map(input, entity);
            entity.InOutcomeTypeIds = (input.InOutcomeTypeIds.IsNullOrEmpty())
                ? null
                : JsonConvert.SerializeObject(input.InOutcomeTypeIds);

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
    }
}
