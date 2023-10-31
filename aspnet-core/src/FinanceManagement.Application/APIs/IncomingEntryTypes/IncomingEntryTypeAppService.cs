using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using FinanceManagement.APIs.AccountTypes.Dto;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.APIs.IncomingEntryTypes.Dto;
using FinanceManagement.IoC;
using Abp.Linq.Extensions;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.Commons;
using FinanceManagement.GeneralModels;
using Abp.Collections.Extensions;
using FinanceManagement.APIs.OutcomingEntryTypes.Dto;
using Newtonsoft.Json;

namespace FinanceManagement.APIs.AccountTypes
{
    [AbpAuthorize]
    public class IncomingEntryTypeAppService : FinanceManagementAppServiceBase
    {
        private readonly ICommonManager _commonManager;
        public IncomingEntryTypeAppService(ICommonManager commonManager, IWorkScope workScope) : base(workScope)
        {
            _commonManager = commonManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType_Create)]
        public async Task<IncomingEntryTypeDto> Create(IncomingEntryTypeDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                //Name and code accountype are unique
                var nameExist = await WorkScope.GetAll<IncomingEntryType>().AnyAsync(s => s.Name == input.Name);
                var codeExist = await WorkScope.GetAll<IncomingEntryType>().AnyAsync(s => s.Code == input.Code);
                if (nameExist)
                {
                    throw new UserFriendlyException("Tên loại thu đã tồn tại");
                }
                if (codeExist)
                {
                    throw new UserFriendlyException("Mã loại thu đã tồn tại");
                }
                var alreadyExistParent = WorkScope.GetAll<IncomingEntry>().Where(x => x.IncomingEntryTypeId == input.ParentId).FirstOrDefault();
                if (alreadyExistParent != default)
                {
                    throw new UserFriendlyException($"Không thể tạo thêm loại thu vì loại thu <b style='font-weight: 700;'>{alreadyExistParent.IncomingEntryType.Name}</b> đã tồn tại ghi nhận thu");
                }
            }

