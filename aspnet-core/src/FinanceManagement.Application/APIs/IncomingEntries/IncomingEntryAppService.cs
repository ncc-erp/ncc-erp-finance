using Abp.Authorization;
using Abp.UI;
using ClosedXML.Excel;
using FinanceManagement.APIs.IncomingEntries.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.IncomingEntries;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using FinanceManagement.Managers.Users;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.IncomingEntries
{
    [AbpAuthorize]
    public class IncomingEntryAppService : FinanceManagementAppServiceBase
    {
        private readonly IMyUserManager _myUserManager;
        private readonly IIncomingEntryManager _incomingEntryManager;
        public IncomingEntryAppService(IMyUserManager myUserManager, IIncomingEntryManager incomingEntryManager, IWorkScope workScope) : base(workScope)
        {
            _myUserManager = myUserManager;
            _incomingEntryManager = incomingEntryManager;
        }

        [HttpPost]
        public async Task<IncomingEntryDto> Create(IncomingEntryDto input)
        {
            var incomingBankTrans = await WorkScope.GetAll<IncomingEntry>().Where(x => x.BankTransactionId == input.BankTransactionId).ToListAsync();
            var totalValue = incomingBankTrans.Sum(x => x.Value) + input.Value;

            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(input.BankTransactionId);
            if (bankTransaction == null)
            {
                throw new UserFriendlyException("Bank transaction doesn't exist");
            }
            if (bankTransaction.ToValue < totalValue)
            {
                throw new UserFriendlyException("Total value Incoming cannot be greater than ToValue BankTransaction");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<IncomingEntry>(input));
            CurrentUnitOfWork.SaveChanges();
            return input;
        }

        [HttpPut]

        public async Task<IncomingEntryDto> Update(IncomingEntryDto input)
        {
            var incomingBankTrans = await WorkScope.GetAll<IncomingEntry>().Where(x => x.BankTransactionId == input.BankTransactionId).ToListAsync();
            var currentBankTrans = await WorkScope.GetAsync<IncomingEntry>(input.Id);
            var totalValue = incomingBankTrans.Sum(x => x.Value);

            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(input.BankTransactionId);
            if (bankTransaction == null)
            {
                throw new UserFriendlyException("Bank transaction doesn't exist");
            }

            var incomingEntry = await WorkScope.GetAsync<IncomingEntry>(input.Id);
            if (incomingEntry == null)
            {
                throw new UserFriendlyException("Incoming entry doesn't exist");
            }
            if (incomingEntry.Status == true)
            {
                throw new UserFriendlyException("Incoming entry is locked");
            }
            if (bankTransaction.ToValue < (totalValue - currentBankTrans.Value + input.Value))
            {
                throw new UserFriendlyException("Total value Incoming cannot be greater than ToValue BankTransaction");
            }

            input.Status = false;

            await WorkScope.UpdateAsync(ObjectMapper.Map<IncomingEntryDto, IncomingEntry>(input, incomingEntry));
            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_IncomingEntry)]
        public async Task<GridResult<IncomingEntryDto>> GetAllPaging(IncomingEntryGridParam input)
        {
            var query = _incomingEntryManager.BuildIncomingQuery().FiltersByIncomingEntryGridParam(input);
            var result = await query.GetGridResult(query, input);
            var creatorUserIds = result.Items.Where(s => s.CreationUserId.HasValue).Select(s => s.CreationUserId.Value);
            var lastModifiedIds = result.Items.Where(s => s.LastModifiedUserId.HasValue).Select(s => s.LastModifiedUserId.Value);
            var dicUsers = await _myUserManager.GetDictionaryUserAudited(creatorUserIds.Union(lastModifiedIds));
            foreach (var item in result.Items)
            {
                item.CreationUser = item.CreationUserId.HasValue ? (dicUsers.ContainsKey(item.CreationUserId.Value) ? dicUsers[item.CreationUserId.Value] : string.Empty) : string.Empty;
                item.LastModifiedUser = item.LastModifiedUserId.HasValue ? (dicUsers.ContainsKey(item.LastModifiedUserId.Value) ? dicUsers[item.LastModifiedUserId.Value] : string.Empty) : string.Empty;
            }
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_IncomingEntry)]
        public async Task<List<GetTotalByCurrencyDto>> GetTotalByCurrency(IncomingEntryGridParam input)
        {
            var query = _incomingEntryManager.BuildIncomingQuery().FiltersByIncomingEntryGridParam(input); ;

            var incomingEntries = await query.FilterByGridParam(input);
            var result = (from q in incomingEntries.AsEnumerable()
                          group q by new { q.CurrencyId, q.CurrencyName, q.CurrencyCode } into g
                          select new GetTotalByCurrencyDto
                          {
                              CurrencyId = g.Key.CurrencyId,
                              CurrencyName = g.Key.CurrencyName,
                              CurrencyCode = g.Key.CurrencyCode,
                              TotalValue = g.Sum(x => x.Value),
                              TotalValueToVND = g.Sum(x => x.ValueToVND)
                          }).ToList();
            return result;
        }

        //private IQueryable<IncomingEntryDto> BuildIncomingQuery()
        //{
        //    var query = (from ie in WorkScope.GetAll<IncomingEntry>().Include(x => x.IncomingEntryType).OrderByDescending(x => x.CreationTime)
        //                 //join cc in WorkScope.GetAll<CurrencyConvert>() on ie.CurrencyId equals cc.CurrencyId into ccs
        //                 //from cc in ccs.DefaultIfEmpty()
        //                 join bt in WorkScope.GetAll<BankTransaction>()
        //                      on ie.BankTransactionId equals bt.Id
        //                 join ba in WorkScope.GetAll<BankAccount>() on bt.FromBankAccountId equals ba.Id
        //                 join a in WorkScope.GetAll<Account>().Include(x => x.AccountType) on ba.AccountId equals a.Id
        //                 select new IncomingEntryDto
        //                 {
        //                     Id = ie.Id,
        //                     IncomingEntryTypeId = ie.IncomingEntryTypeId,
        //                     IncomingEntryTypeName = ie.IncomingEntryType.Name,
        //                     BankTransactionId = ie.BankTransactionId ?? 0,
        //                     Name = ie.Name,
        //                     AccountId = ie.AccountId ?? 0,
        //                     AccountName = ie.Account.Name,
        //                     BranchId = ie.BranchId,
        //                     CurrencyId = ie.CurrencyId ?? ie.BTransactions.BankAccount.CurrencyId,
        //                     CurrencyName = ie.Currency.Code ?? ie.BTransactions.BankAccount.Currency.Name,
        //                     CurrencyCode = ie.Currency.Code ?? ie.BTransactions.BankAccount.Currency.Code,
        //                     BranchName = ie.Branch.Name,
        //                     Status = ie.Status,
        //                     Value = ie.Value,
        //                     //ValueToVND = ie.Value * cc.Value,
        //                     Date = bt.TransactionDate,
        //                     ClientAccountId = a.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT ? ba.AccountId : default,
        //                     ClientName = a.AccountType.Code == Constants.ACCOUNT_TYPE_CLIENT ? ba.Account.Name : null,
        //                     CreationUserId = ie.CreatorUserId,
        //                     CreationTime = ie.CreationTime,
        //                     LastModifiedTime = ie.LastModificationTime,
        //                     LastModifiedUserId = ie.LastModifierUserId,
        //                     RevenueCounted = ie.IncomingEntryType.RevenueCounted
        //                 }).OrderByDescending(x => x.Date);
        //    return query;
        //}

        private IQueryable<CurrencyConvert> IQueryCurrencyConvert()
        {
            return WorkScope.GetAll<CurrencyConvert>().Where(s => s.DateAt < new DateTime(2000, 1, 1));
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_IncomingEntry)]
        public async Task<List<IncomingEntryDto>> GetAll()
        {
            var result = from ice in WorkScope.GetAll<IncomingEntry>()
                         join bt in WorkScope.GetAll<BankTransaction>()
                         on ice.BankTransactionId equals bt.Id
                         join cc in IQueryCurrencyConvert() on ice.CurrencyId equals cc.CurrencyId
                         select new IncomingEntryDto
                         {
                             Id = ice.Id,
                             IncomingEntryTypeId = ice.IncomingEntryTypeId,
                             BankTransactionId = ice.BankTransactionId ?? 0,
                             Name = ice.Name,
                             AccountId = ice.AccountId ?? 0,
                             AccountName = ice.Account.Name,
                             BranchId = ice.BranchId,
                             CurrencyId = ice.CurrencyId,
                             CurrencyName = ice.Currency.Code,
                             BranchName = ice.Branch.Name,
                             Status = ice.Status,
                             Value = ice.Value,
                             ValueToVND = ice.Value * cc.Value,
                         };
            return await result.ToListAsync();
        }

        [HttpDelete]

        public async Task Delete(long id)
        {
            var incomingEntry = await WorkScope.GetAll<IncomingEntry>().FirstOrDefaultAsync(s => s.Id == id);
            if (incomingEntry == null)
            {
                throw new UserFriendlyException("Incoming entry doesn't exist");
            }
            await WorkScope.DeleteAsync<IncomingEntry>(id);
        }

        [HttpGet]

        public async Task<IncomingEntryDto> Get(long id)
        {
            var users = await WorkScope.GetAll<User>().Select(s => new { s.Id, s.Name })
                .ToDictionaryAsync(s => s.Id, s => s.Name);
            return await WorkScope.GetAll<IncomingEntry>().Where(s => s.Id == id).Select(s => new IncomingEntryDto
            {
                Id = s.Id,
                IncomingEntryTypeId = s.IncomingEntryTypeId,
                BankTransactionId = s.BankTransactionId ?? 0,
                Name = s.Name,
                AccountId = s.AccountId ?? 0,
                AccountName = s.Account.Name,
                BranchId = s.BranchId,
                BranchName = s.Branch.Name,
                CurrencyId = s.CurrencyId,
                CurrencyName = s.Currency.Code,
                Status = s.Status,
                Value = s.Value,
                Date = s.BankTransaction.TransactionDate,
                CreationUserId = s.CreatorUserId,
                CreationTime = s.CreationTime,
                CreationUser = s.CreatorUserId.HasValue ? (users.ContainsKey(s.CreatorUserId.Value) ? users[s.CreatorUserId.Value] : string.Empty) : string.Empty,
                LastModifiedTime = s.LastModificationTime,
                LastModifiedUserId = s.LastModifierUserId,
                LastModifiedUser = s.LastModifierUserId.HasValue ? (users.ContainsKey(s.LastModifierUserId.Value) ? users[s.LastModifierUserId.Value] : string.Empty) : string.Empty,
            }).FirstOrDefaultAsync();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry)]
        public async Task<List<IncomingEntryDto>> GetAllByBankTransaction(long bankTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var users = await WorkScope.GetAll<User>().Select(s => new { s.Id, s.Name })
                .ToDictionaryAsync(s => s.Id, s => s.Name);
                return await WorkScope.GetAll<IncomingEntry>().Where(ie => ie.BankTransactionId == bankTransactionId).Select(s => new IncomingEntryDto
                {
                    Id = s.Id,
                    IncomingEntryTypeId = s.IncomingEntryTypeId,
                    IncomingEntryTypeName = s.IncomingEntryType.Name,
                    BankTransactionId = s.BankTransactionId ?? 0,
                    Name = s.Name,
                    AccountId = s.AccountId ?? 0,
                    AccountName = s.Account.Name,
                    BranchId = s.BranchId,
                    BranchName = s.Branch.Name,
                    CurrencyId = s.CurrencyId,
                    CurrencyName = s.Currency.Code,
                    Status = s.Status,
                    Value = s.Value,
                    Date = s.BankTransaction.TransactionDate,
                    RevenueCounted = s.IncomingEntryType.RevenueCounted,
                    CreationUserId = s.CreatorUserId,
                    CreationTime = s.CreationTime,
                    CreationUser = s.CreatorUserId.HasValue ? (users.ContainsKey(s.CreatorUserId.Value) ? users[s.CreatorUserId.Value] : string.Empty) : string.Empty,
                    LastModifiedTime = s.LastModificationTime,
                    LastModifiedUserId = s.LastModifierUserId,
                    LastModifiedUser = s.LastModifierUserId.HasValue ? (users.ContainsKey(s.LastModifierUserId.Value) ? users[s.LastModifierUserId.Value] : string.Empty) : string.Empty,
                }).ToListAsync();
            }
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit)]
        public async Task<UpdateIncomeTypeDto> UpdateIcomeType(UpdateIncomeTypeDto input)
        {
            var income = await WorkScope.GetAll<IncomingEntry>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefaultAsync();

            if(income == default)
            {
                throw new UserFriendlyException($"Can't find incomingEntry with id {input.Id}");
            }

            income.IncomingEntryTypeId = input.IncomeTypeId;

            CurrentUnitOfWork.SaveChanges();

            return input;
        }

        [HttpGet]
        public async Task<List<ClientInfo>> GetAllClient()
        {
            return await _incomingEntryManager.BuildIncomingQuery()
                .Where(s => s.ClientAccountId != default)
                .Select(s => new ClientInfo
                {
                    ClientAccountCode = s.ClientName,
                    ClientAccountId = s.ClientAccountId,
                    ClientAccountName = s.ClientName,
                })
                .Distinct()
                .ToListAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_IncomingEntry_ExportExcel)]
        public async Task<byte[]> ExportExcel(IncomingEntryGridParam input)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var incomeWS = wb.Worksheets.Add("Income");
                    var incomingEntries = await _incomingEntryManager.BuildIncomingQuery()
                        .FiltersByIncomingEntryGridParam(input)
                        .FilterByGridParam(input);
                    var totalIncome = await GetTotalByCurrency(input);
                    int currentRow = 0;
                    foreach (var income in totalIncome)
                    {
                        currentRow++;
                        incomeWS.Cell(currentRow, 1).Value = "Thu " + income.CurrencyName;
                        incomeWS.Cell(currentRow, 2).Value = income.TotalValue.ToString("N0") + " " + income.CurrencyName;
                    }
                    int stt = 0;
                    currentRow = currentRow + 2;
                    incomeWS.Cell(currentRow, 1).Value = "STT";
                    incomeWS.Cell(currentRow, 2).Value = "Tên nguồn thu";
                    incomeWS.Cell(currentRow, 3).Value = "Loại thu";
                    incomeWS.Cell(currentRow, 4).Value = "Tính vào doanh thu";
                    incomeWS.Cell(currentRow, 5).Value = "Số tiền";
                    incomeWS.Cell(currentRow, 6).Value = "Thuộc về công ty";
                    incomeWS.Cell(currentRow, 7).Value = "Chi nhánh";
                    incomeWS.Cell(currentRow, 8).Value = "Khách hàng";
                    incomeWS.Cell(currentRow, 9).Value = "Ngày tạo";
                    foreach (var income in incomingEntries)
                    {
                        currentRow++;
                        stt++;
                        incomeWS.Cell(currentRow, 1).Value = stt;
                        incomeWS.Cell(currentRow, 2).Value = income.Name;
                        incomeWS.Cell(currentRow, 3).Value = income.IncomingEntryTypeName;
                        incomeWS.Cell(currentRow, 4).Value = income.RevenueCounted?"Có":"Không";
                        incomeWS.Cell(currentRow, 5).Value = income.Value.ToString("N0") + " " + income.CurrencyName;
                        incomeWS.Cell(currentRow, 6).Value = income.AccountName;
                        incomeWS.Cell(currentRow, 7).Value = income.BranchName;
                        incomeWS.Cell(currentRow, 8).Value = income.ClientName;
                        incomeWS.Cell(currentRow, 9).Value = income.Date.Date.ToString("dd/MM/yyyy");
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
    }
}
