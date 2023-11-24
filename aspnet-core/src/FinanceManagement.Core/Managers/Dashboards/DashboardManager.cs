using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Managers.Dashboards.Dtos;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Helper;
using Abp.Linq.Extensions;
using FinanceManagement.Extension;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Ncc;
using FinanceManagement.Managers.Periods;
using Abp.Authorization;
using FinanceManagement.Services.HRM;
using Abp.MultiTenancy;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Uitls;
using FinanceManagement.Managers.IncomingEntries;
using DocumentFormat.OpenXml.Bibliography;

namespace FinanceManagement.Managers.Dashboards
{
    public class DashboardManager : DomainManager, IDashboardManager
    {
        private readonly IMySettingManager _mySettingManager;
        private readonly ICommonManager _commonManager;
        private readonly IPeriodManager _periodManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPeriodResolveContributor _periodResolveContributor;
        private readonly HRMService _hrmService;
        private readonly IIncomingEntryManager _incomingEntryManager;

        public DashboardManager(IWorkScope ws,
            IMySettingManager mySettingManager,
            ICommonManager commonManager,
            IPeriodResolveContributor periodResolveContributor,
            IPermissionChecker permissionChecker,
            IPeriodManager periodManager,
            HRMService hrmService,
            IIncomingEntryManager incomingEntryManager
            ) : base(ws)
        {
            _mySettingManager = mySettingManager;
            _commonManager = commonManager;
            _periodResolveContributor = periodResolveContributor;
            _periodManager = periodManager;
            _permissionChecker = permissionChecker;
            _hrmService = hrmService;
            _incomingEntryManager = incomingEntryManager;
        }
        #region đối soát
        public async Task<ResultNewComparativeStatisticDto> GetBankAccountStatistics(bool isIncludeBTransPending)
        {
            var periodBankAccounts = await _ws.GetAll<PeriodBankAccount>()
                .Where(x => x.IsActive)
                .Select(x => new NewComparativeStatisticDto
                {
                    BaseBalanceNumber = x.BaseBalance,
                    BankAccountName = x.BankAccount.HolderName,
                    BankNumber = x.BankAccount.BankNumber,
                    BankAccountId = x.BankAccountId,
                    CurrencyName = x.BankAccount.Currency.Name,
                    ExchangeRate = x.BankAccount.Currency.CurrencyConverts.OrderByDescending(x => x.DateAt).Select(x => x.Value).FirstOrDefault()
                })
                .ToListAsync();

            var perBankAccountIds = periodBankAccounts.Select(x => x.BankAccountId).ToList();

            var dicBTransactions = _ws.GetAll<BTransaction>()
                .WhereIf(!isIncludeBTransPending, x => x.Status == BTransactionStatus.DONE)
                .Where(x => perBankAccountIds.Contains(x.BankAccountId))
                .Select(x => new
                {
                    x.BankAccountId,
                    x.Money
                })
                .AsEnumerable()
                .GroupBy(x => x.BankAccountId)
                .Select(x => new
                {
                    x.Key,
                    Increase = x.Where(s => s.Money > 0).Sum(s => s.Money),
                    Reduce = x.Where(s => s.Money < 0).Sum(s => s.Money)
                })
                .ToDictionary(x => x.Key, x => new { x.Increase, x.Reduce });


            foreach (var periodBankAccount in periodBankAccounts)
            {
                if (dicBTransactions.ContainsKey(periodBankAccount.BankAccountId))
                {
                    periodBankAccount.IncreaseNumber = dicBTransactions[periodBankAccount.BankAccountId].Increase;
                    periodBankAccount.ReduceNumber = dicBTransactions[periodBankAccount.BankAccountId].Reduce;
                }
            }
            return new ResultNewComparativeStatisticDto
            {
                Statistics = periodBankAccounts.OrderByDescending(x => x.CurrentBalananceNumber).ToList(),
            };
        }


