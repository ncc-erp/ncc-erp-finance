using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using FinanceManagement.APIs.BankTransactions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.BankTransactions
{
    public static class BankTransactionQueryEx
    {
        public static IQueryable<DetailBankTransactionDto> FiltersByFromMoney(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(gridParam.FromMoney.HasValue, s => s.FromValue == gridParam.FromMoney.Value);
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByToMoney(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(gridParam.ToMoney.HasValue, s => s.ToValue == gridParam.ToMoney.Value);
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByFee(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(gridParam.Fee.HasValue, s => s.Fee == gridParam.Fee.Value);
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByFromBankAccounts(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(!gridParam.FromBankAccounts.IsNullOrEmpty(), s => s.FromBankAccountId.HasValue && gridParam.FromBankAccounts.Contains(s.FromBankAccountId.Value));
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByToBankAccounts(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(!gridParam.ToBankAccounts.IsNullOrEmpty(), s => s.ToBankAccountId.HasValue && gridParam.ToBankAccounts.Contains(s.ToBankAccountId.Value));
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByToCurency(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(!gridParam.ToCurrencyIds.IsNullOrEmpty(), s => s.ToBankAccountCurrencyId.HasValue && gridParam.ToCurrencyIds.Contains(s.ToBankAccountCurrencyId.Value));
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByFromCurency(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.WhereIf(!gridParam.FromCurrencyIds.IsNullOrEmpty(), s => s.FromBankAccountCurrencyId.HasValue && gridParam.FromCurrencyIds.Contains(s.FromBankAccountCurrencyId.Value));
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByDateTime(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            if (gridParam.FilterDateTime != null)
            {
                switch (gridParam.FilterDateTime.DateTimeType)
                {
                    case BankTransactionFilterDateTimeType.NO_FILTER:
                        break;
                    case BankTransactionFilterDateTimeType.TRANSACTION_TIME:
                        query = query.WhereIf(gridParam.FilterDateTime.FromDate.HasValue, x => x.TransactionDate.Date >= gridParam.FilterDateTime.FromDate)
                                     .WhereIf(gridParam.FilterDateTime.ToDate.HasValue, x => x.TransactionDate.Date <= gridParam.FilterDateTime.ToDate);
                        break;
                    case BankTransactionFilterDateTimeType.CREATE_TIME:
                        query = query.WhereIf(gridParam.FilterDateTime.FromDate.HasValue, x => x.CreateDate.Date >= gridParam.FilterDateTime.FromDate)
                                     .WhereIf(gridParam.FilterDateTime.ToDate.HasValue, x => x.CreateDate.Date <= gridParam.FilterDateTime.ToDate);
                        break;
                }
            }
            return query;
        }
        public static IQueryable<DetailBankTransactionDto> FiltersByGetAllPagingBankTransactionDto(this IQueryable<DetailBankTransactionDto> query, GetAllPagingBankTransactionDto gridParam)
        {
            return query.FiltersByFromMoney(gridParam)
                .FiltersByToMoney(gridParam)
                .FiltersByFee(gridParam)
                .FiltersByFromBankAccounts(gridParam)
                .FiltersByToBankAccounts(gridParam)
                .FiltersByFromCurency(gridParam)
                .FiltersByToCurency(gridParam)
                .FiltersByDateTime(gridParam);
        }
    }
}
