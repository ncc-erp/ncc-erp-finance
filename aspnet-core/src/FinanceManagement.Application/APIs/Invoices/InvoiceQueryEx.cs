using FinanceManagement.Entities;
using FinanceManagement.Managers.Invoices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinanceManagement.Enums;
using Abp.Linq.Extensions;
using static FinanceManagement.Managers.Invoices.Dtos.InvoiceAndIncomByAccountDto;
using FinanceManagement.APIs.Invoices.Dto;
using FinanceManagement.IoC;
using FinanceManagement.APIs.RevenueManageds.Dto;
using FinanceManagement.Paging;

namespace FinanceManagement.APIs.Invoices
{
    public static class InvoiceQueryEx
    {
        public static GridResult<T> GridResultEnumerable<T>(this IEnumerable<T> query, GridParam gridParams) where T : class
        {
            var list = query.TakePageEnumerable(gridParams).ToList();
            var count = query.Count();
            return new GridResult<T>(list, count);
        }
        public static IEnumerable<T> TakePageEnumerable<T>(this IEnumerable<T> query, GridParam gridParam) where T : class
        {
            var newQuery = query;
            return newQuery.Skip(gridParam.SkipCount).Take(gridParam.MaxResultCount);
        }
        public static IQueryable<AllPropInvoiceAndIncomByAccountDto> IgnoreNull(this IQueryable<AllPropInvoiceAndIncomByAccountDto> query)
        {
            query = query.Where(x => x.InvoiceId.HasValue || x.IncomingEntryId.HasValue);
            return query;
        }
        public static IQueryable<AllPropInvoiceAndIncomByAccountDto> FiltersByStatusDebtAccount(this IQueryable<AllPropInvoiceAndIncomByAccountDto> query, InvoiceGridParam gridParam, IWorkScope _ws)
        {
            if (!gridParam.IsDoneDebt.HasValue)
                return query;
            if (gridParam.IsDoneDebt.Value)
            {
                var accountIdsDone = _ws.GetAll<Invoice>()
                    .Select(x => new { x.AccountId, x.Id, x.Status })
                    .AsEnumerable()
                    .GroupBy(x => x.AccountId)
                    .Select(x => new
                    {
                        IsDone = x.Where(s => s.Status != NInvoiceStatus.HOAN_THANH).Any(),
                        AccountId = x.Key
                    })
                    .Where(s => !s.IsDone)
                    .Select(s => s.AccountId)
                    .AsEnumerable();
                return query.Where(x => accountIdsDone.Contains(x.AccountId));
            }
            var accountIdsNotDone = _ws.GetAll<Invoice>()
                    .Where(s => s.Status != NInvoiceStatus.HOAN_THANH && s.Status != NInvoiceStatus.HOAN_THANH)
                    .Select(x => x.AccountId)
                    .Distinct()
                    .AsEnumerable();
            return query.Where(x => accountIdsNotDone.Contains(x.AccountId));
        }
        public static IQueryable<AllPropInvoiceAndIncomByAccountDto> FiltersByDateTime(this IQueryable<AllPropInvoiceAndIncomByAccountDto> query, InvoiceGridParam gridParam)
        {
            if (gridParam.FilterDateTimeParam == null)
                return query;

            return query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.Deadline.HasValue ? x.Deadline.Value.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date : false)
                        .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.Deadline.HasValue ? x.Deadline.Value.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date : false);
        }
        public static IQueryable<AllPropInvoiceAndIncomByAccountDto> FiltersByAccount(this IQueryable<AllPropInvoiceAndIncomByAccountDto> query, InvoiceGridParam gridParam)
        {
            return query.Where(s => gridParam.AccountIds.Contains(s.AccountId));
        }
        public static IQueryable<AllPropInvoiceAndIncomByAccountDto> FiltersByStatus(this IQueryable<AllPropInvoiceAndIncomByAccountDto> query, InvoiceGridParam gridParam)
        {
            if(gridParam.Statuses != null && gridParam.Statuses.Count >= 1)
            {
                return query.Where(s => gridParam.Statuses.Contains(s.Status));
            }
            return query;
        }
        public static List<StatisticInvoiceDto> GetStatisticOverview(IEnumerable<InvoiceAndIncomByAccountDto> query)
        {
            var results = new List<StatisticInvoiceDto>();

            var moneyRevenues = new List<MoneyInfoDto>();
            query.Select(s => s.TotalCollectionDebt)
                .ToList()
                .ForEach(item => moneyRevenues.AddRange(item));
            var revenues = moneyRevenues.GroupBy(s => new { s.CurrencyId, s.CurrencyName })
                .Select(x => new MoneyInfoDto
                {
                    CurrencyId = x.Key.CurrencyId,
                    CurrencyName = x.Key.CurrencyName,
                    TotalMoneyNumber = x.Sum(x => x.TotalMoneyNumber),
                }).ToList();

            var moneyPaid = new List<MoneyInfoDto>();
            query.Select(s => s.TotalPaid)
                .ToList()
                .ForEach(item => moneyPaid.AddRange(item));
            var paids = moneyPaid.GroupBy(s => new { s.CurrencyId, s.CurrencyName })
                .Select(x => new MoneyInfoDto
                {
                    CurrencyId = x.Key.CurrencyId,
                    CurrencyName = x.Key.CurrencyName,
                    TotalMoneyNumber = x.Sum(x => x.TotalMoneyNumber),
                }).ToDictionary(s => s.CurrencyId, s => s.TotalMoneyNumber);

            foreach (var item in revenues)
            {
                results.Add(new StatisticInvoiceDto
                {
                    CurrencyName = item.CurrencyName,
                    CurrencyId = item.CurrencyId,
                    CollectionDebtNumber = item.TotalMoneyNumber,
                    PaidNumber = paids.ContainsKey(item.CurrencyId) ? paids[item.CurrencyId] : 0
                });
            }
            return results;
        }
    }
}