        public async Task<List<NewComparativeStatisticsIncomingEntryDto>> GetComparativeStatisticsIncomingEntry()
        {
            var incomings = await _ws.GetAll<IncomingEntry>()
                .Select(x => new { x.CurrencyId, x.Value, CurrencyName = x.Currency.Name })
                .GroupBy(x => x.CurrencyId)
                .Select(x => new { CurrencyId = x.Key, TotalIncoming = x.Sum(x => x.Value) })
                .ToListAsync();

            var bTransactions = await _ws.GetAll<BTransaction>()
                .Where(x => x.Money > 0)
                .Where(x => x.Status == BTransactionStatus.DONE)
                .Select(x => new { x.BankAccount.CurrencyId, CurrencyName = x.BankAccount.Currency.Name, Value = x.Money })
                .GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                .Select(x => new { x.Key, TotalBTransaction = x.Sum(s => s.Value) })
                .ToListAsync();

            var qResult = from b in bTransactions
                          join incom in incomings on b.Key.CurrencyId equals incom.CurrencyId
                          into gr
                          from bincom in gr.DefaultIfEmpty()
                          select new NewComparativeStatisticsIncomingEntryDto
                          {
                              CurrencyName = b.Key.CurrencyName,
                              TotalIncomingNumber = bincom == default ? 0 : bincom.TotalIncoming,
                              TotalBTransactionNubmer = b.TotalBTransaction
                          };

            return qResult.ToList();
        }
        public async Task<NewComparativeStatisticsOutcomingEntryDto> GetComparativeStatisticsOutcomingEntry()
        {
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
            {
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");
            }

            var result = new NewComparativeStatisticsOutcomingEntryDto();

            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            result.TotalOutcomingNumber = await _ws.GetAll<OutcomingEntry>()
                .Where(x => x.WorkflowStatusId == statusEndId)
                .SumAsync(x => x.Value);

            var qBankTransactions = from b in _ws.GetAll<BTransaction>().Where(x => x.Money < 0)
                                    join bt in _ws.GetAll<BankTransaction>() on b.Id equals bt.BTransactionId
                                    select bt.ToValue;
            result.TotalBankTransactionNumber = await qBankTransactions.SumAsync();

            result.TotalRefundNumber = (double)(await _ws.GetAll<RelationInOutEntry>()
                .Where(x => x.IsRefund)
                .SumAsync(x => x.IncomingEntry.Value * (x.IncomingEntry.ExchangeRate ?? 1)));

            result.CurrencyName = currencyDefault.Name;
            return result;
        }
        public async Task<ResultComparativeStatisticsOutBankTransactionDto> GetComparativeStatisticsOutBankTransaction()
        {
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            var qBankAccount = _ws.GetAll<BankAccount>().Select(x => new { x.Id, x.HolderName, CurrencyName = x.Currency.Name, CurrencyId = x.CurrencyId });
            //TODO::get list outcoming entry with bank transaction info
            var qResult = _ws.GetAll<OutcomingEntry>()
                .Where(x => x.WorkflowStatusId != statusEndId)
                .Where(x => x.OutcomingEntryBankTransactions.Any())
                .OrderByDescending(x => x.Id)
                .Select(x => new ComparativeStatisticsOutBankTransactionDto
                {
                    OutcomingEntryId = x.Id,
                    OutcomingEntryName = x.Name,
                    CurrencyId = x.Currency.Id,
                    CurrencyName = x.Currency.Name,
                    OutcomingEntryStatus = x.WorkflowStatus.Code,
                    OutcomingEntryStatusName = x.WorkflowStatus.Name,
                    OutcomingEntryTypeName = x.OutcomingEntryType.Name,
                    ValueMoney = x.Value,
                    BankTransactions = x.OutcomingEntryBankTransactions
                    .Select(s => new ComparativeStatisticsOutBankTransactionDetailDto()
                    {
                        BankTransactionId = s.BankTransactionId,
                        BankTransactionName = s.BankTransaction.Name,
                        FromBankAccountId = s.BankTransaction.FromBankAccountId,
                        FromValueNumber = s.BankTransaction.FromValue,
                        ToBankAccountId = s.BankTransaction.ToBankAccountId,
                        ToValueNumber = s.BankTransaction.ToValue,
                        TimeAt = s.BankTransaction.TransactionDate,
                        BTransactionId = s.BankTransaction.BTransactionId,
                        BTransactionName = s.BankTransaction.BTransaction.BankAccount.BankNumber,
                        CurrencyBTransaction = s.BankTransaction.BTransaction.BankAccount.Currency.Name,
                        ValueMoneyBTransaction = s.BankTransaction.BTransaction.Money,
                    }).ToList(),
                    IncomingInfos = x.RelationInOutEntries
                    .Where(x => !x.IsDeleted && x.IsRefund)
                    .Select(s => new ComparativeStatisticsRelationInOutDto
                    {
                        IncomingEntryId = s.IncomingEntryId,
                        Name = s.IncomingEntry.Name,
                        Value = (double)(s.IncomingEntry.Value * (s.IncomingEntry.ExchangeRate ?? 1)),
                        CurrencyName = s.IncomingEntry.Currency.Name,
                        CurrencyId = s.IncomingEntry.CurrencyId,
                        BankTransactionInfo = new ComparativeStatisticsOutBankTransactionDetailDto
                        {
                            BankTransactionId = s.IncomingEntry.BankTransactionId,
                            BankTransactionName = s.IncomingEntry.BankTransaction.Name,
                            FromBankAccountId = s.IncomingEntry.BankTransaction.FromBankAccountId,
                            FromValueNumber = s.IncomingEntry.BankTransaction.FromValue,
                            ToBankAccountId = s.IncomingEntry.BankTransaction.ToBankAccountId,
                            ToValueNumber = s.IncomingEntry.BankTransaction.ToValue,
                            BTransactionId = s.IncomingEntry.BTransactionId,
                            BTransactionName = s.IncomingEntry.BTransactions.BankAccount.BankNumber,
                            CurrencyBTransaction = s.IncomingEntry.BTransactions.BankAccount.Currency.Name,
                            ValueMoneyBTransaction = s.IncomingEntry.BTransactions.Money,
                            TimeAt = s.IncomingEntry.BTransactions.TimeAt
                        }
                    })
                });
            var result = await qResult.ToListAsync();
            var bankTransactions = result.SelectMany(x => x.BankTransactions.Union(x.IncomingInfos.Select(s => s.BankTransactionInfo)));

            var frombankAccountIds = bankTransactions
                .Select(x => x.FromBankAccountId)
                .ToList();

            var tobankAccountIds = bankTransactions
                .Select(x => x.ToBankAccountId)
                .ToList();

            var bankAccountIds = frombankAccountIds.Union(tobankAccountIds);
            var dicbankAccounts = await qBankAccount
                .Where(x => bankAccountIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => new { x.HolderName, x.CurrencyName, x.CurrencyId });

            result.ForEach(dto =>
            {
                foreach (var bankTrans in dto.BankTransactions)
                {
                    bankTrans.CurrencyId = dicbankAccounts[bankTrans.FromBankAccountId].CurrencyId;
                    bankTrans.FromBankAccountName = dicbankAccounts[bankTrans.FromBankAccountId].HolderName;
                    bankTrans.FromCurrencyName = dicbankAccounts[bankTrans.FromBankAccountId].CurrencyName;
                    bankTrans.ToBankAccountName = dicbankAccounts[bankTrans.ToBankAccountId].HolderName;
                    bankTrans.ToCurrencyName = dicbankAccounts[bankTrans.ToBankAccountId].CurrencyName;
                }
                foreach (var relationInOut in dto.IncomingInfos)
                {
                    relationInOut.BankTransactionInfo.FromBankAccountName = dicbankAccounts[relationInOut.BankTransactionInfo.FromBankAccountId].HolderName;
                    relationInOut.BankTransactionInfo.FromCurrencyName = dicbankAccounts[relationInOut.BankTransactionInfo.FromBankAccountId].CurrencyName;
                    relationInOut.BankTransactionInfo.ToBankAccountName = dicbankAccounts[relationInOut.BankTransactionInfo.ToBankAccountId].HolderName;
                    relationInOut.BankTransactionInfo.ToCurrencyName = dicbankAccounts[relationInOut.BankTransactionInfo.ToBankAccountId].CurrencyName;
                }
            });

            return new ResultComparativeStatisticsOutBankTransactionDto
            {
                Result = result
            };
        }
        public async Task<ResultComparativeStatisticByCurrencyDto> GetComparativeStatisticByCurrency()
        {
            // du dau ki
            var listDuDauKi = await _ws.GetAll<PeriodBankAccount>()
                .Where(x => x.IsActive)
                .Select(x => new
                {
                    x.BankAccount.CurrencyId,
                    x.BaseBalance
                })
                .GroupBy(x => x.CurrencyId)
                .Select(x => new
                {
                    x.Key,
                    Value = x.Sum(s => s.BaseBalance)
                })
                .ToDictionaryAsync(x => x.Key, x => x.Value);
            // thu sổ, thu sao kê
            var listThuSo = await _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    x.CurrencyId,
                    x.Value
                })
                .GroupBy(x => x.CurrencyId)
                .Select(x => new CurrencyIdAndValueStatisticByCurrency { CurrencyId = x.Key, Value = x.Sum(s => s.Value) })
                .ToListAsync();
            var dicThuSoCurrencyIdNotNull = listThuSo.GetDictionaryComparativeCurrencyIdNotNull();

