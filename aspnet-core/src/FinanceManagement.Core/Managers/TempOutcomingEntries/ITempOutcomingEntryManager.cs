using Abp.Dependency;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.TempOutcomingEntries
{
    public interface ITempOutcomingEntryManager : ITransientDependency
    {
        Task<bool> IsExistTempOutComingEntry(long outommingEntryId);
        Task<bool> IsExistTempOutcomingEntryPendingCEO(long outommingEntryId);
        Task<GetTempOutcomingEntryDto> CreateTempOutCommingEntry(long outommingEntryId);
        Task<RequestChangeOutcomingEntryInfoDto> GetRequestChangeOutcomingEntry(long tempOutcomingEntryId);
        Task<RequestChangeOutcomingEntryInfoDto> ViewHistoryChangeOutcomingEntry(long tempId, long rootId);
        Task<GetRequestChangeOutcomingEntryDetailDto> GetRequestChangeOutcomingEntryDetail(long tempOutcomingEntryId);
        Task<GetRequestChangeOutcomingEntryDetailDto> ViewHistoryChangeOutcomingEntryDetail(long tempId, long rooId);
        Task<RequestChangeOutcomingEntryInfoDto> SaveTempOutCommingEntry(UpdateTempOutcomingEntryDto input);
        Task<GetTempOutcomingEntryDto> GetTempOutcomingEntry(long tempOutcomingEntryId);
        Task<string> RejectTemp(long tempOutcomingEntryId);
        Task<long> GetTempIdByOutcomingId(long tempOutcomingEntryId);
        Task<long> GetTempIdHaveStatusPendingByOutcomingId(long outcomingEntryId);
        Task<string> ApprovedTemp(long tempOutcomingEntryId);
        Task<string> SendTemp(long tempOutcomingEntryId);
        Task CreateTempOutcomingEntryDetail(CreateTempOutcomingEntryDetailDto input);
        Task UpdateTempOutcomingEntryDetail(UpdateTempOutcomingEntryDetailDto input);
        Task DeleteTempOutcomingEntryDetail(long tempOutcomingEntryDetailId);
        Task GetButtonInfo(GetOutcomingEntryDto input);
        Task<GetOutcomingEntryDto> GetRootOutcomingEntry(long outcomingEntryId);
        Task RevertTempOutcomingDetailByRootId(long rootOutcomingEntryDetailId, long rootOutcomingEntryId);
        Task<string> GetStatusCodeByTempId(long tempId);
        Task<long> GetOutcomingEntryIdByTempId(long tempId);
        Task<bool> CheckTempOutcomingEntryApproved(long tempId);
        Task<bool> CheckTempOutCommingEntryHasDetail(long tempOutcomingEntryId);
        Dictionary<long, long> GetDicOutCommingEntryIdToPendingCEOTempId(IEnumerable<long> outCommingEntryIds);
        Dictionary<long, long> GetDicOutCommingEntryIdToOrtherAPPROVEDTempId(IEnumerable<long> outCommingEntryIds);
    }
}
