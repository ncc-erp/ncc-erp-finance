using FinanceManagement.APIs.IncomingEntries;
using FinanceManagement.APIs.IncomingEntries.Dto;
using FinanceManagement.APIs.IncomingEntryReport.Dto;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using FinanceManagement.Managers.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.IncomingEntryReport
{
    public class IncomingEntryReportAppService : FinanceManagementAppServiceBase
    {
        private readonly IMyUserManager _myUserManager;
        private readonly ICommonManager _commonManager;

        public IncomingEntryReportAppService(IMyUserManager myUserManager, ICommonManager commonManager, IWorkScope workScope) : base(workScope)
        {
            _myUserManager = myUserManager;
            _commonManager = commonManager;
        }

        [HttpPost]
        public async Task<List<IncomingEntryTypeReportDto>> GetTreeByIncomingFilter(IncomingEntryGridParam input)
        {
            var incomingQuery = BuildIncomingQuery()
                .FiltersByIncomingEntryGridParam(input)
                .ApplySearchAndFilter(input);

            var incomingEntryTypeIds = await incomingQuery
                .Select(x => x.IncomingEntryTypeId)
                .Distinct()
                .ToListAsync();

            if (!incomingEntryTypeIds.Any())
            {
                return new List<IncomingEntryTypeReportDto>();
            }

            var allIncomingEntryTypes = await WorkScope.GetAll<IncomingEntryType>()
                .Where(i => i.IsActive)
                .Select(s => new IncomingEntryTypeReportDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Level = s.Level,
                    IsActive = s.IsActive,
                    ParentId = s.ParentId,
                    RevenueCounted = s.RevenueCounted
                })
                .ToListAsync();

            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(allIncomingEntryTypes);

            var resultIds = new List<long>();

            foreach(var id in incomingEntryTypeIds)
            {
                resultIds.AddRange(
                    _commonManager.GetAllEntryUpperNodeIds(id, treeHasRoot));
            }

            resultIds = resultIds.Distinct().ToList();

            var resultIncomingEntryTypes = allIncomingEntryTypes
                .Where(x => resultIds.Contains(x.Id))
                .ToList();

            await FillTotalCurrencyForIncomingTypes(resultIncomingEntryTypes, treeHasRoot, incomingQuery, input);

            return resultIncomingEntryTypes;
        }

        [HttpPost]
        public async Task<IncomingEntryReportDto> GetAllPaging(IncomingEntryGridParam input)
        {
            var respone = new IncomingEntryReportDto();

            var query = BuildIncomingQuery().FiltersByIncomingEntryGridParam(input);
            var result = await query.GetGridResult(query, input);

            var creatorUserIds = result.Items.Where(s => s.CreationUserId.HasValue).Select(s => s.CreationUserId.Value);
            var lastModifiedIds = result.Items.Where(s => s.LastModifiedUserId.HasValue).Select(s => s.LastModifiedUserId.Value);
            var dicUsers = await _myUserManager.GetDictionaryUserAudited(creatorUserIds.Union(lastModifiedIds));
            foreach (var item in result.Items)
            {
                item.CreationUser = item.CreationUserId.HasValue ? (dicUsers.ContainsKey(item.CreationUserId.Value) ? dicUsers[item.CreationUserId.Value] : string.Empty) : string.Empty;
                item.LastModifiedUser = item.LastModifiedUserId.HasValue ? (dicUsers.ContainsKey(item.LastModifiedUserId.Value) ? dicUsers[item.LastModifiedUserId.Value] : string.Empty) : string.Empty;
            }

            respone.ResultPagging = result;

            return respone;
        }

        private IQueryable<IncomingEntryDto> BuildIncomingQuery()
        {
            var query = (from ie in WorkScope.GetAll<IncomingEntry>().Include(x => x.IncomingEntryType).OrderByDescending(x => x.CreationTime)
                             //join cc in WorkScope.GetAll<CurrencyConvert>() on ie.CurrencyId equals cc.CurrencyId into ccs
                             //from cc in ccs.DefaultIfEmpty()
                         join bt in WorkScope.GetAll<BankTransaction>()
                              on ie.BankTransactionId equals bt.Id
                         join ba in WorkScope.GetAll<BankAccount>() on bt.FromBankAccountId equals ba.Id
                         join a in WorkScope.GetAll<Account>().Include(x => x.AccountType) on ba.AccountId equals a.Id
                         select new IncomingEntryDto
                         {
                             Id = ie.Id,
                             IncomingEntryTypeId = ie.IncomingEntryTypeId,
                             IncomingEntryTypeName = ie.IncomingEntryType.Name,
                             BankTransactionId = ie.BankTransactionId ?? 0,
                             Name = ie.Name,
                             AccountId = ie.AccountId ?? 0,
                             AccountName = ie.Account.Name,
                             BranchId = ie.BranchId,
                             CurrencyId = ie.CurrencyId ?? ie.BTransactions.BankAccount.CurrencyId,
                             CurrencyName = ie.Currency.Code ?? ie.BTransactions.BankAccount.Currency.Name,
                             CurrencyCode = ie.Currency.Code ?? ie.BTransactions.BankAccount.Currency.Code,
                             BranchName = ie.Branch.Name,
                             Status = ie.Status,
                             Value = ie.Value,
                             //ValueToVND = ie.Value * cc.Value,
                             Date = bt.TransactionDate,
                             ClientAccountId = a.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT ? ba.AccountId : default,
                             ClientName = a.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT ? ba.Account.Name : null,
                             CreationUserId = ie.CreatorUserId,
                             CreationTime = ie.CreationTime,
                             LastModifiedTime = ie.LastModificationTime,
                             LastModifiedUserId = ie.LastModifierUserId,
                             RevenueCounted = ie.IncomingEntryType.RevenueCounted
                         }).OrderByDescending(x => x.Date);
            return query;
        }

        public async Task<IEnumerable<GetTotalIncomingCurrencyDto>> GetTotalCurrencies(IQueryable<IncomingEntryDto> query, IncomingEntryGridParam input)
        {
            return await query
                .Select(x => new
                {
                    x.CurrencyId,
                    x.CurrencyName,
                    x.Value
                })
                .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                .Select(x => new GetTotalIncomingCurrencyDto
                {
                    CurrencyId = x.Key.CurrencyId,
                    CurrencyName = x.Key.CurrencyName,
                    TotalValue = x.Sum(x => x.Value)
                })
                .ToListAsync();
        }
        private async Task<Dictionary<long, double>> GetLatestCurrencyConvertDictionary()
        {
            var listCurrencyConvert = await WorkScope.GetAll<CurrencyConvert>()
                .AsNoTracking()
                .ToListAsync();

            return listCurrencyConvert
                .GroupBy(x => x.CurrencyId)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.DateAt).ThenByDescending(y => y.Id).First().Value
                );
        }
        private double CalculateVNDConvert(
            IEnumerable<GetTotalIncomingCurrencyDto> totalCurrencies,
            IReadOnlyDictionary<long, double> lastestCurrencyConvertDict)
        {
            return totalCurrencies.Sum(x =>
            {
                if (!x.CurrencyId.HasValue)
                {
                    return 0;
                }

                if (!lastestCurrencyConvertDict.TryGetValue(x.CurrencyId.Value, out var exchangeRate) || exchangeRate == 0)
                {
                    return 0;
                }

                return x.TotalValue * exchangeRate;
            });
        }

        private async Task FillTotalCurrencyForIncomingTypes(
            IEnumerable<IncomingEntryTypeReportDto> entryTypes,
            TreeItem<OutputCategoryEntryType> treeHasRoot,
            IQueryable<IncomingEntryDto> incomingQuery,
            IncomingEntryGridParam input)
        {
            foreach (var entryType in entryTypes)
            {
                var childIds = _commonManager.GetAllEntryLowerNodeIds(entryType.Id, treeHasRoot);
                if (childIds == null || childIds.Count == 0)
                {
                    childIds = new List<long> { entryType.Id };
                }

                var query = incomingQuery.Where(x => childIds.Contains(x.IncomingEntryTypeId));
                var totalCurrencies = (await GetTotalCurrencies(query, input)).ToList();

                entryType.TotalCurrencies = totalCurrencies;
            }
        }
    }
}
