using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.UI;
using ClosedXML.Excel;
using FinanceManagement.APIs.BankTransactions.Dto;
using FinanceManagement.APIs.GetOutcomingEntries.Dto;
using FinanceManagement.APIs.OutcomingEntries.Dto;
using FinanceManagement.APIs.OutcomingEntryBankTransactions.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.ExportHelper;
using FinanceManagement.Extension;
using FinanceManagement.Paging;
using FinanceManagement.Uitls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.IoC;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Managers.Users;
using FinanceManagement.Notifications.Komu;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using FinanceManagement.Managers.TempOutcomingEntries;
using FinanceManagement.Entities.NewEntities;
using Abp.Collections.Extensions;
using FinanceManagement.GeneralModels;
using Microsoft.Extensions.Options;
using FinanceManagement.Managers.Commons;
using Abp.Linq.Extensions;
using FinanceManagement.Managers.BTransactions.Dtos;
using Org.BouncyCastle.Asn1.Ocsp;
using FinanceManagement.Notifications.Mezon;
using FinanceManagement.APIs.OutcomingEntryTypes.Dto;
using FinanceManagement.APIs.OutcomingEntries;
using FinanceManagement.APIs.Report.Dto;

namespace FinanceManagement.APIs.Report
{
    public class ReportAppService : FinanceManagementAppServiceBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        private readonly IMyUserManager _myUserManager;
        private readonly IKomuNotification _komuNotification;
        private readonly ITempOutcomingEntryManager _tempOutcomingEntryManager;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly ICommonManager _commonManager;
        private readonly IMezonNotification _mezonNotification;

