using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using FinanceManagement.APIs.BankAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.APIs.BankAccounts
{
    public static class BankAccountFilter
    {
        public static IQueryable<DetailBankAccountDto> FiltersByBankNumber(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(!gridParam.BankNumber.IsNullOrEmpty(), s => s.BankNumber == gridParam.BankNumber);
        }
        public static IQueryable<DetailBankAccountDto> FiltersByBankIds(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(!gridParam.BankIds.IsNullOrEmpty(), s => s.BankId.HasValue && gridParam.BankIds.Contains(s.BankId.Value));
        }
        public static IQueryable<DetailBankAccountDto> FiltersByCurrencyIds(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(!gridParam.CurrencyIds.IsNullOrEmpty(), s => s.CurrencyId.HasValue && gridParam.CurrencyIds.Contains(s.CurrencyId.Value));
        }
        public static IQueryable<DetailBankAccountDto> FiltersByAccountTypeIds(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(!gridParam.AccountTypeIds.IsNullOrEmpty(), s => gridParam.AccountTypeIds.Contains(s.AccountTypeId));
        }
        public static IQueryable<DetailBankAccountDto> FiltersByAccountIds(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(!gridParam.AccountIds.IsNullOrEmpty(), s => gridParam.AccountIds.Contains(s.AccountId));
        }
        public static IQueryable<DetailBankAccountDto> FiltersByMoney(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(gridParam.Amount.HasValue ,s => gridParam.Amount == s.Amount);
        }
        public static IQueryable<DetailBankAccountDto> FiltersByAccountTypeEnum(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(gridParam.AccountTypeEnum.HasValue, s => gridParam.AccountTypeEnum == s.AccountTypeEnum);
        }
        public static IQueryable<DetailBankAccountDto> FiltersByIsActive(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.WhereIf(gridParam.IsActive.HasValue, s => s.IsActive == gridParam.IsActive);
        }

        public static IQueryable<DetailBankAccountDto> FiltersByBankAccountGridParam(this IQueryable<DetailBankAccountDto> query, BankAccountGridParam gridParam)
        {
            return query.FiltersByBankNumber(gridParam)
                .FiltersByBankIds(gridParam)
                .FiltersByCurrencyIds(gridParam)
                .FiltersByAccountTypeIds(gridParam)
                .FiltersByMoney(gridParam)
                .FiltersByIsActive(gridParam)
                .FiltersByAccountIds(gridParam)
                .FiltersByAccountTypeEnum(gridParam);
        }


    }
}
