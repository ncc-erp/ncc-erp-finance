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

namespace FinanceManagement.APIs.OutcomingEntries
{
    [AbpAuthorize]
    public class OutcomingEntryAppService : FinanceManagementAppServiceBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        private readonly IMyUserManager _myUserManager;
        private readonly IKomuNotification _komuNotification;
        private readonly ITempOutcomingEntryManager _tempOutcomingEntryManager;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly ICommonManager _commonManager;
        public OutcomingEntryAppService(
                   IWebHostEnvironment webHostEnvironment,
                   IWorkScope workScope,
                   IOutcomingEntryManager outcomingEntryManager,
                   IMyUserManager myUserManager,
                   IKomuNotification komuNotification,
                   ITempOutcomingEntryManager tempOutcomingEntryManager,
                   IOptions<ApplicationConfig> options,
                   ICommonManager commonManager
        ) : base(workScope)
        {
            _hostingEnvironment = webHostEnvironment;
            _outcomingEntryManager = outcomingEntryManager;
            _myUserManager = myUserManager;
            _komuNotification = komuNotification;
            _tempOutcomingEntryManager = tempOutcomingEntryManager;
            _options = options;
            _commonManager = commonManager;
        }

        [HttpPost]
		[AbpAuthorize(PermissionNames.Finance_OutcomingEntry_Create)]
		public async Task<List<GetOutcomingEntryDto>> CheckWarningCreateRequest(OutcomingEntryDto input)
        {
			var requester = WorkScope.GetAll<User>();

            var matchingEntries =
                from oe in WorkScope.GetAll<OutcomingEntry>()
                where
                    oe.Value == input.Value &&
                    oe.CurrencyId == input.CurrencyId &&
                    oe.OutcomingEntryTypeId == input.OutcomingEntryTypeId &&
                    oe.CreationTime > DateTimeUtils.GetNow().AddMonths(-2)
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
                    ReportDate = oe.ReportDate
                };