        public ReportAppService(
                    IWebHostEnvironment webHostEnvironment,
                    IWorkScope workScope,
                    IOutcomingEntryManager outcomingEntryManager,
                    IMyUserManager myUserManager,
                    IKomuNotification komuNotification,
                    ITempOutcomingEntryManager tempOutcomingEntryManager,
                    IOptions<ApplicationConfig> options,
                    ICommonManager commonManager,
                    IMezonNotification mezonNotification
            ) : base(workScope)
        {
            _hostingEnvironment = webHostEnvironment;
            _outcomingEntryManager = outcomingEntryManager;
            _myUserManager = myUserManager;
            _komuNotification = komuNotification;
            _tempOutcomingEntryManager = tempOutcomingEntryManager;
            _options = options;
            _commonManager = commonManager;
            _mezonNotification = mezonNotification;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Report_View)]
        public async Task<List<ReportOutcomingEntryTypeDto>> GetTreeByOutcomingFilter(GetAllPagingOutComingEntryDto input)
        {
            var outcomingQuery = BuildOutcomingQuery(input)
                .FiltersByOutcomingEntryGridParam(input)
                .ApplySearchAndFilter(input);

            var entryTypeIds = await outcomingQuery
                .Select(x => x.OutcomingEntryTypeId)
                .Distinct()
                .ToListAsync();

            if (!entryTypeIds.Any())
            {
                return new List<ReportOutcomingEntryTypeDto>();
            }

            var allEntryTypes = await WorkScope.GetAll<OutcomingEntryType>()
                .Where(s => s.IsActive)
                .Select(s => new ReportOutcomingEntryTypeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ParentId = s.ParentId,
                    Level = s.Level,
                    IsActive = s.IsActive,
                    ExpenseType = s.ExpenseType.Value,
                    Code = s.Code
                })
                .ToListAsync();

            var treeHasRoot = _commonManager.GetTreeEntryWithRoot(allEntryTypes);

            var resultIds = new List<long>();

            foreach (var id in entryTypeIds)
            {
                resultIds.AddRange(
                    _commonManager.GetAllEntryUpperNodeIds(id, treeHasRoot));
            }

            resultIds = resultIds.Distinct().ToList();

            var resultEntryTypes = allEntryTypes
                .Where(x => resultIds.Contains(x.Id))
                .ToList();

            await FillTotalCurrenciesForEntryTypes(resultEntryTypes, treeHasRoot, outcomingQuery, input);

            return resultEntryTypes;
        }

        private async Task FillTotalCurrenciesForEntryTypes(
            IEnumerable<ReportOutcomingEntryTypeDto> entryTypes,
            TreeItem<OutputCategoryEntryType> treeHasRoot,
            IQueryable<GetOutcomingEntryDto> outcomingQuery,
            GetAllPagingOutComingEntryDto input)
        {
            var latestCurrencyConvertDic = await GetLatestCurrencyConvertDictionary();

            foreach (var entryType in entryTypes)
            {
                var childIds = _commonManager.GetAllEntryLowerNodeIds(entryType.Id, treeHasRoot);
                if (childIds == null || childIds.Count == 0)
                {
                    childIds = new List<long> { entryType.Id };
                }

                var query = outcomingQuery.Where(x => childIds.Contains(x.OutcomingEntryTypeId));
                var totalCurrencies = (await GetTotalCurrencies(query, input)).ToList();
                entryType.TotalCurrencies = totalCurrencies;
                entryType.VNDConvert = CalculateVNDConvert(totalCurrencies, latestCurrencyConvertDic);
            }
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
            IEnumerable<GetTotalCurrencyDto> totalCurrencies,
            IReadOnlyDictionary<long, double> latestCurrencyConvertDic)
        {
            return totalCurrencies.Sum(x =>
            {
                if (!x.CurrencyId.HasValue)
                {
                    return 0;
                }

                if (!latestCurrencyConvertDic.TryGetValue(x.CurrencyId.Value, out var exchangeRate) || exchangeRate == 0)
                {
                    return 0;
                }

                return x.Value * exchangeRate;
            });
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Report_View)]
        public async Task<ReportOutcomingEntryDto> GetAllPaging(GetAllPagingOutComingEntryDto input)
        {
            var response = new ReportOutcomingEntryDto();

            var query = BuildOutcomingQuery(input).FiltersByOutcomingEntryGridParam(input);
            var resultPaging = await query.GetGridResult(query, input);

            var outcomingEntryIds = resultPaging.Items.Select(s => s.Id);
            var outcomingEntryStatusHistories = await _outcomingEntryManager
                .IQGetOutcomingEntryStatusHistory()
                .Where(x => outcomingEntryIds.Contains(x.OutcomingEntryId))
                .AsNoTracking()
                .ToListAsync();

            var outcomingEntryTempHistory = await _outcomingEntryManager.IQGetTempOutcomingHistory()
                .Where(x => outcomingEntryIds.Contains(x.OutcomingEntryId))
                .AsNoTracking()
                .ToListAsync();

            var catOutcomingEntryStatusHistories = outcomingEntryStatusHistories
                .Concat(outcomingEntryTempHistory)
                .OrderByDescending(x => x.CreationTime)
                .ToList();

            var outcomingEntryAuditedIds = resultPaging.Items
                .Where(s => s.CreationUserId.HasValue)
                .Select(s => s.CreationUserId.Value)
                .Union(
                    resultPaging.Items
                    .Where(s => s.LastModifiedUserId.HasValue)
                    .Select(s => s.LastModifiedUserId.Value)
                );

            var outcomingEntryAuditedStatusHistoryIds = catOutcomingEntryStatusHistories
                .Where(s => s.CreationUserId.HasValue)
                .Select(s => s.CreationUserId.Value)
                .ToList();

            var dicUsers = await _myUserManager.GetDictionaryUserAudited(outcomingEntryAuditedIds.Union(outcomingEntryAuditedStatusHistoryIds));
            var dicOutcomingStatusHistories = _outcomingEntryManager.GetDictionaryStatusHistories(catOutcomingEntryStatusHistories, dicUsers);

            var dicTempPenddingId = _tempOutcomingEntryManager.GetDicOutCommingEntryIdToPendingCEOTempId(outcomingEntryIds);
            foreach (var item in resultPaging.Items)
            {
                item.StatusHistories = dicOutcomingStatusHistories.ContainsKey(item.Id) ? dicOutcomingStatusHistories[item.Id] : default;
                item.CreationUser = item.CreationUserId.HasValue ? (dicUsers.ContainsKey(item.CreationUserId.Value) ? dicUsers[item.CreationUserId.Value] : string.Empty) : string.Empty;
                item.LastModifiedUser = item.LastModifiedUserId.HasValue ? (dicUsers.ContainsKey(item.LastModifiedUserId.Value) ? dicUsers[item.LastModifiedUserId.Value] : string.Empty) : string.Empty;
                await _tempOutcomingEntryManager.GetButtonInfo(item);
                item.TempOutcomingEntryId = dicTempPenddingId.ContainsKey(item.Id) ? dicTempPenddingId[item.Id] : default;
            }

            response.ResultPaging = resultPaging;

            return response;
        }
        public async Task<IEnumerable<GetTotalCurrencyDto>> GetTotalCurrencies(IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto input)
        {
            return await query
                .Select(x => new
                {
                    x.CurrencyId,
                    x.CurrencyName,
                    x.Value
                })
                .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                .Select(x => new GetTotalCurrencyDto
                {
                    CurrencyId = x.Key.CurrencyId,
                    CurrencyName = x.Key.CurrencyName,
                    Value = x.Sum(x => x.Value)
                })
                .ToListAsync();
        }

        public IQueryable<GetOutcomingEntryDto> BuildOutcomingQuery(GetAllPagingOutComingEntryDto input)
        {
            var requester = WorkScope.GetAll<User>();
            var roledIds = WorkScope.GetAll<UserRole>()
                            .Where(s => s.UserId == AbpSession.UserId.Value)
                            .Select(s => s.RoleId)
                            .ToList();
            var permission = WorkScope.GetAll<WorkflowStatusTransitionPermission>().Where(x => roledIds.Contains(x.RoleId));

            var action = from wst in WorkScope.GetAll<WorkflowStatusTransition>()
                         .Where(x => permission.Select(t => t.TransitionId).Contains(x.Id))
                         select new ActionDto
                         {
                             StatusTransitionId = wst.Id,
                             WorkflowId = wst.Workflow.Id,
                             FromStatusId = wst.FromStatusId,
                             ToStatusId = wst.ToStatusId,
                             Name = wst.Name
                         };

            var supplier = WorkScope.GetAll<OutcomingEntrySupplier>();
            var requestInBankTransaction = WorkScope.GetAll<OutcomingEntryBankTransaction>()
                                            .GroupBy(oe => oe.OutcomingEntryId)
                                            .Select(oe => new
                                            {
                                                a = oe.Key,
                                                Quantity = oe.Count()
                                            });
            var viewAll = PermissionChecker.IsGranted(PermissionNames.Finance_OutcomingEntry_View);

            var query = (from oe in WorkScope.GetAll<OutcomingEntry>().OrderByDescending(x => x.CreationTime)
                         where viewAll || oe.CreatorUserId == AbpSession.UserId.Value
                         select new GetOutcomingEntryDto
                         {
                             Id = oe.Id,
                             OutcomingEntryTypeId = oe.OutcomingEntryTypeId,
                             OutcomingEntryTypeCode = oe.OutcomingEntryType.Code,
                             OutcomingEntryTypeName = oe.OutcomingEntryType.Name,
                             ExpenseType = oe.OutcomingEntryType.ExpenseType,
                             Name = oe.Name,
                             Requester = requester.FirstOrDefault(r => r.Id == oe.CreatorUserId).Name,
                             AccountId = oe.AccountId,
                             AccountName = oe.Account.Name,
                             BranchId = oe.BranchId,
                             BranchName = oe.Branch.Name,
                             CurrencyId = oe.CurrencyId,
                             CurrencyName = oe.Currency.Code,
                             Value = oe.Value,
                             WorkflowStatusId = oe.WorkflowStatusId,
                             WorkflowStatusName = oe.WorkflowStatus.Name,
                             WorkflowStatusCode = oe.WorkflowStatus.Code,
                             Action = action.Where(x => x.FromStatusId == oe.WorkflowStatusId && oe.OutcomingEntryType.WorkflowId == x.WorkflowId).ToList(),
                             SupplierId = supplier.FirstOrDefault(x => x.OutcomingEntryId == oe.Id).SupplierId,
                             CreatedAt = oe.CreationTime.Date,
                             SendTime = oe.SentTime.Value.Date,
                             ApproveTime = oe.ApprovedTime.Value.Date,
                             ExecuteTime = oe.ExecutedTime.Value.Date,
                             PaymentCode = oe.PaymentCode,
                             IsAcceptFile = oe.IsAcceptFile,
                             CreatorUserId = oe.CreatorUserId,
                             Accreditation = oe.Accreditation,
                             CreationTime = oe.CreationTime,
                             CreationUserId = oe.CreatorUserId,
                             LastModifiedTime = oe.LastModificationTime,
                             LastModifiedUserId = oe.LastModifierUserId,
                             RequestInBankTransaction = requestInBankTransaction.Where(x => x.a == oe.Id).Select(x => x.Quantity).FirstOrDefault(),
                             ReportDate = oe.ReportDate
                         })
                         .Where(x => !input.Money.HasValue || x.Value == input.Money)
                         .Where(x => !input.Id.HasValue || x.Id == input.Id)
                         .Where(x => input.Branchs.IsNullOrEmpty() || (x.BranchId.HasValue && input.Branchs.Contains(x.BranchId.Value)))
                         .Where(x => input.Requesters.IsNullOrEmpty() || (x.CreationUserId.HasValue && input.Requesters.Contains(x.CreationUserId.Value)));

            query = FilterOutComingStatusCode(query, input.OutComingStatusCode);

            if (input.TempStatusCode.HasValue())
            {
                var outComingsHasTemp = GetOutComingHasTemp(input.TempStatusCode);
                query = query.Where(s => outComingsHasTemp.Contains(s.Id));
            }
            if (input.Accreditation) query = query.Where(x => x.Accreditation);
            return query;
        }
        private IQueryable<GetOutcomingEntryDto> FilterOutComingStatusCode(IQueryable<GetOutcomingEntryDto> query, string outComingStatusCode)
        {
            if (!outComingStatusCode.HasValue())
                return query;
            if (outComingStatusCode == Constants.WORKFLOW_STATUS_OR_YCTD_PENDINGCEO)
            {
                var outComingsHasTemp = GetOutComingHasTemp(Constants.WORKFLOW_STATUS_PENDINGCEO);
                return query.Where(x => x.WorkflowStatusCode == Constants.WORKFLOW_STATUS_PENDINGCEO || outComingsHasTemp.Contains(x.Id));
            }
            if (outComingStatusCode == Constants.WORKFLOW_STATUS_OTHER_END)
                return query.Where(x => x.WorkflowStatusCode != Constants.WORKFLOW_STATUS_END);
            return query.Where(x => x.WorkflowStatusCode == outComingStatusCode);
        }
        private List<long> GetOutComingHasTemp(string statusCode)
        {
            return _outcomingEntryManager.IQGetTempOutcomingHistory()
                .Where(x => x.WorkflowStatusCode == statusCode)
                .AsNoTracking()
                .Select(x => x.OutcomingEntryId)
                .ToList();
        }
    }
}
