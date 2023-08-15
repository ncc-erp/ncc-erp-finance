using Abp.Linq.Extensions;
using FinanceManagement.APIs.OutcomingEntryDetails.Dto;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails
{
    public static class GetOutcomingEntryDetailDtoFilter
    {
        public static IQueryable<GetOutcomingEntryDetailDto> FilterByBranchId(this IQueryable<GetOutcomingEntryDetailDto> query, OutcomingEntryDetailFilterDto gridParam)
        {
            return query.WhereIf(gridParam.BranchId.HasValue, s => s.BranchId == gridParam.BranchId.Value);
        }
        public static IQueryable<GetOutcomingEntryDetailDto> FilterByIsNotDone(this IQueryable<GetOutcomingEntryDetailDto> query, OutcomingEntryDetailFilterDto gridParam)
        {
            return query.WhereIf(gridParam.IsNotDone.HasValue, s => s.IsNotDone == gridParam.IsNotDone);
        }
        public static IQueryable<GetOutcomingEntryDetailDto> FilterByOutcomingEntryDetailFilterDto(this IQueryable<GetOutcomingEntryDetailDto> query, OutcomingEntryDetailFilterDto gridParam)
        {
            return query.FilterByBranchId(gridParam)
                .FilterByIsNotDone(gridParam);
        }
    }
}
