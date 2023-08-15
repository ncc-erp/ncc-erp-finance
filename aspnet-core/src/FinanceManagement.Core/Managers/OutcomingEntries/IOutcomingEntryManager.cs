using Abp.Dependency;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.OutcomingEntries
{
    public interface IOutcomingEntryManager : ITransientDependency
    {
        Task<OutcomingEntryMoneyInfo> GetOutcomingEntryMoneyInfo(long outcomingEntryId);
        Task CheckChangeOutcomingEntryStatus(long outcomingEntryId);
        Task<GetWorkflowChangeStatusInfoDto> GetWorkflowCodeWhenChangeStatus(long statusTransitionId);
        Task<bool> CheckCreateRelationInOut(long incomingEntryId);
        Task ChangeAllOutcomingEntryDetail(ChangeAllDetailOutcomingEntryDto input);
        Task<long> CreateRelationInOut(CreateRelationInOutDto input);
        Task SetDoneOutcomingEntry(SetDoneOutcomingEntryDto input);
        Task CreateOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input);
        Task CreateOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input, int periodId);
        Task<bool> UpdateValueToStartOutcomingStatusHistory(CreateOutcomingEntryStatusHistoryDto input);
        Task<IEnumerable<GetOutcomingEntryStatusHistoryDto>> GetOutcomingEntryStatusHistoryByOutcomingEntryId(long outcomingEntryId);
        IQueryable<GetOutcomingEntryStatusHistoryDto> IQGetOutcomingEntryStatusHistory();
        Dictionary<long, IEnumerable<GetOutcomingEntryStatusHistoryDto>> GetDictionaryStatusHistories(IEnumerable<GetOutcomingEntryStatusHistoryDto> input, Dictionary<long, string> dicUsers);
        IQueryable<GetOutcomingEntryStatusHistoryDto> IQGetTempOutcomingHistory();
        Task<int> CountOutcomingEntryNotEndForClosePeriod();
        Task<List<OutcomingEntryInfoDto>> GetOutcomingEntryNotEndForClosePeriod();
        Task Delete(long id);
        Task UpdateReportDate(UpdateReportDateDto input);
        Task UpdateOutcomingEntryType(UpdateOutcomEntryTypeDto input);
        Task<long> CloneOutcomingEntry(CloneOutcomeDto input);
        Task<bool> IsOutcomingEntryHasDetail(long outcomingEntryId);
    }
}
