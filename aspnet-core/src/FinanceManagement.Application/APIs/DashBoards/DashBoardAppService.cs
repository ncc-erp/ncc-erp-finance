using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.UI;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FinanceManagement.APIs.ComparativeStatistics;
using FinanceManagement.APIs.DashBoards.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.CircleChartDetails.Dtos;
using FinanceManagement.Managers.CircleCharts.Dtos;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.Dashboards;
using FinanceManagement.Managers.Dashboards.Dtos;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Managers.Periods.Dtos;
using FinanceManagement.Uitls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.DashBoards
{
    [AbpAuthorize]
    public class DashBoardAppService : FinanceManagementAppServiceBase
    {
        private const string RATE_STRING = "Tỷ giá";
        private const string EXPENSE_DETAIL = "Expense - Detail";
        private readonly ComparativeStatisticAppService _comparativeStatisticAppService;
        private readonly IDashboardManager _dashboardManager;
        private readonly ICommonManager _commonManager;
        private readonly IPeriodManager _periodManager;
        private readonly IWebHostEnvironment _env;
        public DashBoardAppService(
            ComparativeStatisticAppService comparativeStatisticAppService,
            IWorkScope workScope,
            IDashboardManager dashboardManager,
            ICommonManager commonManager,
            IPeriodManager periodManager,
            IWebHostEnvironment env
        ) : base(workScope)
        {
            _comparativeStatisticAppService = comparativeStatisticAppService;
            _dashboardManager = dashboardManager;
            _commonManager = commonManager;
            _periodManager = periodManager;
            _env = env;
        }
        #region Old Dashboard
        [HttpGet]
        public async Task<DashBoardStatusDto> GetStatusDashBoard()
        {
            await Task.CompletedTask;

            var outcomingEntry = WorkScope.GetAll<OutcomingEntry>();
            var tempOutcomingEntry = WorkScope.GetAll<TempOutcomingEntry>();

            var query = new DashBoardStatusDto
            {
                TotalPending = outcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_PENDINGCEO).Count(),
                TotalPendingCFO = outcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_PENDINGCFO).Count(),
                TotalTransfered = outcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_TRANSFERED).Count(),
                TotalEnd = outcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_END).Count(),
                TotalApproved = outcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_APPROVED).Count(),
                TotalRequestChangePending = tempOutcomingEntry.Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_PENDINGCEO).Count(),

            };
            return query;
        }

        [HttpGet]
        public async Task<List<CashFlowDto>> GetCashFlowDashBoard(int year)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var list = new List<CashFlowDto>();

                for (var i = 1; i <= 12; i++)
                {
                    var firstDayOfMonth = new DateTime(year, i, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    if (firstDayOfMonth.Date > DateTime.Now)
                    {
                        break;
                    }
                    var incomings = await GetIncoming(firstDayOfMonth, lastDayOfMonth);
                    var incomingByMonths = await MapIncoming(incomings);

                    var outcomings = await GetOutcoming(firstDayOfMonth, lastDayOfMonth);
                    var outcomingByMonths = await MapOutcoming(outcomings);
                    list.Add(new CashFlowDto
                    {
                        Month = year + "-" + i,
                        IncomingByMonths = incomingByMonths,
                        TotalIncomingByMonth = incomingByMonths.Sum(x => x.Value),
                        OutcomingByMonths = outcomingByMonths,
                        TotalOutcomingByMonth = outcomingByMonths.Sum(x => x.Value),

                    });

                }
                return list;
            }
        }
        public async Task<object> GetChart(int year)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var charts = new List<ChartDto>();
                for (var i = 1; i <= 12; i++)
                {
                    var firstDayOfMonth = new DateTime(year, i, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    if (firstDayOfMonth.Date > DateTime.Now)
                    {
                        break;
                    }
                    var outcomings = await GetOutcoming(firstDayOfMonth, lastDayOfMonth);
                    double totalOutomingSalary = outcomings.Where(x => x.OutComingEntryTypeCode == Constants.OUTCOMING_ENTRY_TYPE_SALARY).Sum(x => x.Value);
                    var incomings = await GetIncoming(firstDayOfMonth, lastDayOfMonth);
                    var incomingClient = (from ic in incomings
                                          join ba in WorkScope.GetAll<BankAccount>().Include(x => x.Account).ThenInclude(x => x.AccountType)
                                          on ic.FromBankAccountId equals ba.Id
                                          where ba.Account.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT
                                          select new
                                          {
                                              ic.ValueToVND
                                          }).ToList();
                    double totalIncomingClient = incomingClient.Sum(x => x.ValueToVND);

                    charts.Add(new ChartDto
                    {
                        Month = year + "-" + i,
                        totalIncomingClient = totalIncomingClient,
                        totalOutomingSalary = totalOutomingSalary
                    });
                }
                return charts;
            }
        }

        private IQueryable<CurrencyConvert> IQueryCurrencyConvert()
        {
            return WorkScope.GetAll<CurrencyConvert>().Where(s => s.DateAt < new DateTime(2000, 1, 1));
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_View)]
        public async Task<ComparativeStatisticDto> ComparativeStatistics(DateTime? startDate, DateTime? endDate, bool isReal)
        {
            var startBankTransactions = new List<GetBankTransactionDto>();
            var qStartBalance = await GetCurrentBalance(null, startDate, startBankTransactions);
            var startBalance = qStartBalance
                .Select(x => new BalanceBankAccountDto
                {
                    BankNumber = x.BankNumber,
                    HolderName = x.HolderName,
                    CurrentBalance = x.CurrentBalance,
                    CurrentBalanceToVND = x.CurrentBalanceToVND,
                    CurrencyName = x.CurrencyName,
                    BankAccountId = x.BankAccountId,
                }).ToList();
            var result = new ComparativeStatisticDto
            {
                StartBalanceVND = startBalance.Where(s => s.CurrencyName == "VND").Sum(s => s.CurrentBalance),
                StartBalanceUSD = startBalance.Where(s => s.CurrencyName == "USD").Sum(s => s.CurrentBalance),
            };
            result.CurrencyRate = await (
                from c in WorkScope.All<Currency>()
                join cc in IQueryCurrencyConvert()
                    on c.Id equals cc.CurrencyId
                where c.Code == "USD"
                select cc.Value).FirstAsync();

            var qbankTransactions = GetBankTransaction1(startDate, endDate);

            var bankTransactions = await qbankTransactions
                .Where(s => s.FromAccountTypeCode != s.ToAccountTypeCode)
                .ToListAsync();

            var exchangeTransactions = await qbankTransactions
                .Where(s => s.FromAccountTypeCode == s.ToAccountTypeCode
                        && s.Currency != s.FromCurrency)
                .ToListAsync();
            result.ExchangeUSD = exchangeTransactions.Sum(s => s.FromValue);
            result.ExchangeVND = exchangeTransactions.Sum(s => s.ToValue);

            var qbankTransHasBTransFromUSDToVND = qbankTransactions
               .Where(s => s.FromAccountTypeCode != s.ToAccountTypeCode)
               .Where(x => !x.IsExchange)
               .Where(x => x.BTransactionId.HasValue)               
               .Where(s => s.FromAccountTypeCode == "COMPANY" && s.FromCurrency == "USD");

            var bankTransHasBTransFromUSDToVND = qbankTransHasBTransFromUSDToVND.ToList();
            Logger.Info(JsonConvert.SerializeObject(bankTransHasBTransFromUSDToVND));

            result.TotalVNDOutTransaction = bankTransactions
                .Where(s => s.FromAccountTypeCode == "COMPANY" && s.FromCurrency == "VND")
                .Sum(s => s.FromValue);

            result.TotalVNDOutTransaction += await qbankTransHasBTransFromUSDToVND.SumAsync(x => x.ToValue);

            result.TotalUSDOutTransaction = bankTransactions
                .Where(s => s.FromAccountTypeCode == "COMPANY" && s.FromCurrency == "USD")
                .Sum(s => s.FromValue);

            qbankTransHasBTransFromUSDToVND = qbankTransactions
               .Where(s => s.FromAccountTypeCode != s.ToAccountTypeCode)               
               .Where(x => x.BTransactionId.HasValue)
               .Where(s => s.FromAccountTypeCode == "COMPANY" && s.FromCurrency == "USD");

            result.TotalUSDOutTransaction -= await qbankTransHasBTransFromUSDToVND.SumAsync(x => x.FromValue);

            result.TotalVNDInTransaction = bankTransactions
                .Where(s => s.ToAccountTypeCode == "COMPANY" && s.Currency == "VND")
                .Sum(s => s.ToValue);
            result.TotalUSDInTransaction = bankTransactions
                .Where(s => s.ToAccountTypeCode == "COMPANY" && s.Currency == "USD")
                .Sum(s => s.ToValue);


            var qincomming =
                from i in WorkScope.All<IncomingEntry>()
                where i.CreationTime.Date <= endDate.Value
                    && i.IncomingEntryType.Code != "Thu từ bán USD"
                select new
                {
                    IncomingEntryId = i.Id,
                    Value = i.Value,
                    Currency = i.Currency.Code,
                    i.IncomingEntryType.Code
                };
            var incoming = await qincomming.ToListAsync();

            // Duong Nguyen: tru khoan duplicate vi chuyen di 2 lan
            var diff = await qincomming
                .Where(i => i.Code == "Thu do chuyển sai tài khoản"
                         || i.Code == "Thu do chuyển sai tài khoản nhận lương")
                .SumAsync(s => s.Value);
            result.TotalVNDOutTransaction = result.TotalVNDOutTransaction - diff;

            var qoutcoming =
                from oce in WorkScope.All<OutcomingEntry>()
                where oce.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_END
                    && oce.ExecutedTime.Value.Date <= endDate.Value
                    && oce.OutcomingEntryType.Code != "Chi chuyển đổi"
                orderby oce.ExecutedTime
                select new
                {
                    OutcomingEntryId = oce.Id,
                    Value = oce.Value,
                    Currency = oce.Currency.Code,
                    oce.OutcomingEntryType.Code
                };
            var outcoming = await qoutcoming.ToListAsync();

            result.TotalVNDOut = outcoming.Where(s => s.Currency == "VND").Sum(s => s.Value);
            result.TotalUSDOut = outcoming.Where(s => s.Currency == "USD").Sum(s => s.Value);
            result.TotalVNDIn = incoming.Where(s => s.Currency == "VND").Sum(s => s.Value);
            result.TotalUSDIn = incoming.Where(s => s.Currency == "USD").Sum(s => s.Value);

            //Giai trinh
            var reviewExplains = await WorkScope.All<FinanceReviewExplain>().ToListAsync();
            result.TotalVNDOut -= reviewExplains.Sum(s => s.OutcomingDiffVND);
            result.TotalUSDOut -= reviewExplains.Sum(s => s.OutcomingDiffUSD);
            result.TotalVNDIn -= reviewExplains.Sum(s => s.IncomingDiffVND);
            result.TotalUSDIn -= reviewExplains.Sum(s => s.IncomingDiffUSD);

            return result;
        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_CreateExplanation)]
        public async Task AddReviewExplain(FinanceReviewExplainDto model)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                if (model.Id == 0)
                {
                    await WorkScope.InsertAsync(new FinanceReviewExplain
                    {
                        IncomingVND = model.IncomingVND,
                        IncomingUSD = model.IncomingUSD,
                        IncomingDiffVNDNote = model.IncomingDiffVNDNote,
                        IncomingDiffUSDNote = model.IncomingDiffUSDNote,
                        IncomingDiffUSD = model.IncomingDiffUSD,
                        IncomingDiffVND = model.IncomingDiffVND,
                        IncomingUSDTransaction = model.IncomingUSDTransaction,
                        IncomingVNDTransaction = model.IncomingVNDTransaction,
                        OutcomingDiffUSD = model.OutcomingDiffUSD,
                        OutcomingDiffUSDNote = model.OutcomingDiffUSDNote,
                        OutcomingDiffVND = model.OutcomingDiffVND,
                        OutcomingDiffVNDNote = model.OutcomingDiffVNDNote,
                        OutcomingUSD = model.OutcomingUSD,
                        OutcomingVND = model.OutcomingVND,
                        OutcomingUSDTransaction = model.OutcomingUSDTransaction,
                        OutcomingVNDTransaction = model.OutcomingVNDTransaction,
                    });
                }
                else
                {
                    var oldData = await WorkScope.All<FinanceReviewExplain>().FirstOrDefaultAsync(s => s.Id == model.Id);
                    oldData.IncomingDiffUSDNote = model.IncomingDiffUSDNote;
                    oldData.IncomingDiffVNDNote = model.IncomingDiffVNDNote;
                    oldData.OutcomingDiffUSDNote = model.OutcomingDiffUSDNote;
                    oldData.OutcomingDiffVNDNote = model.OutcomingDiffVNDNote;
                    await WorkScope.UpdateAsync(oldData);
                }
            }
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_ViewExplanation)]
        public async Task<List<FinanceReviewExplain>> GetReviewExplain()
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await WorkScope.All<FinanceReviewExplain>().ToListAsync();
            }
        }

        private async Task<List<BalanceBankAccountDto>> GetOutComingByTransaction(List<BalanceBankAccountDto> outcomingByTransactions, ExplanationType type, long comparativeStatisticId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var result = (from cvo in outcomingByTransactions
                              join ex in WorkScope.GetAll<Explanation>().Where(x => x.ComparativeStatisticId == comparativeStatisticId && x.Type == type)
                              on cvo.BankAccountId equals ex.BankAccountId
                              select new BalanceBankAccountDto
                              {
                                  BankAccountId = cvo.BankAccountId,
                                  HolderName = cvo.HolderName,
                                  BankNumber = cvo.BankNumber,
                                  CurrentBalance = cvo.CurrentBalance,
                                  CurrentBalanceToVND = cvo.CurrentBalanceToVND,
                                  DifferenceBalance = cvo.DifferenceBalance,
                                  DifferenceBalanceToVND = cvo.DifferenceBalanceToVND,
                                  ReducedBalance = cvo.ReducedBalance,
                                  ReducedBalanceToVND = cvo.ReducedBalanceToVND,
                                  IncreaseBalance = cvo.IncreaseBalance,
                                  IncreaseBalanceToVND = cvo.IncreaseBalanceToVND,
                                  CurrencyName = cvo.CurrencyName,
                                  CurrencyId = cvo.CurrencyId,
                                  BankAccountExplanation = ex.BankAccountExplanation,
                                  ExplanationId = ex.Id
                              }).ToList();
                return result;
            }
        }

        private async Task<List<BalanceBankAccountDto>> GetCurrentBalance(DateTime? fromDate, DateTime? toDate, List<GetBankTransactionDto> bankTransactions)
        {
            var balanceBankAccounts = new List<BalanceBankAccountDto>();
            var mapBalanceBankAccount = new Dictionary<string, double>();
            var bankAccountCompanies = await (
                from ba in WorkScope.GetAll<BankAccount>().Include(x => x.Currency)
                join acc in WorkScope.GetAll<Account>() on ba.AccountId equals acc.Id
                where acc.AccountType.Code == Constants.ACCOUNT_TYPE_COMPANY
                join cc in IQueryCurrencyConvert() on ba.CurrencyId equals cc.Id
                select new
                {
                    BankAccountId = ba.Id,
                    HolderName = ba.HolderName,
                    BankNumber = ba.BankNumber,
                    BaseBalance = ba.BaseBalance,
                    CurrencyId = ba.CurrencyId,
                    CurrencyName = ba.Currency.Name,
                    CurrencyValue = cc.Value
                }).ToListAsync();
            foreach (var bankAccount in bankAccountCompanies)
            {
                double totalIncomingByTransaction = bankTransactions.Where(x => x.ToBankAccountId == bankAccount.BankAccountId).Sum(x => x.ToValue);
                double totalOutcomingByTransaction = bankTransactions.Where(x => x.FromBankAccountId == bankAccount.BankAccountId).Sum(x => x.FromValue);

                double currentBalance = bankAccount.BaseBalance + totalIncomingByTransaction - totalOutcomingByTransaction;
                double differenceBalance = totalIncomingByTransaction - totalOutcomingByTransaction;
                var balanceBankAccount = new BalanceBankAccountDto
                {
                    BankAccountId = bankAccount.BankAccountId,
                    HolderName = bankAccount.HolderName,
                    BankNumber = bankAccount.BankNumber,
                    CurrentBalance = currentBalance,
                    CurrentBalanceToVND = currentBalance * bankAccount.CurrencyValue,
                    DifferenceBalance = differenceBalance,
                    DifferenceBalanceToVND = differenceBalance * bankAccount.CurrencyValue,
                    ReducedBalance = totalOutcomingByTransaction,
                    ReducedBalanceToVND = totalOutcomingByTransaction * bankAccount.CurrencyValue,
                    IncreaseBalance = totalIncomingByTransaction,
                    IncreaseBalanceToVND = totalIncomingByTransaction * bankAccount.CurrencyValue,
                    CurrencyName = bankAccount.CurrencyName,
                    CurrencyId = bankAccount.CurrencyId.Value,
                    //FromBankAcountTypeCode = fromBankAccountTypeCode,
                    //ToBankAcountTypeCode = toBankAccountTypeCode,
                    //OutComingFeeByTransaction = totalOutcomingFee
                };
                balanceBankAccounts.Add(balanceBankAccount);
            }
            return balanceBankAccounts;
        }
        private IQueryable<GetBankTransactionDto> GetBankTransaction(DateTime? startDate, DateTime? endDate)
        {
            return
                from bt in WorkScope.All<BankTransaction>()
                join fba in WorkScope.All<BankAccount>()
                    on bt.FromBankAccountId equals fba.Id
                join tba in WorkScope.All<BankAccount>()
                    on bt.ToBankAccountId equals tba.Id
                join cc in IQueryCurrencyConvert()
                    on tba.CurrencyId equals cc.CurrencyId
                join fcc in IQueryCurrencyConvert()
                    on fba.CurrencyId equals fcc.CurrencyId

                where bt.TransactionDate.Date <= endDate.Value
                select new GetBankTransactionDto
                {
                    BankTransactionId = bt.Id,
                    FromBankAccountId = bt.FromBankAccountId,
                    ToBankAccountId = bt.ToBankAccountId,
                    FromValue = bt.FromValue,
                    ToValue = bt.ToValue,
                    Fee = bt.Fee,
                    FromAccountTypeCode = fba.Account.AccountType.Code,
                    ToAccountTypeCode = tba.Account.AccountType.Code,
                    Currency = cc.Currency.Code,
                    FromCurrency = fcc.Currency.Code,
                    BTransactionId = bt.BTransactionId,
                };
        }

        private IQueryable<GetBankTransactionDto> GetBankTransaction1(DateTime? startDate, DateTime? endDate)
        {
            return
                from bt in WorkScope.All<BankTransaction>()
                join fba in WorkScope.All<BankAccount>()
                    on bt.FromBankAccountId equals fba.Id
                join tba in WorkScope.All<BankAccount>()
                    on bt.ToBankAccountId equals tba.Id
                join cc in IQueryCurrencyConvert()
                    on tba.CurrencyId equals cc.CurrencyId
                join fcc in IQueryCurrencyConvert()
                    on fba.CurrencyId equals fcc.CurrencyId

                where bt.TransactionDate.Date <= endDate.Value
                select new GetBankTransactionDto
                {
                    BankTransactionId = bt.Id,
                    FromBankAccountId = bt.FromBankAccountId,
                    ToBankAccountId = bt.ToBankAccountId,
                    FromValue = bt.FromValue,
                    ToValue = bt.ToValue,
                    Fee = bt.Fee,
                    FromAccountTypeCode = fba.Account.AccountType.Code,
                    ToAccountTypeCode = tba.Account.AccountType.Code,
                    Currency = cc.Currency.Code,
                    FromCurrency = fcc.Currency.Code,
                    BTransactionId = bt.BTransactionId,
                    IsExchange = bt.OutcomingEntryBankTransactions.Any(x => x.OutcomingEntry.OutcomingEntryType.Code == "Chi chuyển đổi")
                };
        }

        [HttpGet]
        public async Task<PercentEntryTypeDto> PercentEntryType(DateTime? startDate, DateTime? endDate)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var outcomings = await GetOutcoming(startDate, endDate);
                var listOutcoming = await MapOutcoming(outcomings);

                var incomings = await GetIncoming(startDate, endDate);
                var listIncoming = await MapIncoming(incomings);
                return new PercentEntryTypeDto
                {
                    OutcomingEntry = listOutcoming,
                    TotalOutcomingEntry = listOutcoming.Sum(x => x.Value),
                    IncomingEntry = listIncoming,
                    TotalIncomingEntry = listIncoming.Sum(x => x.Value)
                };
            }
        }
        private async Task<List<GetIncomingDto>> GetIncoming(DateTime? startDate, DateTime? endDate)
        {
            var qincomming =
                from i in WorkScope.GetAll<IncomingEntry>()
                join b in WorkScope.GetAll<BankTransaction>()
                    on i.BankTransactionId equals b.Id

                where b.TransactionDate.Date <= endDate.Value.Date
                    && i.IncomingEntryType.Code != "Thu do chuyển sai tài khoản"
                    && i.IncomingEntryType.Code != "Thu do chuyển sai tài khoản nhận lương"
                    && i.IncomingEntryType.Code != "Thu từ bán USD"
                orderby i.CreationTime
                select new GetIncomingDto
                {
                    IncomingEntryId = i.Id,
                    ExchangeRate = i.Currency.CurrencyConverts.Select(x => x.Value).FirstOrDefault(),
                    Value = i.Value,
                    IncomingEntryTypeName = i.IncomingEntryType.Name,
                    IncomingEntryTypeCode = i.IncomingEntryType.Code,
                    PathName = i.IncomingEntryType.PathName,
                    FromBankAccountId = b.FromBankAccountId,
                    BankTransactionId = b.Id
                };


            var incomings = await qincomming.ToListAsync();

            return incomings;
        }
        private async Task<List<GetOutcomingDto>> GetOutcoming(DateTime? startDate, DateTime? endDate)
        {
            var query =
                from oce in WorkScope.GetAll<OutcomingEntry>()
                    .Select(o => new
                    {
                        o.Id,
                        o.OutcomingEntryType.ParentId,
                        o.Value,
                        OutcomingEntryTypeCode = o.OutcomingEntryType.Code,
                        o.OutcomingEntryType.PathName,
                        ExchangeRate = o.Currency.CurrencyConverts.Select(x => x.Value).FirstOrDefault(),
                        o.ExecutedTime,
                        WorkflowCode = o.WorkflowStatus.Code
                    })
                where oce.WorkflowCode == Constants.WORKFLOW_STATUS_END
                && oce.ExecutedTime.Value.Date <= endDate.Value.Date
                && oce.OutcomingEntryTypeCode != "Chi chuyển đổi"
                orderby oce.ExecutedTime
                select new GetOutcomingDto
                {
                    OutcomingEntryId = oce.Id,
                    ParentId = oce.ParentId,
                    Value = oce.Value,
                    ExchangeRate = oce.ExchangeRate,
                    OutComingEntryTypeCode = oce.OutcomingEntryTypeCode,
                    PathName = oce.PathName
                };
            var outcomings = await query.ToListAsync();


            return outcomings;
        }
        private async Task<Dictionary<string, double>> MapOutcoming(List<GetOutcomingDto> outcomings)
        {
            var listOutcoming = new Dictionary<string, double>();

            //var outcomings = await GetOutcoming(startDate, endDate);

            var pathNameMap = new Dictionary<long, List<string>>();
            foreach (var item in outcomings)
            {
                var qItem = item.PathName.Trim('|');
                List<string> arrItem = qItem.Split("|").ToList();
                pathNameMap.Add(item.OutcomingEntryId, arrItem);
            }
            var parentPathNames = WorkScope.GetAll<OutcomingEntryType>().
                Where(x => !x.ParentId.HasValue)
                .Select(x => new { ParentName = x.Name, PathName = x.PathName.Trim('|') }).ToList();

            foreach (var parent in parentPathNames)
            {
                double total = 0;
                foreach (var outcome in outcomings)
                {
                    if (pathNameMap[outcome.OutcomingEntryId].Contains(parent.PathName))
                    {
                        total += outcome.Value;
                    }
                }
                if (total > 0)
                {
                    listOutcoming.Add(parent.ParentName, total);
                }
            }
            return listOutcoming;
        }
        private async Task<Dictionary<string, double>> MapIncoming(List<GetIncomingDto> incomings)
        {
            var listIncoming = new Dictionary<string, double>();

            //var qIncomings = await GetIncoming(startDate, endDate);
            //var incomings = qIncomings.Where(x => x.IncomingEntryTypeName)

            var pathNameMap = new Dictionary<long, List<string>>();
            foreach (var item in incomings)
            {
                var qItem = item.PathName.Trim('|');
                List<string> arrItem = qItem.Split("|").ToList();
                pathNameMap.Add(item.IncomingEntryId, arrItem);
            }
            var parentPathNames = WorkScope.GetAll<IncomingEntryType>().
                Where(x => !x.ParentId.HasValue)
                .Select(x => new { ParentName = x.Name, PathName = x.PathName.Trim('|') }).ToList();

            foreach (var parent in parentPathNames)
            {
                double total = 0;
                foreach (var income in incomings)
                {
                    if (pathNameMap[income.IncomingEntryId].Contains(parent.PathName))
                    {
                        total += income.ValueToVND;
                    }
                }
                if (total > 0)
                {
                    listIncoming.Add(parent.ParentName, total);
                }
            }
            return listIncoming;
        }

        public static List<DateTime> GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                monthDiff -= 1;
            }

            List<DateTime> results = new List<DateTime>();
            for (int i = monthDiff; i >= 0; i--)
            {
                results.Add(to.AddMonths(-i));
            }

            return results;
        }
        //[AbpAuthorize(PermissionNames.DashBoard_ExportReport)]
        //[HttpGet]
        //public async Task<byte[]> ExportExcel(DateTime? fromDate, DateTime? toDate, long? branchId)
        //{
        //    try
        //    {
        //        using (var wb = new XLWorkbook())
        //        {
        //            var incomeWS = wb.Worksheets.Add("Income");
        //            var expenseWS = wb.Worksheets.Add("Expense");
        //            var expenseDetailWS = wb.Worksheets.Add(EXPENSE_DETAIL);
        //            var currencyWS = wb.Worksheets.Add(RATE_STRING);
        //            var currencyMap = await CreateCurrencyConvertWS(currencyWS);
        //            var parentTotalMap = await CreateExpenseDetailWS(expenseDetailWS, fromDate, toDate, branchId);
        //            try
        //            {
        //                await CreateIncomeWS(incomeWS, currencyMap, fromDate, toDate, branchId);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new UserFriendlyException(String.Format("CreateIncomeWS - Error: " + ex.Message));
        //            }
        //            await CreateExpenseWS(expenseWS, parentTotalMap, fromDate, toDate);
        //            using (var stream = new MemoryStream())
        //            {
        //                wb.SaveAs(stream);
        //                var content = stream.ToArray();
        //                return content;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException(String.Format("error: " + ex.Message));
        //    }

        //}
        //private async Task CreateExpenseWS(IXLWorksheet ws, Dictionary<string, string> parentTotalMap, DateTime? fromDate, DateTime? toDate)
        //{
        //    int currentRow = 1;
        //    ws.Cell(currentRow, 2).Value = "VND";
        //    foreach (var parent in parentTotalMap)
        //    {
        //        currentRow++;
        //        ws.Cell(currentRow, 1).Value = parent.Key;
        //        ws.Cell(currentRow, 2).FormulaA1 = $"={parent.Value}";
        //    }

        //    var totalExpenseFormula = $"=SUM(B2:C{currentRow})";
        //    ws.Cell(currentRow + 2, 1).Value = "Total";
        //    ws.Cell(currentRow + 2, 2).FormulaA1 = totalExpenseFormula;
        //}
        //private async Task CreateIncomeWS(IXLWorksheet ws, Dictionary<string, string> currencyConvertMap, DateTime? fromDate, DateTime? toDate, long? branchId)
        //{

        //    var qTransactions = WorkScope.GetAll<BankTransaction>()
        //                     .Where(x => !fromDate.HasValue || x.TransactionDate >= fromDate)
        //                     .Where(x => !toDate.HasValue || x.TransactionDate < toDate.Value.AddDays(+1));
        //    var qIncoming = WorkScope.GetAll<IncomingEntry>().Include(x => x.Branch).Include(x => x.Currency);
        //    foreach (var item in qIncoming)
        //    {
        //        if (!item.BranchId.HasValue)
        //        {
        //            var branchHNId = await WorkScope.GetAll<Branch>()
        //                .Where(x => x.Code == Constants.BRANCH_CODE_HN)
        //                .Select(x => x.Id).FirstOrDefaultAsync();
        //            item.BranchId = branchHNId;
        //        }
        //    }
        //    var qbankTransacsions =
        //        from bt in qTransactions
        //        join ice in qIncoming.Where(x => !branchId.HasValue || x.BranchId == branchId)
        //        on bt.Id equals ice.BankTransactionId
        //        join bafrombankaccount in WorkScope.GetAll<BankAccount>()
        //        on bt.FromBankAccountId equals bafrombankaccount.Id
        //        join client in WorkScope.GetAll<Account>().Include(x => x.AccountType)
        //        on bafrombankaccount.AccountId equals client.Id
        //        //join batobankaccount in WorkScope.GetAll<BankAccount>() on bt.ToBankAccountId equals batobankaccount.Id
        //        //join currency in WorkScope.GetAll<Currency>() on batobankaccount.CurrencyId equals currency.Id
        //        where client.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT
        //        select new
        //        {
        //            CurrencyName = ice.Currency.Name,
        //            AccountName = client.Name,
        //            Value = ice.Value,
        //        };

        //    var bankTransactions = await qbankTransacsions.ToListAsync();
        //    var clients = WorkScope.GetAll<Account>()
        //        .Where(x => x.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT)
        //        .Select(x => x.Name).ToList();
        //    var currencies = bankTransactions.Select(x => x.CurrencyName).Distinct().ToList();
        //    var currentRow = 1;
        //    var currentColumn = 1;
        //    var clientMap = FillClientData(ws, clients, currentRow, currentColumn, true);
        //    var currencyMap = FillClientData(ws, currencies, currentRow, currentColumn, false);

        //    foreach (var transaction in bankTransactions)
        //    {
        //        var cell = ws.Cell(clientMap[transaction.AccountName], currencyMap[transaction.CurrencyName]);
        //        var value = cell.GetValue<double?>() ?? 0;
        //        value += transaction.Value;
        //        cell.Value = value;
        //    }
        //    //Tính tổng từng dòng

        //    var clientTotalCol = currencyMap.Count + 2;
        //    ws.Cell(1, clientTotalCol).Value = "To VND";
        //    var clientTotalFormula = "=0";

        //    for (var i = 2; i < clientTotalCol; i++)
        //    {
        //        clientTotalFormula += $"+{GetXLColName(i)}[row]*{currencyConvertMap[ws.Cell(1, i).GetValue<string>()]}";
        //    }
        //    for (var i = 2; i <= clientMap.Count + 1; i++)
        //    {
        //        ws.Cell(i, clientTotalCol).FormulaA1 = clientTotalFormula.Replace("[row]", i.ToString());
        //    }
        //    var totalClientRange =
        //        $"{GetXLColName(clientTotalCol)}2:{GetXLColName(clientTotalCol)}{clientMap.Count + 1}";

        //    var totalIncomeRow = clientMap.Count + 3;
        //    ws.Cell(totalIncomeRow, 1).Value = "Total";
        //    ws.Cell(totalIncomeRow, 2).FormulaA1 = $"=SUM({totalClientRange})";
        //}
        //private async Task<Dictionary<string, string>> CreateExpenseDetailWS(IXLWorksheet ws, DateTime? fromDate, DateTime? toDate, long? branchId)
        //{
        //    var qOutcomingEntry = WorkScope.GetAll<OutcomingEntry>()
        //                     .Include(x => x.OutcomingEntryType)
        //                     .Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_END)
        //                     .Where(x => !fromDate.HasValue || x.ExecutedTime >= fromDate)
        //                     .Where(x => !toDate.HasValue || x.ExecutedTime < toDate.Value.AddDays(+1));
        //    foreach (var item in qOutcomingEntry)
        //    {
        //        if (!item.BranchId.HasValue)
        //        {
        //            var branchHNId = await WorkScope.GetAll<Branch>().Where(x => x.Code == Constants.BRANCH_CODE_HN).Select(x => x.Id).FirstOrDefaultAsync();
        //            item.BranchId = branchHNId;
        //        }
        //    }
        //    var outComingEntries = await (from oc in qOutcomingEntry.Where(x => !branchId.HasValue || x.BranchId == branchId)
        //                                  group oc by new { oc.OutcomingEntryTypeId, oc.OutcomingEntryType.Name, oc.OutcomingEntryType.ParentId, oc.OutcomingEntryType.PathName } into g
        //                                  select new
        //                                  {
        //                                      OutcomingEntryTypeId = g.Key.OutcomingEntryTypeId,
        //                                      OutcomingEntryTypeName = g.Key.Name,
        //                                      PathName = g.Key.PathName,
        //                                      ParentId = g.Key.ParentId,
        //                                      SumExpense = g.Sum(x => x.Value)
        //                                  }).ToListAsync();

        //    var expenses = (from oce in outComingEntries
        //                    join oct in WorkScope.GetAll<OutcomingEntryType>() on oce.ParentId equals oct.Id into octs
        //                    from oct in octs.DefaultIfEmpty()
        //                    select new
        //                    {
        //                        OutcomingEntryTypeId = oce.OutcomingEntryTypeId,
        //                        OutcomingEntryTypeName = oce.OutcomingEntryTypeName,
        //                        ParentName = !oce.ParentId.HasValue ? oce.OutcomingEntryTypeName : oct.Name,
        //                        PathName = oce.PathName,
        //                        SumExpense = oce.SumExpense
        //                    }).OrderByDescending(x => x.ParentName).ToList();
        //    var outComingEntryTypeParents = await WorkScope.GetAll<OutcomingEntryType>()
        //        .Where(x => !x.ParentId.HasValue)
        //        .Select(x => x.Name).ToListAsync() /*expenses.Select(x => x.ParentName).Distinct().ToList()*/;
        //    var currentRow = 1;
        //    var outComingEntryTypeParentMap = FillClientData(ws, outComingEntryTypeParents, 1, currentRow, true);
        //    var parentTotalMap = new Dictionary<string, string>();

        //    foreach (var parent in outComingEntryTypeParents)
        //    {
        //        var firstChildRow = currentRow + 1;
        //        var children = expenses.Where(x => x.PathName.Trim('|').Split("|")[0] == parent);
        //        var childrenCount = children.Count() == 0 ? 1 : children.Count();
        //        var range = ws.Range($"A{currentRow + 1}:A{currentRow + childrenCount}").Merge();
        //        range.Value = parent;
        //        foreach (var child in children)
        //        {
        //            currentRow++;
        //            var arrPathName = child.PathName.Trim('|').Split("|");
        //            string OutcomingEntryTypeName = arrPathName.Count() < 2 ? arrPathName[0] : arrPathName[1];
        //            ws.Cell(currentRow, 2).Value = child.OutcomingEntryTypeName;
        //            ws.Cell(currentRow, 3).Value = child.SumExpense;
        //        }
        //        currentRow++;
        //        if (children.Count() > 0)
        //        {
        //            range = ws.Range($"A{currentRow}:B{currentRow}").Merge();
        //            range.Value = "Total";
        //            ws.Cell(currentRow, 3).FormulaA1 = $"=SUM(C{firstChildRow}:C{currentRow - 1})";
        //        }
        //        parentTotalMap.Add(parent, $"\'{ws.Name}\'!$C${currentRow}");
        //    }
        //    var totalExpenseFormula = $"=0";
        //    foreach (var p in parentTotalMap)
        //    {
        //        totalExpenseFormula += $"+{p.Value}";
        //    }
        //    ws.Cell(currentRow + 2, 2).Value = "Total";
        //    ws.Cell(currentRow + 2, 3).FormulaA1 = totalExpenseFormula;
        //    return parentTotalMap;
        //}

        //private string GetXLColName(int columnNumber)
        //{
        //    int dividend = columnNumber;
        //    string columnName = String.Empty;
        //    int modulo;

        //    while (dividend > 0)
        //    {
        //        modulo = (dividend - 1) % 26;
        //        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        //        dividend = (int)((dividend - modulo) / 26);
        //    }
        //    return columnName;
        //}

        //private Dictionary<string, int> FillClientData(IXLWorksheet ws, List<string> data, int startCol, int startRow, bool isCol)
        //{
        //    var index = isCol ? startCol : startRow;
        //    var result = new Dictionary<string, int>();
        //    foreach (var item in data)
        //    {
        //        index++;
        //        var col = ws.Cell(isCol ? index : startCol, isCol ? startRow : index);
        //        col.Value = item;
        //        result.Add(item, index);
        //    }
        //    return result;
        //}
        //private async Task<Dictionary<string, string>> CreateCurrencyConvertWS(IXLWorksheet ws)
        //{
        //    var currentRow = 1;
        //    ws.Cell(currentRow, 2).Value = RATE_STRING;

        //    var currencyConverts = await IQueryCurrencyConvert()
        //        .Include(x => x.Currency)
        //        .Select(x => new
        //        {
        //            Name = x.Currency.Name,
        //            Value = x.Value
        //        }).ToListAsync();
        //    var currencyMap = new Dictionary<string, string>();
        //    foreach (var currency in currencyConverts)
        //    {
        //        currentRow++;
        //        ws.Cell(currentRow, 1).Value = currency.Name;
        //        ws.Cell(currentRow, 2).Value = currency.Value;
        //        currencyMap.Add(currency.Name, $"\'{ws.Name}\'!$B${currentRow}");
        //    }
        //    return currencyMap;
        //}
        #endregion

        #region new version v2
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatisticNew_View)]
        public async Task<ResultNewComparativeStatisticDto> GetBankAccountStatistics(bool isIncludeBTransPending = false)
        {
            return await _dashboardManager.GetBankAccountStatistics(isIncludeBTransPending);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatisticNew_View)]
        public async Task<List<NewComparativeStatisticsIncomingEntryDto>> GetComparativeStatisticsIncomingEntry()
        {
            return await _dashboardManager.GetComparativeStatisticsIncomingEntry();
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatisticNew_View)]
        public async Task<NewComparativeStatisticsOutcomingEntryDto> GetComparativeStatisticsOutcomingEntry()
        {
            return await _dashboardManager.GetComparativeStatisticsOutcomingEntry();
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatisticNew_View)]
        public async Task<ResultComparativeStatisticsOutBankTransactionDto> GetComparativeStatisticsOutBankTransaction()
        {
            return await _dashboardManager.GetComparativeStatisticsOutBankTransaction();
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<ResultComparativeStatisticByCurrencyDto> GetComparativeStatisticByCurrency()
        {
            return await _dashboardManager.GetComparativeStatisticByCurrency();
        }
        [HttpGet]
        public async Task<List<OverviewOutcomingEntryStatisticDto>> OverviewOutcomingEntryStatistics()
        {
            return await _dashboardManager.OverviewOutcomingEntryStatistics();
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<OverviewInvoiceStatisticDto> OverviewInvoiceStatistics()
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _dashboardManager.OverviewInvoiceStatistics();
            }
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<List<OverviewBTransactionStatisticDto>> OverviewBTransactionStatistics()
        {
            return await _dashboardManager.OverviewBTransactionStatistics();
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<ResultChartDto> GetNewChart([Required] DateTime startDate, [Required] DateTime endDate, bool isByPeriod)
        {
            if (isByPeriod)
            {
                return await GetDataNewChart(startDate, endDate);
            }
            else
            {
                using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
                {
                    return await GetDataNewChart(startDate, endDate);
                }
            }
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpPost]
        public async Task<List<ResultCircleChartDto>> GetCircleChart(InputListCircleChartDto input)
        {
            return await GetDataCircleChartByIds(input.CircleChartIds, input.StartDate, input.EndDate);
        }
        private async Task<ResultChartDto> GetDataNewChart([Required] DateTime startDate, [Required] DateTime endDate)
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetLastDayOfMonth(endDate);

            var labels = DateTimeUtils.GetMonthYearLabelChartFromDate(startDate, endDate);
            var dicCurrencyConvert = _dashboardManager.GetDictionaryCurrencyConvertByYearMonth(startDate, endDate);
            //TODO::ktra du tien chuyen doi trong thoi gian tim kiem 
            _dashboardManager.CheckDictionaryCurrencyConvertByYearMonth(dicCurrencyConvert, startDate, endDate);
            var treeOutcomingEntry = _commonManager.GetTreeOutcomingEntries();
            var treeIncomingEntry = _commonManager.GetTreeIncomingEntries();
            var outcomingEntryStatusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);

            var lineCharts = await WorkScope.GetAll<LineChart>()
                .Where(x => x.IsActive)
                .Select(x => new
                {
                    x.Name,
                    x.Color,
                    x.Type,
                    EntryIds = x.LineChartSettings.Select(x => x.ReferenceId)
                })
                .ToListAsync();

            var result = new ResultChartDto();
            result.Labels = labels;

            foreach (var lineChart in lineCharts)
            {
                var chart = new NewChartDto();
                chart.Name = lineChart.Name;
                chart.ItemStyle = new ChartStyleDto
                {
                    Color = lineChart.Color
                };
                chart.Type = "line";

                var setEntryIds = new HashSet<long>();
                if (lineChart.Type == LineChartSettingType.Income)
                {
                    _commonManager.GetEntryTypeIdsFromTree(lineChart.EntryIds, treeIncomingEntry, false, setEntryIds);
                    chart.Data = _dashboardManager.GetLineChartIncomingEntry(startDate, endDate, setEntryIds, labels, dicCurrencyConvert);
                }
                else
                {
                    _commonManager.GetEntryTypeIdsFromTree(lineChart.EntryIds, treeOutcomingEntry, false, setEntryIds);
                    chart.Data = _dashboardManager.GetLineChartOutcomingEntry(startDate, endDate, setEntryIds, labels, dicCurrencyConvert, outcomingEntryStatusEndId);
                }

                result.Charts.Add(chart);
            }

            result.Charts.Add(_dashboardManager.GetBarChartIncoming(startDate, endDate, labels, dicCurrencyConvert));
            result.Charts.Add(_dashboardManager.GetBarChartOutcomingEntry(startDate, endDate, labels, dicCurrencyConvert, outcomingEntryStatusEndId));

            return result;
        }

        private async Task<List<ResultCircleChartDto>> GetDataCircleChartByIds(
            List<long> circleChartIds, 
            [Required] DateTime startDate, 
            [Required] DateTime endDate)
        {
            var query = WorkScope.GetAll<CircleChart>().Where(s => s.IsActive == true);

            if (circleChartIds != null && circleChartIds.Any())
            {
                query = query.Where(s => circleChartIds.Contains(s.Id));
            }
            var circleChartInfo = await query
                .Select(s => new CircleChartInfoDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsIncome = s.IsIncome,
                    Details = s.CircleChartDetails.Select(x => new CircleChartDetailInfoDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Color = x.Color,
                        Branch = new BranchInfoDto
                        {
                            BranchId = x.Branch.Id,
                            BranchName = x.Branch.Name
                        },
                        ClientIds = x.ClientIds,
                        InOutcomeTypeIds = x.InOutcomeTypeIds,
                    }).ToList()
                }).ToListAsync();
            if (circleChartInfo.IsNullOrEmpty())
            {
                return null;
            }
            var totalResult = new List<ResultCircleChartDto>();
            foreach (var chartInfo in circleChartInfo)
            {
                var result = await GetDataCircleChartById(chartInfo, startDate, endDate);
                totalResult.Add(result);
            }
            return totalResult;
        }
        private async Task<ResultCircleChartDto> GetDataCircleChartById(
            CircleChartInfoDto chartInfo, 
            [Required] DateTime startDate, 
            [Required] DateTime endDate)
        {
            var dicCurrencyConvert = _dashboardManager.GetDictionaryCurrencyConvertByYearMonth(startDate, endDate);
            //TODO::ktra du tien chuyen doi trong thoi gian tim kiem 
            _dashboardManager.CheckDictionaryCurrencyConvertByYearMonth(dicCurrencyConvert, startDate, endDate);
            var outcomingEntryStatusEndId = await _commonManager.GetStatusIdByCode(FinanceManagementConsts.WORKFLOW_STATUS_END);

            var result = new ResultCircleChartDto
                {
                    ChartName = chartInfo.Name,
                };
            foreach (var detail in chartInfo.Details)
            { 
                var chart = new ResultCircleChartDetailDto();
                chart.Name = detail.Name;
                chart.Color = detail.Color;
                var setEntryIds = new HashSet<long>();
                if (chartInfo.IsIncome)
                {
                    var treeIncomingEntry = _commonManager.GetTreeIncomingEntries();
                    _commonManager.GetEntryTypeIdsFromTree(detail.ListInOutcomeTypeIds, treeIncomingEntry, false, setEntryIds);
                    chart.Value = _dashboardManager.GetValueCircleChartIncomingEntry(startDate, endDate, setEntryIds, dicCurrencyConvert, detail.ListClientIds);
                }
                else
                {
                    var treeOutcomingEntry = _commonManager.GetTreeOutcomingEntries();
                    _commonManager.GetEntryTypeIdsFromTree(detail.ListInOutcomeTypeIds, treeOutcomingEntry, false, setEntryIds);
                    chart.Value = _dashboardManager.GetValueCircleChartOutcomingEntry(startDate, endDate, setEntryIds, dicCurrencyConvert, outcomingEntryStatusEndId, detail.Branch.BranchId);
                }
                result.Details.Add(chart);
            }
            return result;
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<List<PieChartDto>> GetPieChartIncoming(
            long? rootId,
            [Required] DateTime startDate,
            [Required] DateTime endDate,
            bool isByPeriod
        )
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetLastDayOfMonth(endDate);
            if (isByPeriod)
            {
                return await _dashboardManager.GetPieChartIncoming(rootId, startDate, endDate);
            }
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _dashboardManager.GetPieChartIncoming(rootId, startDate, endDate);
            }
        }
        [AbpAuthorize(PermissionNames.Dashboard)]
        [HttpGet]
        public async Task<List<PieChartDto>> GetPieChartOutcoming(
            long? rootId,
            [Required] DateTime startDate,
            [Required] DateTime endDate,
            bool isByPeriod
        )
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetLastDayOfMonth(endDate);
            if (isByPeriod)
            {
                return await _dashboardManager.GetPieChartOutcoming(rootId, startDate, endDate);
            }
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _dashboardManager.GetPieChartOutcoming(rootId, startDate, endDate);
            }
        }
        [HttpGet]
        public async Task<byte[]> ExportStatisticDashboard(int periodId, DateTime startDate, DateTime endDate)
        {
            var periodInfo = await _periodManager
                .IQGetPeriod()
                .Where(x => x.Id == periodId)
                .FirstOrDefaultAsync();

            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "BAO_CAO_Finfast.xlsx" });
            var currencyDefault = await GetCurrencyDefaultAsync();
            using (CurrentUnitOfWork.SetFilterParameter(nameof(IMustHavePeriod),"PeriodId", periodId))
            {
                var dataBCChung = await _dashboardManager.GetDataBaoCaoChung(startDate, endDate, -1, null);
                var dataBCChi = await _dashboardManager.GetAllRequestChiForBaoCao(startDate, endDate, -1, null);
                var dataBCThu = await _dashboardManager.GetDataBaoCaoThu(startDate, endDate,null);

                using (ExcelPackage pck = new ExcelPackage(file.OpenRead()))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    var sheetBCChung = pck.Workbook.Worksheets[0];
                    AssignDataForSheetBCChung(dataBCChung, ref sheetBCChung, periodInfo, startDate, endDate);

                    var sheetBCThu = pck.Workbook.Worksheets[1];
                    AssignDataForSheetBCThu(dataBCThu, ref sheetBCThu, periodInfo, currencyDefault?.Name, startDate, endDate);

                    var sheetBCChi = pck.Workbook.Worksheets[2];
                    AssignDataForSheetBCChi(dataBCChi, ref sheetBCChi, periodInfo, currencyDefault?.Name, startDate, endDate);

                    using (var stream = new MemoryStream())
                    {
                        pck.SaveAs(stream);
                        var content = stream.ToArray();
                        return content;
                    }
                }
            }
        }

        [HttpGet]
        public async Task<byte[]> ExportBCC(DateTime startDate, DateTime endDate, long branchId, ExpenseType? isExpense)
        {
            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "Template_BaoCaoChi.xlsx" });
            var currencyDefault = await GetCurrencyDefaultAsync();

            var dataChi = await _dashboardManager.GetAllRequestChiForBaoCao(startDate, endDate, branchId, isExpense);

            using (ExcelPackage epck = new ExcelPackage(file.OpenRead()))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var sheetBCChi = epck.Workbook.Worksheets[0];

                var cellRangeToStartIndex = sheetBCChi.Cells["A6"];
                var cellRangeToRangeDate = sheetBCChi.Cells["B1"];
                var cellRangeToTotal = sheetBCChi.Cells["B3"];

                // Đặt tên cho dãy cell
                sheetBCChi.Names.Add("StartRow", cellRangeToStartIndex);
                sheetBCChi.Names.Add("RangeDate", cellRangeToRangeDate);
                sheetBCChi.Names.Add("Total", cellRangeToTotal);


                FillDataForSheetBCTotalChi(dataChi,ref sheetBCChi, currencyDefault?.Name, startDate, endDate);

                using(var stream = new MemoryStream())
                {
                    epck.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
        private void AssignDataForSheetBCChung(
            List<BaoCaoChungDto> data, 
            ref ExcelWorksheet sheetBCChung, 
            GetPeriodDto periodInfo, 
            DateTime startDate, 
            DateTime endDate
        )
        {
            sheetBCChung.AssignInfomationForSheet(periodInfo, startDate, endDate);
            var startRow = sheetBCChung.Names["StartRowBCTongHop"].Start.Row;
            int rowIndex = startRow;
            var length = data.Count;
            for (int idx = 0; idx < length; idx++)
            {
                if(idx == length - 1)
                {
                    sheetBCChung.Cells[rowIndex, 1].Value = data[idx].BranchName;
                    sheetBCChung.Cells[rowIndex, 2].Value = data[idx].TongThu;
                    sheetBCChung.Cells[rowIndex, 3].Value = data[idx].TongThuThuc;            
                    sheetBCChung.Cells[rowIndex, 4].Value = data[idx].TongChi;
                    sheetBCChung.Cells[rowIndex, 5].Value = data[idx].TongChiThuc;                   
                    sheetBCChung.Cells[rowIndex, 6].Value = data[idx].Du;
                    sheetBCChung.Cells[rowIndex, 7].Value = data[idx].DuThuc;
                    sheetBCChung.Cells[rowIndex, 8].Value = data[idx].ChenhLech;
                    sheetBCChung.Row(rowIndex).Style.Font.Bold = true;
                }
                else
                {
                    sheetBCChung.Cells[rowIndex, 1].Value = data[idx].BranchName;
                    sheetBCChung.Cells[rowIndex, 4].Value = data[idx].TongChi;
                    sheetBCChung.Cells[rowIndex, 5].Value = data[idx].TongChiThuc;
                }
                rowIndex++;
            }
            var range = sheetBCChung.Cells[startRow, 1, rowIndex, 8];
            range.SetBorderRangeCells();
        }
        private void AssignDataForSheetBCThu(
            List<BaoCaoThuDto> data, 
            ref ExcelWorksheet sheetBCThu, 
            GetPeriodDto periodInfo, 
            string currencyDefaultName, 
            DateTime startDate, 
            DateTime endDate
        )
        {
            sheetBCThu.AssignInfomationForSheet(periodInfo, startDate, endDate);
            var startRow = sheetBCThu.Names["StartRowBCThu"].Start.Row;
            int rowIndex = startRow;
            int stt = 0;
            double tongThu = 0;
            double tongThuThuc = 0;
            foreach (var item in data)
            {
                sheetBCThu.Cells[rowIndex, 1].Value = ++stt;
                sheetBCThu.Cells[rowIndex, 2].Value = item.Id;
                sheetBCThu.Cells[rowIndex, 3].Value = item.Name;
                sheetBCThu.Cells[rowIndex, 4].Value = item.ClientName;
                sheetBCThu.Cells[rowIndex, 5].Value = item.MonthYear;
                sheetBCThu.Cells[rowIndex, 6].Value = item.TransactionDate.HasValue ? item.TransactionDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                sheetBCThu.Cells[rowIndex, 7].Value = item.Value;
                sheetBCThu.Cells[rowIndex, 8].Value = item.CurrencyName;
                sheetBCThu.Cells[rowIndex, 9].Value = item.ExchangeRate;
                sheetBCThu.Cells[rowIndex, 10].Value = item.TotalVND;
                sheetBCThu.Cells[rowIndex, 11].Value = item.IncomingEntryType;
                sheetBCThu.Cells[rowIndex, 12].Value = item.TinhDoanhThu;

                tongThu += item.TotalVND;
                tongThuThuc += item.IsDoanhThu ? item.TotalVND : 0;

                rowIndex++;
            }
            var range = sheetBCThu.Cells[startRow, 1, rowIndex, 12];
            range.SetBorderRangeCells();

            sheetBCThu.Names["TONG_THU"].Value = Helpers.FormatMoney(tongThu);
            sheetBCThu.Names["TONG_THU_THUC"].Value = Helpers.FormatMoney(tongThuThuc);
            sheetBCThu.Names["THANH_TIEN"].Value = $"Thành tiền ({currencyDefaultName})";
        }
        private void AssignDataForSheetBCChi(
            IEnumerable<GetThongTinRequestChi> data, 
            ref ExcelWorksheet sheetBCChi, 
            GetPeriodDto periodInfo, 
            string currencyDefaultName, 
            DateTime startDate, 
            DateTime endDate
        )
        {
            sheetBCChi.AssignInfomationForSheet(periodInfo, startDate, endDate);
            var startRow = sheetBCChi.Names["StartRowBCChi"].Start.Row;
            int rowIndex = startRow;
            int stt = 0;
            double tongChi = 0;
            double tongChiThuc = 0;
            foreach (var item in data)
            { 
                sheetBCChi.Cells[rowIndex, 1].Value = ++stt;
                sheetBCChi.Cells[rowIndex, 2].Value = item.Id;
                sheetBCChi.Cells[rowIndex, 3].Value = item.BranchName;
                sheetBCChi.Cells[rowIndex, 4].Value = item.Name;
                sheetBCChi.Cells[rowIndex, 5].Value = item.DetailName;
                sheetBCChi.Cells[rowIndex, 6].Value = item.ReportDate.HasValue ? item.ReportDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                sheetBCChi.Cells[rowIndex, 7].Value = item.Total;
                sheetBCChi.Cells[rowIndex, 8].Value = item.CurrencyName;
                sheetBCChi.Cells[rowIndex, 9].Value = item.ExchangeRate;
                sheetBCChi.Cells[rowIndex, 10].Value = item.TotalVND;
                sheetBCChi.Cells[rowIndex, 11].Value = item.OutcomingEntryType;
                sheetBCChi.Cells[rowIndex, 12].Value = item.LaChiPhi;

                tongChi += item.TotalVND;
                tongChiThuc += item.ExpenseType == ExpenseType.REAL_EXPENSE ? item.TotalVND : 0;

                rowIndex++;
            }
            var range = sheetBCChi.Cells[startRow, 1, rowIndex, 12];
            range.SetBorderRangeCells();

            sheetBCChi.Names["TONG_CHI"].Value = Helpers.FormatMoney(tongChi);
            sheetBCChi.Names["TONG_CHI_THUC"].Value = Helpers.FormatMoney(tongChiThuc);
            sheetBCChi.Names["THANH_TIEN"].Value = $"Thành tiền ({currencyDefaultName})";
        }

        private void FillDataForSheetBCTotalChi(
            IEnumerable<GetThongTinRequestChi> data,
            ref ExcelWorksheet sheetBCChi,
            string currencyDefaultName,
            DateTime startDate,
            DateTime endDate)
        {
            var startRow = sheetBCChi.Names["StartRow"].Start.Row;
            int rowIndex = startRow;
            int stt = 0;
            double tongChi = 0;

            foreach (var item in data)
            {
                sheetBCChi.Cells[rowIndex, 1].Value = ++stt;
                sheetBCChi.Cells[rowIndex, 2].Value = item.Id;
                sheetBCChi.Cells[rowIndex, 3].Value = item.BranchName;
                sheetBCChi.Cells[rowIndex, 4].Value = item.Name;
                sheetBCChi.Cells[rowIndex, 5].Value = item.DetailName;
                sheetBCChi.Cells[rowIndex, 6].Value = item.ReportDate.HasValue ? item.ReportDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                sheetBCChi.Cells[rowIndex, 7].Value = item.Total;
                sheetBCChi.Cells[rowIndex, 8].Value = item.CurrencyName;
                sheetBCChi.Cells[rowIndex, 9].Value = item.ExchangeRate;
                sheetBCChi.Cells[rowIndex, 10].Value = item.TotalVND;
                sheetBCChi.Cells[rowIndex, 11].Value = item.OutcomingEntryType;
                sheetBCChi.Cells[rowIndex, 12].Value = item.LaChiPhi;
                tongChi += item.TotalVND;
                rowIndex++;
            }
            if(startRow != rowIndex)
            {
                var range = sheetBCChi.Cells[startRow, 1, (rowIndex - 1), 12];
                range.SetBorderRangeCells();
            }

            sheetBCChi.Names["RangeDate"].Value = String.Format("{0} - {1}",startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"));
            sheetBCChi.Names["Total"].Value = Helpers.FormatMoney(tongChi);
        }

        [HttpGet]
        public async Task<List<BaoCaoChungDto>> GetDataBaoCaoChung(DateTime startDate, DateTime endDate, long branchId, ExpenseType? isExpense)
        {
            return await _dashboardManager.GetDataBaoCaoChung(startDate, endDate, branchId, isExpense);
        }

        [HttpGet]
        public async Task<List<BaoCaoThuDto>> GetDataBaoCaoThu(DateTime startDate, DateTime endDate, bool? isDoanhThu)
        {
            return await _dashboardManager.GetDataBaoCaoThu(startDate, endDate, isDoanhThu);
        }

        [HttpGet]
        public async Task<List<GetThongTinRequestChi>> GetDataBaoCaoChi(DateTime startDate, DateTime endDate, long branchId, ExpenseType? isExpense)
        {
            return (await _dashboardManager.GetAllRequestChiForBaoCao(startDate, endDate, branchId, isExpense)).ToList();
        }

        [HttpGet]
        public async Task<DebtStatisticFromHRMDto> GetHRMDebtStatistic()
        {
            return await _dashboardManager.GetHRMDebtStatistic(AbpSession.TenantId);
        }

        #endregion
    }
}