            var listThuSaoKe = await QueryThuSaoKe()
                .GroupBy(x => x.CurrencyId)
                .Select(x => new CurrencyIdAndValueStatisticByCurrency { CurrencyId = x.Key, Value = x.Sum(s => s.Value) })
                .ToListAsync();
            var dicThuSaoKeCurrencyIdNotNull = listThuSaoKe.GetDictionaryComparativeCurrencyIdNotNull();

            //chi sổ, chi sao kê
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            var listChiSo = await _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    x.CurrencyId,
                    x.Value,
                    x.WorkflowStatusId
                })
                .Where(x => x.WorkflowStatusId == statusEndId)
                .GroupBy(x => x.CurrencyId)
                .Select(x => new CurrencyIdAndValueStatisticByCurrency { CurrencyId = x.Key, Value = x.Sum(s => s.Value) })
                .ToListAsync();
            var dicChiSoCurrencyIdNotNull = listChiSo.GetDictionaryComparativeCurrencyIdNotNull();

            var listChiSaoKe = await QueryChiSaoKe()
                .GroupBy(x => x.CurrencyId)
                .Select(x => new CurrencyIdAndValueStatisticByCurrency { CurrencyId = x.Key, Value = x.Sum(s => s.Value) })
                .ToListAsync();
            var dicChiSaoKeCurrencyIdNotNull = listChiSaoKe.GetDictionaryComparativeCurrencyIdNotNull();

            var listHoanTien = await _ws.GetAll<RelationInOutEntry>()
                .Where(x => !x.IncomingEntry.IsDeleted && !x.OutcomingEntry.IsDeleted && x.OutcomingEntry.WorkflowStatusId == statusEndId && x.IsRefund)
                .Select(x => new { x.IncomingEntry.CurrencyId, x.IncomingEntry.Value })
                .GroupBy(x => x.CurrencyId)
                .Select(x => new CurrencyIdAndValueStatisticByCurrency { CurrencyId = x.Key, Value = x.Sum(s => s.Value) })
                .ToListAsync();
            var dicHoanTienCurrencyIdNotNull = listHoanTien.GetDictionaryComparativeCurrencyIdNotNull();

            var result = _ws.GetAll<Currency>()
                .Select(x => new ComparativeStatisticByCurrencyDto
                {
                    TienTe = x.Name,
                    ChiSo = dicChiSoCurrencyIdNotNull.ContainsKey(x.Id) ? dicChiSoCurrencyIdNotNull[x.Id] : 0,
                    ChiSaoKe = dicChiSaoKeCurrencyIdNotNull.ContainsKey(x.Id) ? dicChiSaoKeCurrencyIdNotNull[x.Id] : 0,
                    HoanTien = dicHoanTienCurrencyIdNotNull.ContainsKey(x.Id) ? dicHoanTienCurrencyIdNotNull[x.Id] : 0,
                    ThuSo = dicThuSoCurrencyIdNotNull.ContainsKey(x.Id) ? dicThuSoCurrencyIdNotNull[x.Id] : 0,
                    ThuSaoKe = dicThuSaoKeCurrencyIdNotNull.ContainsKey(x.Id) ? dicThuSaoKeCurrencyIdNotNull[x.Id] : 0,
                    DuDauKi = listDuDauKi.ContainsKey(x.Id) ? listDuDauKi[x.Id] : 0,
                    ExchangeRate = x.CurrencyConverts.OrderByDescending(x => x.DateAt).Select(x => x.Value).FirstOrDefault(),
                })
                .ToList();

            //them truong hop co tien te la NULL
            result.Add(new ComparativeStatisticByCurrencyDto
            {
                TienTe = "NULL",
                ChiSo = listChiSo.GetComparativeCurrencyIdNull(),
                ChiSaoKe = listChiSaoKe.GetComparativeCurrencyIdNull(),
                HoanTien = listHoanTien.GetComparativeCurrencyIdNull(),
                ThuSo = listThuSo.GetComparativeCurrencyIdNull(),
                ThuSaoKe = listThuSaoKe.GetComparativeCurrencyIdNull(),
            });

            return new ResultComparativeStatisticByCurrencyDto
            {
                Statistics = result.Where(x => x.IsShow).ToList()
            };
        }
        private IQueryable<CurrencyIdAndValueStatisticByCurrency> QueryThuSaoKe()
        {
            return from bt in _ws.GetAll<BankTransaction>()
                   join tba in _ws.GetAll<BankAccount>()
                                   .Select(x => new { x.Id, x.Account.Type, x.CurrencyId })
                                   .Where(x => x.Type == AccountTypeEnum.COMPANY)
                           on bt.ToBankAccountId equals tba.Id
                   select new CurrencyIdAndValueStatisticByCurrency
                   {
                       Value = bt.FromValue,
                       CurrencyId = tba.CurrencyId
                   };
        }
        private IQueryable<CurrencyIdAndValueStatisticByCurrency> QueryChiSaoKe()
        {
            return from bt in _ws.GetAll<BankTransaction>()
                   join fba in _ws.GetAll<BankAccount>()
                                   .Select(x => new { x.Id, x.Account.Type, x.CurrencyId })
                                   .Where(x => x.Type == AccountTypeEnum.COMPANY)
                           on bt.FromBankAccountId equals fba.Id
                   select new CurrencyIdAndValueStatisticByCurrency
                   {
                       Value = bt.FromValue,
                       CurrencyId = fba.CurrencyId
                   };
        }
        #endregion

        #region thống kê HomePage
        public async Task<List<OverviewOutcomingEntryStatisticDto>> OverviewOutcomingEntryStatistics()
        {
            Dictionary<string, int> dicWorkflowStatusCodes = new Dictionary<string, int>()
            {
                { FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO, 0},
                { FinanceManagementConsts.WORKFLOW_STATUS_START, 1 },
                { FinanceManagementConsts.WORKFLOW_STATUS_REJECTED, 2},
                { FinanceManagementConsts.WORKFLOW_STATUS_APPROVED, 3},
                { FinanceManagementConsts.WORKFLOW_STATUS_END, 4 }
            };
            var workflowStatuses = _ws.GetAll<WorkflowStatus>()
                .Where(x => dicWorkflowStatusCodes.Keys.Contains(x.Code.Trim()))
                .Select(x => new OverviewOutcomingEntryStatisticDto
                {
                    StatusId = x.Id,
                    StatusCode = x.Code,
                    StatusName = x.Name,
                    Index = dicWorkflowStatusCodes[x.Code.Trim()]
                })
                .ToList();

            var statusIds = workflowStatuses.Select(x => x.StatusId).ToList();

            var dicOutcomingEntryStatistic = await _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    WorkflowStatusId = x.WorkflowStatusId,
                    OutcomingEntryId = x.Id
                })
                .Where(x => statusIds.Contains(x.WorkflowStatusId))
                .GroupBy(x => x.WorkflowStatusId)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            workflowStatuses.ForEach(dto =>
            {
                dto.Count = dicOutcomingEntryStatistic.ContainsKey(dto.StatusId) ? dicOutcomingEntryStatistic[dto.StatusId] : 0;
            });

            var countTempPending = await _ws.GetAll<TempOutcomingEntry>()
                .Select(x => new
                {
                    x.Id,
                    StatusCode = x.WorkflowStatus.Code.Trim()
                })
                .Where(x => x.StatusCode == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                .CountAsync();
            var pendingStatusName = workflowStatuses
                .Where(x => x.StatusCode == FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO)
                .Select(x => x.StatusName)
                .FirstOrDefault();

            workflowStatuses.Add(new OverviewOutcomingEntryStatisticDto
            {
                StatusCode = FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO,
                Count = countTempPending,
                Index = dicWorkflowStatusCodes[FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO],
                StatusName = $"[YCTĐ] Chờ CEO duyệt"
            });

            return workflowStatuses.OrderBy(x => x.Index).ThenBy(x => x.StatusName.Length).ToList();
        }
        public async Task<OverviewInvoiceStatisticDto> OverviewInvoiceStatistics()
        {
            var currencies = await _ws.GetAll<Currency>()
                .Select(x => new InvoiceCurrencyStatisticDto
                {
                    CurrencyId = x.Id,
                    CurrencyCode = x.Code
                })
                .ToListAsync();

            var dicInvoiceDiffDone = _ws.GetAll<Invoice>()
                .Select(x => new InvoiceCalculationDebt
                {
                    CurrencyId = x.CurrencyId,
                    CollectionDebt = x.CollectionDebt,
                    Paid = x.IncomingEntries.Sum(s => s.Value * s.ExchangeRate) ?? 0,
                    Status = x.Status
                })
                .Where(x => x.Status == NInvoiceStatus.CHUA_TRA || x.Status == NInvoiceStatus.TRA_1_PHAN)
                .AsEnumerable()
                .GroupBy(x => x.CurrencyId)
                .Select(x => new
                {
                    x.Key,
                    Sum = x.Sum(s => s.Debt)
                })
                .ToDictionary(x => x.Key, x => x.Sum);

            var incomingEntryTypeWhenClientPaidBefore = await _mySettingManager.GetBalanceClientAsync();
            var dicIncomingEntryPrepaidClient = await _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    CurrencyId = x.BTransactions.BankAccount.CurrencyId,
                    Balance = -(x.Value * x.ExchangeRate),
                    x.IncomingEntryTypeId
                })
                .Where(x => x.IncomingEntryTypeId == incomingEntryTypeWhenClientPaidBefore.Id)
                .GroupBy(x => x.CurrencyId)
                .Select(x => new
                {
                    x.Key,
                    Sum = x.Sum(s => s.Balance)
                })
                .ToDictionaryAsync(x => x.Key, x => x.Sum);

            currencies.ForEach(dto =>
            {
                var debt = dicInvoiceDiffDone.ContainsKey(dto.CurrencyId) ? dicInvoiceDiffDone[dto.CurrencyId] : 0;
                var balance = dicIncomingEntryPrepaidClient.ContainsKey(dto.CurrencyId) ? dicIncomingEntryPrepaidClient[dto.CurrencyId] ?? 0 : 0;

                dto.Value = debt + balance;
            });

            return new OverviewInvoiceStatisticDto
            {
                InvoiceCurrencies = currencies.Where(x => x.Value != 0).ToList(),
                QuantityInvoiceDebt = await _ws.GetAll<Invoice>()
                                              .Select(x => new { x.Id, x.Status })
                                              .Where(x => x.Status == NInvoiceStatus.CHUA_TRA || x.Status == NInvoiceStatus.TRA_1_PHAN)
                                              .CountAsync()
            };
        }
        public async Task<List<OverviewBTransactionStatisticDto>> OverviewBTransactionStatistics()
        {
            var dicBTransactions = await _ws.GetAll<BTransaction>()
                .Select(x => new
                {
                    x.Status
                })
                .GroupBy(x => x.Status)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var result = Enum.GetValues(typeof(BTransactionStatus))
                        .Cast<BTransactionStatus>()
                        .Select(s => new OverviewBTransactionStatisticDto
                        {
                            Status = s
                        })
                        .ToList();
            result.ForEach(dto =>
            {
                dto.Quantity = dicBTransactions.ContainsKey(dto.Status) ? dicBTransactions[dto.Status] : 0;
            });
            return result;
        }
        public Dictionary<CurrencyYearMonthDto, double> GetDictionaryCurrencyConvertByYearMonth(DateTime startDate, DateTime endDate)
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetLastDayOfMonth(endDate);
            return _ws.GetAll<CurrencyConvert>()
                .Select(x => new
                {
                    x.CurrencyId,
                    x.Value,
                    x.DateAt
                })
                .Where(x => x.DateAt >= startDate && x.DateAt <= endDate)
                .Select(x => new
                {
                    x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.DateAt.Month,
                        Year = x.DateAt.Year,
                    }
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
        public void CheckDictionaryCurrencyConvertByYearMonth(Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, DateTime startDate, DateTime endDate)
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetLastDayOfMonth(endDate);
            var dicMonthYear = dicCurrencyConvert.Keys
                .Select(x => new { x.Month, x.Year })
                .Distinct()
                .ToDictionary(x => x);
            var date = startDate;
            while (date <= endDate)
            {
                if (!dicMonthYear.ContainsKey(new { date.Month, date.Year }))
                {
                    throw new UserFriendlyException($"Bạn cần thêm tỉ giá chuyển đổi tiền cho tháng {date.Month}/{date.Year}");
                }
                date = date.AddMonths(1);
            }
        }
        public List<double> GetLineChartIncomingEntry(
            DateTime startDate,
            DateTime endDate,
            HashSet<long> incomingEntryTypeIds,
            IEnumerable<string> labels,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert
        )
        {
            var incoms = _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    x.IncomingEntryTypeId,
                    x.CurrencyId,
                    TimeAt = x.BankTransaction.TransactionDate.Date,
                    x.Value,

                })
                .WhereIf(incomingEntryTypeIds.Any(), x => incomingEntryTypeIds.Contains(x.IncomingEntryTypeId))
                .Where(x => x.TimeAt >= startDate.Date && x.TimeAt <= endDate.Date)
                .Select(x => new KeyValuePairChart
                {
                    Value = x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.TimeAt.Month,
                        Year = x.TimeAt.Year
                    }
                })
                .AsEnumerable()
                .GetBaseDataCharts(dicCurrencyConvert);

            var result = from l in labels
                         join i in incoms on l equals i.Label
                         into g
                         from li in g.DefaultIfEmpty()
                         select li == default ? 0 : li.Value;

            return result.ToList();
        }

        public double GetValueCircleChartIncomingEntry(
            DateTime startDate,
            DateTime endDate,
            HashSet<long> incomingEntryTypeIds,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert,
            List<long> listClientIds
        )
        {
            var qIncoms = _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    x.IncomingEntryTypeId,
                    x.CurrencyId,
                    TimeAt = x.BankTransaction.TransactionDate.Date,
                    x.Value,

                });

            qIncoms = qIncoms.Where(x => x.TimeAt >= startDate.Date && x.TimeAt <= endDate.Date);

            qIncoms = qIncoms.WhereIf(incomingEntryTypeIds.Any(), x => incomingEntryTypeIds.Contains(x.IncomingEntryTypeId));

            //var qIncom = qIncoms.WhereIf(listClientIds.Any(), x => x.ClientAccountId != 0 && listClientIds.Contains(x.ClientAccountId));

            var incoms = qIncoms.Select(x => new KeyValuePairChart
            {
                Value = x.Value,
                Key = new CurrencyYearMonthDto
                {
                    CurrencyId = x.CurrencyId,
                    Month = x.TimeAt.Month,
                    Year = x.TimeAt.Year
                }
            })
               .AsEnumerable()
               .GetBaseDataCharts(dicCurrencyConvert)
               .Select(s => s.Value);
            return incoms.Sum(); ;
        }
        public List<double> GetLineChartOutcomingEntry(
            DateTime startDate,
            DateTime endDate,
            HashSet<long> outcomingEntryTypeIds,
            IEnumerable<string> labels,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert,
            long statusEndId
        )
        {
            var outcoms = _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    x.Value,
                    x.WorkflowStatusId,
                    x.OutcomingEntryTypeId,
                    x.ReportDate,
                    x.CurrencyId
                })
                .Where(x => x.WorkflowStatusId == statusEndId)
                .WhereIf(outcomingEntryTypeIds.Any(), x => outcomingEntryTypeIds.Contains(x.OutcomingEntryTypeId))
                .Where(x => x.ReportDate.HasValue && (x.ReportDate.Value >= startDate && x.ReportDate.Value <= endDate))
                .Select(x => new KeyValuePairChart
                {
                    Value = x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.ReportDate.Value.Month,
                        Year = x.ReportDate.Value.Year
                    }
                })
                .AsEnumerable()
                .GetBaseDataCharts(dicCurrencyConvert);

            var result = from l in labels
                         join o in outcoms on l equals o.Label
                         into g
                         from lo in g.DefaultIfEmpty()
                         select lo == default ? 0 : lo.Value;

            return result.ToList();
        }

        public double GetValueCircleChartOutcomingEntry(
            DateTime startDate,
            DateTime endDate,
            HashSet<long> outcomingEntryTypeIds,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert,
            long statusEndId,
            long? branchId
        )
        {
            var outcoms = _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    x.Value,
                    x.WorkflowStatusId,
                    x.OutcomingEntryTypeId,
                    x.BranchId,
                    x.ReportDate,
                    x.CurrencyId
                })
                .Where(x => x.WorkflowStatusId == statusEndId)
                .WhereIf(branchId.HasValue, x => x.BranchId == branchId)
                .WhereIf(outcomingEntryTypeIds.Any(), x => outcomingEntryTypeIds.Contains(x.OutcomingEntryTypeId))
                .Where(x => x.ReportDate.HasValue && (x.ReportDate.Value >= startDate && x.ReportDate.Value <= endDate))
                .Select(x => new KeyValuePairChart
                {
                    Value = x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.ReportDate.Value.Month,
                        Year = x.ReportDate.Value.Year
                    }
                })
                .AsEnumerable()
                .GetBaseDataCharts(dicCurrencyConvert)
                .Select(x => x.Value)
                .Sum();

            return outcoms;
        }
        public NewChartDto GetBarChartIncoming(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<string> labels,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert
        )
        {
            var result = new NewChartDto();
            result.Name = "Tổng doanh thu";
            result.ItemStyle = null;
            result.Type = "bar";

            var incoms = _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    x.IncomingEntryType.RevenueCounted,
                    x.CurrencyId,
                    TimeAt = x.BankTransaction.TransactionDate.Date,
                    x.Value,
                })
                .Where(x => x.RevenueCounted)
                .Where(x => x.TimeAt >= startDate.Date && x.TimeAt <= endDate.Date)
                .Select(x => new KeyValuePairChart
                {
                    Value = x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.TimeAt.Month,
                        Year = x.TimeAt.Year
                    }
                })
                .AsEnumerable()
                .GetBaseDataCharts(dicCurrencyConvert);

            var leftJoinIncom = from l in labels
                                join i in incoms on l equals i.Label
                                into g
                                from li in g.DefaultIfEmpty()
                                select li == default ? 0 : li.Value;
            result.Data = leftJoinIncom.ToList();

            return result;
        }
        public NewChartDto GetBarChartOutcomingEntry(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<string> labels,
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert,
            long statusEndId
        )
        {
            var result = new NewChartDto();
            result.Name = "Tổng chi phí";
            result.ItemStyle = new ChartStyleDto
            {
                Color = "#eb6470"
            };
            result.Type = "bar";
            var outcoms = _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    x.Value,
                    x.WorkflowStatusId,
                    x.CurrencyId,
                    x.OutcomingEntryType.ExpenseType,
                    x.ReportDate
                })
                .Where(x => x.WorkflowStatusId == statusEndId)
                .Where(x => x.ExpenseType == ExpenseType.REAL_EXPENSE)
                .Where(x => x.ReportDate.HasValue && (x.ReportDate.Value >= startDate && x.ReportDate.Value <= endDate))
                .Select(x => new KeyValuePairChart
                {
                    Value = x.Value,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.ReportDate.Value.Month,
                        Year = x.ReportDate.Value.Year
                    }
                })
                .AsEnumerable()
                .GetBaseDataCharts(dicCurrencyConvert);

            var resOutcom = from l in labels
                            join o in outcoms on l equals o.Label
                            into g
                            from lo in g.DefaultIfEmpty()
                            select lo == default ? 0 : lo.Value;

            result.Data = resOutcom.ToList();

            return result;
        }
        public async Task<List<PieChartDto>> GetPieChartIncoming(
            long? rootId,
            DateTime startDate,
            DateTime endDate
        )
        {
            var tree = _commonManager.GetTreeIncomingEntries();
            var dicCurrency = GetDictionaryCurrencyConvertByYearMonth(startDate, endDate);
            CheckDictionaryCurrencyConvertByYearMonth(dicCurrency, startDate, endDate);

            var qincome = _ws.GetAll<IncomingEntry>()
                .Select(x => new
                {
                    x.BTransactions.BankAccount.CurrencyId,
                    TimeAt = x.BTransactions.TimeAt.Date,
                    x.Value,
                    x.IncomingEntryTypeId
                })
                .Where(x => x.TimeAt >= startDate && x.TimeAt <= endDate)
                .AsEnumerable()
                .Select(x => new
                {
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.TimeAt.Month,
                        Year = x.TimeAt.Year
                    },
                    x.IncomingEntryTypeId,
                    x.Value
                });

            var result = new List<PieChartDto>();
            var childNodes = await _ws.GetAll<IncomingEntryType>()
                    .Where(x => x.ParentId == rootId)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

            foreach (var childNode in childNodes)
            {
                var pieChart = new PieChartDto();
                pieChart.Name = childNode.Name;

                var incomingEntryTypeIds = _commonManager.GetNodeIdsFromTree(childNode.Id, tree);

                pieChart.Value = qincome
                    .Where(x => incomingEntryTypeIds.Contains(x.IncomingEntryTypeId))
                    .Select(x => new
                    {
                        ExchangeRate = dicCurrency.ContainsKey(x.Key) ? dicCurrency[x.Key] : 1,
                        x.Value
                    })
                    .Sum(x => x.ExchangeRate * x.Value);

                result.Add(pieChart);
            }

            return result.Where(x => x.Value != 0).ToList();
        }
        public async Task<List<PieChartDto>> GetPieChartOutcoming(
            long? rootId,
            DateTime startDate,
            DateTime endDate
        )
        {
            var tree = _commonManager.GetTreeOutcomingEntries();
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);
            var dicCurrency = GetDictionaryCurrencyConvertByYearMonth(startDate, endDate);
            CheckDictionaryCurrencyConvertByYearMonth(dicCurrency, startDate, endDate);

            var qoutcoming = _ws.GetAll<OutcomingEntry>()
                .Select(x => new
                {
                    x.Value,
                    x.WorkflowStatusId,
                    x.ReportDate,
                    x.OutcomingEntryTypeId,
                    x.CurrencyId
                })
                .Where(x => x.WorkflowStatusId == statusEndId)
                .Where(x => x.ReportDate.HasValue && (x.ReportDate.Value >= startDate && x.ReportDate.Value <= endDate))
                .Select(x => new
                {
                    x.OutcomingEntryTypeId,
                    Key = new CurrencyYearMonthDto
                    {
                        CurrencyId = x.CurrencyId,
                        Month = x.ReportDate.Value.Month,
                        Year = x.ReportDate.Value.Year
                    },
                    x.Value
                })
                .AsEnumerable();

            var result = new List<PieChartDto>();
            var childNodes = await _ws.GetAll<OutcomingEntryType>()
                    .Where(x => x.ParentId == rootId)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

            foreach (var childNode in childNodes)
            {
                var pieChart = new PieChartDto();
                pieChart.Name = childNode.Name;

                var outcomingEntryTypeIds = _commonManager.GetNodeIdsFromTree(childNode.Id, tree);

                pieChart.Value = qoutcoming
                    .Where(x => outcomingEntryTypeIds.Contains(x.OutcomingEntryTypeId))
                    .Select(x => new
                    {
                        ExchangeRate = dicCurrency.ContainsKey(x.Key) ? dicCurrency[x.Key] : 1,
                        x.Value
                    })
                    .Sum(x => x.Value * x.ExchangeRate);

                result.Add(pieChart);
            }

            return result.Where(x => x.Value != 0).ToList();
        }
        public async Task<List<BaoCaoChungDto>> GetDataBaoCaoChung(DateTime startDate, DateTime endDate, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, long branchId, ExpenseType? isExpense)
        {

            var qtongChi = await GetAllRequestChiForBaoCao(startDate, endDate, dicCurrencyConvert, branchId, isExpense);             
            var tongChiTheoChiNhanh = qtongChi
                .GroupBy(x => new { x.BranchId, x.BranchName })
                .Select(x => new BaoCaoChungDto
                {
                    BranchId = x.Key.BranchId,
                    BranchName = x.Key.BranchName,
                    TongChi = x.Sum(s => s.TotalVND)
                })
                .OrderByDescending(x => x.BranchId)
                .ToList();

            var tongChiThucTheoChiNhanh = qtongChi
                .Where(x => x.ExpenseType == ExpenseType.REAL_EXPENSE)
                .GroupBy(x => x.BranchId)
                .Select(x => new GetThongTinChiTheoChiNhanh
                {
                    BranchId = x.Key,
                    TotalVND = x.Sum(s => s.TotalVND)
                })
                .ToDictionary(x => x.BranchId, x => x.TotalVND);
            //lay theo thu
            var qtongThu = await GetDataBaoCaoThu(startDate, endDate, dicCurrencyConvert, null);

            foreach (var dto in tongChiTheoChiNhanh)
            {
                dto.TongChiThuc = tongChiThucTheoChiNhanh.ContainsKey(dto.BranchId) ? tongChiThucTheoChiNhanh[dto.BranchId] : 0;
            }

            var tong = new BaoCaoChungDto
            {
                BranchName = "Tổng cộng",
                TongThu = qtongThu.Sum(x => x.TotalVND),
                TongThuThuc = qtongThu.Where(x => x.IsDoanhThu).Sum(x => x.TotalVND),
                TongChi = tongChiTheoChiNhanh.Sum(x => x.TongChi),
                TongChiThuc = tongChiTheoChiNhanh.Sum(x => x.TongChiThuc)
            };
            tongChiTheoChiNhanh.Add(tong);

            return tongChiTheoChiNhanh;
        }
        public async Task<IEnumerable<GetThongTinRequestChi>> GetAllRequestChiForBaoCao(
            DateTime startDate, DateTime endDate, 
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, 
            long? branchId, ExpenseType? isExpense, 
            HashSet<long> outcomingEntryTypeIds = null)
        {
            var statusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END.Trim());
            //lay theo chi
            var qrequestChi = IQOutcomingEntryForDashboard(statusEndId)
                .Where(x => x.ReportDate.HasValue ? (startDate.Date <= x.ReportDate.Value.Date && x.ReportDate.Value.Date <= endDate.Date) : false);
            
            if (isExpense.HasValue)
            {
               
                qrequestChi = qrequestChi.Where(x => x.ExpenseType == isExpense.Value);
                
            }

            var list = qrequestChi.ToList();
            foreach (var item in list)
            {
                foreach (var detail in item.Details)
                {
                    detail.Id = item.Id;
                    detail.ExpenseType = item.ExpenseType;
                    detail.Name = item.Name;
                    detail.ReportDate = item.ReportDate;
                    detail.ExchangeRate = item.ExchangeRate;
                    detail.CurrencyId = item.CurrencyId;
                    detail.CurrencyName = item.CurrencyName;
                    detail.OutcomingEntryTypeId = item.OutcomingEntryTypeId;
                    detail.OutcomingEntryType = item.OutcomingEntryType;
                }
            };


            var requestChiHasDetail = list
                .Where(x => x.Details.Any())
                .SelectMany(x => x.Details)
                .ToList();

            var requestChiHasNotDetail = list
                .Where(x => !x.Details.Any())
                .Select(x => new GetThongTinRequestChi
                {
                    BranchId = x.BranchId,
                    Id = x.Id,
                    Name = x.Name,
                    Total = x.Total,
                    ExpenseType = x.ExpenseType,
                    ReportDate = x.ReportDate,
                    ExchangeRate = x.ExchangeRate,
                    BranchName = x.BranchName,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.CurrencyName,
                    OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                    OutcomingEntryType = x.OutcomingEntryType
                })
                .ToList();

            var requestChiUnio = requestChiHasDetail.Union(requestChiHasNotDetail).OrderBy(x => x.BranchName).ThenBy(x => x.ReportDate).AsQueryable();

            if (branchId.HasValue && branchId.Value > 0)
            {
                requestChiUnio = requestChiUnio.Where(x => x.BranchId == branchId);
            }
            if (outcomingEntryTypeIds != null && outcomingEntryTypeIds.Any())
            {
                requestChiUnio = requestChiUnio.Where(x => outcomingEntryTypeIds.Contains(x.OutcomingEntryTypeId));
            }
            return requestChiUnio.Select(x => new GetThongTinRequestChi
            {
                BranchId = x.BranchId,
                BranchName = x.BranchName,
                Id = x.Id,
                Name = x.Name,
                Total = x.Total,
                ExpenseType = x.ExpenseType,
                ReportDate = x.ReportDate,
                ExchangeRate = GetExchangeRateByDicCurrencyConvert(dicCurrencyConvert, x.CurrencyId, x.ReportDate),
                CurrencyId = x.CurrencyId,
                CurrencyName = x.CurrencyName,
                OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                OutcomingEntryType = x.OutcomingEntryType,
                Details = x.Details
            }).OrderBy(x => x.ReportDate);
        }
        private IQueryable<GetThongTinRequestChi> IQOutcomingEntryForDashboard(long? statusEndId = null)
        {
            return _ws.GetAll<OutcomingEntry>()
                .WhereIf(statusEndId.HasValue, x => x.WorkflowStatusId == statusEndId)
                .Select(x => new GetThongTinRequestChi
                {
                    BranchId = x.BranchId,
                    BranchName = x.Branch.Name,
                    Id = x.Id,
                    Name = x.Name,
                    Total = x.Value,
                    ExpenseType = x.OutcomingEntryType.ExpenseType ?? ExpenseType.NON_EXPENSE,
                    ReportDate = x.ReportDate,
                    ExchangeRate = x.Currency.CurrencyConverts.OrderByDescending(x => x.DateAt).Select(x => x.Value).FirstOrDefault(),
                    CurrencyId = x.CurrencyId.Value, 
                    CurrencyName = x.Currency.Name, 
                    OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                    OutcomingEntryType = x.OutcomingEntryType.Name,
                    Details = x.OutcomingEntryDetails.Select(s => new GetThongTinRequestChi
                    {
                        BranchId = s.BranchId,
                        BranchName = s.Branch.Name,
                        DetailName = s.Name,
                        Total = s.Total,
                    })
                });
        }
        public async Task<List<BaoCaoThuDto>> GetDataBaoCaoThu(
            DateTime startDate, DateTime endDate, 
            Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, 
            bool? isDoanhThu, List<long> listClientIds = null, 
            HashSet<long> incomingEntryTypeIds = null)
        {
            var query = IQGetIncomingEntryForBaoCao()
            .Where(x => x.TransactionDate.HasValue ? (startDate.Date <= x.TransactionDate.Value.Date && x.TransactionDate.Value.Date <= endDate.Date) : false);

            if (isDoanhThu.HasValue)
            {
                query = query.Where(x => x.IsDoanhThu == isDoanhThu.Value);
            }
            if (listClientIds != null && listClientIds.Any())
            {
                query = query.Where(x => listClientIds.Contains(x.ClientId.Value));
            }

            if (incomingEntryTypeIds != null && incomingEntryTypeIds.Any()) {
                query = query.Where(x => incomingEntryTypeIds.Contains(x.IncomingEntryTypeId));
            }

            return await query
                .Select(x => new BaoCaoThuDto
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ClientName = x.ClientName,
                    Name = x.Name,
                    IncomingEntryTypeId = x.IncomingEntryTypeId,
                    IncomingEntryType = x.IncomingEntryType,
                    IsDoanhThu = x.IsDoanhThu,
                    ExchangeRate = GetExchangeRateByDicCurrencyConvert(dicCurrencyConvert, x.CurrencyId, x.TransactionDate),
                    Value = x.Value,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.CurrencyName,
                    Month = x.Month,
                    Year = x.Year,
                    TransactionDate = x.TransactionDate,
                    BankTransactionId = x.BankTransactionId
                })
                .OrderBy(x => x.TransactionDate)
                .ToListAsync();
        }
        private IQueryable<BaoCaoThuDto> IQGetIncomingEntryForBaoCao()
        {
            return _ws.GetAll<IncomingEntry>()
                .Select(x => new BaoCaoThuDto
                {
                    Id = x.Id,
                    ClientId = x.BTransactions.FromAccountId,
                    ClientName = x.BTransactions.FromAccount.Name,
                    Name = x.Name,
                    IncomingEntryTypeId = x.IncomingEntryTypeId,
                    IncomingEntryType = x.IncomingEntryType.Name,
                    IsDoanhThu = x.IncomingEntryType.RevenueCounted,
                    ExchangeRate = x.Currency.CurrencyConverts.OrderByDescending(x => x.DateAt).Select(x => x.Value).FirstOrDefault(),
                    Value = x.Value,
                    CurrencyId = x.CurrencyId.Value,
                    CurrencyName = x.Currency.Name,
                    Month = x.Invoices.Month,
                    Year = x.Invoices.Year,
                    TransactionDate = x.BTransactions.TimeAt,
                    BankTransactionId = x.BankTransactionId.Value,
                });
        }
        public async Task<DebtStatisticFromHRMDto> GetHRMDebtStatistic(int? tenantId)
        {
            var response = await _hrmService.GetHRMDebtStatistic();
            if (response != null)
            {
                return response;
            }
            return new DebtStatisticFromHRMDto();
        }
        private static double GetExchangeRateByDicCurrencyConvert(Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, long currencyId, DateTime? date)
        {
            if (date.HasValue)
            {
                var key = new CurrencyYearMonthDto
                {
                    CurrencyId = currencyId,
                    Year = date.Value.Year,
                    Month = date.Value.Month
                };

                return dicCurrencyConvert.ContainsKey(key) ? dicCurrencyConvert[key] : 1;
            }

            return 1;
        }

        #endregion
    }
}
