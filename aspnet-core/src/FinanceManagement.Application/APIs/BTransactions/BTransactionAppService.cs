using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using FinanceManagement.APIs.Commons.Dtos;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BankAccounts;
using FinanceManagement.Managers.BankAccounts.Dtos;
using FinanceManagement.Managers.BankTransactions;
using FinanceManagement.Managers.BankTransactions.Dtos;
using FinanceManagement.Managers.BTransactions;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.Dashboards;
using FinanceManagement.Managers.IncomingEntries;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.BTransactions
{
    [AbpAuthorize]
    public class BTransactionAppService : FinanceManagementAppServiceBase
    {
        private readonly IBTransactionManager _btransactionManager;
        private readonly IBankTransactionManager _bankTransactionManager;
        private readonly IIncomingEntryManager _incomingEntryManager;
        private readonly IBankAccountManager _bankAccountManager;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        private readonly IPeriodManager _periodManager;
        private readonly IMySettingManager _mySettingManager;
        private readonly IWebHostEnvironment _env;
        public BTransactionAppService(
            IBTransactionManager btransactionManager,
            IBankTransactionManager bankTransactionManager,
            IIncomingEntryManager incomingEntryManager,
            IBankAccountManager bankAccountManager,
            IOutcomingEntryManager outcomingEntryManager,
            PeriodManager periodManager,
            IMySettingManager mySettingManager,
            IWorkScope workScope,
            IWebHostEnvironment env
        ) : base(workScope)
        {
            _btransactionManager = btransactionManager;
            _bankTransactionManager = bankTransactionManager;
            _incomingEntryManager = incomingEntryManager;
            _bankAccountManager = bankAccountManager;
            _outcomingEntryManager = outcomingEntryManager;
            _periodManager = periodManager;
            _mySettingManager = mySettingManager;
            _env = env;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD)]
        public async Task<GridResult<GetAllBTransactionDto>> GetAllPaging(BTransactionGridParam gridParam, long? id)
        {
            var query = _btransactionManager
                .IQGetAllBTransaction()
                .WhereIf(id.HasValue, s => s.BTransactionId == id)
                .FiltersByMoney(gridParam)
                .FiltersByDateTime(gridParam);

            var result = await query.GetGridResult(query, gridParam);
            UpdateAccountNameInfo(result.Items.ToList());

            return result;
        }

        private void UpdateAccountNameInfo(List<GetAllBTransactionDto> BTransactions)
        {
            var bankTransIds = BTransactions.Where(s => s.BankTransactionId != null).Select(s => s.BankTransactionId).ToList();
            var dicBankTransIDToAccountName = (from bat in WorkScope.GetAll<BankTransaction>().Where(s => bankTransIds.Contains(s.Id))
                                               join fba in WorkScope.GetAll<BankAccount>() on bat.FromBankAccountId equals fba.Id
                                               join tba in WorkScope.GetAll<BankAccount>() on bat.ToBankAccountId equals tba.Id
                                               select new
                                               {
                                                   BankTransId = bat.Id,
                                                   FromAccountName = fba.Account.Name,
                                                   ToAccountName = tba.Account.Name
                                               }).AsNoTracking()
                                               .ToDictionary(s => s.BankTransId);

            foreach (var item in BTransactions)
            {
                if (!item.BankTransactionId.HasValue)
                {
                    continue;
                }
                item.FromAccountName = dicBankTransIDToAccountName.ContainsKey(item.BankTransactionId.Value) ? dicBankTransIDToAccountName[item.BankTransactionId.Value].FromAccountName : null;
                item.ToAccountName = dicBankTransIDToAccountName.ContainsKey(item.BankTransactionId.Value) ? dicBankTransIDToAccountName[item.BankTransactionId.Value].ToAccountName : null;
            }
        }

        [HttpPost]
        public async Task<byte[]> ExportBienDongSoDu(BTransactionGridParam gridParam)
        {
            var result = await _btransactionManager
                .IQGetAllBTransaction()
                .FiltersByMoney(gridParam)
                .FiltersByDateTime(gridParam)
                .ApplySearchAndFilter(gridParam)
                .ApplySort(gridParam)
                .ToListAsync();

            UpdateAccountNameInfo(result);

            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "Template_BDSD.xlsx" });
            using (ExcelPackage pck = new ExcelPackage(file.OpenRead()))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var sheet = pck.Workbook.Worksheets[0];
                int startRow = sheet.Names["StartRow"].Start.Row;
                int rowIndex = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    sheet.Cells[rowIndex, 1].Value = ++stt;
                    sheet.Cells[rowIndex, 2].Value = item.BankAccountNumber;
                    sheet.Cells[rowIndex, 3].Value = item.BankAccountName;
                    sheet.Cells[rowIndex, 4].Value = item.StrFromTo;
                    sheet.Cells[rowIndex, 5].Value = item.BankTransactionId.HasValue ? (item.IsShowFromAccountName ? item.FromAccountName : item.ToAccountName) : "";
                    sheet.Cells[rowIndex, 6].Value = $"{(item.BankTransactionId.HasValue ? "#"+item.BankTransactionId : "" )} {item.BankTransactionName}";
                    sheet.Cells[rowIndex, 7].Value = item.Money;
                    sheet.Cells[rowIndex, 8].Value = item.CurrencyName;
                    sheet.Cells[rowIndex, 9].Value = item.TimeAt.ToString("dd/MM/yyyy");
                    sheet.Cells[rowIndex, 10].Value = item.BTransactionStatusName;
                    sheet.Cells[rowIndex, 11].Value = item.CreationTime.ToString("dd/MM/yyyy");
                    sheet.Cells[rowIndex, 12].Value = string.IsNullOrEmpty(item.CreationUser) ? "Trinh" : item.CreationUser;
                    sheet.Cells[rowIndex, 13].Value = item.Note;
                    rowIndex++;
                }
                var range = sheet.Cells[startRow, 1, rowIndex, 13];
                range.SetBorderRangeCells();

                using (var stream = new MemoryStream())
                {
                    pck.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
        [HttpGet]
        public async Task<List<CurrencyNeedConvertDto>> CheckAccount(long btransactionId, long accountId)
        {
            return await _btransactionManager.CheckCurrencyBetweenAccountAndBTrasaction(btransactionId, accountId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_KhachHangThanhToan)]
        public async Task PaymentInvoiceByAccount(PaymentInvoiceForAccountDto input)
        {
            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId);
            if (bTransactionInfo.Money < 0)
                throw new UserFriendlyException("Không link Yêu cầu chi với số tiền < 0");

            if (input.IsCreateBonus)
                await CheckCreateBonus(input, bTransactionInfo);

            var debtIncomingEntryType = await _mySettingManager.GetDebtClientAsync();

            if (debtIncomingEntryType.Id == default) throw new UserFriendlyException("Bạn cần setting khách hàng thanh toán");

            var resultAddClientPaid = await _btransactionManager.AddClientPaid(input.AccountId, input.BTransactionId);

            var clientBankAccount = await WorkScope.GetAll<Account>()
                .Where(s => s.Id == input.AccountId)
                .Select(s => new
                {
                    AccountName = s.Name,
                    BankAccountId = s.BankAccounts.Where(x => x.CurrencyId == resultAddClientPaid.CurrencyId)
                                                 .Select(x => x.Id)
                                                 .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
            long clientBankAccountId = clientBankAccount.BankAccountId;
            if (clientBankAccountId == 0)
            {
                clientBankAccountId = await _bankAccountManager.CreateBankAccount(new CreateBankAccountDto
                {
                    AccountId = input.AccountId,
                    BankNumber = "1",
                    CurrencyId = resultAddClientPaid.CurrencyId,
                    HolderName = clientBankAccount.AccountName + " - " + resultAddClientPaid.CurrencyName
                });
            }

            var bankTransactionId = await _bankTransactionManager.CreateBankTransaction(new CreateBankTransactionDto
            {
                Name = resultAddClientPaid.BankTransactionName,
                BTransactionId = resultAddClientPaid.BTransactionId,
                FromBankAccountId = clientBankAccountId,
                FromValue = resultAddClientPaid.Money,
                ToBankAccountId = resultAddClientPaid.BankAccountId,
                ToValue = resultAddClientPaid.Money,
                TransactionDate = resultAddClientPaid.TimeAt
            });
            if (input.IsCreateBonus)
            {
                await _incomingEntryManager.CreateIncomingEntry(new CreateIncomingEntryDto
                {
                    BankTransactionId = bankTransactionId,
                    BTransactionId = input.BTransactionId,
                    IncomingEntryTypeId = input.IncomingEntryTypeId.Value,
                    Name = input.IncomingEntryName,
                    Value = input.IncomingEntryValue.Value,
                    CurrencyId = bTransactionInfo.CurrencyId
                });
            }
            await _btransactionManager.PaymentInvoiceByAccount(input);
        }
        private async Task CheckCreateBonus(PaymentInvoiceForAccountDto input, LinkBTransactionInfomationDto bTransactionInfo)
        {
            if (input.IncomingEntryName.IsEmpty())
                throw new UserFriendlyException("Vui lòng nhập tên ghi nhận thu");
            if (!input.IncomingEntryTypeId.HasValue)
                throw new UserFriendlyException("Vui lòng chọn loại ghi nhận thu");
            if (!input.IncomingEntryValue.HasValue)
                throw new UserFriendlyException("Vui lòng nhập giá trị ghi nhận thu");
            if (input.IsCreateBonus && bTransactionInfo.Money < input.IncomingEntryValue)
                throw new UserFriendlyException("Số tiền của Bonus không thể > tiền của biến động số dư");
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_Create)]
        public async Task<GetAllBTransactionDto> CreateTransaction(CreateBTransactionDto input)
        {
            return await _btransactionManager.CreateTransaction(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_Edit)]
        public async Task<CreateBTransactionDto> UpdateTransaction(CreateBTransactionDto input)
        {
            return await _btransactionManager.UpdateTransaction(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _btransactionManager.Delete(id);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_Edit)]
        public async Task<CheckCurrencyLinkOutcomToBTransactionDto> CheckCurrencyLinkOutcomingEntryWithBTransaction(long bTransactionId)
        {
            var currencyBTransaction = await WorkScope.GetAll<BTransaction>()
                .Where(s => s.Id == bTransactionId)
                .Select(s => s.BankAccount.CurrencyId)
                .FirstOrDefaultAsync();

            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");

            if (currencyBTransaction != currencyDefault.Id)
                return new CheckCurrencyLinkOutcomToBTransactionDto
                {
                    CurrencyCode = currencyDefault.Code,
                    IsDifferent = true
                };

            return new CheckCurrencyLinkOutcomToBTransactionDto();
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_LinkToRequestChi)]
        public async Task LinkOutcomingEntryWithBTransaction(LinkOutcomingWithBTransactionDto input)
        {
            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId);

            if (bTransactionInfo.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Không thể link Yêu cầu chi với Giao dịch đã Hoàn Thành");
            if (bTransactionInfo.Money > 0)
                throw new UserFriendlyException("Không link Yêu cầu chi với số tiền > 0");

            await _btransactionManager.CheckCreateOutcomingBankTransaction(input.OutcomingEntryId, bTransactionInfo.CurrencyId, bTransactionInfo.Money * input.ExchangeRate);

            var bankTransaction = await WorkScope.GetAll<BankTransaction>()
                .Where(s => s.BTransactionId == input.BTransactionId)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();
            if (bankTransaction != null)
                throw new UserFriendlyException($"Biến động số dư #{input.BTransactionId} đã được link tới GDNH #{bankTransaction.Id} - {bankTransaction.Name}");

            if (await IsAllowOutcomingEntryByMutipleCurrency())
            {
                input.ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
            }
            var toValue = input.ToValue.HasValue ? input.ToValue.Value : Helpers.RoundMoneyToEven(Math.Abs(bTransactionInfo.Money) * input.ExchangeRate);

            var bankTransactionName = "";
            if (input.BankTransactionNameEqualOutcomingName)
            {
                bankTransactionName = (await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId)).Name;
            }
            else
            {
                bankTransactionName = Helpers.GetNameBankTransaction(new InputGetNameBankTransaction
                {
                    BankNumber = bTransactionInfo.BankNumber,
                    CurrencyName = bTransactionInfo.CurrencyName,
                    Money = bTransactionInfo.Money,
                    TimeAt = bTransactionInfo.TimeAt
                });
            }
            var bankTransactionId = await _bankTransactionManager.CreateBankTransaction(new CreateBankTransactionDto
            {
                BTransactionId = input.BTransactionId,
                FromBankAccountId = bTransactionInfo.BankAccountId,
                ToBankAccountId = input.ToBankAccountId,
                FromValue = Math.Abs(bTransactionInfo.Money),
                Name = bankTransactionName,
                ToValue = toValue,
                TransactionDate = bTransactionInfo.TimeAt
            });

            await _btransactionManager.CreateOutcomingBankTransaction(input.OutcomingEntryId, bankTransactionId);

            await _btransactionManager.SetDoneBTransaction(input.BTransactionId);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_ChiLuong)]
        public async Task<bool> CheckLinkOutcomingEntrySalaryWithBTransactions(LinkOutcomingSalaryWithBTransactionsDto input)
        {
            if (input == null)
                throw new UserFriendlyException("INPUT null");
            if (input.BTransactionIds == null || input.BTransactionIds.IsNullOrEmpty())
                throw new UserFriendlyException("Vui lòng chọn biến động số dư");
            if (input.OutcomingEntryId == default)
                throw new UserFriendlyException("Vui lòng chọn Request chi");
            if (input.ToBankAccountId == default)
                throw new UserFriendlyException("Vui lòng chọn tài khoản ngân hàng chi");

            if (!await IsAllowOutcomingEntryByMutipleCurrency())
            {
                await CheckCurrencyLinkOutcomingEntrySalaryWithBTransactions(input);
            }
            else
            {
                await _btransactionManager.CheckCurrencyBTransactionWithOutCome(input.BTransactionIds.FirstOrDefault(), input.OutcomingEntryId);
                await _btransactionManager.CheckCurrencyBTransactionWithBankAccount(input.BTransactionIds.FirstOrDefault(), input.ToBankAccountId);
            }

            var bTransactions = _btransactionManager
                .IQGetAllBTransaction()
                .Where(s => input.BTransactionIds.Contains(s.BTransactionId));

            var isBTransactionsPositive = bTransactions.Where(s => s.MoneyNumber > 0).Any();
            var totalValueBTansaction = bTransactions.Sum(s => s.MoneyNumber);
            if (isBTransactionsPositive)
            {
                throw new UserFriendlyException("Không thể chi lương bằng các biến động số dư dương");
            }

            var outcomgingEntryMoneyInfo = await _outcomingEntryManager.GetOutcomingEntryMoneyInfo(input.OutcomingEntryId);

            if (outcomgingEntryMoneyInfo.Avalible < -totalValueBTansaction)
            {
                throw new UserFriendlyException($"<div class='text-left'>Tổng tiền biến động số dư: <strong>{Helpers.FormatMoney(-totalValueBTansaction)}</strong>" +
                    $"</br>lớn hơn số tiền chi khả dụng là: " +
                    $"<strong> {Helpers.FormatMoney(outcomgingEntryMoneyInfo.Avalible)}</strong> " +
                    $"<i class='fa fa-question-circle' " +
                    $"data-toggle = 'tooltip' data-placement = 'right' title = 'Số tiền cần chi: {Helpers.FormatMoney(outcomgingEntryMoneyInfo.NeedToSpend)}, " +
                    $"số tiền đã chi: {Helpers.FormatMoney(-outcomgingEntryMoneyInfo.Spent)}'></i>" +
                    $"</div>"
                    );
            }

            return true;

        }
        [HttpGet]
        public async Task<BankAccountOption> GetDefaultToBankAccount()
        {
            try
            {
                var bankAccountId = long.Parse(await _mySettingManager.GetOutcomingSalary());

                return await WorkScope.GetAll<BankAccount>()
                    .Where(s => s.Id == bankAccountId)
                    .Select(s => new BankAccountOption
                    {
                        BankAccountId = s.Id,
                        BankAccountHolderName = s.HolderName,
                        AccountId = s.AccountId,
                        AccountName = s.Account.Name,
                        AccountTypeId = s.Account.AccountTypeId,
                        AccountTypeName = s.Account.AccountType.Name,
                        AccountTypeEnum = s.Account.Type,
                        BankAccountCurrencyCode = s.Currency.Code,
                        BankAccountCurrencyId = s.CurrencyId,
                        BankAccountCurrencyName = s.Currency.Name,
                        BankAccountTypeCode = s.Account.AccountType.Code,
                        BankAccountNumber = s.BankNumber
                    }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return default;
            }
        }
        [HttpGet]
        public async Task<long?> GetCurrentOutcomingSalary()
        {
            return await WorkScope.GetAll<OutcomingEntry>()
                .Where(s => s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .Where(s => s.OutcomingEntryType.Code == FinanceManagementConsts.OUTCOMING_ENTRY_TYPE_SALARY)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task CheckCurrencyLinkOutcomingEntrySalaryWithBTransactions(LinkOutcomingSalaryWithBTransactionsDto input)
        {
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");

            var isNotCurrencyDefault = await WorkScope.GetAll<BTransaction>()
                .Where(s => input.BTransactionIds.Contains(s.Id))
                .Select(s => s.BankAccount.CurrencyId)
                .Where(currencyId => currencyId != currencyDefault.Id)
                .AnyAsync();

            if (isNotCurrencyDefault)
            {
                throw new UserFriendlyException($"Không thể chi lương bằng các biến động số dư có loại tiền khác {currencyDefault.Name}");
            }


        }
        [HttpPost]
        public async Task LinkOutcomingEntrySalaryWithBTransactions(LinkOutcomingSalaryWithBTransactionsDto input)
        {
            await CheckLinkOutcomingEntrySalaryWithBTransactions(input);
            foreach (var BtransactionId in input.BTransactionIds)
            {
                await LinkOutcomingEntryWithBTransaction(new LinkOutcomingWithBTransactionDto
                {
                    BTransactionId = BtransactionId,
                    ExchangeRate = input.ExchangeRate,
                    OutcomingEntryId = input.OutcomingEntryId,
                    ToBankAccountId = input.ToBankAccountId
                });
            }
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_CreateIncomingEntry)]
        public async Task<ResultCreateIncoming> CreateIncomingEntry(LinkIncomingEntryDto input)
        {
            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId);
            if (bTransactionInfo.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Không thể link Ghi nhận thu với Giao dịch đã Hoàn Thành");
            if (bTransactionInfo.Money <= 0)
                throw new UserFriendlyException("Không link ghi nhận thu với số tiền <= 0");

            if (await IsAllowOutcomingEntryByMutipleCurrency())
            {
                await _btransactionManager.CheckCurrencyBTransactionWithBankAccount(input.BTransactionId, input.FromBankAccountId);
                var bankAccountInfo = await _bankAccountManager.GetBankAccountInfo(input.FromBankAccountId);
                if (bankAccountInfo.AccountTypeEnum == AccountTypeEnum.COMPANY)
                {
                    throw new UserFriendlyException("Tài khoản ngân hàng phải khác COMPANY");
                }
            }

            var bankTransactionName = "";
            if (input.BankTransactionNameEqualOutcomingName)
            {
                bankTransactionName = input.Name;
            }
            else
            {
                bankTransactionName = Helpers.GetNameBankTransaction(new InputGetNameBankTransaction
                {
                    BankNumber = bTransactionInfo.BankNumber,
                    CurrencyName = bTransactionInfo.CurrencyName,
                    Money = bTransactionInfo.Money,
                    TimeAt = bTransactionInfo.TimeAt
                });
            }

            var bankTransactionId = await _bankTransactionManager.CreateBankTransaction(new CreateBankTransactionDto
            {
                BTransactionId = input.BTransactionId,
                FromBankAccountId = input.FromBankAccountId,
                ToBankAccountId = bTransactionInfo.BankAccountId,
                FromValue = Math.Abs(bTransactionInfo.Money),
                Name = bankTransactionName,
                ToValue = Math.Abs(bTransactionInfo.Money),
                TransactionDate = bTransactionInfo.TimeAt
            });

            var incomingEntryId = await _incomingEntryManager.CreateIncomingEntry(new CreateIncomingEntryDto
            {
                BankTransactionId = bankTransactionId,
                BTransactionId = input.BTransactionId,
                IncomingEntryTypeId = input.IncomingEntryTypeId,
                Name = input.Name,
                Value = bTransactionInfo.Money,
                CurrencyId = bTransactionInfo.CurrencyId
            });

            await _btransactionManager.SetDoneBTransaction(input.BTransactionId);
            return new ResultCreateIncoming
            {
                IncomingEntryId = incomingEntryId,
                BankTransactionId = bankTransactionId
            };

        }
        [HttpPost]
        public Task<DifferentBetweenBankTransAndBTransDto> CheckDifferentBetweenBankTransAndBTrans(LinkBankTransactionToBTransactionDto input)
        {
            return _btransactionManager.CheckDifferentBetweenBankTransAndBTrans(input);
        }
        [HttpPost]
        public async Task LinkBankTransactionToBTransaction(LinkBankTransactionToBTransactionDto input)
        {
            await _btransactionManager.CheckDifferentBetweenBankTransAndBTrans(input);
            await _btransactionManager.LinkBankTransactionToBTransaction(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi)]
        public async Task<object> LinkMultipleOutcomingEntryWithBTransaction(LinkMultipleOutcomingEntryWithBTransactionDto input)
        {
            var outcomgingEntryInfos = await WorkScope.GetAll<OutcomingEntry>()
                .Where(s => input.OutcomingEntryIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Value,
                    s.Name,
                    StatusCode = s.WorkflowStatus.Code,
                    Details = s.OutcomingEntryDetails
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.Total),
                    HasLinkBankTransaction = s.OutcomingEntryBankTransactions.Where(x => !x.IsDeleted).Any(),
                    HasLinkIncomingEntry = s.RelationInOutEntries.Where(x => !x.IsDeleted).Any(),
                })
                .ToListAsync();

            var diffValueAndValueOfDetails = outcomgingEntryInfos.Find(s => s.Details.Any() && s.Value != s.Details.Sum());
            if (diffValueAndValueOfDetails != null)
                return new
                {
                    ErrorMessage = $"#{diffValueAndValueOfDetails.Id} {diffValueAndValueOfDetails.Name} " +
                                    $"có tổng tiền detail {Helpers.FormatMoney(diffValueAndValueOfDetails.Details.Sum())} " +
                                    $"khác {Helpers.FormatMoney(diffValueAndValueOfDetails.Value)}",
                    Success = false,
                    OutcomingEntries = new List<object>() { diffValueAndValueOfDetails },
                };

            var diffApprovedStatus = outcomgingEntryInfos.Where(s => s.StatusCode != FinanceManagementConsts.WORKFLOW_STATUS_APPROVED).Select(s => new { s.Id, s.Name }).ToList();
            if (diffApprovedStatus.Any())
                return new
                {
                    ErrorMessage = $"Không thể link với các request chi khác trạng thái APPROVED: {string.Join(",", diffApprovedStatus.Select(s => s.Id))}",
                    Success = false,
                    OutcomingEntries = diffApprovedStatus,
                };

            var moneyOfOutcomingEntries = outcomgingEntryInfos.Sum(s => s.Value);

            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId);

            if (bTransactionInfo.Status == BTransactionStatus.DONE)
                return new
                {
                    ErrorMessage = "Không thể link request chi đến biến động số dư đã DONE",
                    Success = false
                };

            if (Math.Abs(bTransactionInfo.Money * input.ExchangeRate + moneyOfOutcomingEntries) >= 1)
                return new
                {
                    ErrorMessage = $"Không thể link do chênh lệch tiền BTransaction: {Helpers.FormatMoney(bTransactionInfo.Money * input.ExchangeRate)} và Request chi {Helpers.FormatMoney(moneyOfOutcomingEntries)}",
                    Success = false,
                };

            //TODO::ktra request chi đã link tới giao dịch ngân hàng nào chưa, ghi nhận thu nào chưa
            var linkedOutcomingEntries = outcomgingEntryInfos.Where(s => s.HasLinkBankTransaction || s.HasLinkIncomingEntry)
                .Select(s => new { s.Id, s.Name })
                .ToList();
            if (linkedOutcomingEntries.Any())
                return new
                {
                    ErrorMessage = $"Không thể link Biến động số dự tới Request chi đã có liên kết tới ghi nhận thu hoặc Giao dịch ngân hàng",
                    Success = false,
                    OutcomingEntries = linkedOutcomingEntries,
                };
            //tạo bank transaction từ BTransaction
            var bankTransactionId = await _bankTransactionManager.CreateBankTransaction(new CreateBankTransactionDto
            {
                BTransactionId = input.BTransactionId,
                FromBankAccountId = bTransactionInfo.BankAccountId,
                ToBankAccountId = input.ToBankAccountId,
                FromValue = Math.Abs(bTransactionInfo.Money),
                ToValue = Math.Abs(Helpers.RoundMoneyToEven(bTransactionInfo.Money * input.ExchangeRate)),
                TransactionDate = bTransactionInfo.TimeAt,
                Name = Helpers.GetNameBankTransaction(new InputGetNameBankTransaction
                {
                    TimeAt = bTransactionInfo.TimeAt,
                    BankNumber = bTransactionInfo.BankNumber,
                    CurrencyName = bTransactionInfo.CurrencyName,
                    Money = bTransactionInfo.Money,
                })
            });

            //tạo liên kết bank transaction với request chi
            foreach (var outcomingEntryId in input.OutcomingEntryIds)
            {
                await _btransactionManager.CreateOutcomingBankTransaction(outcomingEntryId, bankTransactionId);
                await _outcomingEntryManager.SetDoneOutcomingEntry(new SetDoneOutcomingEntryDto
                {
                    OutcomingEntryId = outcomingEntryId,
                    ExcutedTime = bTransactionInfo.TimeAt,
                });
            }

            await _btransactionManager.SetDoneBTransaction(input.BTransactionId);

            return new
            {
                Success = true,
                BankTransactionId = bankTransactionId,
            };
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Finance_BĐSD_Import)]
        public async Task<object> ImportBTransaction([FromForm] ImportBTransactionDto input)
        {
            return await _btransactionManager.ImportBTransaction(input);
        }
        [HttpGet]
        public async Task<GetInfoRollbackOutcomingEntryWithBTransactionDto> GetInfoRollbankOutcomingEntryWithBTransaction(long bTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _btransactionManager.GetInfoRollbankOutcomingEntryWithBTransaction(bTransactionId);
            }
        }
        [HttpGet]
        public async Task RollBackOutcomingEntryWithBTransaction(long bTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var currentPeriodId = await _periodManager.GetCurrentPeriodId();
                await _btransactionManager.RollBackOutcomingEntryWithBTransaction(bTransactionId, currentPeriodId);
            }
        }
        [HttpPost]
        public async Task<bool> CheckConversionTransaction(ConversionTransactionDto conversionTransactionDto)
        {
            return await _btransactionManager.CheckConversionTransaction(conversionTransactionDto);
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Finance_BĐSD_BanNgoaiTe)]
        public async Task ConversionTransaction(ConversionTransactionDto conversionTransactionDto)
        {
            await _btransactionManager.CheckConversionTransaction(conversionTransactionDto);
            var bTransactions = WorkScope.GetAll<BTransaction>()
                .Where(s => conversionTransactionDto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    CurrencyCode = s.BankAccount.Currency.Code,
                    CurrencyName = s.BankAccount.Currency.Name

                });

            var incomingEntryTypeId = WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.Id == conversionTransactionDto.IncomingEntryTypeId)
                .Select(s => s.Id)
                .FirstOrDefault();

            var bTransactionMinus = bTransactions.Where(s => conversionTransactionDto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var bTransactionPlus = bTransactions.Where(s => conversionTransactionDto.PlusBTransactionIds.Contains(s.Id)).ToList();

            var totalBTransactionMinus = bTransactionMinus.Sum(s => s.Money);
            var totalBTransactionPlus = bTransactionPlus.Sum(s => s.Money);

            var exchangeRate = Math.Abs(totalBTransactionPlus / totalBTransactionMinus);

            foreach (var bTransaction in bTransactionMinus)
            {
                await LinkOutcomingEntryWithBTransaction(new LinkOutcomingWithBTransactionDto
                {
                    BTransactionId = bTransaction.Id,
                    ExchangeRate = exchangeRate,
                    OutcomingEntryId = conversionTransactionDto.OutcomingEntryId.Value,
                    ToBankAccountId = conversionTransactionDto.ToBankAccountId.Value,
                });
            }

            foreach (var bTransaction in bTransactionPlus)
            {
                // Tạo tên ghi nhận thu
                var incomingName = string.Format(FinanceManagementConsts.TEN_GHI_NHAN_THU_WHEN_BAN_NGOAI_TE, Helpers.FormatMoney(Math.Abs(totalBTransactionMinus)), bTransactionMinus[0].CurrencyName, Helpers.FormatMoney4PartAfterDot(Math.Round(exchangeRate, 4)));

                var resultCreateIncoming = await CreateIncomingEntry(new LinkIncomingEntryDto
                {
                    BTransactionId = bTransaction.Id,
                    FromBankAccountId = conversionTransactionDto.FromBankAccountId.Value,
                    Name = incomingName,
                    IncomingEntryTypeId = incomingEntryTypeId
                });
                await WorkScope.InsertAndGetIdAsync(new RelationInOutEntry
                {
                    IncomingEntryId = resultCreateIncoming.IncomingEntryId,
                    OutcomingEntryId = conversionTransactionDto.OutcomingEntryId.Value,
                });
            }


        }
        [HttpPost]
        public async Task<bool> CheckMuaNgoaiTe(ConversionTransactionDto conversionTransactionDto)
        {
            return await _btransactionManager.CheckMuaNgoaiTe(conversionTransactionDto);
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Finance_BĐSD_MuaNgoaiTe)]
        public async Task MuaNgoaiTe(ConversionTransactionDto conversionTransactionDto)
        {
            await _btransactionManager.CheckMuaNgoaiTe(conversionTransactionDto);
            var bTransactions = WorkScope.GetAll<BTransaction>()
                .Where(s => conversionTransactionDto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    CurrencyCode = s.BankAccount.Currency.Code,
                    CurrencyName = s.BankAccount.Currency.Name

                });

            var incomingEntryTypeId = WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.Id == conversionTransactionDto.IncomingEntryTypeId)
                .Select(s => s.Id)
                .FirstOrDefault();

            var bTransactionMinus = bTransactions.Where(s => conversionTransactionDto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var bTransactionPlus = bTransactions.Where(s => conversionTransactionDto.PlusBTransactionIds.Contains(s.Id)).ToList();

            var totalBTransactionMinus = bTransactionMinus.Sum(s => s.Money);
            var totalBTransactionPlus = bTransactionPlus.Sum(s => s.Money);

            var exchangeRate = Math.Abs(totalBTransactionMinus / totalBTransactionPlus);

            foreach (var bTransaction in bTransactionMinus)
            {
                await LinkOutcomingEntryWithBTransaction(new LinkOutcomingWithBTransactionDto
                {
                    BTransactionId = bTransaction.Id,
                    OutcomingEntryId = conversionTransactionDto.OutcomingEntryId.Value,
                    ToBankAccountId = conversionTransactionDto.ToBankAccountId.Value,
                });
            }

            foreach (var bTransaction in bTransactionPlus)
            {
                // Tạo tên ghi nhận thu
                var incomingName = string.Format(FinanceManagementConsts.TEN_GHI_NHAN_THU_KHI_MUA_NGOAI_TE, Helpers.FormatMoney(Math.Abs(totalBTransactionPlus)), bTransactionPlus[0].CurrencyName, Helpers.FormatMoney4PartAfterDot(Math.Round(exchangeRate, 4)));

                var resultCreateIncoming = await CreateIncomingEntry(new LinkIncomingEntryDto
                {
                    BTransactionId = bTransaction.Id,
                    FromBankAccountId = conversionTransactionDto.FromBankAccountId.Value,
                    Name = incomingName,
                    IncomingEntryTypeId = incomingEntryTypeId
                });
                await WorkScope.InsertAndGetIdAsync(new RelationInOutEntry
                {
                    IncomingEntryId = resultCreateIncoming.IncomingEntryId,
                    OutcomingEntryId = conversionTransactionDto.OutcomingEntryId.Value,
                });
            }
        }

        [HttpPost]
        public async Task<bool> CheckCreateMultiIncomingEntry(LinkMultiIncomingEntryDto input)
        {
            if (input.IncomingEntries == null || input.IncomingEntries.IsEmpty())
                throw new UserFriendlyException("Vui lòng thêm ghi nhận thu");
            if (input.IncomingEntries.Where(s => s.Value <= 0).Any())
                throw new UserFriendlyException("Không thể tạo ghi nhận thu có số tiền <= 0");
            if (!input.BTransactionId.HasValue)
                throw new UserFriendlyException("Vui lòng chọn biến động số dư");
            if (!input.FromBankAccountId.HasValue)
                throw new UserFriendlyException("Vui lòng chọn BANK gửi");

            var fromBankAccount = await _bankAccountManager.GetBankAccountInfo(input.FromBankAccountId.Value);

            if (fromBankAccount == default)
                throw new UserFriendlyException($"Không tồn tại Bank Id = {input.FromBankAccountId}");

            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId.Value);
            if (bTransactionInfo.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Không thể tạo ghi nhận thu với biến động số dư hoàn thành");
            if (bTransactionInfo.Money <= 0)
                throw new UserFriendlyException("Không link ghi nhận thu với biến động số dư có số tiền <= 0");

            if (fromBankAccount.CurrencyId != bTransactionInfo.CurrencyId)
                throw new UserFriendlyException($"Bank gửi có loại tiền khác biến động số dư");


            if (input.IncomingTotalMoney != bTransactionInfo.Money)
                throw new UserFriendlyException($"Tổng tiền của các ghi nhận thu KHÁC tiền của biến động số dư");


            return true;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry)]
        public async Task<ResultCreateMultiIncoming> CreateMultiIncomingEntry(LinkMultiIncomingEntryDto input)
        {
            await CheckCreateMultiIncomingEntry(input);
            var bTransactionInfo = await _btransactionManager.GetBTransactionInformation(input.BTransactionId.Value);
            var bankTransactionId = await _bankTransactionManager.CreateBankTransaction(new CreateBankTransactionDto
            {
                BTransactionId = input.BTransactionId,
                FromBankAccountId = input.FromBankAccountId.Value,
                ToBankAccountId = bTransactionInfo.BankAccountId,
                FromValue = Math.Abs(bTransactionInfo.Money),
                Name = Helpers.GetNameBankTransaction(new InputGetNameBankTransaction
                {
                    BankNumber = bTransactionInfo.BankNumber,
                    CurrencyName = bTransactionInfo.CurrencyName,
                    Money = bTransactionInfo.Money,
                    TimeAt = bTransactionInfo.TimeAt
                }),
                ToValue = Math.Abs(bTransactionInfo.Money),
                TransactionDate = bTransactionInfo.TimeAt
            });
            var incomingEntryIds = new List<long>();
            foreach (var incomingEntry in input.IncomingEntries)
            {
                incomingEntryIds.Add(
                    await _incomingEntryManager.CreateIncomingEntry(new CreateIncomingEntryDto
                    {
                        BankTransactionId = bankTransactionId,
                        BTransactionId = input.BTransactionId,
                        IncomingEntryTypeId = incomingEntry.IncomingEntryTypeId,
                        Name = incomingEntry.Name,
                        Value = incomingEntry.Value,
                        CurrencyId = bTransactionInfo.CurrencyId
                    }));
            }

            await _btransactionManager.SetDoneBTransaction(input.BTransactionId.Value);

            return new ResultCreateMultiIncoming
            {
                IncomingEntryIds = incomingEntryIds,
                BankTransactionId = bankTransactionId
            };

        }

        [HttpPost]
        public async Task<bool> CheckChiChuyenDoi(ChiChuyenDoiDto chiChuyenDoiDto)
        {
            return await _btransactionManager.CheckChiChuyenDoi(chiChuyenDoiDto);
        }
        [HttpPost]
        public async Task ChiChuyenDoi(ChiChuyenDoiDto chiChuyenDoiDto)
        {
            await _btransactionManager.CheckChiChuyenDoi(chiChuyenDoiDto);
            var bTransactions = WorkScope.GetAll<BTransaction>()
                .Where(s => chiChuyenDoiDto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    CurrencyCode = s.BankAccount.Currency.Code,
                    CurrencyName = s.BankAccount.Currency.Name

                });

            var incomingEntryTypeId = WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.Id == chiChuyenDoiDto.IncomingEntryTypeId)
                .Select(s => s.Id)
                .FirstOrDefault();

            var bTransactionMinus = bTransactions.Where(s => chiChuyenDoiDto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var bTransactionPlus = bTransactions.Where(s => chiChuyenDoiDto.PlusBTransactionIds.Contains(s.Id)).ToList();

            foreach (var bTransaction in bTransactionMinus)
            {
                await LinkOutcomingEntryWithBTransaction(new LinkOutcomingWithBTransactionDto
                {
                    BTransactionId = bTransaction.Id,
                    OutcomingEntryId = chiChuyenDoiDto.OutcomingEntryId.Value,
                    ToBankAccountId = chiChuyenDoiDto.ToBankAccountId.Value,
                    BankTransactionNameEqualOutcomingName = true,
                });
            }

            foreach (var bTransaction in bTransactionPlus)
            {
                // Tạo tên ghi nhận thu
                //var incomingName = string.Format(FinanceManagementConsts.TEN_GHI_NHAN_THU_KHI_MUA_NGOAI_TE, Helpers.FormatMoney(Math.Abs(totalBTransactionPlus)), bTransactionPlus[0].CurrencyName, Helpers.FormatMoney4PartAfterDot(Math.Round(exchangeRate, 4)));

                var resultCreateIncoming = await CreateIncomingEntry(new LinkIncomingEntryDto
                {
                    BTransactionId = bTransaction.Id,
                    FromBankAccountId = chiChuyenDoiDto.FromBankAccountId.Value,
                    Name = chiChuyenDoiDto.InComingEntryName,
                    IncomingEntryTypeId = incomingEntryTypeId,
                    BankTransactionNameEqualOutcomingName = true,
                });
                await WorkScope.InsertAndGetIdAsync(new RelationInOutEntry
                {
                    IncomingEntryId = resultCreateIncoming.IncomingEntryId,
                    OutcomingEntryId = chiChuyenDoiDto.OutcomingEntryId.Value,
                });
            }
        }

        [HttpGet]
        public async Task<GetInfoRollbackClientPaidDto> GetInfoRollbackClientPaid(long bTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _btransactionManager.GetInfoRollbackClientPaid(bTransactionId);
            }
        }

        [HttpGet]
        public async Task RollbackClientPaid(long bTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                await _btransactionManager.RollbackClientPaid(bTransactionId);
            }
        }
    }
}
