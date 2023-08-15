using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.APIs.BankAccounts.Dto;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using FinanceManagement.Enums;
using FinanceManagement.APIs.BankTransactions.Dto;
using System.Linq.Dynamic.Core;
using ClosedXML.Excel;
using System.IO;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Managers.Periods.Dtos;
using Abp.Linq.Extensions;
using FinanceManagement.GeneralModels;
using FinanceManagement.Managers.BankAccounts.Dtos;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Helper;
using FinanceManagement.Ncc;

namespace FinanceManagement.APIs.BankAccounts
{
    [AbpAuthorize]
    public class BankAccountAppService : FinanceManagementAppServiceBase
    {
        private readonly IPeriodManager _periodManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPeriodResolveContributor _periodResolveContributor;
        public BankAccountAppService(IWorkScope workScope, 
            IPeriodManager periodManager,
            IPermissionChecker permissionChecker,
            IPeriodResolveContributor periodResolveContributor) : base(workScope)
        {
            _periodManager = periodManager;
            _periodResolveContributor = periodResolveContributor;
            _permissionChecker = permissionChecker;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_Create)]
        public async Task<BankAccountDto> Create(BankAccountDto input)
        {
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<BankAccount>(input));

            var typeAccount = await WorkScope.GetAll<Account>()
                .Where(x => x.Id == input.AccountId)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();
            var isNotExistedPeriod = await _periodManager.IsTheFirstRecord();
            if (typeAccount == AccountTypeEnum.COMPANY && !isNotExistedPeriod)
            {
                await _periodManager.CreatePeriodBankAccount(new CreatePeriodBankAccountDto
                {
                    BankAccountId = input.Id,
                    BaseBalance = input.BaseBalance
                });
            }

            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_Edit)]
        public async Task<BankAccountDto> Update(BankAccountDto input)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(input.Id);
            input.IsActive = bankAccount.IsActive;
            input.BaseBalance = bankAccount.BaseBalance;
            if (bankAccount.LockedStatus)
            {
                throw new UserFriendlyException("BankAccount is Locked !");
            }

            var typeAccount = await WorkScope.GetAll<Account>()
                .Where(x => x.Id == input.AccountId || x.Id == bankAccount.AccountId)
                .Select(x => new
                {
                    x.Id,
                    x.Type
                })
                .ToListAsync();

            var oldTypeAccount = typeAccount.Where(x => x.Id == bankAccount.AccountId)
                .Select(x => x.Type)
                .FirstOrDefault();
            var newTypeAccount = typeAccount.Where(x => x.Id == input.AccountId)
                .Select(x => x.Type)
                .FirstOrDefault();

            if (oldTypeAccount != newTypeAccount && oldTypeAccount == AccountTypeEnum.COMPANY)
                throw new UserFriendlyException("Không thể update loại đối tượng kế COMPANY sang loại khác");

            if (oldTypeAccount != newTypeAccount && newTypeAccount == AccountTypeEnum.COMPANY)
                throw new UserFriendlyException("Không thể update loại đối tượng kế sang COMPANY");

            /*if (oldTypeAccount == AccountTypeEnum.COMPANY && bankAccount.BaseBalance != input.BaseBalance)
            {
                await _periodManager.UpdateBaseBalancePeriodBankAccount(input.Id, input.BaseBalance);
            }*/

            await WorkScope.UpdateAsync(ObjectMapper.Map<BankAccountDto, BankAccount>(input, bankAccount));

            return input;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount, PermissionNames.Finance_ComparativeStatistic)]
        public async Task<GridResult<DetailBankAccountDto>> GetAllPaging(BankAccountGridParam input)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var query = IQGetAllBankAccount()
                .WhereIf(input.Id.HasValue, s => s.Id == input.Id)
                .FiltersByBankAccountGridParam(input);
                return await query.GetGridResult(query, input);
            }
        }
        public IQueryable<DetailBankAccountDto> IQGetAllBankAccount()
        {
            var bankTransactions = WorkScope.GetAll<BankTransaction>();

            var query = from b in WorkScope.GetAll<BankAccount>()
                        select new DetailBankAccountDto
                        {
                            Id = b.Id,
                            HolderName = b.HolderName,
                            BankNumber = b.BankNumber,
                            BankId = b.BankId,
                            BankName = b.Bank.Name,
                            CurrencyId = b.CurrencyId,
                            CurrencyName = !b.CurrencyId.HasValue ? "" : b.Currency.Code,
                            AccountId = b.AccountId,
                            AccountName = b.Account.Name,
                            IsActive = b.IsActive,
                            AccountTypeCode = b.Account.AccountType.Code,
                            AccountTypeId = b.Account.AccountTypeId,
                            AccountTypeEnum = b.Account.Type,
                            Increase =  bankTransactions.Where(x => x.ToBankAccountId == b.Id).Sum(x => x.ToValue),
                            Reduce = bankTransactions.Where(x => x.FromBankAccountId == b.Id).Sum(x => x.FromValue),
                            BaseBalance = b.BaseBalance,
                            LockedStatus = b.LockedStatus
                        };
            return query;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_ActiveDeactive)]
        public async Task ChangeStatus(DetailBankAccountDto input)
        {
            var bankAccount = WorkScope.GetAll<BankAccount>().Where(s => s.Id == input.Id).FirstOrDefault();
            if (bankAccount == default)
                throw new UserFriendlyException("Không tồn tại tài khoản id = " + input.Id);
            if (bankAccount.LockedStatus)
                throw new UserFriendlyException("BankAccount is Locked !");

            bankAccount.IsActive = input.IsActive;
            await WorkScope.UpdateAsync(bankAccount);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_ActiveDeactive)]
        public async Task<string> Active(ActiveBankAccountDto input)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(input.BankAccountId);

            if (bankAccount.Account.Type == AccountTypeEnum.COMPANY)
            {
                var periodBankAccount = await WorkScope.GetAll<PeriodBankAccount>()
                    .FirstOrDefaultAsync(x => x.BankAccountId == bankAccount.Id);

                if (periodBankAccount.IsNullOrDefault())
                {
                    await _periodManager.CreatePeriodBankAccount(new CreatePeriodBankAccountDto
                    {
                        BankAccountId = input.BankAccountId,
                        BaseBalance = input.BaseBalance
                    });
                }
                else
                {
                    periodBankAccount.IsActive = true;
                    periodBankAccount.BaseBalance = input.BaseBalance;
                }
            }

            bankAccount.IsActive = true;
            await CurrentUnitOfWork.SaveChangesAsync();
            return "Active tài khoản thành công!";
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_ActiveDeactive)]
        public async Task<string> DeActive(long bankAccountId)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);

            if (bankAccount.Account.Type == AccountTypeEnum.COMPANY)
            {
                var hasTransaction = await WorkScope.GetAll<BTransaction>()
                    .Where(x => x.BankAccountId == bankAccountId && x.Status == BTransactionStatus.DONE)
                    .AnyAsync();

                if (hasTransaction)
                    throw new UserFriendlyException("Không thể DeActive tài khoản vì đã tồn tại Biến động số dư DONE!");

                var maxItf = await WorkScope.GetAll<Currency>()
                    .Where(x => x.Id == bankAccount.CurrencyId)
                    .Select(x => x.MaxITF)
                    .FirstOrDefaultAsync();

                var periodBankAccountBaseBalance = bankAccount.PeriodBankAccounts.OrderByDescending(x => x.CreationTime).Select(x => x.BaseBalance).FirstOrDefault();

                if (periodBankAccountBaseBalance > maxItf)
                    throw new UserFriendlyException($"Không thể DeActive tài khoản vì số dư đầu kì vượt quá giới hạn {Helpers.FormatMoney(periodBankAccountBaseBalance)} > {Helpers.FormatMoney(maxItf)} (danh mục -> tiền tệ -> Max ITF)");

                var periodBankAccount = await WorkScope.GetAll<PeriodBankAccount>()
                    .FirstOrDefaultAsync(x => x.BankAccountId == bankAccount.Id);

                if (!periodBankAccount.IsNullOrDefault())
                    periodBankAccount.IsActive = false;
            }

            bankAccount.IsActive = false;
            await CurrentUnitOfWork.SaveChangesAsync();
            return "DeActive tài khoản thành công!";
        }
        public async Task<List<DetailBankAccountDto>> GetAll()
        {
            var bankTransactions = WorkScope.GetAll<BankTransaction>();

            var bankAccount = from b in WorkScope.GetAll<BankAccount>()
                              select new DetailBankAccountDto
                              {
                                  Id = b.Id,
                                  HolderName = b.HolderName,
                                  BankNumber = b.BankNumber,
                                  BankId = b.BankId,
                                  CurrencyId = b.CurrencyId,
                                  CurrencyName = !b.CurrencyId.HasValue ? "" : b.Currency.Code,
                                  AccountId = b.AccountId,
                                  IsActive = b.IsActive,
                                  AccountName = b.Account.Name,
                                  AccountTypeCode = b.Account.AccountType.Code,
                                  AccountTypeId = b.Account.AccountTypeId,
                                  BaseBalance = b.BaseBalance,
                                  LockedStatus = b.LockedStatus
                              };
            return await bankAccount.ToListAsync();

        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_Delete)]
        public async Task Delete(long id)
        {
            var bankAccount = await WorkScope.GetAll<BankAccount>().FirstOrDefaultAsync(s => s.Id == id);
            if (bankAccount.LockedStatus)
            {
                throw new UserFriendlyException("BankAccount is Locked !");
            }

            var relatedTransactions = WorkScope.GetAll<BankTransaction>().Any(t => t.FromBankAccountId == bankAccount.Id || t.ToBankAccountId == bankAccount.Id);
            if (relatedTransactions)
            {
                throw new UserFriendlyException("There are transactions linked to this bank account");
            }

            await WorkScope.DeleteAsync<BankAccount>(id);
        }
        [HttpGet]
        public async Task<DetailBankAccountDto> Get(long id)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var bankTransaction = WorkScope.GetAll<BankTransaction>()
                                    .Where(x => x.FromBankAccountId == id || x.ToBankAccountId == id);
                var balance = bankTransaction.Select(x => new
                {
                    ToValue = x.ToBankAccountId == id ? x.ToValue : 0,
                    FromValue = x.FromBankAccountId == id ? x.FromValue : 0
                });

                return await WorkScope.GetAll<BankAccount>().Where(s => s.Id == id).Select(s => new DetailBankAccountDto
                {
                    Id = s.Id,
                    HolderName = s.HolderName,
                    BankNumber = s.BankNumber,
                    BankId = s.BankId,
                    BankName = s.Bank.Name,
                    CurrencyId = s.CurrencyId,
                    IsActive = s.IsActive,
                    CurrencyName = s.Currency.Name,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    Increase = balance.Sum(x => x.ToValue),
                    Reduce = balance.Sum(x => x.FromValue),
                    BaseBalance = s.BaseBalance
                }).FirstOrDefaultAsync();
            }
        }

        [HttpGet]
        public async Task<DetailBankAccountDto> GetByPeriod(long id)
        {
            var bankTransaction = WorkScope.GetAll<BankTransaction>()
                                    .Where(x => x.FromBankAccountId == id || x.ToBankAccountId == id);
            var balance = bankTransaction.Select(x => new
            {
                ToValue = x.ToBankAccountId == id ? x.ToValue : 0,
                FromValue = x.FromBankAccountId == id ? x.FromValue : 0
            });

            var countPeriod = await _periodManager.CountPeriod();

            return await WorkScope.GetAll<BankAccount>().Where(s => s.Id == id)
            .Select(s => new DetailBankAccountDto
            {
                Id = s.Id,
                HolderName = s.HolderName,
                BankNumber = s.BankNumber,
                BankId = s.BankId,
                BankName = s.Bank.Name,
                CurrencyId = s.CurrencyId,
                IsActive = s.IsActive,
                CurrencyName = s.Currency.Name,
                AccountId = s.AccountId,
                AccountName = s.Account.Name,
                Increase = balance.Sum(x => x.ToValue),
                Reduce = balance.Sum(x => x.FromValue),
                IsActiveInPeriod = s.PeriodBankAccounts.OrderByDescending(x => x.Id).Select(s => s.IsActive).FirstOrDefault(),
                BaseBalance = countPeriod > 1 ? s.PeriodBankAccounts.OrderByDescending(x => x.Id).Select(x => x.BaseBalance).FirstOrDefault() : s.BaseBalance
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<BankAccountStatementDto> BankAccountStatement(long bankAccountId, DateTime startDate, DateTime endDate)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);

                var bankTransaction = await WorkScope.GetAll<BankTransaction>().OrderByDescending(x => x.TransactionDate)
                                        .Where(x => x.FromBankAccountId == bankAccountId || x.ToBankAccountId == bankAccountId)
                                        .Where(x => x.TransactionDate.Date >= startDate.Date && x.TransactionDate.Date <= endDate.Date).ToListAsync();

                var bankAccountIdsInBankTransactions = bankTransaction.Select(s => s.FromBankAccountId).Union(bankTransaction.Select(s => s.ToBankAccountId));
                var dicBankAccounts = await WorkScope.GetAll<BankAccount>()
                    .Where(s => bankAccountIdsInBankTransactions.Contains(s.Id))
                    .Select(s => new
                    {
                        s.Id,
                        BankAccountInfo = s.HolderName + " (" + s.Account.Name + ")",
                    })
                    .ToDictionaryAsync(x => x.Id, x => x.BankAccountInfo);

                var beginBankTransaction = await WorkScope.GetAll<BankTransaction>()
                                        .Where(x => x.FromBankAccountId == bankAccountId || x.ToBankAccountId == bankAccountId)
                                        .Where(x => x.TransactionDate.Date < startDate.Date).ToListAsync();

                var beginningBalance = beginBankTransaction.Select(x => new
                {
                    ToValue = x.ToBankAccountId == bankAccountId ? x.ToValue : 0,
                    FromValue = x.FromBankAccountId == bankAccountId ? x.FromValue : 0
                });
                return new BankAccountStatementDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    BeginningBalance = bankAccount.BaseBalance + beginningBalance.Sum(x => x.ToValue) - beginningBalance.Sum(x => x.FromValue),
                    BankTransaction = bankTransaction.Select(x => new GetBankTransaction
                    {
                        Id = x.Id,
                        BankTransactionName = x.Name,
                        Increase = x.ToBankAccountId == bankAccountId ? x.ToValue : 0,
                        Reduce = x.FromBankAccountId == bankAccountId ? x.FromValue : 0,
                        TransactionDate = x.TransactionDate,
                        FromBankAccountInfo = dicBankAccounts.ContainsKey(x.FromBankAccountId) ? dicBankAccounts[x.FromBankAccountId] : string.Empty,
                        ToBankAccountInfo = dicBankAccounts.ContainsKey(x.ToBankAccountId) ? dicBankAccounts[x.ToBankAccountId] : string.Empty
                    }).ToList()
                };
            }

        }
        [HttpGet]
        public async Task<BankAccountStatementDto> BankAccountStatementByPeriod(long bankAccountId, DateTime startDate, DateTime endDate)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);

            var bankTransaction = await WorkScope.GetAll<BankTransaction>().OrderByDescending(x => x.TransactionDate)
                                    .Where(x => x.FromBankAccountId == bankAccountId || x.ToBankAccountId == bankAccountId)
                                    .Where(x => x.TransactionDate.Date >= startDate.Date && x.TransactionDate.Date <= endDate.Date).ToListAsync();

            var bankAccountIdsInBankTransactions = bankTransaction.Select(s => s.FromBankAccountId).Union(bankTransaction.Select(s => s.ToBankAccountId));
            var dicBankAccounts = await WorkScope.GetAll<BankAccount>()
                .Where(s => bankAccountIdsInBankTransactions.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    BankAccountInfo = s.HolderName + " (" + s.Account.Name + ")",
                })
                .ToDictionaryAsync(x => x.Id, x => x.BankAccountInfo);

            var beginBankTransaction = await WorkScope.GetAll<BankTransaction>()
                                    .Where(x => x.FromBankAccountId == bankAccountId || x.ToBankAccountId == bankAccountId)
                                    .Where(x => x.TransactionDate.Date < startDate.Date).ToListAsync();

            var beginningBalance = beginBankTransaction.Select(x => new
            {
                ToValue = x.ToBankAccountId == bankAccountId ? x.ToValue : 0,
                FromValue = x.FromBankAccountId == bankAccountId ? x.FromValue : 0
            });
            var countPeriod = await _periodManager.CountPeriod();

            return new BankAccountStatementDto
            {
                StartDate = startDate,
                EndDate = endDate,
                BeginningBalance = (countPeriod > 1 ? bankAccount.PeriodBankAccounts.OrderByDescending(x => x.Id).Select(x => x.BaseBalance).FirstOrDefault() : bankAccount.BaseBalance) + beginningBalance.Sum(x => x.ToValue) - beginningBalance.Sum(x => x.FromValue),
                BankTransaction = bankTransaction.Select(x => new GetBankTransaction
                {
                    Id = x.Id,
                    BankTransactionName = x.Name,
                    Increase = x.ToBankAccountId == bankAccountId ? x.ToValue : 0,
                    Reduce = x.FromBankAccountId == bankAccountId ? x.FromValue : 0,
                    TransactionDate = x.TransactionDate,
                    FromBankAccountInfo = dicBankAccounts.ContainsKey(x.FromBankAccountId) ? dicBankAccounts[x.FromBankAccountId] : string.Empty,
                    ToBankAccountInfo = dicBankAccounts.ContainsKey(x.ToBankAccountId) ? dicBankAccounts[x.ToBankAccountId] : string.Empty
                }).ToList()
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_LockUnlock)]
        public async Task LockBankAccount(long bankAccountId)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);

            bankAccount.LockedStatus = true;
            await WorkScope.UpdateAsync(bankAccount);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_LockUnlock)]
        public async Task UnlockBankAccount(long bankAccountId)
        {
            var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);

            bankAccount.LockedStatus = false;
            await WorkScope.UpdateAsync(bankAccount);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_Export)]
        public async Task<byte[]> ExportExcel(GridParam input)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var incomeWS = wb.Worksheets.Add("Bank Account");
                    var bankTransactions = WorkScope.GetAll<BankTransaction>();

                    var results = await WorkScope.GetAll<BankAccount>().Select(s => new DetailBankAccountDto
                    {
                        HolderName = s.HolderName,
                        BankNumber = s.BankNumber,
                        BankName = s.Bank.Name,
                        CurrencyName = s.Currency.Name,
                        AccountName = s.Account.Name,
                        Increase =  bankTransactions.Where(x => x.ToBankAccountId == s.Id).Sum(x => x.ToValue),
                        Reduce = bankTransactions.Where(x => x.FromBankAccountId == s.Id).Sum(x => x.FromValue),
                    }).FilterByGridParam(input);
                    int currentRow = 1;
                    int stt = 0;

                    incomeWS.Cell(currentRow, 1).Value = "STT";
                    incomeWS.Cell(currentRow, 2).Value = "Tên tài khoản";
                    incomeWS.Cell(currentRow, 3).Value = "Số tài khoản";
                    incomeWS.Cell(currentRow, 4).Value = "Ngân hàng";
                    incomeWS.Cell(currentRow, 5).Value = "Tiền tệ";
                    incomeWS.Cell(currentRow, 6).Value = "Đối tượng kế toán";
                    incomeWS.Cell(currentRow, 7).Value = "Số tiền";
                    foreach (var outcome in results)
                    {
                        currentRow++;
                        stt++;
                        incomeWS.Cell(currentRow, 1).SetValue(stt);
                        incomeWS.Cell(currentRow, 2).SetValue(outcome.HolderName);
                        incomeWS.Cell(currentRow, 3).SetValue(outcome.BankNumber);
                        incomeWS.Cell(currentRow, 4).SetValue(outcome.BankName);
                        incomeWS.Cell(currentRow, 5).SetValue(outcome.CurrencyName);
                        incomeWS.Cell(currentRow, 6).SetValue(outcome.AccountName);
                        incomeWS.Cell(currentRow, 7).SetValue(outcome.Amount);
                        incomeWS.Cell(currentRow, 7).Style.NumberFormat.Format = "#,###,###";


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
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_BankAccount_Export)]
        public async Task<byte[]> ExportExcelDetail(long bankAccountId, DateTime startDate, DateTime endDate, double Surplus, double firstBalance, double lastBalance)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var incomeWS = wb.Worksheets.Add("Bank Account Detail");
                    var bankTransactionsDetail = await BankAccountStatement(bankAccountId, startDate, endDate);
                    incomeWS.Cell(1, 1).Value = $"Từ ngày:{startDate.Day}/{startDate.Month}/{startDate.Year}";
                    incomeWS.Cell(1, 2).Value = $"đến ngày:{endDate.Day}/{endDate.Month}/{endDate.Year}";
                    incomeWS.Cell(2, 1).Value = $"Số dư đầu tiên:";
                    incomeWS.Cell(2, 2).Value = firstBalance;
                    incomeWS.Cell(2, 2).Style.NumberFormat.Format = "#,###,###";
                    incomeWS.Cell(3, 1).Value = $"Số dư cuối cùng:";
                    incomeWS.Cell(3, 2).Value = lastBalance;
                    incomeWS.Cell(3, 2).Style.NumberFormat.Format = "#,###,###";
                    int currentRow = 4;
                    incomeWS.Cell(currentRow, 1).Value = "Ngày";
                    incomeWS.Cell(currentRow, 2).Value = "Tên giao dịch";
                    incomeWS.Cell(currentRow, 3).Value = "Phát sinh tăng";
                    incomeWS.Cell(currentRow, 4).Value = "Phát sinh giảm";
                    incomeWS.Cell(currentRow, 5).Value = "Số dư sau phát sinh";
                    incomeWS.Cell(currentRow, 6).Value = "Từ tài khoản";
                    incomeWS.Cell(currentRow, 7).Value = "Tới tài khoản";
                    foreach (var outcome in bankTransactionsDetail.BankTransaction)
                    {
                        currentRow++;
                        incomeWS.Cell(currentRow, 1).SetValue(outcome.TransactionDate);
                        incomeWS.Cell(currentRow, 2).SetValue(outcome.BankTransactionName);
                        incomeWS.Cell(currentRow, 3).SetValue(outcome.Increase);
                        incomeWS.Cell(currentRow, 3).Style.NumberFormat.Format = "#,###,###";
                        incomeWS.Cell(currentRow, 4).SetValue(outcome.Reduce);
                        incomeWS.Cell(currentRow, 4).Style.NumberFormat.Format = "#,###,###";
                        incomeWS.Cell(currentRow, 5).SetValue(Surplus);
                        incomeWS.Cell(currentRow, 5).Style.NumberFormat.Format = "#,###,###";
                        incomeWS.Cell(currentRow, 6).SetValue(outcome.FromBankAccountInfo);
                        incomeWS.Cell(currentRow, 7).SetValue(outcome.ToBankAccountInfo);
                        Surplus = Surplus - (outcome.Increase - outcome.Reduce);

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


        public async Task<string> EditBalanace(ActiveBankAccountDto input)
        {
            if (!await _permissionChecker.IsGrantedAsync(PermissionNames.Account_Directory_BankAccount_EditBaseBalanace))
                throw new UserFriendlyException($"Bạn không được cấp quyền");

            int? periodId = _periodResolveContributor.ResolvePeriodId();
            if (!periodId.HasValue)
                throw new UserFriendlyException($"Không có kì!");

            int currentperiodId = await _periodManager.GetCurrentPeriodId();
            if (periodId.Value != currentperiodId)
                throw new UserFriendlyException($"Không thể cập nhật số dư đầu kì của kì Inactive");

            var bankAccount = await WorkScope.GetAsync<BankAccount>(input.BankAccountId);

            if (bankAccount == default)
                throw new UserFriendlyException($"Không tìm thấy tài khoản ngân hàng: {input.BankAccountId}");
            var haveBTransactionsDone = await WorkScope.GetAll<BTransaction>()
                .Where(s => s.BankAccountId == input.BankAccountId)
                .Where(s => s.Status == BTransactionStatus.DONE)
                .AnyAsync();
            if (haveBTransactionsDone)
                throw new UserFriendlyException($"Tài khoản ngân hàng đã phát sinh giao dịch");

            var periodBankAccount = await WorkScope.GetAll<PeriodBankAccount>()
                   .FirstOrDefaultAsync(x => x.BankAccountId == input.BankAccountId);

            if (periodBankAccount.IsNullOrDefault())
            {
                await _periodManager.CreatePeriodBankAccount(new CreatePeriodBankAccountDto
                {
                    BankAccountId = input.BankAccountId,
                    BaseBalance = input.BaseBalance
                });
            }
            else
            {
                periodBankAccount.BaseBalance = input.BaseBalance;
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return "Cập nhật số dư đầu kì thành công!";
        }

    }
}


