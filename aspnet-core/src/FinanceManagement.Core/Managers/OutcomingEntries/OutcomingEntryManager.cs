using Abp.Authorization.Users;
using Abp.Runtime.Session;
using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Managers.TempOutcomingEntries;
using FinanceManagement.Managers.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.OutcomingEntries
{
    public class OutcomingEntryManager : DomainManager, IOutcomingEntryManager
    {
        private readonly IAbpSession _abpSession;
        private readonly IMyUserManager _myUserManager;
        private readonly IMySettingManager _mySettingManager;
        private readonly ICommonManager _commonManager;
        public OutcomingEntryManager(IWorkScope ws,
            IAbpSession abpSession,
            IMyUserManager myUserManager,
            IMySettingManager mySettingManager,
            ICommonManager commonManager) : base(ws)
        {
            _abpSession = abpSession;
            _myUserManager = myUserManager;
            _mySettingManager = mySettingManager;
            _commonManager = commonManager;
        }
        public async Task<OutcomingEntryMoneyInfo> GetOutcomingEntryMoneyInfo(long outcomingEntryId)
        {
            var deviantIncomingEntryTypeInfo = await _mySettingManager.GetDeviantClientAsync();
            var isAllowOutcomingEntryByMultipleCurrency = await IsAllowOutcomingEntryByMultipleCurrency();
            var result = await _ws.GetAll<OutcomingEntry>()
                .Where(s => s.Id == outcomingEntryId)
                .Select(s => new OutcomingEntryMoneyInfomation
                {
                    Value = s.Value,

                    TotalPaidToValue = s.OutcomingEntryBankTransactions
                        .Where(x => !x.IsDeleted).Select(x => -x.BankTransaction.ToValue)
                        .Sum(),

                    TotalPaidFromValue = s.OutcomingEntryBankTransactions
                        .Where(x => !x.IsDeleted).Select(x => -x.BankTransaction.FromValue)
                        .Sum(),

                    TotalIncomingRefund = s.RelationInOutEntries
                        .Where(x => !x.IsDeleted && x.IsRefund)
                        .Where(x => x.IncomingEntry.IncomingEntryTypeId != deviantIncomingEntryTypeInfo.Id)
                        .Select(x => x.IncomingEntry.Value)
                        .Sum(),
                })
                .FirstOrDefaultAsync();
            return new OutcomingEntryMoneyInfo
            {
                NeedToSpend = result.Value,
                Spent = isAllowOutcomingEntryByMultipleCurrency ? (result.TotalPaidFromValue + result.TotalIncomingRefund) : (result.TotalPaidToValue + result.TotalIncomingRefund),
            };
        }
        public async Task<bool> IsOutcomingEntryHasDetail(long outcomingEntryId)
        {
            var hasDetail = _ws.GetAll<OutcomingEntryDetail>()
                .Where(x => x.OutcomingEntryId == outcomingEntryId)
                .Any();
            return hasDetail;
        }
        public async Task CheckChangeOutcomingEntryStatus(long outcomingEntryId)
        {
            var outcomgingEntryMoneyInfo = await GetOutcomingEntryMoneyInfo(outcomingEntryId);
            if (Math.Abs(outcomgingEntryMoneyInfo.Avalible) >= 1)
                throw new UserFriendlyException($"Số tiền cần chi là {Helpers.FormatMoney(outcomgingEntryMoneyInfo.NeedToSpend)} khác với số tiền đã chi là {Helpers.FormatMoney(outcomgingEntryMoneyInfo.Spent)}");
        }
        public async Task<bool> CheckCreateRelationInOut(long incomingEntryId)
        {
            var incomingEntryCurrency = await _ws.GetAll<IncomingEntry>()
                .Where(s => s.Id == incomingEntryId)
                .Select(s => s.BTransactions.BankAccount.Currency.Name)
                .FirstOrDefaultAsync();
            //Using VND
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (incomingEntryCurrency != currencyDefault.Name)
                return false;
            return true;
        }
        public async Task ChangeAllOutcomingEntryDetail(ChangeAllDetailOutcomingEntryDto input)
        {
            var outcomingEntryDetails = await _ws.GetAll<OutcomingEntryDetail>()
                .Where(s => s.OutcomingEntryId == input.OutcomingEntryId)
                .ToListAsync();
            var outcomingEntry = await _ws.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            double newValue = 0;
            foreach (var item in input.OutcomingEntryDetails)
            {
                if (item.Id == 0)
                {
                    await CreateOutcomingEntryDetail(item);
                    newValue += item.Total;
                }

                var outcomingEntryDetail = outcomingEntryDetails.Find(s => s.Id == item.Id);
                if (outcomingEntryDetail == null)
                    continue;

                await _ws.UpdateAsync(ObjectMapper.Map(item, outcomingEntryDetail));
                newValue += outcomingEntryDetail.Total;

                outcomingEntryDetails.Remove(outcomingEntryDetail);
            }

            await _ws.SoftDeleteRangeAsync(outcomingEntryDetails);

            outcomingEntry.Value = newValue;
            await _ws.UpdateAsync(outcomingEntry);
        }
        public async Task<GetWorkflowChangeStatusInfoDto> GetWorkflowCodeWhenChangeStatus(long statusTransitionId)
        {
            var workflowStatusTransition = _ws.GetAll<WorkflowStatusTransition>().FirstOrDefault(wst => wst.Id == statusTransitionId);

            if (workflowStatusTransition == null)
            {
                throw new UserFriendlyException("Next status doesn't exist");
            }

            var currentUserRoleIds = _ws.GetAll<UserRole>().Where(ur => ur.UserId == _abpSession.UserId).Select(ur => ur.RoleId);

            // Roles have permission to transit the workflow 
            var permissionRoleIds = _ws.GetAll<WorkflowStatusTransitionPermission>()
                    .Where(wstp => wstp.TransitionId == workflowStatusTransition.Id)
                    .Select(wftp => wftp.RoleId);

            if (!currentUserRoleIds.Any(urId => permissionRoleIds.Any(p => p == urId)))
            {
                throw new UserFriendlyException("Bạn không có quyền để thay đổi trạng thái");
            }
            var statusCode = await _ws.GetAsync<WorkflowStatus>(workflowStatusTransition.ToStatusId);
            var fromStatusCode = await _ws.GetAsync<WorkflowStatus>(workflowStatusTransition.FromStatusId);
            return new GetWorkflowChangeStatusInfoDto
            {
                ToStatusCode = statusCode.Code,
                ToTransitionStatusId = workflowStatusTransition.ToStatusId,
                FromStatusCode = fromStatusCode.Code,
                ToTransitionName = workflowStatusTransition.Name
            };
        }
        public async Task<long> CreateOutcomingEntryDetail(ChangeDetailOutcomingEntryDto input)
        {
            return await _ws.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntryDetail>(input));
        }
        public async Task UpdateOutcomgingEntryDetail(ChangeDetailOutcomingEntryDto input)
        {
            var outcomgingEntryDetail = await _ws.GetAsync<OutcomingEntryDetail>(input.Id);
            ObjectMapper.Map(input, outcomgingEntryDetail);
            await _ws.UpdateAsync(outcomgingEntryDetail);
        }
        public async Task<long> CreateRelationInOut(CreateRelationInOutDto input)
        {
            return await _ws.InsertAndGetIdAsync(ObjectMapper.Map<RelationInOutEntry>(input));
        }
        public async Task SetDoneOutcomingEntry(SetDoneOutcomingEntryDto input)
        {
            var workflowStatusId = await _ws.GetAll<WorkflowStatus>()
                .Where(s => s.Code == FinanceManagementConsts.WORKFLOW_STATUS_END)
                .Select(s => s.Id)
                .FirstAsync();

            var outcomingEntry = await _ws.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            outcomingEntry.WorkflowStatusId = workflowStatusId;
            outcomingEntry.ExecutedTime = input.ExcutedTime;
            await _ws.UpdateAsync(outcomingEntry);

            await CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
            {
                OutcomingEntryId = input.OutcomingEntryId,
                WorkflowStatusId = workflowStatusId,
                Value = outcomingEntry.Value
            });
        }
        public async Task CreateOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input)
        {
            await _ws.InsertAsync(ObjectMapper.Map<OutcomingEntryStatusHistory>(input));
        }

        public async Task CreateOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input, int periodId)
        {
            var entity = ObjectMapper.Map<OutcomingEntryStatusHistory>(input);
            entity.PeriodId = periodId;
            await _ws.InsertAsync(entity);
        }

        public async Task<IEnumerable<GetOutcomingEntryStatusHistoryDto>> GetOutcomingEntryStatusHistoryByOutcomingEntryId(long outcomingEntryId)
        {
            var outcomingStatusHistory = await IQGetOutcomingEntryStatusHistory()
                .Where(x => x.OutcomingEntryId == outcomingEntryId)
                .ToListAsync();
            var requestChangeHistory = await IQGetTempOutcomingHistory()
                .Where(x => x.OutcomingEntryId == outcomingEntryId)
                .ToListAsync();

            var result = outcomingStatusHistory
                .Concat(requestChangeHistory)
                .OrderByDescending(x => x.CreationTime)
                .ToList();

            var creatorUserIds = result.Where(s => s.CreationUserId.HasValue).Select(s => s.CreationUserId.Value);
            var dicUsers = await _myUserManager.GetDictionaryUserAudited(creatorUserIds);

            result.ForEach(item => item.CreationUser = item.CreationUserId.HasValue ?
                                                        (dicUsers.ContainsKey(item.CreationUserId.Value) ? dicUsers[item.CreationUserId.Value] : string.Empty)
                                                        : string.Empty);
            return result;
        }
        public async Task<bool> UpdateValueToStartOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input)
        {
            var outcomingEntry = await _ws.GetAsync<OutcomingEntry>(input.OutcomingEntryId);

            if (outcomingEntry == default) return false;

            var outcomingStatusHistory = (from his in _ws.GetAll<OutcomingEntryStatusHistory>()
                                         join workflow in _ws.GetAll<WorkflowStatus>() on his.WorkflowStatusId equals workflow.Id
                                         where his.OutcomingEntryId == input.OutcomingEntryId
                                         where workflow.Code == FinanceManagementConsts.WORKFLOW_STATUS_START
                                         orderby his.CreationTime descending
                                         select his)
                                         .FirstOrDefault();

            if (outcomingStatusHistory == default) return false;

            outcomingStatusHistory.Value = input.Value;
            outcomingStatusHistory.CurrencyName = outcomingEntry.Currency.Name;
            await CurrentUnitOfWork.SaveChangesAsync();
            return true;
        }
        public IQueryable<GetOutcomingEntryStatusHistoryDto> IQGetOutcomingEntryStatusHistory()
        {
            return from his in _ws.GetAll<OutcomingEntryStatusHistory>()
                   join outcom in _ws.GetAll<OutcomingEntry>() on his.OutcomingEntryId equals outcom.Id
                   join workflow in _ws.GetAll<WorkflowStatus>() on his.WorkflowStatusId equals workflow.Id
                   orderby his.CreationTime descending
                   select new GetOutcomingEntryStatusHistoryDto
                   {
                       OutcomingEntryId = outcom.Id,
                       OutcomingEntryName = outcom.Name,
                       WorkflowStatusId = workflow.Id,
                       ValueNumber = his.Value,
                       WorkflowStatusName = workflow.Name,
                       WorkflowStatusCode = workflow.Code,
                       CreationTime = his.CreationTime,
                       CreationUserId = his.CreatorUserId,
                       CurrencyName = his.CurrencyName
                   };
        }
        public IQueryable<GetOutcomingEntryStatusHistoryDto> IQGetTempOutcomingHistory()
        {
            return from temp in _ws.GetAll<TempOutcomingEntry>()
                   where !temp.IsOriginal
                   select new GetOutcomingEntryStatusHistoryDto
                   {
                       CreationTime = temp.CreationTime,
                       CreationUserId = temp.CreatorUserId,
                       TempId = temp.Id,
                       ValueNumber = temp.Value,
                       WorkflowStatusCode = temp.WorkflowStatus.Code,
                       WorkflowStatusName = temp.WorkflowStatus.Name,
                       OutcomingEntryId = temp.RootOutcomingEntryId,
                       WorkflowStatusId = temp.WorkflowStatus.Id,
                       CurrencyName = temp.Currency.Name
                   };
        }
        public Dictionary<long, IEnumerable<GetOutcomingEntryStatusHistoryDto>> GetDictionaryStatusHistories(IEnumerable<GetOutcomingEntryStatusHistoryDto> input, Dictionary<long, string> dicUsers)
        {
            return input.GroupBy(x => x.OutcomingEntryId)
                .Select(x => new
                {
                    OutcomingEntryId = x.Key,
                    StatusHistories = x.Select(s => new GetOutcomingEntryStatusHistoryDto
                    {
                        OutcomingEntryId = s.OutcomingEntryId,
                        OutcomingEntryName = s.OutcomingEntryName,
                        WorkflowStatusId = s.WorkflowStatusId,
                        WorkflowStatusName = s.WorkflowStatusName,
                        WorkflowStatusCode = s.WorkflowStatusCode,
                        CreationTime = s.CreationTime,
                        CreationUserId = s.CreationUserId,
                        CreationUser = s.CreationUserId.HasValue ? (dicUsers.ContainsKey(s.CreationUserId.Value) ? dicUsers[s.CreationUserId.Value] : string.Empty) : string.Empty,
                        TempId = s.TempId,
                        ValueNumber = s.ValueNumber,
                        CurrencyName = s.CurrencyName,
                    })
                })
                .ToDictionary(x => x.OutcomingEntryId, x => x.StatusHistories);
        }
        public async Task Delete(long id)
        {
            var outcomingEntry = await _ws.GetAsync<OutcomingEntry>(id);

            if (outcomingEntry.WorkflowStatus.Code.Trim() != FinanceManagementConsts.WORKFLOW_STATUS_START &&
                    outcomingEntry.WorkflowStatus.Code.Trim() != FinanceManagementConsts.WORKFLOW_STATUS_REJECTED)
                throw new UserFriendlyException("Chỉ xóa được request chi có trạng thái là [START] hoặc [REJECTED]");

            var temps = await _ws.GetAll<TempOutcomingEntry>()
                .Where(x => x.RootOutcomingEntryId == id)
                .ToListAsync();
            foreach (var temp in temps)
            {
                temp.IsDeleted = true;
            }

            var tempDetails = await _ws.GetAll<TempOutcomingEntryDetail>()
                .Where(x => temps.Select(s => s.Id).Contains(x.RootTempOutcomingEntryId))
                .ToListAsync();
            foreach (var tempDetail in tempDetails)
            {
                tempDetail.IsDeleted = true;
            }

            var statusHistories = await _ws.GetAll<OutcomingEntryStatusHistory>()
                .Where(x => x.OutcomingEntryId == id)
                .ToListAsync();
            foreach (var statusHistory in statusHistories)
            {
                statusHistory.IsDeleted = true;
            }

            var relationInOuts = await _ws.GetAll<RelationInOutEntry>()
                .Where(x => x.OutcomingEntryId == id)
                .ToListAsync();
            foreach (var relationInOut in relationInOuts)
            {
                relationInOut.IsDeleted = true;
            }

            var outcomingBankTransactions = await _ws.GetAll<OutcomingEntryBankTransaction>()
                .Where(x => x.OutcomingEntryId == id)
                .ToListAsync();
            foreach (var outcomingBankTransaction in outcomingBankTransactions)
            {
                outcomingBankTransaction.IsDeleted = true;
            }

            var outcomingEntryDetails = await _ws.GetAll<OutcomingEntryDetail>()
                .Where(x => x.OutcomingEntryId == id)
                .ToListAsync();
            foreach (var outcomingEntryDetail in outcomingEntryDetails)
            {
                outcomingEntryDetail.IsDeleted = true;
            }

            await _ws.DeleteAsync(outcomingEntry);

            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<List<OutcomingEntryInfoDto>> GetOutcomingEntryNotEndForClosePeriod()
        {
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            return await _ws.GetAll<OutcomingEntry>()
                .Where(x => x.WorkflowStatusId != statusEndId)
                .Where(x => x.OutcomingEntryBankTransactions.Any())
                .Select(x => new OutcomingEntryInfoDto
                {
                    AccountId = x.AccountId,
                    AccountName = x.Account.Name,
                    Id = x.Id,
                    Name = x.Name,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.Currency.Name,
                    OutcomingEntryTypeCode = x.OutcomingEntryType.Code,
                    OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                    WorkflowStatusCode = x.WorkflowStatus.Code,
                    WorkflowStatusName = x.WorkflowStatus.Name,
                    WorkflowStatusId = x.WorkflowStatusId,
                    Value = x.Value,
                })
                .ToListAsync();
        }
        public async Task<int> CountOutcomingEntryNotEndForClosePeriod()
        {
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            return await _ws.GetAll<OutcomingEntry>()
                    .AsNoTracking()
                    .CountAsync(x => x.WorkflowStatusId != statusEndId && x.OutcomingEntryBankTransactions.Any(x => !x.IsDeleted));
        }
        public async Task UpdateReportDate(UpdateReportDateDto input)
        {
            var outcom = await _ws.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            outcom.ReportDate = input.ReportDate;

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateOutcomingEntryType(UpdateOutcomEntryTypeDto input)
        {
            var outcom = await _ws.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            outcom.OutcomingEntryTypeId = input.OutcomingEntryTypeId;

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<long> CloneOutcomingEntry(CloneOutcomeDto input)
        {
            var outcomEntryToClone = await _ws.GetAll<OutcomingEntry>()
                .Where(s => s.Id == input.OutcomeEntryId)
                .FirstOrDefaultAsync();

            var hasDetail = await IsOutcomingEntryHasDetail(outcomEntryToClone.Id);

            var startlWorkflowStatusId = await _ws.GetAll<WorkflowStatus>()
                .Where(x => x.Code == FinanceManagementConsts.WORKFLOW_STATUS_START)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var newOutcomingEntry = new OutcomingEntry
            {
                Name = input.Name,
                OutcomingEntryTypeId = outcomEntryToClone.OutcomingEntryTypeId,
                WorkflowStatusId = startlWorkflowStatusId,
                CurrencyId = input.CurrencyId,
                BranchId = outcomEntryToClone.BranchId,
                AccountId = outcomEntryToClone.AccountId,
                PaymentCode = outcomEntryToClone.PaymentCode,
                PeriodId = outcomEntryToClone.PeriodId,
                Value = hasDetail ? outcomEntryToClone.Value : input.Value,
                Accreditation = outcomEntryToClone.Accreditation,
            };

            var newOutcomeEntryId = await _ws.InsertAndGetIdAsync(newOutcomingEntry);

            await CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
            {
                OutcomingEntryId = newOutcomeEntryId,
                WorkflowStatusId = startlWorkflowStatusId,
                Value = outcomEntryToClone.Value,
            });

            await CloneOutcomingEntryDetail(outcomEntryToClone.Id, newOutcomeEntryId);

            return newOutcomeEntryId;
        }

        public async Task CloneOutcomingEntryDetail(long outcomeEntryToCloneId, long newOutcomeEntryId)
        {
            var outcomeDetailsToClone = _ws.GetAll<OutcomingEntryDetail>()
                .Where(x => x.OutcomingEntryId == outcomeEntryToCloneId)
                .ToList();

            var insertDetails = new List<OutcomingEntryDetail>();
            foreach (var detail in outcomeDetailsToClone)
            {
                var newOutcomeDetail = new OutcomingEntryDetail
                {
                    AccountId = detail.AccountId,
                    Name = detail.Name,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    Total = detail.Total,
                    OutcomingEntryId = newOutcomeEntryId,
                    BranchId = detail.BranchId,
                };
                insertDetails.Add(newOutcomeDetail);
            }
            await _ws.InsertRangeAsync(insertDetails);
        }
    }
}
