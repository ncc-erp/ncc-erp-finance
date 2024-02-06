
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using FinanceManagement.APIs.OutcomingEntries.Dto;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntries
{
    public static class OutcomingEntryQueryEx
    {
        public static IQueryable<GetOutcomingEntryDto> FiltersByOutcomingEntryGridParam(this IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto gridParam)
        {
            return query.FiltersByOutcomingEntryType(gridParam)
                        .FiltersByExpenseType(gridParam)
                        .FiltersByDateTime(gridParam);
        }
        public static IQueryable<GetOutcomingEntryDto> FiltersByDateTime(this IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto gridParam)
        {
            if (gridParam.FilterDateTimeParam != null)
            {
                switch (gridParam.FilterDateTimeParam.DateTimeType)
                {
                    case OutcomingEntryFilterDateTimeType.NO_FILTER:
                        break;
                    case OutcomingEntryFilterDateTimeType.REPORT_DATE:
                        query = query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.ReportDate.Value.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date)
                                     .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.ReportDate.Value.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date);
                        break;
                }
            }
            return query;

        }

        public static IQueryable<GetOutcomingEntryDto> FiltersByExpenseType(this IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto gridParam)
        {
            return query.WhereIf(gridParam.ExpenseType.HasValue, s => s.ExpenseType.Value == gridParam.ExpenseType.Value);
        }

        public static IQueryable<GetOutcomingEntryDto> FiltersByOutcomingEntryType(this IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto gridParam)
        {
            if (gridParam.OutComingEntryTypeIds == null || gridParam.OutComingEntryTypeIds.IsNullOrEmpty()) return query;
            if (gridParam.OutComingEntryTypeIds.Count == 1) return query.Where(s => gridParam.OutComingEntryTypeIds[0] == s.OutcomingEntryTypeId);
            return query.Where(s => gridParam.OutComingEntryTypeIds.Contains(s.OutcomingEntryTypeId));
        }
    }
}
