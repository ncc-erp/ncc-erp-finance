using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.RelationInOutEntrys.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.RelationInOutEntrys
{
    [AbpAuthorize]
    public class RelationInOutEntryAppService : FinanceManagementAppServiceBase
    {
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        private readonly IMySettingManager _mySettingManager;
        private readonly ICommonManager _commonManager;
        public RelationInOutEntryAppService(
            IWorkScope workScope,
            IOutcomingEntryManager outcomingEntryManager,
            IMySettingManager mySettingManager,
            ICommonManager commonManager
        ) : base(workScope)
        {
            _outcomingEntryManager = outcomingEntryManager;
            _mySettingManager = mySettingManager;
            _commonManager = commonManager;
        }

        [HttpPost]
        // [AbpAuthorize(PermissionNames.Finance_RelationInOutEntry_ViewAll)]
        public async Task<GridResult<RelationInOutEntryDto>> GetAllPaging(GridParam input)
        {
            var query = from r in WorkScope.GetAll<RelationInOutEntry>()
                        select new RelationInOutEntryDto
                        {
                            Id = r.Id,
                            IncomingEntryId = r.IncomingEntryId,
                            OutcomingEntryId = r.OutcomingEntryId
                        };
            return await query.GetGridResult(query, input);
        }

        [HttpGet]
        //  [AbpAuthorize(PermissionNames.Finance_RelationInOutEntry_ViewInOut)]
        public async Task<List<OutComingEntryByInComingDto>> GetOutByInId(long id)
        {
            var query = from r in WorkScope.GetAll<RelationInOutEntry>().Where(x => x.IncomingEntryId == id)
                        select new RelationInOutEntryDto
                        {
                            Id = r.Id,
                            IncomingEntryId = r.IncomingEntryId,
                            OutcomingEntryId = r.OutcomingEntryId
                        };

            return await WorkScope.GetAll<RelationInOutEntry>().Where(x => x.IncomingEntryId == id)
                            .Select(x => new OutComingEntryByInComingDto
                            {
                                Id = x.OutcomingEntryId,
                                Name = x.OutcomingEntry.Name,
                                Value = x.OutcomingEntry.Value,
                            }).ToListAsync();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource)]
        public async Task<List<InComingEntryByOutComingDto>> GetInByOutId(long id)
        {
            var query = from r in WorkScope.GetAll<RelationInOutEntry>().Where(x => x.OutcomingEntryId == id)
                        select new RelationInOutEntryDto
                        {
                            Id = r.Id,
                            IncomingEntryId = r.IncomingEntryId,
                            OutcomingEntryId = r.OutcomingEntryId,
                        };

            return await WorkScope.GetAll<RelationInOutEntry>().Where(x => x.OutcomingEntryId == id)
                            .Select(x => new InComingEntryByOutComingDto
                            {
                                Id = x.IncomingEntryId,
                                Name = x.IncomingEntry.Name,
                                Status = x.IncomingEntry.Status,
                                Value = x.IncomingEntry.Value,
                                IncomingEntryTypeName = x.IncomingEntry.IncomingEntryType.Name,
                                RevenueCounted = x.IncomingEntry.IncomingEntryType.RevenueCounted,
                                IsRefund = x.IsRefund,
                                RelationInOutId = x.Id
                            }).ToListAsync();
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry)]
        public async Task<RelationInOutEntryDto> Delete(long IntId, long OutId)
        {
            var getR = await WorkScope.GetAll<RelationInOutEntry>().Where(x => x.IncomingEntryId == IntId && x.OutcomingEntryId == OutId).ToListAsync();
            if (getR.Count() <= 0)
            {
                throw new UserFriendlyException("Relation in out entry doesn't exist.");
            }
            await WorkScope.DeleteAsync(getR.First());
            return new RelationInOutEntryDto
            {
                Id = getR.First().Id,
                IncomingEntryId = getR.First().IncomingEntryId,
                OutcomingEntryId = getR.First().OutcomingEntryId
            };
        }
        //[HttpGet]
        //public async Task<bool> CheckCreateRelationInOut(long incomingEntryId)
        //{
        //    return await _outcomingEntryManager.CheckCreateRelationInOut(incomingEntryId);
        //}
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry)]
        public async Task<RelationInOutEntryDto> Create(RelationInOutEntryDto input)
        {
            var getR = await WorkScope.GetAll<RelationInOutEntry>().AnyAsync(x => x.IncomingEntryId == input.IncomingEntryId && x.OutcomingEntryId == input.OutcomingEntryId);
            if (getR)
            {
                throw new UserFriendlyException("Relation in out entry existed.");
            }

            var getInCurrency = await WorkScope.GetAll<IncomingEntry>()
                .Where(s => s.Id == input.IncomingEntryId)
                .Where(s => s.IncomingEntryType.IsClientPaid || s.IncomingEntryType.IsClientPrePaid)
                .Select(s => s.BTransactions.BankAccount.CurrencyId)
                .FirstOrDefaultAsync();

            if (getInCurrency != default)
            {
                throw new UserFriendlyException("Không thể link tới ghi nhận thu khách hàng trả nợ!");
            }

            var isAllowOutcomingEntryByMutipleCurrency = await IsAllowOutcomingEntryByMutipleCurrency();
            if (!isAllowOutcomingEntryByMutipleCurrency)
            {
                var currencyDefault = await GetCurrencyDefaultAsync();
                if (getInCurrency != currencyDefault.Id)
                {
                    throw new UserFriendlyException($"Không thể link tới ghi nhận thu khác tiền {currencyDefault.Name}");
                }
            }
            else if (input.IsRefund)
            {
                var hasInRequestChi = await WorkScope.GetAll<RelationInOutEntry>()
                    .Where(x => x.IncomingEntryId == input.IncomingEntryId && x.IsRefund)
                    .Select(x => new {x.OutcomingEntryId, x.OutcomingEntry.Name})
                    .FirstOrDefaultAsync();

                if (!hasInRequestChi.IsNullOrDefault())
                    throw new UserFriendlyException($"Không thể link vì đã tồn tại trong Request chi {hasInRequestChi.OutcomingEntryId} {hasInRequestChi.Name} với trạng thái [HOÀN TIỀN]");
            }

            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            var outcomingEntry = await WorkScope.GetAll<OutcomingEntry>()
                .Select(x => new { x.Id, x.WorkflowStatusId, x.CurrencyId, x.Currency.Name })
                .FirstOrDefaultAsync(x => x.Id == input.OutcomingEntryId);

            var currencyIncomingEntry = await WorkScope.GetAll<IncomingEntry>()
                .Where(x => x.Id == input.IncomingEntryId)
                .Select(x => new { x.CurrencyId, x.Currency.Name })
                .FirstOrDefaultAsync();

            if (outcomingEntry.CurrencyId != currencyIncomingEntry.CurrencyId)
                throw new UserFriendlyException($"Không thể link ghi nhận thu tiền {currencyIncomingEntry?.Name} với request chi tiền {outcomingEntry?.Name}");

            if(!outcomingEntry.IsNullOrDefault() && outcomingEntry.WorkflowStatusId == statusEndId && input.IsRefund)
                throw new UserFriendlyException($"Không thể link Ghi nhận thu tới Request chi đã [DONE] với trạng thái [HOÀN TIỀN] ");

            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<RelationInOutEntry>(input));
            return input;
        }
        [HttpPost]
        //TODO::Permission
        public async Task SetIsRefund(SetRefundRelationInOutDto input)
        {
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            var relationInOut = await WorkScope.GetAsync<RelationInOutEntry>(input.Id);
            var outcomingEntry = await WorkScope.GetAll<OutcomingEntry>()
                .Select(x => new { x.Id, x.WorkflowStatusId })
                .FirstOrDefaultAsync(x => x.Id == relationInOut.OutcomingEntryId && x.WorkflowStatusId == statusEndId);

            if (outcomingEntry != default)
                throw new UserFriendlyException($"Không thể đổi trạng thái [Hoàn tiền] khi Request chi đã [DONE]");

            relationInOut.IsRefund = input.IsRefund;
            await WorkScope.UpdateAsync(relationInOut);
        }
    }
}