			return await matchingEntries.ToListAsync();
		}

		[HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_Create)]
        public async Task<OutcomingEntryDto> Create(OutcomingEntryDto input)
        {
            var initialWorkflowStatus = WorkScope.GetAll<WorkflowStatus>().FirstOrDefault(ws => ws.Code == Constants.WORKFLOW_STATUS_START);

            if (initialWorkflowStatus == null)
            {
                throw new UserFriendlyException("Workflow status with code [START] doesn't exist");
            }            
            input.WorkflowStatusId = initialWorkflowStatus.Id;

            var outcomingEntry = ObjectMapper.Map<OutcomingEntry>(input);

            if (!await IsAllowOutcomingEntryByMutipleCurrency())
            {
                var currencyDefault = await GetCurrencyDefaultAsync();
                if (currencyDefault.IsNullOrDefault())
                {
                    throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
                }
                outcomingEntry.CurrencyId = currencyDefault.Id;
            }

            outcomingEntry.Id = await WorkScope.InsertAndGetIdAsync(outcomingEntry);

            if (input.SupplierId != null)
            {
                var OutcomingEntrySupplier = new OutcomingEntrySupplier
                {
                    OutcomingEntryId = outcomingEntry.Id,
                    SupplierId = (long)input.SupplierId
                };

                await WorkScope.InsertAndGetIdAsync<OutcomingEntrySupplier>(OutcomingEntrySupplier);
            }

            var currency = await WorkScope.GetAsync<Currency>(outcomingEntry.CurrencyId.Value);

            await _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
            {
                OutcomingEntryId = outcomingEntry.Id,
                WorkflowStatusId = input.WorkflowStatusId,
                CurrencyName = currency.Name,
                Value = outcomingEntry.Value
            });
            return input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit)]
        public async Task<OutcomingEntryDto> Update(OutcomingEntryDto input)
        {
            var outcomingEntry = await WorkScope.GetAll<OutcomingEntry>()
                .Include(s => s.WorkflowStatus)
                .Where(s => s.Id == input.Id)
                .FirstOrDefaultAsync();

            if (outcomingEntry.WorkflowStatus.Code != FinanceManagementConsts.WORKFLOW_STATUS_START)
                throw new UserFriendlyException("Không sửa được request có trạng thái khác Tạo Mới");

            if (input.Value != outcomingEntry.Value)
            {
                bool hasOutcomingDetail = await _outcomingEntryManager.IsOutcomingEntryHasDetail(input.Id);
                if (hasOutcomingDetail)
                    throw new UserFriendlyException("Không thể sửa được value của request chi khi đã có chi tiết");
            }

            var isAllowOutcomingEntryByMultipleCurrency = await IsAllowOutcomingEntryByMutipleCurrency();
            if (!isAllowOutcomingEntryByMultipleCurrency)
            {
                var currencyDefault = await GetCurrencyDefaultAsync();
                if (currencyDefault.IsNullOrDefault())
                {
                    throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
                }
                if (currencyDefault.Id != input.CurrencyId)
                {
                    throw new UserFriendlyException("Không đúng loại tiền mặc định Request chi");
                }
            }
            else
            {
                var hasBankTransaction = outcomingEntry.OutcomingEntryBankTransactions.Any(x => !x.IsDeleted);
                if (outcomingEntry.CurrencyId != input.CurrencyId && hasBankTransaction)
                    throw new UserFriendlyException("Không thể cập nhật loại tiền khi Request chi đã có GDNH liên quan");
            }

            await WorkScope.UpdateAsync(ObjectMapper.Map<OutcomingEntryDto, OutcomingEntry>(input, outcomingEntry));

            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_View, PermissionNames.Finance_OutcomingEntry_ViewOnlyMe)]
        public async Task<ResultGetOutcomingEntryDto> GetAllPaging(GetAllPagingOutComingEntryDto input)
        {
            var response = new ResultGetOutcomingEntryDto();

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

            response.TotalCurrencies = await GetTotalCurrencies(query, input);
            return response;
        }
        private async Task<IEnumerable<GetTotalCurrencyDto>> GetTotalCurrencies(IQueryable<GetOutcomingEntryDto> query, GetAllPagingOutComingEntryDto input)
        {
            return await query.ApplySearchAndFilter(input)
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


        private IQueryable<GetOutcomingEntryDto> BuildOutcomingQuery(GetAllPagingOutComingEntryDto input)
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

        private IQueryable<CurrencyConvert> IQueryCurrencyConvert()
        {
            return WorkScope.GetAll<CurrencyConvert>().Where(s => s.DateAt < new DateTime(2000, 1, 1));
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_View)]
        public async Task<bool> IsOutcomingEntryHasDetail(long outcomingEntryId)
        {
            return await _outcomingEntryManager.IsOutcomingEntryHasDetail(outcomingEntryId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_View, PermissionNames.Finance_OutcomingEntry_ViewOnlyMe)]
        public async Task<List<GetTotalValueOutComingEntryDto>> GetTotalValueOutcomingEntry(GetAllPagingOutComingEntryDto input)
        {
            var query = BuildOutcomingQuery(input).FiltersByOutcomingEntryGridParam(input);
            var outcomings = await query.FilterByGridParam(input);

            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
            {
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
            }

            foreach (var outcoming in outcomings)
            {
                if (!outcoming.CurrencyId.HasValue)
                {
                    outcoming.CurrencyId = currencyDefault.Id;
                    //var currencyVNDId = await WorkScope.GetAll<Currency>().Where(x => x.Code == Constants.CURRENCY_VND).Select(x => x.Id).FirstOrDefaultAsync();
                    //outcoming.CurrencyId = currencyVNDId;
                }
            }

            var result = (from oc in outcomings.ToList()
                          group oc by new { oc.CurrencyId, oc.CurrencyName } into g
                          select new GetTotalValueOutComingEntryDto
                          {
                              CurrencyId = g.Key.CurrencyId,
                              CurrencyName = g.Key.CurrencyName,
                              TotalValue = g.Sum(x => x.Value),
                          }).ToList();

            return result;
        }

        [HttpGet]
        public async Task<string> UpdateValueForAllOutcomingEntry()
        {
            var oeDetail = WorkScope.GetAll<OutcomingEntryDetail>();
            var listUpdate = await (from oe in WorkScope.GetAll<OutcomingEntry>()
                                    select new
                                    {
                                        OutcomingEntryId = oe.Id,
                                        NewValue = oeDetail.Where(x => x.OutcomingEntryId == oe.Id).Sum(x => x.Total)
                                    }).ToListAsync();

            foreach (var oe in listUpdate)
            {
                var outcoming = await WorkScope.GetAsync<OutcomingEntry>(oe.OutcomingEntryId);
                outcoming.Value = oe.NewValue;
                await WorkScope.UpdateAsync(outcoming);
            }
            return $"Đã cập nhật value cho {listUpdate.Count()} Request Chi";
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_Delete)]
        public async Task Delete(long id)
        {
            await _outcomingEntryManager.Delete(id);
        }
        [HttpGet]
        public async Task<GetOutcomingEntryDto> Get(long id)
        {
            return await _tempOutcomingEntryManager.GetRootOutcomingEntry(id);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_UpdateReportDate, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate)]
        public async Task UpdateReportDate(UpdateReportDateDto input)
        {
            await _outcomingEntryManager.UpdateReportDate(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_UpdateRequestType, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType)]
        public async Task UpdateOutcomingEntryType(UpdateOutcomEntryTypeDto input)
        {
            await _outcomingEntryManager.UpdateOutcomingEntryType(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_ChangeStatus)]
        public async Task<ChangeStatusDto> ChangeStatus(ChangeStatusDto Input)
        {
            var statusTransition = await _outcomingEntryManager.GetWorkflowCodeWhenChangeStatus(Input.StatusTransitionId);

            if (statusTransition.ToStatusCode.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_END)
            {
                var isExistTempPending = await _tempOutcomingEntryManager.IsExistTempOutcomingEntryPendingCEO(Input.OutcomingEntryId);
                if (isExistTempPending)
                    throw new UserFriendlyException("Không thể [THỰC THI] Request chi khi có YCTĐ [PENDING]");

                await _outcomingEntryManager.CheckChangeOutcomingEntryStatus(Input.OutcomingEntryId);
            }

            var outcomingEntry = await WorkScope.GetAll<OutcomingEntry>()
                                .Include(x => x.OutcomingEntryType)
                                .Include(x => x.Currency)
                                .Where(s => s.Id == Input.OutcomingEntryId)
                                .FirstOrDefaultAsync();
           
            if(statusTransition.FromStatusCode.Trim() == Constants.WORKFLOW_STATUS_START)
            {
                await _outcomingEntryManager.UpdateValueToStartOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto 
                { 
                    OutcomingEntryId = outcomingEntry.Id,
                    Value = outcomingEntry.Value
                });
            }

            switch (statusTransition.ToStatusCode.Trim())
            {
                case Constants.WORKFLOW_STATUS_PENDINGCEO:
                    outcomingEntry.SentTime = DateTimeUtils.GetNow();
                    break;

                case Constants.WORKFLOW_STATUS_APPROVED:
                    outcomingEntry.ApprovedTime = DateTimeUtils.GetNow();
                    break;

                case Constants.WORKFLOW_STATUS_END:
                    outcomingEntry.ExecutedTime = DateTimeUtils.GetNow();
                    if (!outcomingEntry.ReportDate.HasValue)
                    {
                        outcomingEntry.ReportDate = GetLastBTransactionDate(outcomingEntry.Id);
                    }

                    // cập nhật mã phiếu chi PaymentCode cho request chi OutcomingEntry nếu null
                    if (String.IsNullOrEmpty(outcomingEntry.PaymentCode))
                    {
                        outcomingEntry.PaymentCode = createPaymentCode(outcomingEntry.Id);
                    }
                    break;

                case Constants.WORKFLOW_STATUS_REJECTED:
                    var tempId = await _tempOutcomingEntryManager.GetTempIdHaveStatusPendingByOutcomingId(outcomingEntry.Id);
                    if(tempId != 0)
                    {
                        await _tempOutcomingEntryManager.RejectTemp(tempId);
                    }
                    break;
            }

            outcomingEntry.WorkflowStatusId = statusTransition.ToTransitionStatusId;

            await CurrentUnitOfWork.SaveChangesAsync();

            _komuNotification.NotifyChangeStatus(Input.OutcomingEntryId, statusTransition.ToTransitionName.Trim());

            await _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
            {
                OutcomingEntryId = outcomingEntry.Id,
                WorkflowStatusId = outcomingEntry.WorkflowStatusId,
                Value = outcomingEntry.Value,
                CurrencyName = outcomingEntry.Currency.Name,
            });

            return Input;
        }

        private DateTime GetLastBTransactionDate(long outcomingEntryId)
        {
            return WorkScope.GetAll<OutcomingEntryBankTransaction>()
                .Include(s => s.BankTransaction)             
                .Where(s => s.OutcomingEntryId == outcomingEntryId)
                .Select(s => s.BankTransaction.TransactionDate)
                .Max();

        }

        #region new version v2
        [HttpGet]
        public async Task<IEnumerable<GetOutcomingEntryStatusHistoryDto>> GetOutcomingEntryStatusHistoryByOutcomingEntryId(long outcomingEntryId)
        {
            return await _outcomingEntryManager.GetOutcomingEntryStatusHistoryByOutcomingEntryId(outcomingEntryId);
        }
        [HttpGet]
        public async Task<bool> CheckExistTempOutCommingEntry(long outcomingEntryId)
        {
            return await _tempOutcomingEntryManager.IsExistTempOutComingEntry(outcomingEntryId);
        }
        [HttpGet]
        public async Task<bool> CheckTempOutCommingEntryHasDetail(long tempOutcomingEntryId)
        {
            return await _tempOutcomingEntryManager.CheckTempOutCommingEntryHasDetail(tempOutcomingEntryId);
        }
        [HttpGet]
        public async Task<GetTempOutcomingEntryDto> CreateTempOutCommingEntry(long outcomingEntryId)
        {
            return await _tempOutcomingEntryManager.CreateTempOutCommingEntry(outcomingEntryId);
        }
        [HttpGet]

        public async Task<RequestChangeOutcomingEntryInfoDto> GetRequestChangeOutcomingEntry(long tempOutcomingEntryId)
        {
            return await _tempOutcomingEntryManager.GetRequestChangeOutcomingEntry(tempOutcomingEntryId);
        }
        [HttpGet]
       // [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_ViewRequestChange)]
        public async Task<GetRequestChangeOutcomingEntryDetailDto> GetRequestChangeOutcomingEntryDetail(long tempOutcomingEntryId)
        {
            return await _tempOutcomingEntryManager.GetRequestChangeOutcomingEntryDetail(tempOutcomingEntryId);
        }
        [HttpPost]
       // [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_UpdateTemp)]
        public async Task<RequestChangeOutcomingEntryInfoDto> SaveTempOutCommingEntry(UpdateTempOutcomingEntryDto input)
        {
            return await _tempOutcomingEntryManager.SaveTempOutCommingEntry(input);
        }
        [HttpGet]
      //  [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_SendRequestChange)]
        public async Task SendTemp(long tempOutcomingEntryId)
        {
            var transitionName = await _tempOutcomingEntryManager.SendTemp(tempOutcomingEntryId);
            _komuNotification.NotifyRequestChangePending(tempOutcomingEntryId, transitionName);
        }
        [HttpGet]
       // [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_RejectMyRequestChange, PermissionNames.Finance_TempOutcomingEntry_RejectAllRequestChange)]
        public async Task RejectTemp(long tempOutcomingEntryId)
        {
            var transitionName = await _tempOutcomingEntryManager.RejectTemp(tempOutcomingEntryId);
            _komuNotification.NotifyRequestChangeReject(tempOutcomingEntryId, transitionName);
        }
        [HttpGet]
     //   [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_ApproveRequestChange)]
        public async Task ApproveTemp(long tempOutcomingEntryId)
        {
            var transitionName = await _tempOutcomingEntryManager.ApprovedTemp(tempOutcomingEntryId);
            _komuNotification.NotifyRequestChangeApprove(tempOutcomingEntryId, transitionName);
        }
        [HttpPost]
     //   [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_CreateTempDetail)]
        public async Task CreateTempOutcomingEntryDetail(CreateTempOutcomingEntryDetailDto input)
        {
            await _tempOutcomingEntryManager.CreateTempOutcomingEntryDetail(input);
        }
        [HttpPost]
       // [AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_EditTempDetail)]
        public async Task UpdateTempOutcomingEntryDetail(UpdateTempOutcomingEntryDetailDto input)
        {
            await _tempOutcomingEntryManager.UpdateTempOutcomingEntryDetail(input);
        }
        [HttpDelete]
        //[AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_DeleteTempDetail)]
        public async Task DeleteTempOutcomingEntryDetail(long tempOutcomingEntryDetailId)
        {
            await _tempOutcomingEntryManager.DeleteTempOutcomingEntryDetail(tempOutcomingEntryDetailId);
        }
        [HttpGet]
        //[AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_RevertTempDetail)]
        public async Task RevertTempOutcomingDetailByRootId(long rootOutcomingEntryDetailId, long rootOutcomingEntryId)
        {
            await _tempOutcomingEntryManager.RevertTempOutcomingDetailByRootId(rootOutcomingEntryDetailId, rootOutcomingEntryId);
        }
        [HttpGet]
        //[AbpAuthorize(PermissionNames.Finance_TempOutcomingEntry_ViewRequestChange)]
        public async Task<bool> CheckTempOutcomingEntryApproved(long tempId)
        {
            return await _tempOutcomingEntryManager.CheckTempOutcomingEntryApproved(tempId);
        }
        [HttpGet]
        public async Task<RequestChangeOutcomingEntryInfoDto> ViewHistoryChangeOutcomingEntry(long tempId)
        {
            var tempInfo = await WorkScope.GetAll<TempOutcomingEntry>()
                .Where(x => x.Id == tempId)
                .Select(x => new { WorkflowStatusCode = x.WorkflowStatus.Code, RootId = x.RootOutcomingEntryId })
                .FirstOrDefaultAsync();

            if (tempInfo.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
            {
                return await _tempOutcomingEntryManager.ViewHistoryChangeOutcomingEntry(tempId, tempInfo.RootId);
            }
            else
            {
                return await _tempOutcomingEntryManager.GetRequestChangeOutcomingEntry(tempId);
            }
        }
        [HttpGet]
        public async Task<GetRequestChangeOutcomingEntryDetailDto> ViewHistoryChangeOutcomingEntryDetail(long tempId)
        {
            var tempInfo = await WorkScope.GetAll<TempOutcomingEntry>()
                .Where(x => x.Id == tempId)
                .Select(x => new { WorkflowStatusCode = x.WorkflowStatus.Code, RootId = x.RootOutcomingEntryId })
                .FirstOrDefaultAsync();

            if (tempInfo.WorkflowStatusCode == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
            {
                return await _tempOutcomingEntryManager.ViewHistoryChangeOutcomingEntryDetail(tempId, tempInfo.RootId);
            }
            else
            {
                return await _tempOutcomingEntryManager.GetRequestChangeOutcomingEntryDetail(tempId);
            }
        }
        #endregion
        private string createPaymentCode(long outcomingEntryId)
        {
            var now = DateTimeUtils.GetNow();
            var getAllPaymentCode = WorkScope.GetAll<OutcomingEntry>()
                .Where(x => x.Id != outcomingEntryId && x.ExecutedTime.Value.Year == now.Year && !String.IsNullOrEmpty(x.PaymentCode))
                .Select(x => x.PaymentCode);

            var valueUpInYear = 0;
            foreach (var i in getAllPaymentCode)
            {
                var temp = i.Split("_");
                var valueMaxNow = Int32.Parse(temp[temp.Length - 1]);
                if (valueMaxNow > valueUpInYear)
                {
                    valueUpInYear = valueMaxNow;
                }
            }
            return "PC_" + now.Year + "_" + now.Month + "_" + (valueUpInYear + 1).ToString();
        }

        [HttpGet]
        public async Task<string> CFOTransfer()
        {
            var allRequestApproved = WorkScope.GetAll<OutcomingEntry>().Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_PENDINGCFO);

            var approveStatusId = await WorkScope.GetAll<WorkflowStatus>().Where(x => x.Code == Constants.WORKFLOW_STATUS_TRANSFERED).FirstOrDefaultAsync();

            foreach (var rq in allRequestApproved)
            {
                rq.WorkflowStatusId = approveStatusId.Id;
                await WorkScope.UpdateAsync(rq);
            }
            _komuNotification.NotifyWithMessage(
                new StringBuilder()
                .AppendLine($"Chào bạn, CFO vừa mới chuyển tiền cho **{allRequestApproved.Count()}** Request")
                .AppendLine($"{_options.Value.ClientRootAddress}app/expenditure-request?pageNumber=1&pageSize=20&searchText=&filterItems=[]&status=TRANSFERED")
                .ToString()
            );

            return $"Đã xuất tiền {allRequestApproved.Count()} Request Chi";
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_Create)]
        // Trong hàm tạo mới bank trans có gọi đến TransferToEnd()
        public async Task<string> TransferToEnd(BankTransactionDto input)
        {
            //Thêm mới giao dịch ngân hàng
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<BankTransaction>(input));

            //Sửa số dư các tài khoản ngân hàng liên quan
            var bankFromValue = await WorkScope.GetAsync<BankAccount>(input.FromBankAccountId);
            bankFromValue.Amount = bankFromValue.Amount - input.FromValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

            var bankToValue = await WorkScope.GetAsync<BankAccount>(input.ToBankAccountId);
            bankToValue.Amount = bankToValue.Amount + input.ToValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);

            var allRequestTransfered = await WorkScope.GetAll<OutcomingEntry>().Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_TRANSFERED).ToListAsync();
            var endStatusId = await WorkScope.GetAll<WorkflowStatus>().Where(x => x.Code == Constants.WORKFLOW_STATUS_END).FirstOrDefaultAsync();

            foreach (var rq in allRequestTransfered)
            {
                rq.ExecutedTime = DateTimeUtils.GetNow();
                rq.WorkflowStatusId = endStatusId.Id;
                // cập nhật mã phiếu chi PaymentCode cho request chi OutcomingEntry nếu null
                if (String.IsNullOrEmpty(rq.PaymentCode))
                {
                    rq.PaymentCode = createPaymentCode(rq.Id);
                }
                await WorkScope.UpdateAsync(rq);

                var od = new OutcomingEntryBankTransactionDto
                {
                    BankTransactionId = input.Id,
                    OutcomingEntryId = rq.Id
                };

                await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntryBankTransaction>(od));
            }

            return $"Đã thực thi {allRequestTransfered.Count()} Request Chi";
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO)]
        public async Task RemindCEO()
        {
            var countPendingCEO = await WorkScope.GetAll<OutcomingEntry>()
                .Where(x => x.WorkflowStatus.Code.ToLower() == Constants.WORKFLOW_STATUS_PENDINGCEO.ToLower())
                .CountAsync();

            _komuNotification.NotifyWithMessage(
                new StringBuilder()
                .AppendLine($"Chào bạn, hiện tại có **{countPendingCEO} request ** đang chờ bạn duyệt")
                .AppendLine($"{_options.Value.ClientRootAddress}app/expenditure-request?pageNumber=1&pageSize=20&searchText=&filterItems=[]&status=PENDINGCEO")
                .ToString()
            );
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO)]
        public async Task RemindCEORequestChange()
        {
            var countPendingCEO = await WorkScope.GetAll<TempOutcomingEntry>()
                .Where(x => x.WorkflowStatus.Code.ToLower() == Constants.WORKFLOW_STATUS_PENDINGCEO.ToLower())
                .CountAsync();

            _komuNotification.NotifyWithMessage(
                new StringBuilder()
                .AppendLine($"Chào bạn, hiện tại có **{countPendingCEO} yêu cầu thay đổi ** đang chờ bạn duyệt")
                .AppendLine($"{_options.Value.ClientRootAddress}app/expenditure-request?pageNumber=1&pageSize=20&searchText=&filterItems=[]&statusRequestChange=PENDINGCEO")
                .ToString()
            );
        }

        [HttpGet]
        public async Task RemindCFO()
        {
            var countApproved = await WorkScope.GetAll<OutcomingEntry>()
                .Where(x => x.WorkflowStatus.Code.ToLower() == Constants.WORKFLOW_STATUS_PENDINGCFO.ToLower())
                .CountAsync();

            _komuNotification.NotifyWithMessage(
                new StringBuilder()
                .AppendLine($"Chào bạn, hiện tại có **{countApproved}** request đang chờ bạn chuyển tiền.")
                .AppendLine($"{_options.Value.ClientRootAddress}app/expenditure-request?pageNumber=1&pageSize=20&searchText=&filterItems=[]&status=PENDINGCFO")
                .ToString()
            );
        }

        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile)]
        public async Task<GetFileDto> UploadFile([FromForm] UploadFilesForOutComingEntryDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            if (input.file == null || input.file.Length < 1)
                throw new UserFriendlyException(String.Format("Vui lòng chọn file."));

            String path = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "outcomingentries");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = DateTimeOffset.Now.ToUnixTimeMilliseconds() + "_" + input.file.FileName;
            var fileCreate = Path.Combine(path, filePath);
            using (var stream = System.IO.File.Create(fileCreate))
            {
                await input.file.CopyToAsync(stream);
            }

            // nếu đã confirm thì thôi
            if (outcomingEntry.IsAcceptFile != OutcomingEntryFileStatus.Confirmred)
            {
                outcomingEntry.IsAcceptFile = OutcomingEntryFileStatus.NotYetConfirmed;
                await WorkScope.UpdateAsync(outcomingEntry);
            }

            var fileId = await WorkScope.InsertAndGetIdAsync(new OutcomingEntryFile
            {
                OutcomingEntryId = outcomingEntry.Id,
                FilePath = filePath
            });
            var data = await System.IO.File.ReadAllBytesAsync(fileCreate);

            return new GetFileDto
            {
                Id = fileId,
                FileName = filePath,
                Data = data
            };
        }

        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile)]
        public async Task DeleteFile(long id)
        {
            var outcomingEntryFile = await WorkScope.GetAsync<OutcomingEntryFile>(id);

            File.Delete(Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "outcomingentries", outcomingEntryFile.FilePath));
            await WorkScope.DeleteAsync<OutcomingEntryFile>(id);

            var updateAcceptFile = WorkScope.GetAll<OutcomingEntryFile>()
            .Where(x => x.OutcomingEntryId == outcomingEntryFile.OutcomingEntryId && x.Id != id).ToList();
            if (updateAcceptFile.Count < 1)
            {
                var outcomingEntry = WorkScope.Get<OutcomingEntry>(outcomingEntryFile.OutcomingEntryId);
                if (outcomingEntry.IsAcceptFile != OutcomingEntryFileStatus.Confirmred)
                {
                    outcomingEntry.IsAcceptFile = OutcomingEntryFileStatus.NoFileYet;
                }
                await WorkScope.UpdateAsync(outcomingEntry);
            }
        }

        public async Task<IActionResult> GetFiles(long outcomingEntryId)
        {
            try
            {
                var files = await WorkScope.GetAll<OutcomingEntryFile>()
                    .Where(x => x.OutcomingEntryId == outcomingEntryId)
                    .Select(x => new GetFileDto
                    {
                        FileName = x.FilePath,
                        Id = x.Id,
                    }).ToListAsync();
                if (files.Count < 1)
                    return new OkObjectResult(new List<GetFileDto>());

                foreach (var i in files)
                {
                    if (string.IsNullOrEmpty(i.FileName) || i.FileName.Length < 1)
                        continue;
                    var path = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "outcomingentries", i.FileName);
                    if (File.Exists(path))
                    {
                        var data = await System.IO.File.ReadAllBytesAsync(path);
                        i.Data = data;
                    }
                }
                return new OkObjectResult(files);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile)]
        public async Task AcceptFile(long id, bool isConfirmFile)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(id);
            if (isConfirmFile)
            {
                outcomingEntry.IsAcceptFile = OutcomingEntryFileStatus.Confirmred;
            }
            else if (await WorkScope.GetAll<OutcomingEntryFile>().AnyAsync(x => x.OutcomingEntryId == id))
            {
                outcomingEntry.IsAcceptFile = OutcomingEntryFileStatus.NotYetConfirmed;
            }
            else
            {
                outcomingEntry.IsAcceptFile = OutcomingEntryFileStatus.NoFileYet;
            }
            await WorkScope.UpdateAsync(outcomingEntry);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_ExportExcel)]
        public async Task<byte[]> ExportExcel(GetAllPagingOutComingEntryDto input)
        {
            try
            {
                var currencyDefault = await GetCurrencyDefaultAsync();
                if (currencyDefault.IsNullOrDefault())
                {
                    throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
                }

                using (var wb = new XLWorkbook())
                {
                    var incomeWS = wb.Worksheets.Add("Outcome");
                    var qOutcomings = BuildOutcomingQuery(input).FiltersByOutcomingEntryGridParam(input);
                    var outcomingEntries = await qOutcomings.FilterByGridParam(input);
                    var totalOutcoming = await GetTotalCurrencies(qOutcomings, input);
                    int currentRow = 0;
                    foreach (var outcome in totalOutcoming)
                    {
                        currentRow++;
                        incomeWS.Cell(currentRow, 1).Value = outcome.CurrencyName + ": ";
                        incomeWS.Cell(currentRow, 2).Value = outcome.ValueFormat;
                    }
                    currentRow += 2;
                    int stt = 0;
                    incomeWS.Cell(currentRow, 1).Value = "STT";
                    incomeWS.Cell(currentRow, 2).Value = "Nội dung";
                    incomeWS.Cell(currentRow, 3).Value = "Người request";
                    incomeWS.Cell(currentRow, 4).Value = "Chi nhánh";
                    incomeWS.Cell(currentRow, 5).Value = "Cho công ty";
                    incomeWS.Cell(currentRow, 6).Value = "Giá trị";
                    incomeWS.Cell(currentRow, 7).Value = "Loại yêu cầu";
                    incomeWS.Cell(currentRow, 8).Value = "Tính vào chi phí";
                    incomeWS.Cell(currentRow, 9).Value = "Ngày tạo";
                    incomeWS.Cell(currentRow, 10).Value = "Ngày gửi";
                    incomeWS.Cell(currentRow, 11).Value = "Ngày chấp nhận";
                    incomeWS.Cell(currentRow, 12).Value = "Ngày thực thi";
                    incomeWS.Cell(currentRow, 13).Value = "Ngày báo cáo";
                    incomeWS.Cell(currentRow, 14).Value = "Payment code";
                    incomeWS.Cell(currentRow, 15).Value = "Has File";
                    incomeWS.Cell(currentRow, 16).Value = "Trạng thái";
                    incomeWS.Cell(currentRow, 17).Value = "Ủy nhiệm chi";
                    foreach (var outcome in outcomingEntries)
                    {
                        currentRow++;
                        stt++;
                        incomeWS.Cell(currentRow, 1).Value = stt;
                        incomeWS.Cell(currentRow, 2).Value = outcome.Name;
                        incomeWS.Cell(currentRow, 3).Value = outcome.Requester;
                        incomeWS.Cell(currentRow, 4).Value = outcome.BranchName;
                        incomeWS.Cell(currentRow, 5).Value = outcome.AccountName;
                        incomeWS.Cell(currentRow, 6).Value = outcome.Value.ToString("N0") + " " + outcome.CurrencyName;
                        incomeWS.Cell(currentRow, 7).Value = outcome.OutcomingEntryTypeName;
                        incomeWS.Cell(currentRow, 8).Value = !outcome.ExpenseType.HasValue ? "" : outcome.ExpenseType.Value == ExpenseType.REAL_EXPENSE ? "Có" : "Không";
                        incomeWS.Cell(currentRow, 9).Value = outcome.CreatedAt.Date.ToString("dd/MM/yyyy");
                        incomeWS.Cell(currentRow, 10).Value = outcome.SendTime.HasValue ? outcome.SendTime.Value.ToString("dd/MM/yyyy") : "";
                        incomeWS.Cell(currentRow, 11).Value = outcome.ApproveTime.HasValue ? outcome.ApproveTime.Value.ToString("dd/MM/yyyy") : "";
                        incomeWS.Cell(currentRow, 12).Value = outcome.ExecuteTime.HasValue ? outcome.ExecuteTime.Value.ToString("dd/MM/yyyy") : "";
                        incomeWS.Cell(currentRow, 13).Value = outcome.ReportDate.HasValue ? outcome.ReportDate.Value.Date.ToString("dd/MM/yyyy") : "";
                        incomeWS.Cell(currentRow, 14).Value = outcome.PaymentCode;
                        incomeWS.Cell(currentRow, 15).Value = outcome.IsAcceptFile == OutcomingEntryFileStatus.NoFileYet ? "No File Yet" : outcome.IsAcceptFile == OutcomingEntryFileStatus.Confirmred ? "Confirmred" : "Not Yet Confirmed";
                        incomeWS.Cell(currentRow, 16).Value = outcome.WorkflowStatusName;
                        incomeWS.Cell(currentRow, 17).Value = (outcome.Accreditation) ? "Yes" : "No";

                    }
                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        var content = stream.ToArray();
                        return content;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(String.Format("error: " + ex.Message));
            }

        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_ExportPdf)]
        public async Task<FileExportDto> ExportPdfById(long Id)
        {
            try
            {
                var bankAccount = WorkScope.GetAll<BankAccount>();
                var roledIds = await WorkScope.GetAll<UserRole>()
                              .Where(s => s.UserId == AbpSession.UserId.Value)
                              .Select(s => s.RoleId)
                              .ToListAsync();

                var requestInBankTransaction = WorkScope.GetAll<OutcomingEntryBankTransaction>()
                                           .GroupBy(oe => new { oe.OutcomingEntryId, oe.BankTransaction.TransactionDate, oe.BankTransaction.ToBankAccountId })
                                           .Select(oe => new
                                           {
                                               OutcomingEntryId = oe.Key.OutcomingEntryId,
                                               TransactionDate = oe.Key.TransactionDate,
                                               ToBankAccountId = oe.Key.ToBankAccountId
                                           });

                var query = WorkScope.GetAll<OutcomingEntry>().Where(s => s.Id == Id).Select(s => new GetInformationExport
                {
                    Id = s.Id,
                    Receiver = bankAccount.FirstOrDefault(r => r.Id == requestInBankTransaction.FirstOrDefault(x => x.OutcomingEntryId == s.Id).ToBankAccountId).Account.Name,
                    Name = s.Name,
                    CurrencyId = s.CurrencyId,
                    CurrencyName = s.Currency.Code,
                    Value = s.Value,
                    CreatedAt = s.CreationTime.Date,
                    SendTime = s.SentTime.Value.Date,
                    ApproveTime = s.ApprovedTime.Value.Date,
                    TransactionDate = requestInBankTransaction.Where(x => x.OutcomingEntryId == s.Id).Select(x => x.TransactionDate).FirstOrDefault(),
                    PaymentCode = s.PaymentCode,
                    IsAcceptFile = s.IsAcceptFile
                }).FirstOrDefault();
                ExportByData export = new ExportByData(this._hostingEnvironment);
                FileExportDto data = export.ExportPhieuChiPdfSelecect(query);
                return data;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new FileExportDto { Message = ex.Message, Html = "" };
            }
        }
        [HttpGet] 
        public async Task<List<ValueAndNameDto>> GetAllRequester()
        {

            var requesterId = WorkScope.GetAll<OutcomingEntry>()
                .Select(s => s.CreatorUserId)
                .Distinct()
                .ToList();
            return await WorkScope.GetAll<User>().Where(s => requesterId.Contains(s.Id)).Select(s => new ValueAndNameDto
            {
                Value = s.Id,
                Name = s.Name
            }).ToListAsync();
        }

        [HttpPost]
        public async Task<long> CloneOutcomingEntry(CloneOutcomeDto input)
        {
            return await _outcomingEntryManager.CloneOutcomingEntry(input);
        }

        [HttpPut]
        public async Task<UpdateRequestBranchDto> ForceUpdateBranch(UpdateRequestBranchDto input)
        {
            var outcomingEntry = WorkScope.GetAll<OutcomingEntry>()
                .Where(x=> x.Id == input.RequestId)
                .FirstOrDefault();
            if(outcomingEntry == default)
            {
                throw new UserFriendlyException($"Can not found outcoming entry with Id = {input.RequestId}");
            }
            outcomingEntry.BranchId = input.BranchId;
            await WorkScope.UpdateAsync(outcomingEntry);
            return input;


        }

        [HttpGet]
        public long GetDoneWorkFlowStatusTransaction()
        {
            var endWorkflowStatusId = WorkScope.GetAll<WorkflowStatus>()
                .Where(x => x.Code == FinanceManagementConsts.WORKFLOW_STATUS_END || x.Code == FinanceManagementConsts.WORKFLOW_STATUS_START)
                .ToDictionary(x => x.Code, x => x.Id);

            if (!endWorkflowStatusId.ContainsKey(FinanceManagementConsts.WORKFLOW_STATUS_END) || !endWorkflowStatusId.ContainsKey(FinanceManagementConsts.WORKFLOW_STATUS_START))
            {
                throw new UserFriendlyException($"WORKFLOW Error");
            }

            return WorkScope.GetAll<WorkflowStatusTransition>()
             .Where(x => x.ToStatusId == endWorkflowStatusId[FinanceManagementConsts.WORKFLOW_STATUS_END])
             .Where(x => x.FromStatusId != endWorkflowStatusId[FinanceManagementConsts.WORKFLOW_STATUS_START])
             .Select(x => x.Id)
             .FirstOrDefault();
        }

    }
}
