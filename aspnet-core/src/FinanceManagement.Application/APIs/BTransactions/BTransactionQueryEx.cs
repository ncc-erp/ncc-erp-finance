using Abp.Linq.Extensions;
using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.BTransactions
{
    public static class BTransactionQueryEx
    {
        public static IQueryable<GetAllBTransactionDto> FiltersByMoney(this IQueryable<GetAllBTransactionDto> query, BTransactionGridParam gridParam)
        {
            if(gridParam.FilterMoneyParam != null)
            {
                switch (gridParam.FilterMoneyParam.Type)
                {
                    case ExpressionEnum.NO_FILTER:
                        break;
                    case ExpressionEnum.LESS_OR_EQUAL:
                        query = query.WhereIf(gridParam.FilterMoneyParam.FromValue.HasValue, x => x.MoneyNumber <= gridParam.FilterMoneyParam.FromValue);
                        break;
                    case ExpressionEnum.LARGER_OR_EQUAL:
                        query = query.WhereIf(gridParam.FilterMoneyParam.FromValue.HasValue, x => x.MoneyNumber >= gridParam.FilterMoneyParam.FromValue);
                        break;
                    case ExpressionEnum.EQUAL:
                        query = query.WhereIf(gridParam.FilterMoneyParam.FromValue.HasValue, x => x.MoneyNumber == gridParam.FilterMoneyParam.FromValue);
                        break;
                    case ExpressionEnum.FT:
                        query = query.WhereIf(gridParam.FilterMoneyParam.FromValue.HasValue, x => x.MoneyNumber >= gridParam.FilterMoneyParam.FromValue)
                                     .WhereIf(gridParam.FilterMoneyParam.ToValue.HasValue,x => x.MoneyNumber <= gridParam.FilterMoneyParam.ToValue);
                        break;
                }
            }
            return query;
        }
        public static IQueryable<GetAllBTransactionDto> FiltersByDateTime(this IQueryable<GetAllBTransactionDto> query, BTransactionGridParam gridParam)
        {
            if(gridParam.FilterDateTimeParam == null)
                return query;

            return query.WhereIf(gridParam.FilterDateTimeParam.FromDate.HasValue, x => x.TimeAt.Date >= gridParam.FilterDateTimeParam.FromDate.Value.Date)
                        .WhereIf(gridParam.FilterDateTimeParam.ToDate.HasValue, x => x.TimeAt.Date <= gridParam.FilterDateTimeParam.ToDate.Value.Date);
        }
    }
}