            IncomingEntryType item = ObjectMapper.Map<IncomingEntryType>(input);
            item.IsActive = true;
            input.Id = await WorkScope.InsertAndGetIdAsync(item);
            CurrentUnitOfWork.SaveChanges();
            var incoming = await WorkScope.GetAsync<IncomingEntryType>(input.Id);
            if (input.ParentId.HasValue)
            {
                var parent = await WorkScope.GetAsync<IncomingEntryType>(input.ParentId.Value);
                incoming.PathName = parent.PathName + input.Name + "|";
                incoming.Level = parent.Level + 1;
            }
            else
            {
                incoming.PathName = "|" + input.Name + "|";
                incoming.Level = 1;
            }
            input.PathName = incoming.PathName;
            input.Level = incoming.Level;
            await WorkScope.UpdateAsync<IncomingEntryType>(incoming);
            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType_Edit)]
        public async Task<UpdateIncomingEntryTypeDto> Update(UpdateIncomingEntryTypeDto input)
        {
            var item = await WorkScope.GetAsync<IncomingEntryType>(input.Id);
            var isParentChanged = item.ParentId != input.ParentId;

            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                if (await WorkScope.GetAll<IncomingEntryType>().Where(x => x.Id != input.Id && input.Code == x.Code).AnyAsync())
                    throw new UserFriendlyException("Mã loại thu đã tồn tại");

                var alreadyExistParent = WorkScope.GetAll<IncomingEntry>().Where(x => x.IncomingEntryTypeId == input.ParentId).FirstOrDefault();
                if (isParentChanged && alreadyExistParent != default)
                    throw new UserFriendlyException($"Không thể cập nhập thành loại thu <b style='font-weight: 700;'>{alreadyExistParent.IncomingEntryType.Name}</b> vì loại thu này đã tồn tại ghi nhận thu");
            }

            if (input.IsActive != item.IsActive)
            {
                await ChangeActive(item.Id, input.IsActive);
            }

            var oldName = item.Name.Trim();
            ObjectMapper.Map<UpdateIncomingEntryTypeDto, IncomingEntryType>(input, item);
            await WorkScope.UpdateAsync<IncomingEntryType>(item);

            //var parentChanged = await WorkScope.GetAll<IncomingEntryType>().AnyAsync(s => item.ParentId != input.ParentId);
            if (item.Name.Trim() != oldName)
            {
                await ChangePathNameWhenChangeName(oldName, input.Name.Trim());
            }

            if (isParentChanged)
            {
                string parentPathName = "|";
                long level = 1;

                if (item.ParentId.HasValue)
                {
                    var parent = await WorkScope.GetAsync<IncomingEntryType>(item.ParentId.Value);
                    parentPathName = parent.PathName;
                    level = parent.Level + 1;
                    item.Level = level;
                }

                await ChangePathNameWhenChangeParent(item.Name, parentPathName);


            }
            await WorkScope.UpdateAsync<IncomingEntryType>(item);
            return input;
        }
        private async Task ChangeActive(long id, bool isActive)
        {
            if (isActive)
            {
                await Active(id);
            }
            else
            {
                await DeActive(id);
            }
        }

        private Task ChangeParent(string name, object pathName)
        {
            throw new NotImplementedException();
        }

        private async Task<Object> ChangePathNameWhenChangeName(string name, string newName)
        {
            var list = await WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.PathName.Contains("|" + name + "|"))
                .ToListAsync();
            foreach (var child in list)
            {
                child.PathName = child.PathName.Replace("|" + name + "|", "|" + newName + "|");
                await WorkScope.UpdateAsync(child);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return null;
        }

        private async Task<Object> ChangePathNameWhenChangeParent(string name, string parentPathName)
        {

            var list = await WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.PathName.Contains("|" + name + "|"))
                //.Select(s => s)
                .ToListAsync();
            foreach (var child in list)
            {
                child.PathName = parentPathName + child.PathName.Substring(child.PathName.LastIndexOf($"|{name}|") + 1);
                child.Level = child.PathName.Split("|").Length - 2;

                await WorkScope.UpdateAsync(child);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return null;

        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType)]
        public async Task<List<IncomingEntryTypeDto>> GetAll(InputFilterIncomingEntryTypeDto input)
        {
            var allIncoming = await WorkScope.GetAll<IncomingEntryType>()
                .Select(s => new IncomingEntryTypeDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    PathId = s.PathId,
                    PathName = s.PathName,
                    Level = s.Level,
                    IsActive = s.IsActive,
                    ParentId = s.ParentId,
                    RevenueCounted = s.RevenueCounted,
                    IsClientPaid= s.IsClientPaid,
                    IsClientPrePaid = s.IsClientPrePaid,
                })
                .ToListAsync();
            if (input.IsGetAll())
            {
                return allIncoming.ToList();
            }

            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(allIncoming as IEnumerable<OutputCategoryEntryType>);

            var incomingEntryTypeIds = allIncoming
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                .WhereIf(input.RevenueCounted.HasValue, x => x.RevenueCounted == input.RevenueCounted)
                .WhereIf(!string.IsNullOrEmpty(input.SearchText), x => x.Name.ToLower().Contains(input.SearchText.ToLower()))
                .Select(x => x.Id)
                .ToList();

            var resultIds = new List<long>();
            foreach (var id in incomingEntryTypeIds)
            {
                if (resultIds.Contains(id)) continue;

                resultIds.AddRange(_commonManager.GetAllEntryUpperNodeIds(id, treeHasRoot));
                if (input.IsGetAllNodeUpperAndLower())
                {
                    resultIds.AddRange(_commonManager.GetAllEntryLowerNodeIds(id, treeHasRoot));
                }
            }
            resultIds = resultIds.Distinct().ToList();

            return allIncoming.Where(x => resultIds.Contains(x.Id)).ToList();
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType_Delete)]
        public async Task Delete(long id)
        {
            var IncomingEntryType = await WorkScope.GetAll<IncomingEntryType>().FirstOrDefaultAsync(s => s.Id == id);
            // TODO: validation when has Entry var hasEntry = await WorkScope.GetAll<IncomingEntry>().AnyAsync(m => m.IncomingEntryTypeId == id);
            if (IncomingEntryType == null)
            {
                throw new UserFriendlyException("Account Type doesn't exist");
            }
            var hasParentId = await WorkScope.GetAll<IncomingEntryType>().AnyAsync(x => id == x.ParentId);
            if (hasParentId)
            {
                throw new UserFriendlyException("IncomingEntryType has ParentId");
            }
            // if (hasEntry)
            // {
            //     throw new UserFriendlyException("Can't delete account type when you have linked account");
            // }
            var hasIncomingEntries = await WorkScope.GetAll<IncomingEntry>().AnyAsync(ie => ie.IncomingEntryTypeId == id);
            if (hasIncomingEntries)
            {
                throw new UserFriendlyException("Can not delete Incoming Entry type when you have linked Incoming entries");
            }
            await WorkScope.DeleteAsync<IncomingEntryType>(id);
        }
        [HttpGet]
        public async Task<IncomingEntryTypeDto> Get(long id)
        {
            return await WorkScope.GetAll<IncomingEntryType>().Where(s => s.Id == id).Select(s => new IncomingEntryTypeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                PathId = s.PathId,
                PathName = s.PathName,
                Level = s.Level,
                ParentId = s.ParentId,
                RevenueCounted = s.RevenueCounted,
                IsClientPaid = s.IsClientPaid,
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<List<IncomingEntryTypeDto>> GetExistInComeInChartSetting(long lineChartId)
        {
            var existInChartSetting = await WorkScope.GetAll<LineChartSetting>()
                .Where(x => x.LinechartId == lineChartId)
                .Select(x => x.ReferenceId)
                .ToListAsync();

            return await WorkScope.GetAll<IncomingEntryType>().Where(x => existInChartSetting.Contains(x.Id))
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => new IncomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            IsActive = x.IsActive,
                            RevenueCounted = x.RevenueCounted
                        }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<IncomingEntryTypeDto>> GetExistInComeInCircleChartDetail(long id)
        {
            var inOutComeTypeIds = WorkScope.Get<CircleChartDetail>(id).InOutcomeTypeIds;
            var listInOutcomeTypeIds = (string.IsNullOrWhiteSpace(inOutComeTypeIds))
                ? new List<long>()
                : JsonConvert.DeserializeObject<List<long>>(inOutComeTypeIds);

            return await WorkScope.GetAll<IncomingEntryType>().Where(x => listInOutcomeTypeIds.Contains(x.Id))
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => new IncomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            IsActive = x.IsActive,
                            RevenueCounted = x.RevenueCounted
                        }).ToListAsync();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType_Edit)]
        public async Task<long> Active(long id)
        {
            var treeNotRoot = _commonManager.GetTreeIncomingEntries();
            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(treeNotRoot);

            var nodeAndLeafEntryTypeIdByParentIds = _commonManager
                .GetNodeIdsFromTree(id, treeNotRoot)
                .ToList();

            var allUpperNodeIds = _commonManager
                .GetAllEntryUpperNodeIds(id, treeHasRoot);

            var incomingEntryTypeIds = nodeAndLeafEntryTypeIdByParentIds
                .Union(allUpperNodeIds);

            var incomingType = await WorkScope.GetAll<IncomingEntryType>()
                .Where(s => incomingEntryTypeIds.Contains(s.Id) || s.Id == id)
               .ToListAsync();

            foreach (var item in incomingType)
            {
                item.IsActive = true;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }


        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_IncomingEntryType_Edit)]
        public async Task<long> DeActive(long id)
        {
            var nodeAndLeafEntryTypeIdByParentIds = _commonManager
                .GetAllNodeAndLeafEntryTypeIdByParentId<IncomingEntryType>(new List<long> { id });

            var incomingType = await WorkScope.GetAll<IncomingEntryType>()
                .Where(s => nodeAndLeafEntryTypeIdByParentIds.Contains(s.Id) || s.Id == id)
               .ToListAsync();

            foreach (var item in incomingType)
            {
                item.IsActive = false;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }

        [HttpGet]
        public async Task<List<IncomingEntryTypeDto>> GetAllForDropdown()
        {
            return await WorkScope.GetAll<IncomingEntryType>()
                        .Where(s => s.IsActive)
                        .Select(x => new IncomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Level = x.Level,
                            IsActive = x.IsActive,
                            ParentId = x.ParentId,
                            PathName = x.PathName,
                            RevenueCounted = x.RevenueCounted
                        }).ToListAsync();
        }
    }
}


