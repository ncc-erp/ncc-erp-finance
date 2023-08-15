using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.APIs.OutcomingEntryTypes.Dto;
using FinanceManagement.APIs.RoleOutcomingTypes.Dto;
using Abp.Authorization.Users;
using FinanceManagement.Authorization.Users;
using Microsoft.AspNetCore.Http;
using FinanceManagement.Enums;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.IoC;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using Abp.Linq.Extensions;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Extension;
using Abp.Collections.Extensions;
using System.Linq.Expressions;

namespace FinanceManagement.APIs.AccountTypes
{
    [AbpAuthorize]
    public class OutcomingEntryTypeAppService : FinanceManagementAppServiceBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly ICommonManager _commonManager;

        public OutcomingEntryTypeAppService(IHttpContextAccessor httpContextAccessor, ICommonManager commonManager, RoleManager roleManager, UserManager userManager, IWorkScope workScope) : base(workScope)
        {
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            _userManager = userManager;
            _commonManager = commonManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Create)]
        public async Task<OutcomingEntryTypeDto> Create(OutcomingEntryTypeDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var alreadyExistParent = WorkScope.GetAll<OutcomingEntry>()
                    .Where(x => x.OutcomingEntryTypeId == input.ParentId)
                    .Select(x => x.OutcomingEntryType.Name)
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(alreadyExistParent))
                {
                    throw new UserFriendlyException($"Không thể tạo thêm loại chi vì loại chi <b style='font-weight: 700;'>{alreadyExistParent}</b> đã tồn tại Request chi");
                }
                //Name and code accountype are unique
                var nameExist = await WorkScope.GetAll<OutcomingEntryType>().AnyAsync(s => s.Name == input.Name);
                var codeExist = await WorkScope.GetAll<OutcomingEntryType>().AnyAsync(s => s.Code == input.Code);
                if (nameExist)
                {
                    throw new UserFriendlyException("Tên loại chi đã tồn tại");
                }
                if (codeExist)
                {
                    throw new UserFriendlyException("Mã loại chi đã tồn tại");
                }
            }
            
            OutcomingEntryType item = ObjectMapper.Map<OutcomingEntryType>(input);
            item.IsActive = true;
            input.Id = await WorkScope.InsertAndGetIdAsync(item);
            CurrentUnitOfWork.SaveChanges();
            var Outcoming = await WorkScope.GetAsync<OutcomingEntryType>(input.Id);
            if (input.ParentId.HasValue)
            {
                var parent = await WorkScope.GetAsync<OutcomingEntryType>(input.ParentId.Value);
                Outcoming.PathName = parent.PathName + input.Name + "|";
                Outcoming.Level = parent.Level + 1;
            }
            else
            {
                Outcoming.PathName = "|" + input.Name + "|";
                Outcoming.Level = 1;
            }
            input.PathName = Outcoming.PathName;
            input.Level = Outcoming.Level;
            await WorkScope.UpdateAsync<OutcomingEntryType>(Outcoming);

            string[] roleNames = { Constants.ROLE_KETOAN, Constants.ROLE_ADMIN, Constants.ROLE_SUPPORT_KE_TOAN };
            var roleIds = await WorkScope.GetAll<Role, int>().Where(s => roleNames.Contains(s.Name)).Select(s => s.Id).ToListAsync();
            var listUserIds = await WorkScope.All<UserRole>().Where(s => roleIds.Contains(s.RoleId)).Select(x => x.UserId).Distinct().ToListAsync();

            foreach (var userId in listUserIds)
            {
                var autoAddType = new UserOutcomingType
                {
                    UserId = userId,
                    OutcomingEntryTypeId = input.Id
                };
                await WorkScope.InsertAndGetIdAsync(autoAddType);
            }

            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit)]
        public async Task<UpdateOutcomingEntryTypeDto> Update(UpdateOutcomingEntryTypeDto input)
        {
            var item = await WorkScope.GetAsync<OutcomingEntryType>(input.Id);
            var isParentChanged = item.ParentId != input.ParentId;
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                if (await WorkScope.GetAll<OutcomingEntryType>().AnyAsync(x => x.Id != input.Id && input.Code == x.Code))
                    throw new UserFriendlyException("Mã loại chi đã tồn tại");
                var alreadyExistParent = WorkScope.GetAll<OutcomingEntry>()
                    .Where(x => x.OutcomingEntryTypeId == input.ParentId)
                    .Select(x => x.OutcomingEntryType.Name)
                    .FirstOrDefault();
                if (isParentChanged && !string.IsNullOrEmpty(alreadyExistParent))
                {
                    throw new UserFriendlyException($"Không thể tạo thêm loại chi vì loại chi <b style='font-weight: 700;'>{alreadyExistParent}</b> đã tồn tại Request chi");
                }
            }
            

            var oldName = item.Name.Trim();

            if (input.IsActive != item.IsActive)
            {
                await ChangeActive(item.Id, input.IsActive);
            }

            ObjectMapper.Map<UpdateOutcomingEntryTypeDto, OutcomingEntryType>(input, item);
            await WorkScope.UpdateAsync<OutcomingEntryType>(item);

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
                    var parent = await WorkScope.GetAsync<OutcomingEntryType>(item.ParentId.Value);
                    parentPathName = parent.PathName;
                    level = parent.Level + 1;
                    item.Level = level;
                }
                await ChangePathNameWhenChangeParent(item.Name, parentPathName);
            }
            await WorkScope.UpdateAsync<OutcomingEntryType>(item);
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

        private async Task<Object> ChangePathNameWhenChangeName(string name, string newName)
        {
            var list = await WorkScope.GetAll<OutcomingEntryType>()
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

            var list = await WorkScope.GetAll<OutcomingEntryType>()
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
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType)]
        public async Task<List<OutcomingEntryTypeDto>> GetAll(InputFilterOutcomingEntryTypeDto input)
        {
            var allOutcoming = await WorkScope.GetAll<OutcomingEntryType>()
                .Select(s => new OutcomingEntryTypeDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    PathName = s.PathName,
                    Level = s.Level,
                    ParentId = s.ParentId,
                    WorkflowId = s.WorkflowId,
                    IsActive = s.IsActive,
                    ExpenseType = s.ExpenseType.Value
                }).ToListAsync();
            if (input.IsGetAll())
            {
                return allOutcoming;
            }

            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(allOutcoming as IEnumerable<OutputCategoryEntryType>);

            var outcomingEntryTypeIds = allOutcoming
               .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
               .WhereIf(input.ExpenseType.HasValue, x => x.ExpenseType == input.ExpenseType)
               .WhereIf(!string.IsNullOrEmpty(input.SearchText), x => x.Name.ToLower().Contains(input.SearchText.ToLower()))
               .Select(x => x.Id)
               .ToList();

            var resultIds = new List<long>();
            foreach (var id in outcomingEntryTypeIds)
            {
                if (resultIds.Contains(id)) continue;

                resultIds.AddRange(_commonManager.GetAllEntryUpperNodeIds(id, treeHasRoot));
                if (input.IsGetAllNodeUpperAndLower())
                {
                    resultIds.AddRange(_commonManager.GetAllEntryLowerNodeIds(id, treeHasRoot));
                }
            }
            resultIds = resultIds.Distinct().ToList();

            return allOutcoming.Where(x => resultIds.Contains(x.Id)).ToList();
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Delete)]
        public async Task Delete(long id)
        {
            var OutcomingEntryType = await WorkScope.GetAll<OutcomingEntryType>().FirstOrDefaultAsync(s => s.Id == id);
            if (OutcomingEntryType == null)
            {
                throw new UserFriendlyException("Account Type doesn't exist");
            }
            var hasEntry = await WorkScope.GetRepo<OutcomingEntry>().GetAllIncluding(x => x.OutcomingEntryType)
                                    .AnyAsync(x => x.OutcomingEntryTypeId == id);
            if (hasEntry)
            {
                throw new UserFriendlyException("Can not delete Outcoming Entry Type when have linked Outcoming Entry");
            }
            await WorkScope.DeleteAsync<OutcomingEntryType>(id);
        }
        [HttpGet]
        public async Task<OutcomingEntryTypeDto> Get(long id)
        {
            return await WorkScope.GetAll<OutcomingEntryType>().Where(s => s.Id == id).Select(s => new OutcomingEntryTypeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                PathName = s.PathName,
                Level = s.Level,
                ParentId = s.ParentId,
                WorkflowId = s.WorkflowId,
                ExpenseType = s.ExpenseType.Value
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        // [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_ViewAll)]
        public async Task<List<OutcomingEntryTypeDto>> GetAllByUser()
        {
            var listTypes = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == AbpSession.UserId.Value)
                                  .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            return await WorkScope.GetAll<OutcomingEntryType>().Where(oet => listTypes.Contains(oet.Id))
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Level = x.Level,
                            ParentId = x.ParentId,
                            PathName = x.PathName,
                            ExpenseType = x.ExpenseType.Value
                        }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<OutcomingEntryTypeDto>> GetAllForDropdownByUser()
        {
            var listTypes = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == AbpSession.UserId.Value)
                                   .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            return await WorkScope.GetAll<OutcomingEntryType>().Where(oet => listTypes.Contains(oet.Id))
                        .Where(x => x.IsActive)
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Level = x.Level,
                            ParentId = x.ParentId,
                            PathName = x.PathName,
                            ExpenseType = x.ExpenseType.Value
                        }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<OutcomingEntryTypeDto>> GetExistOutComeInChartSetting(long lineChartId)
        {
            var existInChartSetting = await WorkScope.GetAll<LineChartSetting>()
                .Where(x => x.LinechartId == lineChartId)
                .Select(x => x.ReferenceId)
                .ToListAsync();

            return await WorkScope.GetAll<OutcomingEntryType>().Where(x => existInChartSetting.Contains(x.Id))
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            ExpenseType = x.ExpenseType,
                            Level = x.Level,
                            ParentId = x.ParentId,
                            IsActive = x.IsActive
                        }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<OutcomingEntryTypeDto>> GetAllForDropdown()
        {
            return await WorkScope.GetAll<OutcomingEntryType>()
                        .Where(s => s.IsActive)
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Level = x.Level,
                            IsActive = x.IsActive,
                            ParentId = x.ParentId,
                            PathName = x.PathName,
                            ExpenseType = x.ExpenseType.Value
                        }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<OutcomingEntryTypeDto>> GetAllForLinechartSetting()
        {
            return await WorkScope.GetAll<OutcomingEntryType>()
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            IsActive = x.IsActive,
                            ExpenseType = x.ExpenseType.Value,
                            ParentId = x.ParentId,
                            Level = x.Level
                        }).ToListAsync();
        }

        [HttpGet]
        //  [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_ViewAll)]
        public async Task<List<OutcomingEntryTypeDto>> GetAllByUserId(long id)
        {
            var listTypes = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == id)
                                  .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            return await WorkScope.GetAll<OutcomingEntryType>().Where(oet => listTypes.Contains(oet.Id))
                        .Select(x => new OutcomingEntryTypeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Level = x.Level,
                            ParentId = x.ParentId,
                            PathName = x.PathName,
                            ExpenseType = x.ExpenseType.Value
                        }).ToListAsync();
        }
        [HttpGet]
        // [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_ViewAll)]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetAllByUserIdNew(long id)
        {
            return await _commonManager.GetTreeOutcomingTypeByUserId(id, true);
        }
        [HttpGet]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetAllForDropdownByUserNew(bool isShowAll)
        {
            return await _commonManager.GetTreeOutcomingTypeByUserId(AbpSession.UserId.Value, isShowAll);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit, PermissionNames.Admin_User_Edit)]
        public async Task<UserOutcomingTypeDto> AddOutcomingTypeToUser(UserOutcomingTypeDto input)
        {
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<UserOutcomingType>(input));
            CurrentUnitOfWork.SaveChanges();

            return input;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit, PermissionNames.Admin_User_Edit)]
        public async Task AddAllOutcomingTypeToUser(UserOutcomingTypeDto input)
        {
            var listTypes = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == input.UserId)
                                 .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            var outcomings = await WorkScope.GetAll<OutcomingEntryType>()
                .Where(oet => !listTypes.Contains(oet.Id))
                .Select(s => new UserOutcomingType
                {
                    UserId = input.UserId,
                    OutcomingEntryTypeId = s.Id
                })
                .ToListAsync();

            await WorkScope.InsertRangeAsync(outcomings);
            CurrentUnitOfWork.SaveChanges();
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit, PermissionNames.Admin_User_Edit)]
        public async Task DeleteAllOutcomingTypeToUser(UserOutcomingTypeDto input)
        {
            var listTypes = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == input.UserId)
                                 .ToListAsync();
            await WorkScope.SoftDeleteRangeAsync(listTypes);
            CurrentUnitOfWork.SaveChanges();
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Delete, PermissionNames.Admin_User_Edit)]
        public async Task<UserOutcomingTypeDto> DeleteOutcomingTypeToUser(UserOutcomingTypeDto input)
        {
            var list = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == input.UserId && uot.OutcomingEntryTypeId == input.OutcomingEntryTypeId).ToListAsync();
            foreach (var child in list)
            {
                await WorkScope.DeleteAsync<UserOutcomingType>(child.Id);
            }
            CurrentUnitOfWork.SaveChanges();

            return input;
        }

        [HttpGet]
        public async Task<List<GetComboxExpenseTypeDto>> GetComboxExpenseTypes()
        {
            return new List<GetComboxExpenseTypeDto>
            {
                new GetComboxExpenseTypeDto{Value = (long)ExpenseType.NON_EXPENSE, Name =  "NON EXPENSE"},
                new GetComboxExpenseTypeDto{Value = (long)ExpenseType.REAL_EXPENSE, Name =  "REAL EXPENSE"},
            };
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit)]
        public async Task<long> Active(long id)
        {
            var treeNotRoot = _commonManager.GetTreeOutcomingEntries();
            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(treeNotRoot);

            var nodeAndLeafEntryTypeIdByParentIds = _commonManager
                .GetNodeIdsFromTree(id, treeNotRoot)
                .ToList();

            var allUpperNodeIds = _commonManager
                .GetAllEntryUpperNodeIds(id, treeHasRoot);

            var outcomingEntryTypeIds = nodeAndLeafEntryTypeIdByParentIds
                .Union(allUpperNodeIds);

            var outcomingTypes = await WorkScope.GetAll<OutcomingEntryType>()
                .Where(s => outcomingEntryTypeIds.Contains(s.Id) || s.Id == id)
               .ToListAsync();

            foreach (var item in outcomingTypes)
            {
                item.IsActive = true;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }


        [HttpGet]
        [AbpAuthorize(PermissionNames.Directory_OutcomingEntryType_Edit)]
        public async Task<long> DeActive(long id)
        {
            var nodeAndLeafEntryTypeIdByParentIds = _commonManager
                .GetAllNodeAndLeafEntryTypeIdByParentId<OutcomingEntryType>(new List<long> { id });

            var outcomingTypes = await WorkScope.GetAll<OutcomingEntryType>()
                .Where(s => nodeAndLeafEntryTypeIdByParentIds.Contains(s.Id) || s.Id == id)
               .ToListAsync();

            foreach (var item in outcomingTypes)
            {
                item.IsActive = false;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }

        [HttpPost]
        public async Task<IEnumerable<TreeItem<OutcomingEntryTypeByUserDto>>> GetOutcomingEntryTypeAllByUserId(InputFilterOutcomingEntryTypeByUserDto input)
        {
            var listOutcomingEntryTypeByUserId = await WorkScope.GetAll<UserOutcomingType>().Where(uot => uot.UserId == input.UserId)
                                  .Select(x => x.OutcomingEntryTypeId).ToListAsync();

            var listOutcomingEntryTypeInfo = await WorkScope.GetAll<OutcomingEntryType>()
                .Select(x => new OutcomingEntryTypeByUserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Level = x.Level,
                    ParentId = x.ParentId,
                    PathName = x.PathName,
                    ExpenseType = x.ExpenseType.Value,
                    IsActive = x.IsActive,
                }).ToListAsync();

            listOutcomingEntryTypeInfo.ForEach(s => s.IsGranted = listOutcomingEntryTypeByUserId.Contains(s.Id));

            if (input.IsGetAll())
            {
                var listOutcomingEntryType = listOutcomingEntryTypeInfo.WhereIf(input.IsGranted.HasValue, s => s.IsGranted).ToList();
                return listOutcomingEntryType.GenerateTree(c => c.Id, c => c.ParentId);
            }

            var treeHasRoot = _commonManager.GetTreeOutcomingEntryWithRoot(false);
            var inGrantedIds = listOutcomingEntryTypeInfo
                .WhereIf(string.IsNullOrEmpty(input.SearchText), x => x.IsGranted == input.IsGranted)
                .WhereIf(!string.IsNullOrEmpty(input.SearchText), x => x.Name.Trim().ToLower().Contains(input.SearchText.ToLower()))
                .Select(x => x.Id)
                .ToList();

            var resultIds = new List<long>();
            foreach (var idGranted in inGrantedIds)
            {
                resultIds.AddRange(_commonManager.GetAllEntryUpperNodeIds(idGranted, treeHasRoot));
            }
            resultIds = resultIds.Distinct().ToList();

            var listOutcomingEntryTypes = listOutcomingEntryTypeInfo
                .Where(x => resultIds.Contains(x.Id)).ToList();

            return listOutcomingEntryTypes.GenerateTree(c => c.Id, c => c.ParentId);
        }

        [HttpPost]
        public async Task UpdateOutcomingEntryTypeByUserId(UserOutcomingTypeDto input)
        {
            var listOutcomingEntryByUserId = await WorkScope.GetAll<UserOutcomingType>()
                .Where(s => s.UserId == input.UserId)
                .Select(s => s.OutcomingEntryTypeId)
                .ToListAsync();

            var nodeAndLeafEntryTypeIdByParentIds = _commonManager
                .GetAllNodeAndLeafEntryTypeIdByParentId<OutcomingEntryType>(new List<long> { input.OutcomingEntryTypeId })
                .ToList();

            var treeHasRoot = _commonManager
                .GetTreeOutcomingEntryWithRoot(false);

            var nodeParentEntryTypeIdByChildrenIds = _commonManager
                .GetAllEntryUpperNodeIds(input.OutcomingEntryTypeId, treeHasRoot);

            var listOutcomingEntryNotExistedUser = nodeAndLeafEntryTypeIdByParentIds
                .Where(s => !listOutcomingEntryByUserId.Contains(s))
                .ToList();

            if (!listOutcomingEntryNotExistedUser.IsNullOrEmpty())
            {
                await AddOutcomingEntryTypeIntoUser(nodeAndLeafEntryTypeIdByParentIds, nodeParentEntryTypeIdByChildrenIds, listOutcomingEntryByUserId, input);
            }
            else
            {
                await RemoveOutcomingEntryTypeOutOfUser(nodeAndLeafEntryTypeIdByParentIds, input);
            }

            CurrentUnitOfWork.SaveChanges();
        }
        /// <summary>
        /// thêm mới khi user chưa có loại chi đó (nếu là cha thì add toàn bộ con)
        /// </summary>
        /// <param name="nodeAndLeafEntryTypeIdByParentIds"></param>
        /// <param name="nodeParentEntryTypeIdByChildrenIds"></param>
        /// <param name="listOutcomingEntryByUserId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task AddOutcomingEntryTypeIntoUser(
            List<long> nodeAndLeafEntryTypeIdByParentIds,
            List<long> nodeParentEntryTypeIdByChildrenIds,
            List<long> listOutcomingEntryByUserId,
            UserOutcomingTypeDto input)
        {
            var listOutcomingEntryTypeChildren = QueryOutputCategoryEntries(x => nodeAndLeafEntryTypeIdByParentIds.Contains(x.Id) || x.Id == input.OutcomingEntryTypeId)
                   .AsEnumerable()
                   .Select(s => new UserOutcomingType
                   {
                       UserId = input.UserId,
                       OutcomingEntryTypeId = s.Id
                   })
                   .ToList();

            await WorkScope.InsertRangeAsync(listOutcomingEntryTypeChildren);

            var outComingEntryTypeParentId = await WorkScope.GetAll<OutcomingEntryType>()
                .Where(s => s.Id == input.OutcomingEntryTypeId)
                .Select(s => s.ParentId)
                .FirstOrDefaultAsync();

            if (outComingEntryTypeParentId == default || listOutcomingEntryByUserId.Contains(outComingEntryTypeParentId.Value))
            {
                return;
            }

            var listOutcomingEntryTypeParent = await QueryOutputCategoryEntries(x => nodeParentEntryTypeIdByChildrenIds.Contains(x.Id) || x.Id == input.OutcomingEntryTypeId)
               .Select(s => new UserOutcomingType
               {
                   UserId = input.UserId,
                   OutcomingEntryTypeId = s.Id
               })
               .ToListAsync();

            await WorkScope.InsertRangeAsync(listOutcomingEntryTypeParent);
        }
        /// <summary>
        /// xóa cha thì xóa tất con, nếu nó là lá và là tk con cuối cùng được chọn thì xóa cả cha và con
        /// </summary>
        /// <param name="nodeAndLeafEntryTypeIdByParentIds"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task RemoveOutcomingEntryTypeOutOfUser(List<long> nodeAndLeafEntryTypeIdByParentIds, UserOutcomingTypeDto input)
        {
            var listOutcomingEntryTypeByUser = await WorkScope.GetAll<UserOutcomingType>()
                .Where(s => s.UserId == input.UserId)
                .ToListAsync();

            var nodeAndLeafEntryTypeByUser = listOutcomingEntryTypeByUser
                .Where(s => nodeAndLeafEntryTypeIdByParentIds.Contains(s.OutcomingEntryTypeId));

            await WorkScope.SoftDeleteRangeAsync(nodeAndLeafEntryTypeByUser);

            var outcomingEntryTypeParentId = WorkScope.GetAll<OutcomingEntryType>()
                .Where(s => s.Id == input.OutcomingEntryTypeId)
                .Select(s => s.ParentId)
                .FirstOrDefault();

            var listOutcomingEntryType = await WorkScope.GetAll<OutcomingEntryType>()
                .Select(x => new OutputCategoryEntryType
                {
                    Id = x.Id,
                    ParentId = x.ParentId
                })
                .ToListAsync();

            if (!outcomingEntryTypeParentId.HasValue)
            {
                return;
            }

            var listKhongLaConCuaLoaiChiTheoUser = listOutcomingEntryTypeByUser
                .Where(s => !nodeAndLeafEntryTypeByUser.Contains(s))
                .ToList();
            await RemoveOutcomingEntryTypeByParentId(outcomingEntryTypeParentId.Value, listOutcomingEntryType, listKhongLaConCuaLoaiChiTheoUser);
        }
        private IQueryable<OutputCategoryEntryType> QueryOutputCategoryEntries(Expression<Func<OutputCategoryEntryType, bool>> expression)
        {
            return WorkScope.GetAll<OutcomingEntryType>()
                    .Select(x => new OutputCategoryEntryType
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                    })
                    .Where(expression);
        }
        private async Task RemoveOutcomingEntryTypeByParentId(
            long parentId,
            IEnumerable<OutputCategoryEntryType> listOutcomingEntryType,
            IEnumerable<UserOutcomingType> listUserOutcomingEntryType)
        {
            //lấy ra loại thu có cha là input:[parentId] của user 
            var listParentIdByUserOutcomingEntryType = listOutcomingEntryType
                .Where(s => s.ParentId == parentId)
                .Where(s => listUserOutcomingEntryType.Select(x => x.OutcomingEntryTypeId).Contains(s.Id))
                .ToList();

            //nếu tồn tại thì không xóa
            if (listParentIdByUserOutcomingEntryType.Any())
                return;

            var listUserOutcomingTypeByParentId = listUserOutcomingEntryType.Where(s => s.OutcomingEntryTypeId == parentId).ToList();

            if (!listUserOutcomingTypeByParentId.Any())
            {
                return;
            }

            await WorkScope.SoftDeleteRangeAsync(listUserOutcomingTypeByParentId);

            var grandparentId = listOutcomingEntryType.Where(s => s.Id == parentId).Select(s => s.ParentId).FirstOrDefault();
            if (grandparentId == default)
            {
                return;
            }

            var listUserOutcomingEntryTypeNew = listUserOutcomingEntryType.Where(s => s.OutcomingEntryTypeId != parentId);
            if (listUserOutcomingEntryTypeNew.Any())
            {
                await RemoveOutcomingEntryTypeByParentId(grandparentId.Value, listOutcomingEntryType, listUserOutcomingEntryTypeNew.ToList());
            }
        }
    }
}


