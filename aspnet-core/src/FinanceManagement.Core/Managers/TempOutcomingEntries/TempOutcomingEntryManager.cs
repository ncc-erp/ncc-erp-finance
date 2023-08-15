using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Runtime.Session;
using Abp.UI;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using FinanceManagement.Uitls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.TempOutcomingEntries
{
    public class TempOutcomingEntryManager : DomainManager, ITempOutcomingEntryManager
    {
        private readonly IAbpSession _session;
        private readonly IPermissionChecker _permissionChecker;
        private readonly ICommonManager _commonManager;
        public TempOutcomingEntryManager(IWorkScope ws, IAbpSession session, IPermissionChecker permissionChecker, ICommonManager commonManager) : base(ws)
        {
            _session = session;
            _permissionChecker = permissionChecker;
            _commonManager = commonManager;
        }
        public async Task<bool> IsExistTempOutComingEntry(long outcommingEntryId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == outcommingEntryId)
                .Where(x => x.WorkflowStatus.Code.Trim() != FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .AnyAsync();
        }
        public async Task<bool> IsExistTempOutcomingEntryPendingCEO(long outcommingEntryId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == outcommingEntryId)
                .Where(x => x.WorkflowStatus.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                .AnyAsync();
        }
        public async Task<GetTempOutcomingEntryDto> CreateTempOutCommingEntry(long outcommingEntryId)
        {
            var isExistTemp = await IsExistTempOutComingEntry(outcommingEntryId);
            if (isExistTemp)
                throw new UserFriendlyException("Đã tồn tại Request Change");

            var hasOriginal = await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == outcommingEntryId && x.IsOriginal)
                .AnyAsync();

            var workflowStatusStart = await _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code == FinanceManagementConsts.WORKFLOW_STATUS_START)
                .FirstOrDefaultAsync();

            var outcomingEntry = await _ws.GetAll<OutcomingEntry>()
                .Include(x => x.OutcomingEntryDetails)
                .Where(x => x.Id == outcommingEntryId)
                .FirstOrDefaultAsync();

            //if not exists root outcoming -> insert with prop IsOriginal = true
            if (!hasOriginal)
            {
                var rootTempOutcomingEntry = ObjectMapper.Map(outcomingEntry, new TempOutcomingEntry());
                rootTempOutcomingEntry.Id = 0;
                rootTempOutcomingEntry.RootOutcomingEntryId = outcommingEntryId;
                rootTempOutcomingEntry.IsOriginal = true;
                rootTempOutcomingEntry.CreationTime = DateTimeUtils.GetNow();
                rootTempOutcomingEntry.LastModificationTime = DateTimeUtils.GetNow();
                var newId = await _ws.InsertAndGetIdAsync(rootTempOutcomingEntry);

                foreach (var outcomingEntryDetail in outcomingEntry.OutcomingEntryDetails)
                {
                    var tempOutcomingEntryDetail = ObjectMapper.Map(outcomingEntryDetail, new TempOutcomingEntryDetail());
                    tempOutcomingEntryDetail.RootOutcomingEntryDetailId = outcomingEntryDetail.Id;
                    tempOutcomingEntryDetail.Id = 0;
                    tempOutcomingEntryDetail.RootTempOutcomingEntryId = newId;
                    tempOutcomingEntryDetail.CreationTime = DateTimeUtils.GetNow();
                    tempOutcomingEntryDetail.LastModificationTime = DateTimeUtils.GetNow();
                    await _ws.InsertAsync(tempOutcomingEntryDetail);
                }
            }

            //create new temp from root
            var tempOutcomingEntry = ObjectMapper.Map(outcomingEntry, new TempOutcomingEntry());
            tempOutcomingEntry.RootOutcomingEntryId = outcommingEntryId;
            tempOutcomingEntry.Id = 0;
            tempOutcomingEntry.WorkflowStatusId = workflowStatusStart.Id;
            tempOutcomingEntry.WorkflowStatus = workflowStatusStart;
            tempOutcomingEntry.CreationTime = DateTimeUtils.GetNow();
            tempOutcomingEntry.LastModificationTime = DateTimeUtils.GetNow();
            var tempOutcomingEntryId = await _ws.InsertAndGetIdAsync(tempOutcomingEntry);

            //clone temp detail from root detail
            foreach (var outcomingEntryDetail in outcomingEntry.OutcomingEntryDetails)
            {
                var tempOutcomingEntryDetail = ObjectMapper.Map(outcomingEntryDetail, new TempOutcomingEntryDetail());
                tempOutcomingEntryDetail.RootOutcomingEntryDetailId = outcomingEntryDetail.Id;
                tempOutcomingEntryDetail.Id = 0;
                tempOutcomingEntryDetail.RootTempOutcomingEntryId = tempOutcomingEntryId;
                tempOutcomingEntryDetail.CreationTime = DateTimeUtils.GetNow();
                tempOutcomingEntryDetail.LastModificationTime = DateTimeUtils.GetNow();
                await _ws.InsertAsync(tempOutcomingEntryDetail);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetTempOutcomingEntry(tempOutcomingEntryId);
        }
        public async Task<RequestChangeOutcomingEntryInfoDto> GetRequestChangeOutcomingEntry(long tempOutcomingEntryId)
        {
            var rootId = await GetOutcomingEntryIdByTempId(tempOutcomingEntryId);

            return new RequestChangeOutcomingEntryInfoDto
            {
                RootOutcomingEntry = await GetRootOutcomingEntry(rootId),
                TempOutcomingEntry = await GetTempOutcomingEntry(tempOutcomingEntryId)
            };
        }
        public async Task<RequestChangeOutcomingEntryInfoDto> ViewHistoryChangeOutcomingEntry(long tempId, long rootId)
        {
            var listTemp = await IQGetTempOutcomingEntry()
                .Where(x => x.RootOutcomingEntryId == rootId)
                .OrderBy(x => x.LastModifiedTime ?? DateTime.MinValue)
                .ToArrayAsync();

            for (int index = 0; index < listTemp.Length; index++)
            {
                if (listTemp[index].Id != tempId) continue;
                return new RequestChangeOutcomingEntryInfoDto
                {
                    RootOutcomingEntry = ObjectMapper.Map<GetOutcomingEntryDto>(listTemp[index - 1]),
                    TempOutcomingEntry = listTemp[index]
                };
            }

            throw new UserFriendlyException("Không tìm thấy lịch sử request change.");
        }
        /// <summary>
        /// Get YCTĐ có trạng thái khác APPROVED
        /// </summary>
        /// <param name="outcomingEntryId"></param>
        /// <returns></returns>
        public async Task<long> GetTempIdByOutcomingId(long outcomingEntryId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == outcomingEntryId)
                .Where(x => x.WorkflowStatus.Code != FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<long> GetTempIdHaveStatusPendingByOutcomingId(long outcomingEntryId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == outcomingEntryId)
                .Where(x => x.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<GetTempOutcomingEntryDto> GetTempOutcomingEntry(long tempOutcomingEntryId)
        {
            return await IQGetTempOutcomingEntry()
                .Where(x => x.Id == tempOutcomingEntryId)
                .FirstOrDefaultAsync();
        }
        private IQueryable<GetTempOutcomingEntryDto> IQGetTempOutcomingEntry()
        {
            var roledIds = _ws.GetAll<UserRole>()
                          .Where(s => s.UserId == _session.UserId)
                          .Select(s => s.RoleId);

            var permission = _ws.GetAll<WorkflowStatusTransitionPermission>()
                .Where(x => roledIds.Contains(x.RoleId));

            var requestInBankTransaction = _ws.GetAll<OutcomingEntryBankTransaction>()
                                           .GroupBy(oe => oe.OutcomingEntryId)
                                           .Select(oe => new
                                           {
                                               a = oe.Key,
                                               Quantity = oe.Count()
                                           });

            var action = from wst in _ws.GetAll<WorkflowStatusTransition>()
                         .Where(x => permission.Select(t => t.TransitionId).Contains(x.Id))
                         select new ActionDto
                         {
                             StatusTransitionId = wst.Id,
                             WorkflowId = wst.Workflow.Id,
                             FromStatusId = wst.FromStatusId,
                             ToStatusId = wst.ToStatusId,
                             Name = wst.Name
                         };
            return _ws.GetAll<TempOutcomingEntry>()
                .Select(s => new GetTempOutcomingEntryDto
                {
                    Id = s.Id,
                    OutcomingEntryTypeId = s.OutcomingEntryTypeId,
                    OutcomingEntryTypeCode = s.OutcomingEntryType.Code,
                    Requester = _ws.GetAll<User>().FirstOrDefault(r => r.Id == s.CreatorUserId).Name,
                    Name = s.Name,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    Accreditation=s.Accreditation,
                    BranchId = s.BranchId,
                    BranchName = s.Branch.Name,
                    CurrencyId = s.CurrencyId,
                    CurrencyName = s.Currency.Code,
                    Value = s.Value,
                    WorkflowStatusId = s.WorkflowStatusId,
                    WorkflowStatusName = s.WorkflowStatus.Name,
                    WorkflowStatusCode = s.WorkflowStatus.Code,
                    Action = action.Where(x => x.FromStatusId == s.WorkflowStatusId && s.OutcomingEntryType.WorkflowId == x.WorkflowId).ToList(),
                    RequestInBankTransaction = requestInBankTransaction
                                                .Where(x => x.a == s.Id)
                                                .Select(x => x.Quantity)
                                                .FirstOrDefault(),
                    CreatedAt = s.CreationTime.Date,
                    SendTime = s.SentTime.Value.Date,
                    ApproveTime = s.ApprovedTime.Value.Date,
                    ExecuteTime = s.ExecutedTime.Value.Date,
                    PaymentCode = s.PaymentCode,
                    IsAcceptFile = s.IsAcceptFile,
                    LastModifiedTime = s.LastModificationTime,
                    RootOutcomingEntryId = s.RootOutcomingEntryId,
                    Reason = s.Reason
                });
        }
        public async Task<GetOutcomingEntryDto> GetRootOutcomingEntry(long outcomingEntryId)
        {
            var roledIds = _ws.GetAll<UserRole>()
                          .Where(s => s.UserId == _session.UserId)
                          .Select(s => s.RoleId);

            var permission = _ws.GetAll<WorkflowStatusTransitionPermission>().Where(x => roledIds.Contains(x.RoleId));

            var requestInBankTransaction = _ws.GetAll<OutcomingEntryBankTransaction>()
                                           .GroupBy(oe => oe.OutcomingEntryId)
                                           .Select(oe => new
                                           {
                                               OutcomingEntryId = oe.Key,
                                               Quantity = oe.Count()
                                           });

            var action = from wst in _ws.GetAll<WorkflowStatusTransition>()
                         .Where(x => permission.Select(t => t.TransitionId).Contains(x.Id))
                         select new ActionDto
                         {
                             StatusTransitionId = wst.Id,
                             WorkflowId = wst.Workflow.Id,
                             FromStatusId = wst.FromStatusId,
                             ToStatusId = wst.ToStatusId,
                             Name = wst.Name
                         };

            var tempOutcomingEntry = await _ws.GetAll<OutcomingEntry>()
                .Where(s => s.Id == outcomingEntryId)
                .Select(s => new GetOutcomingEntryDto
                {
                    Id = s.Id,
                    OutcomingEntryTypeId = s.OutcomingEntryTypeId,
                    OutcomingEntryTypeCode = s.OutcomingEntryType.Code,
                    OutcomingEntryTypeName = s.OutcomingEntryType.Name,
                    Requester = _ws.GetAll<User>().FirstOrDefault(r => r.Id == s.CreatorUserId).Name,
                    Name = s.Name,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    Accreditation=s.Accreditation,
                    BranchId = s.BranchId,
                    BranchName = s.Branch.Name,
                    CurrencyId = s.CurrencyId,
                    CurrencyName = s.Currency.Code,
                    Value = s.Value,
                    WorkflowStatusId = s.WorkflowStatusId,
                    WorkflowStatusName = s.WorkflowStatus.Name,
                    WorkflowStatusCode = s.WorkflowStatus.Code,
                    Action = action.Where(x => x.FromStatusId == s.WorkflowStatusId && s.OutcomingEntryType.WorkflowId == x.WorkflowId).ToList(),
                    RequestInBankTransaction = requestInBankTransaction
                                                .Where(x => x.OutcomingEntryId == s.Id)
                                                .Select(x => x.Quantity)
                                                .FirstOrDefault(),
                    CreatedAt = s.CreationTime.Date,
                    SendTime = s.SentTime.Value.Date,
                    ApproveTime = s.ApprovedTime.Value.Date,
                    ExecuteTime = s.ExecutedTime.Value.Date,
                    PaymentCode = s.PaymentCode,
                    IsAcceptFile = s.IsAcceptFile,
                    ReportDate = s.ReportDate
                }).FirstOrDefaultAsync();

            await GetButtonInfo(tempOutcomingEntry);

            return tempOutcomingEntry;
        }
        public async Task<GetRequestChangeOutcomingEntryDetailDto> GetRequestChangeOutcomingEntryDetail(long tempOutcomingEntryId)
        {

            //get information status temp
            var tempStatus = await _ws.GetAll<TempOutcomingEntry>()
                .Where(s => s.Id == tempOutcomingEntryId)
                .Select(x => new { x.WorkflowStatus.Code, x.WorkflowStatus.Name })
                .FirstOrDefaultAsync();

            //Get temp detail by temp id
            var tempOutcomingEntryDetails = await GetTempOutCommingEntryDetail(tempOutcomingEntryId);

            //Get dictionary root detail by root id
            var rootId = await GetOutcomingEntryIdByTempId(tempOutcomingEntryId);
            var dicOutcomingEntryDetails = await GetRootOutcomingEntryDetail(rootId);

            var result = new List<RequestChangeOutcomingEntryDetailInfoDto>();

            //Get temp detail is cloned from root
            var tempDetailIdsHasValueRootDetail = tempOutcomingEntryDetails
                .Where(x => x.RootOutcomingEntryDetailId.HasValue)
                .Select(s => s.RootOutcomingEntryDetailId.Value);

            var detailIds = dicOutcomingEntryDetails.Keys;

            //GET delete
            var outcomingIdsDeleted = detailIds.Except(tempDetailIdsHasValueRootDetail);
            foreach (var key in outcomingIdsDeleted)
            {
                result.Add(new RequestChangeOutcomingEntryDetailInfoDto { ActionType = ActionTypeEnum.DELETE, OutcomingEntryDetail = dicOutcomingEntryDetails[key] });
            }

            //GET create
            tempOutcomingEntryDetails
                .Where(s => !s.RootOutcomingEntryDetailId.HasValue)
                .ToList()
                .ForEach(item => result.Add(new RequestChangeOutcomingEntryDetailInfoDto { ActionType = ActionTypeEnum.NEW, TempOutcomingEntryDetailDto = item }));

            //GET edit
            //get dictionary temp detail compare to root detail
            var dicTempOutcomingDetails = tempOutcomingEntryDetails
                .Where(x => x.RootOutcomingEntryDetailId.HasValue)
                .ToDictionary(x => x.RootOutcomingEntryDetailId);
            var outcomingIdsEdit = detailIds.Intersect(tempDetailIdsHasValueRootDetail);

            foreach (var key in outcomingIdsEdit)
            {
                bool isChanged = CheckEditOutcomingEntryDetail(dicOutcomingEntryDetails[key], dicTempOutcomingDetails[key]);
                result.Add(new RequestChangeOutcomingEntryDetailInfoDto
                {
                    ActionType = isChanged ? ActionTypeEnum.UPDATE : ActionTypeEnum.NO_ACTION,
                    OutcomingEntryDetail = dicOutcomingEntryDetails[key],
                    TempOutcomingEntryDetailDto = dicTempOutcomingDetails[key]
                });
            }

            return new GetRequestChangeOutcomingEntryDetailDto
            {
                RequestChangeDetails = result,
                StatusCode = tempStatus.Code,
                StatusName = tempStatus.Name,
            };
        }
        public async Task<GetRequestChangeOutcomingEntryDetailDto> ViewHistoryChangeOutcomingEntryDetail(long tempId, long rooId)
        {
            var listTemp = await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == rooId)
                .Select(x => new
                {
                    x.Id,
                    WorkflowStatusName = x.WorkflowStatus.Name,
                    x.LastModificationTime
                })
                .OrderBy(x => x.LastModificationTime ?? DateTime.MinValue)
                .ToArrayAsync();
            for (int i = 0; i < listTemp.Length; i++)
            {
                if (listTemp[i].Id != tempId) continue;

                long rootTempId = listTemp[i-1].Id;

                var currentTempDetails = await IQGetTempOutCommingEntryDetail()
                .Where(x => x.RootTempOutcomingEntryId == tempId)
                .ToListAsync();

                var rootTempDetails = await IQGetTempOutCommingEntryDetail()
                    .Where(x => x.RootTempOutcomingEntryId == rootTempId)
                    .ToListAsync();

                var currentDetailIds = currentTempDetails.Select(x => x.RootOutcomingEntryDetailId);
                var rootDetailIds = rootTempDetails.Select(x => x.RootOutcomingEntryDetailId);
                var result = new List<RequestChangeOutcomingEntryDetailInfoDto>();

                currentTempDetails
                    .Where(x => !rootDetailIds.Contains(x.RootOutcomingEntryDetailId))
                    .ToList()
                    .ForEach(item =>
                    {
                        result.Add(new RequestChangeOutcomingEntryDetailInfoDto { ActionType = ActionTypeEnum.NEW, TempOutcomingEntryDetailDto = item });
                    });

                rootTempDetails
                    .Where(x => !currentDetailIds.Contains(x.RootOutcomingEntryDetailId))
                    .ToList()
                    .ForEach(item => result.Add(new RequestChangeOutcomingEntryDetailInfoDto { ActionType = ActionTypeEnum.DELETE, OutcomingEntryDetail = ObjectMapper.Map<GetOutcomingEntryDetailDto>(item) }));

                currentTempDetails
                    .Where(x => rootDetailIds.Contains(x.RootOutcomingEntryDetailId))
                    .ToList()
                    .ForEach(item =>
                    {
                        var preItem = rootTempDetails.Where(x => x.RootOutcomingEntryDetailId == item.RootOutcomingEntryDetailId).FirstOrDefault();
                        var mapPreItem = ObjectMapper.Map<GetOutcomingEntryDetailDto>(preItem);
                        bool isChanged = CheckEditOutcomingEntryDetail(mapPreItem, item);
                        result.Add(new RequestChangeOutcomingEntryDetailInfoDto
                        {
                            ActionType = isChanged ? ActionTypeEnum.UPDATE : ActionTypeEnum.NO_ACTION,
                            OutcomingEntryDetail = mapPreItem,
                            TempOutcomingEntryDetailDto = item
                        });
                    });
                return new GetRequestChangeOutcomingEntryDetailDto()
                {
                    RequestChangeDetails = result,
                    StatusCode = FinanceManagementConsts.WORKFLOW_STATUS_APPROVED,
                    StatusName = listTemp[i-1].WorkflowStatusName
                };
            }

            throw new UserFriendlyException("Không tìm thấy lịch sử request change.");
        }
        private bool CheckEditOutcomingEntryDetail(GetOutcomingEntryDetailDto root, GetTempOutcomingEntryDetailDto temp)
        {
            var rootType = typeof(GetOutcomingEntryDetailDto);
            var tempType = typeof(GetTempOutcomingEntryDetailDto);

            var propertiesRoot = rootType.GetProperties();
            foreach (var propertyRoot in propertiesRoot)
            {
                var propertyTemp = tempType.GetProperty(propertyRoot.Name);
                if (propertyTemp == null || propertyRoot.Name.Equals("Id")) continue;

                var rootPropValue = propertyRoot.GetValue(root);
                var tempPropValue = propertyTemp.GetValue(temp);

                if (rootPropValue != null && !rootPropValue.Equals(tempPropValue))
                {
                    return true;
                }
                if (tempPropValue != null && !tempPropValue.Equals(rootPropValue))
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<List<GetTempOutcomingEntryDetailDto>> GetTempOutCommingEntryDetail(long tempOutcomingEntryId)
        {
            var tempOutcomingEntryDetails = await IQGetTempOutCommingEntryDetail()
                .Where(oed => oed.RootTempOutcomingEntryId == tempOutcomingEntryId)
                .ToListAsync();
            return tempOutcomingEntryDetails;
        }
        private IQueryable<GetTempOutcomingEntryDetailDto> IQGetTempOutCommingEntryDetail()
        {
            return from oe in _ws.GetAll<TempOutcomingEntryDetail>()
                   join account in _ws.GetAll<Account>() on oe.AccountId equals account.Id into accounts
                   from acc in accounts.DefaultIfEmpty()
                   select new GetTempOutcomingEntryDetailDto
                   {
                       Id = oe.Id,
                       OutcomingEntryId = oe.OutcomingEntryId,
                       AccountId = oe.AccountId,
                       AccountName = acc.Name,
                       Name = oe.Name,
                       Quantity = oe.Quantity,
                       UnitPrice = oe.UnitPrice,
                       Total = oe.Total,
                       BranchId = oe.BranchId,
                       BranchName = oe.Branch.Name ?? string.Empty,
                       RootOutcomingEntryDetailId = oe.RootOutcomingEntryDetailId,
                       RootTempOutcomingEntryId = oe.RootTempOutcomingEntryId
                   };
        }
        private async Task<Dictionary<long, GetOutcomingEntryDetailDto>> GetRootOutcomingEntryDetail(long outcomingEntryId)
        {
            return await (from oe in _ws.GetAll<OutcomingEntryDetail>().Where(oed => oed.OutcomingEntryId == outcomingEntryId)
                          join account in _ws.GetAll<Account>() on oe.AccountId equals account.Id into accounts
                          from acc in accounts.DefaultIfEmpty()
                          select new GetOutcomingEntryDetailDto
                          {
                              Id = oe.Id,
                              OutcomingEntryId = oe.OutcomingEntryId,
                              AccountId = oe.AccountId,
                              AccountName = acc.Name,
                              Name = oe.Name,
                              Quantity = oe.Quantity,
                              UnitPrice = oe.UnitPrice,
                              Total = oe.Total,
                              BranchId = oe.BranchId,
                              BranchName = oe.Branch.Name ?? string.Empty,
                          }).ToDictionaryAsync(x => x.Id);
        }
        public async Task<RequestChangeOutcomingEntryInfoDto> SaveTempOutCommingEntry(UpdateTempOutcomingEntryDto input)
        {
            var tempOutcomingEntry = await _ws.GetAll<TempOutcomingEntry>()
                .Include(s => s.WorkflowStatus)
                .Where(s => s.Id == input.Id)
                .FirstOrDefaultAsync();

            var hasOutcomingDetail = await _ws.GetAll<TempOutcomingEntryDetail>()
                .Where(x => x.RootTempOutcomingEntryId == tempOutcomingEntry.Id)
                .AnyAsync();
            if (hasOutcomingDetail && input.Value != tempOutcomingEntry.Value)
                throw new UserFriendlyException("Không thể sửa được value của request chi khi đã có chi tiết");

            var isAllowOutcomingEntryByMultipleCurrency = await IsAllowOutcomingEntryByMultipleCurrency();
            if (!isAllowOutcomingEntryByMultipleCurrency)
            {
                var currencyDefault = await GetCurrencyDefaultAsync();
                if (currencyDefault.IsNullOrDefault())
                {
                    throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
                }
                if (currencyDefault.Id != input.CurrencyId)
                {
                    throw new UserFriendlyException("Không đúng loại tiền mặc định Request chi");
                }
            }
            else
            {
                var hasBankTransaction = await _ws.GetAll<OutcomingEntry>()
                    .Where(x => x.Id == tempOutcomingEntry.RootOutcomingEntryId)
                    .Select(x => x.OutcomingEntryBankTransactions.Any(s => !s.IsDeleted))
                    .FirstOrDefaultAsync();

                if (tempOutcomingEntry.CurrencyId != input.CurrencyId && hasBankTransaction)
                    throw new UserFriendlyException("Không thể cập nhật loại tiền khi Request chi đã có GDNH liên quan");
            }
            await _ws.UpdateAsync(ObjectMapper.Map(input, tempOutcomingEntry));

            return await GetRequestChangeOutcomingEntry(tempOutcomingEntry.Id);
        }
        public async Task<string> SendTemp(long tempOutcomingEntryId)
        {
            var tempOutcomingEntry = await _ws.GetAsync<TempOutcomingEntry>(tempOutcomingEntryId);
            var fromStatusId = tempOutcomingEntry.WorkflowStatusId;
            var toStatusId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO);

            tempOutcomingEntry.WorkflowStatusId = toStatusId;
            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetTransitionName(fromStatusId, toStatusId);
        }
        public async Task<string> RejectTemp(long tempOutcomingEntryId)
        {
            var tempOutcomingEntry = await _ws.GetAsync<TempOutcomingEntry>(tempOutcomingEntryId);
            var fromStatusId = tempOutcomingEntry.WorkflowStatusId;
            var statusRejectId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_REJECTED);
            tempOutcomingEntry.WorkflowStatusId = statusRejectId;
            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetTransitionName(fromStatusId, statusRejectId);
        }
        public async Task<string> ApprovedTemp(long tempOutcomingEntryId)
        {
            //Update temp -> APPROVED
            var tempOutcomingEntry = await _ws.GetAsync<TempOutcomingEntry>(tempOutcomingEntryId);
            var fromStatusId = tempOutcomingEntry.WorkflowStatusId;
            var statusApproveId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_APPROVED);
            tempOutcomingEntry.WorkflowStatusId = statusApproveId;
            await _ws.UpdateAsync(tempOutcomingEntry);
            //End

            //Update root from temp
            var rootId = await GetOutcomingEntryIdByTempId(tempOutcomingEntryId);
            var rootOutcoming = await _ws.GetAsync<OutcomingEntry>(rootId);
            ObjectMapper.Map(tempOutcomingEntry, rootOutcoming);
            rootOutcoming.Id = rootId;
            await _ws.UpdateAsync(rootOutcoming);
            //End

            var rootOutcomingDetails = rootOutcoming.OutcomingEntryDetails.ToList();
            var tempDetails = await _ws.GetAll<TempOutcomingEntryDetail>()
                .Where(x => x.RootTempOutcomingEntryId == tempOutcomingEntryId)
                .ToListAsync();

            //Delete root not exist in temp
            var rootDetailIdsInTemp = tempDetails
                .Where(x => x.RootOutcomingEntryDetailId.HasValue)
                .Select(x => x.RootOutcomingEntryDetailId.Value)
                .ToList();
            var deleteRootDetailIds = rootOutcomingDetails.Select(x => x.Id).Except(rootDetailIdsInTemp);
            if (deleteRootDetailIds.Any())
            {
                var deleteRootDetails = rootOutcomingDetails
                .Where(x => deleteRootDetailIds.Contains(x.Id))
                .ToList();
                await _ws.SoftDeleteRangeAsync(deleteRootDetails);
            }
            //End

            //Update Outcoming Entry Detail
            foreach (var tempDetail in tempDetails)
            {
                if (tempDetail.RootOutcomingEntryDetailId.HasValue)
                {
                    var updateOutDetail = rootOutcomingDetails.Find(x => x.Id == tempDetail.RootOutcomingEntryDetailId.Value);
                    ObjectMapper.Map(tempDetail, updateOutDetail);
                    updateOutDetail.Id = tempDetail.RootOutcomingEntryDetailId.Value;
                    await _ws.UpdateAsync(updateOutDetail);
                    continue;
                }
                var outDetail = ObjectMapper.Map<OutcomingEntryDetail>(tempDetail);
                outDetail.Id = 0;
                var newOutId = await _ws.InsertAndGetIdAsync(outDetail);
                tempDetail.RootOutcomingEntryDetailId = newOutId;
                await _ws.UpdateAsync(tempDetail);
            }
            //End

            return await GetTransitionName(fromStatusId, statusApproveId);
        }
        private async Task<string> GetTransitionName(long fromStatusId, long toStatusId)
        {
            return await _ws.GetAll<WorkflowStatusTransition>()
                .Where(s => s.FromStatusId == fromStatusId)
                .Where(s => s.ToStatusId == toStatusId)
                .Select(s => s.Name.Trim())
                .FirstOrDefaultAsync();
        }
        public async Task CreateTempOutcomingEntryDetail(CreateTempOutcomingEntryDetailDto input)
        {
            //create new temp detail
            var tempId = await GetTempIdByOutcomingId(input.OutcomingEntryId);
            input.RootTempOutcomingEntryId = tempId;
            await _ws.InsertAsync(ObjectMapper.Map<TempOutcomingEntryDetail>(input));
            await CurrentUnitOfWork.SaveChangesAsync();
            //update value temp
            var temp = await _ws.GetAsync<TempOutcomingEntry>(tempId);
            temp.Value = await _ws.GetAll<TempOutcomingEntryDetail>()
                .Where(x => x.RootTempOutcomingEntryId == tempId)
                .SumAsync(x => x.Total);
            await _ws.UpdateAsync(temp);
        }
        public async Task UpdateTempOutcomingEntryDetail(UpdateTempOutcomingEntryDetailDto input)
        {
            var tempDetail = await _ws.GetAsync<TempOutcomingEntryDetail>(input.Id);
            var isChangeMoney = input.Total != tempDetail.Total;
            ObjectMapper.Map(input, tempDetail);
            await _ws.UpdateAsync(tempDetail);
            await CurrentUnitOfWork.SaveChangesAsync();

            if(isChangeMoney)
            {
                var temp = await _ws.GetAsync<TempOutcomingEntry>(tempDetail.RootTempOutcomingEntryId);
                temp.Value = await _ws.GetAll<TempOutcomingEntryDetail>()
                .Where(x => x.RootTempOutcomingEntryId == temp.Id)
                .SumAsync(x => x.Total);
                await _ws.UpdateAsync(temp);
            }
        }
        public async Task DeleteTempOutcomingEntryDetail(long tempOutcomingEntryDetailId)
        {
            var tempDetail = await _ws.GetAsync<TempOutcomingEntryDetail>(tempOutcomingEntryDetailId);
            var temp = await _ws.GetAsync<TempOutcomingEntry>(tempDetail.RootTempOutcomingEntryId);
            temp.Value = temp.Value - tempDetail.Total;
            await _ws.DeleteAsync<TempOutcomingEntryDetail>(tempOutcomingEntryDetailId);
            await _ws.UpdateAsync(temp);
        }
        public async Task<string> GetStatusCodeByTempId(long tempId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.Id == tempId)
                .Select(x => x.WorkflowStatus.Code.Trim())
                .FirstOrDefaultAsync();
        }
        public async Task<long> GetOutcomingEntryIdByTempId(long tempId)
        {
            return await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.Id == tempId)
                .Select(x => x.RootOutcomingEntryId)
                .FirstOrDefaultAsync();
        }
        public async Task GetButtonInfo(GetOutcomingEntryDto input)
        {
            var isExistTemp = await IsExistTempOutComingEntry(input.Id);
            input.WorkflowStatusCode = input.WorkflowStatusCode.Trim();
            var permissionViewYCTĐ =  await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_ViewYCTD);
            var permissionToYCTĐ = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD);
            if (input.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED && !isExistTemp && permissionToYCTĐ)
                input.IsShowButtonRequestChange = true;

            if (input.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED && isExistTemp)
            {
                if (permissionViewYCTĐ)
                {
                    input.IsShowButtonViewRequestChange = true;
                }

                var tempId = await GetTempIdByOutcomingId(input.Id);
                input.TempOutcomingEntryId = tempId;

                var temp = await _ws.GetAll<TempOutcomingEntry>()
                    .Where(x => x.Id == tempId)
                    .Select(x => new
                    {
                        WorkflowStatusCode = x.WorkflowStatus.Code.Trim(),
                        CreatorUserId = x.CreatorUserId
                    })
                    .FirstOrDefaultAsync();

                var hasPermissionApproveRequestChange = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_AcceptYCTD);
                if (hasPermissionApproveRequestChange && (temp.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO || temp.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_REJECTED))
                    input.IsShowButtonApproveRequestChange = true;

                //var isRequestChangeOfUser = temp.CreatorUserId.HasValue ? temp.CreatorUserId.Value == _session.UserId : false;

                var hasPermissionRejectRequestChange =  await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_RejectYCTD);
                if (hasPermissionRejectRequestChange && temp.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                    input.IsShowButtonRejectRequestChange = true;

                bool hasPermissionSendRequestChange = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD);
                if (hasPermissionSendRequestChange && (temp.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_START || temp.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_REJECTED))
                    input.IsShowButtonSendRequestChange = true;
            }
        }
        public async Task RevertTempOutcomingDetailByRootId(long rootOutcomingEntryDetailId, long rootOutcomingEntryId)
        {
            var tempId = await GetTempIdByOutcomingId(rootOutcomingEntryId);
            using (CurrentUnitOfWork.DisableFilter("SoftDelete"))
            {
                //revert temp detail -> IsDeleted = fasle;
                var tempDetail = await _ws.GetAll<TempOutcomingEntryDetail>()
                    .Where(x => x.RootOutcomingEntryDetailId == rootOutcomingEntryDetailId && x.RootTempOutcomingEntryId == tempId)
                    .FirstOrDefaultAsync();
                tempDetail.IsDeleted = false;

                //revert value temp
                var temp = await _ws.GetAsync<TempOutcomingEntry>(tempId);
                temp.Value += tempDetail.Total;

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
        public async Task<bool> CheckTempOutcomingEntryApproved(long tempId)
        {
            var temp = await _ws.GetAll<TempOutcomingEntry>()
                .Select(s => new
                {
                    s.Id,
                    s.WorkflowStatus.Code
                })
                .Where(s => s.Id == tempId)
                .FirstOrDefaultAsync();

            if (temp == default) throw new UserFriendlyException($"Not Request change by Id : {tempId}");

            return temp.Code.Equals(FinanceManagementConsts.WORKFLOW_STATUS_APPROVED);
        }

        public async Task<bool> CheckTempOutCommingEntryHasDetail(long tempOutcomingEntryId)
        {
            var getRequestChangeOutcomingEntryDetail = await GetRequestChangeOutcomingEntryDetail(tempOutcomingEntryId);
            return !getRequestChangeOutcomingEntryDetail.RequestChangeDetails.IsEmpty();
        }
        public Dictionary<long, long> GetDicOutCommingEntryIdToPendingCEOTempId(IEnumerable<long> outCommingEntryIds)
        {
            return _ws.GetAll<TempOutcomingEntry>()
                .Where(s => s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                .Where(s => outCommingEntryIds.Contains(s.RootOutcomingEntryId))
                .Select(s => new
                {
                    s.Id,
                    s.RootOutcomingEntryId
                })
                .AsEnumerable()
                .GroupBy(s => s.RootOutcomingEntryId)                
                .ToDictionary(s => s.Key, s => s.Select(x => x.Id).FirstOrDefault());
        }
        public Dictionary<long, long> GetDicOutCommingEntryIdToOrtherAPPROVEDTempId(IEnumerable<long> outCommingEntryIds)
        {
            return _ws.GetAll<TempOutcomingEntry>()
                .Where(s => s.WorkflowStatus.Code != FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .Where(s => outCommingEntryIds.Contains(s.RootOutcomingEntryId))
                .Select(s => new
                {
                    s.Id,
                    s.RootOutcomingEntryId
                })
                .AsEnumerable()
                .GroupBy(s => s.RootOutcomingEntryId)
                .ToDictionary(s => s.Key, s => s.Select(x => x.Id).FirstOrDefault());
        }
        
    }
}
