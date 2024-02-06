using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using FinanceManagement.APIs.IncomingEntries.Dto;
using FinanceManagement.Extension;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.IncomingEntries
{
    public static class IncomingEntryQueryEx
    {
        public static IQueryable<IncomingEntryDto> FiltersByMoney(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            return query.WhereIf(gridParam.Money.HasValue, s => s.Value == gridParam.Money.Value);
        }
        public static IQueryable<IncomingEntryDto> FiltersByCurrency(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            return query.WhereIf(gridParam.CurrencyId.HasValue, s => s.CurrencyId == gridParam.CurrencyId.Value);
        }
        public static IQueryable<IncomingEntryDto> FiltersByClient(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            if(gridParam.ClientAccountIds != null && !gridParam.ClientAccountIds.IsEmpty())
            {
                return query.Where(s => gridParam.ClientAccountIds.Contains(s.ClientAccountId));
            }
            return query;
        }
        public static IQueryable<IncomingEntryDto> FiltersByIncomingEntryType(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            if (gridParam.IncomingEntryTypeIds.IsNullOrEmpty()) return query;
            if (gridParam.IncomingEntryTypeIds.Count == 1) return query.Where(s => gridParam.IncomingEntryTypeIds[0] == s.IncomingEntryTypeId);
            return query.Where(s => gridParam.IncomingEntryTypeIds.Contains(s.IncomingEntryTypeId));
        }
        public static IQueryable<IncomingEntryDto> FiltersById(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            return query.WhereIf(gridParam.Id.HasValue, s => s.Id == gridParam.Id.Value);
        }
        public static IQueryable<IncomingEntryDto> FiltersByRevenueCounted(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            return query.WhereIf(gridParam.RevenueCounted.HasValue, s => s.RevenueCounted == gridParam.RevenueCounted.Value);
        }
        
        public static IQueryable<IncomingEntryDto> FiltersByDateTime(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            if (gridParam.FilterDateTimeParam != null)
            {
                switch (gridParam.FilterDateTimeParam.DateTimeType)
                {
                    case IncomingEntryFilterDateTimeType.NO_FILTER:
                        break;
                    case IncomingEntryFilterDateTimeType.CREATION_TIME:
                        query = query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.CreationTime.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date)
                                     .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.CreationTime.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date);
                        break;
                    case IncomingEntryFilterDateTimeType.TRANSACTION_TIME:
                        query = query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.Date.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date)
                                     .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.Date.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date);
                        break;
                    case IncomingEntryFilterDateTimeType.UPDATED_TIME:
                        query = query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.UpdatedTime.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date)
                                     .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.UpdatedTime.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date);
                        break;
                }
            }    
            return query;

        }
        public static IQueryable<IncomingEntryDto> FiltersByIncomingEntryGridParam(this IQueryable<IncomingEntryDto> query, IncomingEntryGridParam gridParam)
        {
            return query.FiltersById(gridParam)
                        .FiltersByClient(gridParam)
                        .FiltersByCurrency(gridParam)
                        .FiltersByMoney(gridParam)
                        .FiltersByIncomingEntryType(gridParam)
                        .FiltersByRevenueCounted(gridParam)
                        .FiltersByDateTime(gridParam);
        }



    }
}
