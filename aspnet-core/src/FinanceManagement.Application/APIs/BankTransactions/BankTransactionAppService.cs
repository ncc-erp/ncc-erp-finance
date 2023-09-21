using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ClosedXML.Excel;
using FinanceManagement.APIs.BankTransactions.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BankTransactions.Dtos;
using FinanceManagement.Managers.Users;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.BankTransactions
{
    public class BankTransactionAppService : FinanceManagementAppServiceBase
    {
        private readonly IMyUserManager _myUserManager;
        private readonly IWebHostEnvironment _env;
        public BankTransactionAppService(IMyUserManager myUserManager, IWorkScope workScope, IWebHostEnvironment env) : base(workScope)
        {
            _myUserManager = myUserManager;
            _env = env;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_Create)]
        public async Task<BankTransactionDto> Create(BankTransactionDto input)
        {
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<BankTransaction>(input));

            var bankFromValue = await WorkScope.GetAsync<BankAccount>(input.FromBankAccountId);
            bankFromValue.Amount = bankFromValue.Amount - input.FromValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

            var bankToValue = await WorkScope.GetAsync<BankAccount>(input.ToBankAccountId);
            bankToValue.Amount = bankToValue.Amount + input.ToValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);

            if (input.Fee > 0)
            {
                var toAccount = await WorkScope.GetAsync<Account>(bankToValue.AccountId);
                var toAccountType = await WorkScope.GetAsync<AccountType>(toAccount.AccountTypeId);
                var toBankFEE = toAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                var fromAccount = await WorkScope.GetAsync<Account>(bankFromValue.AccountId);
                var FromAccountType = await WorkScope.GetAsync<AccountType>(fromAccount.AccountTypeId);
                var fromBankFEE = FromAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                var bankAccountFEE = await WorkScope.GetAll<BankAccount>().Where(x => x.HolderName == Constants.BANKACCOUNT_TRANSFER_FEE).FirstOrDefaultAsync();

                if ((fromBankFEE && !toBankFEE) || (fromBankFEE && toBankFEE))
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.FromBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate,
                    };
                    input.Id = await WorkScope.InsertAndGetIdAsync(transferFee);

                    bankFromValue.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(bankFromValue);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
                else if (toBankFEE && !fromBankFEE)
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.ToBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate
                    };
                    input.Id = await WorkScope.InsertAndGetIdAsync(transferFee);

                    bankToValue.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(bankToValue);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
            }

            if (!input.InvoiceBankTransactions.IsEmpty() && input.InvoiceBankTransactions.Count >= 1)
            {
                var fromAccount = await WorkScope.GetAsync<Account>(bankFromValue.AccountId);
                var FromAccountType = await WorkScope.GetAsync<AccountType>(fromAccount.AccountTypeId);
                if (FromAccountType.Code == Constants.ACCOUNT_TYPE_CLIENT)
                {
                    foreach (var i in input.InvoiceBankTransactions)
                    {
                        var x = new InvoiceBankTransaction
                        {
                            InvoiceId = i.InvoiceId,
                            BankTransactionId = input.Id,
                            PaymentAmount = i.PaymentAmount
                        };
                        i.Id = await WorkScope.InsertAndGetIdAsync(x);
                    }
                }

            }

            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_Edit, PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit)]
        public async Task<BankTransactionDto> Edit(BankTransactionDto input)
        {
            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(input.Id);
            if (bankTransaction.LockedStatus)
            {
                throw new UserFriendlyException("Bank Transaction is Locked !");
            }
            var frombank = await WorkScope.GetAsync<BankAccount>(input.FromBankAccountId);
            var tobank = await WorkScope.GetAsync<BankAccount>(input.ToBankAccountId);

            var bankAccountFEE = await WorkScope.GetAll<BankAccount>().Where(x => x.HolderName == Constants.BANKACCOUNT_TRANSFER_FEE).FirstOrDefaultAsync();

            var transferFeeRefund = await WorkScope.GetAll<BankTransaction>()
                                            .Where(x => x.Name == $"Tính phí giao dịch của giao dịch: {bankTransaction.Id}-{bankTransaction.Name}")
                                            .FirstOrDefaultAsync();
            if (transferFeeRefund != null)
            {
                await Delete(transferFeeRefund.Id);
            }
            if (input.Fee > 0)
            {
                var toAccount = await WorkScope.GetAsync<Account>(tobank.AccountId);
                var toAccountType = await WorkScope.GetAsync<AccountType>(toAccount.AccountTypeId);
                var toBankFEE = toAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                var fromAccount = await WorkScope.GetAsync<Account>(frombank.AccountId);
                var FromAccountType = await WorkScope.GetAsync<AccountType>(fromAccount.AccountTypeId);
                var fromBankFEE = FromAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                if ((fromBankFEE && !toBankFEE) || (fromBankFEE && toBankFEE))
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.FromBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate,
                    };
                    await WorkScope.InsertAndGetIdAsync(transferFee);

                    frombank.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(frombank);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
                else if (toBankFEE && !fromBankFEE)
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.ToBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate
                    };
                    await WorkScope.InsertAndGetIdAsync(transferFee);

                    tobank.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(tobank);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
            }

            if (bankTransaction.FromBankAccountId != input.FromBankAccountId || bankTransaction.ToBankAccountId != input.ToBankAccountId
                || bankTransaction.FromValue != input.FromValue || bankTransaction.ToValue != input.ToValue)
            {
                if (bankTransaction.FromBankAccountId > 0)
                {
                    var fromBankRefund = await WorkScope.GetAsync<BankAccount>(bankTransaction.FromBankAccountId);
                    fromBankRefund.Amount = fromBankRefund.Amount + bankTransaction.FromValue;
                    await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(fromBankRefund);
                }

                if (bankTransaction.ToBankAccountId > 0)
                {
                    var toBankRefund = await WorkScope.GetAsync<BankAccount>(bankTransaction.ToBankAccountId);
                    toBankRefund.Amount = toBankRefund.Amount - bankTransaction.ToValue;
                    await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(toBankRefund);
                }


                frombank.Amount = frombank.Amount - input.FromValue;
                await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(frombank);

                tobank.Amount = tobank.Amount + input.ToValue;
                await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(tobank);
            }

            await WorkScope.UpdateAsync(ObjectMapper.Map<BankTransactionDto, BankTransaction>(input, bankTransaction));
            return input;
        }

        private IQueryable<DetailBankTransactionDto> GetAllTransactions()
        {
            var bankAccounts = WorkScope.GetAll<BankAccount>();
            var incomingEntries = WorkScope.GetAll<IncomingEntry>();
            var transactionsInfo = WorkScope.GetAll<BankTransaction>().Select(s => new
            {
                id = s.Id,
                name = s.Name,
                fromBankAccount = bankAccounts.FirstOrDefault(ba => ba.Id == s.FromBankAccountId),
                toBankAccount = bankAccounts.FirstOrDefault(ba => ba.Id == s.ToBankAccountId),
                fromValue = s.FromValue,
                toValue = s.ToValue,
                fee = s.Fee,
                transactionDate = s.TransactionDate,
                note = s.Note,
                numberOfIncomingEntries = incomingEntries.Count(ie => ie.BankTransactionId == s.Id),
                createDate = s.CreationTime,
                LockedStatus = s.LockedStatus,
                BTransaction = s.BTransaction,
                CreationTime = s.CreationTime,
                CreationUserId = s.CreatorUserId,
                LastModifiedTime = s.LastModificationTime,
                LastModifiedUserId = s.LastModifierUserId,
            });

            var query = transactionsInfo.Select(t => new DetailBankTransactionDto
            {
                Id = t.id,
                Name = t.name,
                FromBankAccountId = t.fromBankAccount.Id,
                FromBankAccountName = t.fromBankAccount.HolderName,
                FromBankAccountCurrency = t.fromBankAccount.Currency.Code,
                FromBankAccountCurrencyId = t.fromBankAccount.CurrencyId,
                FromBankAccountTypeCode = t.fromBankAccount.Account.AccountType.Code,
                FromBankAccountTypeEnum = t.fromBankAccount.Account.Type,
                FromBankAccountNumber = t.fromBankAccount.BankNumber,
                ToBankAccountId = t.toBankAccount.Id,
                ToBankAccountCurrency = t.toBankAccount.Currency.Code,
                ToBankAccountCurrencyId = t.toBankAccount.CurrencyId,
                ToBankAccountName = t.toBankAccount.HolderName,
                ToBankAccountTypeCode = t.toBankAccount.Account.AccountType.Code,
                ToBankAccountTypeEnum = t.toBankAccount.Account.Type,
                ToBankAccountNumber = t.toBankAccount.BankNumber,
                Fee = t.fee,
                FromValue = t.fromValue,
                ToValue = t.toValue,
                TransactionDate = t.transactionDate.Date,
                Note = t.note,
                NumberOfIncomingEntries = t.numberOfIncomingEntries,
                CreateDate = t.createDate.Date,
                LockedStatus = t.LockedStatus,
                CreationTime = t.CreationTime,
                CreationUserId = t.CreationUserId,
                LastModifiedTime = t.LastModifiedTime,
                LastModifiedUserId = t.LastModifiedUserId,
                BTransactionId = t.BTransaction.Id,
                BTransactionBankNumber = t.BTransaction.BankAccount.BankNumber,
                BTransactionCurrencyId = t.BTransaction.BankAccount.CurrencyId,
                BTransactionCurrencyName = t.BTransaction.BankAccount.Currency.Name,
                BTransactionMoneyNumber = t.BTransaction.Money,
                BTransactionTimeAt = t.BTransaction.TimeAt
            });
            return query;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction)]
        public async Task<GridResult<DetailBankTransactionDto>> GetAllPaging(GetAllPagingBankTransactionDto input)
        {
            var query = GetAllTransactions()
                .WhereIf(input.Id.HasValue, s => s.Id == input.Id.Value)
                .FiltersByGetAllPagingBankTransactionDto(input);

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
        public async Task<List<DetailBankTransactionDto>> GetAll()
        {
            var query = GetAllTransactions();
            return await query.ToListAsync();
        }
        public async Task<List<BankAccountOption>> GetAllFromBankAccountInTransaction()
        {
            return await GetAllTransactions()
                .Where(s => s.FromBankAccountId.HasValue)
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.FromBankAccountId.Value,
                    BankAccountHolderName = s.FromBankAccountName,
                    AccountId = s.FromBankAccountId,
                    AccountName = s.FromBankAccountName,
                    AccountTypeName = s.FromBankAccountTypeCode,
                    AccountTypeEnum = s.FromBankAccountTypeEnum,
                    BankAccountCurrencyCode = s.FromBankAccountCurrency,
                    BankAccountCurrencyId = s.FromBankAccountCurrencyId,
                    BankAccountCurrencyName = s.FromBankAccountCurrency,
                    BankAccountTypeCode = s.FromBankAccountTypeCode,
                    BankAccountNumber = s.FromBankAccountNumber
                }).Distinct()
                .OrderBy(s => Math.Abs(s.AccountTypeEnum - Enums.AccountTypeEnum.COMPANY))
                .ToListAsync();
        }
        public async Task<List<BankAccountOption>> GetAllToBankAccountInTransaction()
        {
            return await GetAllTransactions()
                .Where(s => s.ToBankAccountId.HasValue)
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.ToBankAccountId.Value,
                    BankAccountHolderName = s.ToBankAccountName,
                    AccountId = s.ToBankAccountId,
                    AccountName = s.ToBankAccountName,
                    AccountTypeName = s.ToBankAccountTypeCode,
                    AccountTypeEnum = s.ToBankAccountTypeEnum,
                    BankAccountCurrencyCode = s.ToBankAccountCurrency,
                    BankAccountCurrencyId = s.ToBankAccountCurrencyId,
                    BankAccountCurrencyName = s.ToBankAccountCurrency,
                    BankAccountTypeCode = s.ToBankAccountTypeCode,
                    BankAccountNumber = s.ToBankAccountNumber
                }).Distinct()
                .OrderBy(s => Math.Abs(s.AccountTypeEnum - Enums.AccountTypeEnum.COMPANY))
                .ToListAsync();
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_Delete)]
        public async Task Delete(long id)
        {
            var transaction = await WorkScope.GetAll<BankTransaction>().FirstOrDefaultAsync(s => s.Id == id);
            if (transaction.LockedStatus)
            {
                throw new UserFriendlyException("Bank Transaction is Locked !");
            }

            var hasIncomingEntries = await WorkScope.GetAll<IncomingEntry>().AnyAsync(ie => ie.BankTransactionId == id);
            if (hasIncomingEntries)
            {
                throw new UserFriendlyException("Can not delete Bank transaction when you have linked Incoming entry");
            }

            var hasOutcomingEntryLink = await WorkScope.GetAll<OutcomingEntryBankTransaction>().AnyAsync(oebt => oebt.BankTransactionId == id);
            if (hasOutcomingEntryLink)
            {
                throw new UserFriendlyException("Can not delete Bank transaction when it has linked to Outcoming entry");
            }

            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(id);

            var bankFromValue = await WorkScope.GetAsync<BankAccount>(bankTransaction.FromBankAccountId);
            bankFromValue.Amount = bankFromValue.Amount + bankTransaction.FromValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

            var bankToValue = await WorkScope.GetAsync<BankAccount>(bankTransaction.ToBankAccountId);
            bankToValue.Amount = bankToValue.Amount - bankTransaction.ToValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);

            var bankAccountFEE = await WorkScope.GetAll<BankAccount>().Where(x => x.HolderName == Constants.BANKACCOUNT_TRANSFER_FEE).FirstOrDefaultAsync();
            var transferFeeRefund = await WorkScope.GetAll<BankTransaction>()
                                           .Where(x => x.Name == $"Tính phí giao dịch của giao dịch: {bankTransaction.Id}-{bankTransaction.Name}")
                                           .FirstOrDefaultAsync();
            if (transferFeeRefund != null)
            {
                await Delete(transferFeeRefund.Id);
            }

            await WorkScope.DeleteAsync<BankTransaction>(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_Delete)]
        public async Task DeleteFromOutcomingEntry(long id)
        {
            var transaction = await WorkScope.GetAll<BankTransaction>().FirstOrDefaultAsync(s => s.Id == id);

            var bankFromValue = await WorkScope.GetAsync<BankAccount>(transaction.FromBankAccountId);
            bankFromValue.Amount = bankFromValue.Amount + transaction.FromValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

            var bankToValue = await WorkScope.GetAsync<BankAccount>(transaction.ToBankAccountId);
            bankToValue.Amount = bankToValue.Amount - transaction.ToValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);

            if (transaction == null)
            {
                throw new UserFriendlyException("Transaction doesn't exist");
            }

            var hasIncomingEntries = await WorkScope.GetAll<IncomingEntry>().AnyAsync(ie => ie.BankTransactionId == id);
            if (hasIncomingEntries)
            {
                throw new UserFriendlyException("Can not delete Bank transaction when you have linked Incoming entry");
            }
            var countOutcomingEntryLink = WorkScope.GetAll<OutcomingEntryBankTransaction>().Where(oebt => oebt.BankTransactionId == id).Count();
            var OutcomingEntryLink = await WorkScope.GetAll<OutcomingEntryBankTransaction>().Where(oebt => oebt.BankTransactionId == id).ToListAsync();
            if (countOutcomingEntryLink >= 2)
            {
                throw new UserFriendlyException("Can not delete Bank transaction when it has more than 2 linked Outcoming entries");
            }
            if (countOutcomingEntryLink == 1)
            {
                foreach (var item in OutcomingEntryLink)
                {
                    await WorkScope.DeleteAsync<OutcomingEntryBankTransaction>(item.Id);
                }
                await WorkScope.DeleteAsync<BankTransaction>(id);
            }

            var bankAccountFEE = await WorkScope.GetAll<BankAccount>().Where(x => x.HolderName == Constants.BANKACCOUNT_TRANSFER_FEE).FirstOrDefaultAsync();
            var transferFeeRefund = await WorkScope.GetAll<BankTransaction>()
                                           .Where(x => x.Name == $"Tính phí giao dịch của giao dịch: {transaction.Id}-{transaction.Name}")
                                           .FirstOrDefaultAsync();
            if (transferFeeRefund != null)
            {
                await Delete(transferFeeRefund.Id);
            }
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_BankTransactionDetail)]
        public async Task<DetailBankTransactionDto> Get(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Incoming not link with banktransaction");
            }
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var bankTransaction = await WorkScope.GetAll<BankTransaction>()
                .Include(x => x.BTransaction)
                .ThenInclude(x => x.BankAccount)
                .ThenInclude(x => x.Currency)
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
                if(bankTransaction == null)
                {
                    throw new UserFriendlyException(string.Format("Giao dịch ngân hàng: {0} không tồn tại hoặc đã bị xóa", id));
                }
                var dicUsers = await _myUserManager.GetDictionaryUserAudited(new List<long>
                {
                    bankTransaction.CreatorUserId ?? 0,
                    bankTransaction.LastModifierUserId ?? 0,
                });
                var fromBankAccountName = await WorkScope.GetAll<BankAccount>().Where(s => s.Id == bankTransaction.FromBankAccountId).FirstOrDefaultAsync();
                var curencyBankFrom = fromBankAccountName?.CurrencyId != null ? WorkScope.GetAsync<Currency>((long)fromBankAccountName.CurrencyId).Result : null;

                var toBankAccountName = await WorkScope.GetAll<BankAccount>().Where(s => s.Id == bankTransaction.ToBankAccountId).FirstOrDefaultAsync();
                var curencyBankTo = toBankAccountName?.CurrencyId != null ? await WorkScope.GetAsync<Currency>((long)toBankAccountName.CurrencyId) : null;

                var numberOfIncomingEntries = await WorkScope.GetAll<IncomingEntry>().CountAsync(ie => ie.BankTransactionId == id);

                return new DetailBankTransactionDto
                {
                    Id = bankTransaction.Id,
                    Name = bankTransaction.Name,
                    FromBankAccountId = bankTransaction?.FromBankAccountId,
                    FromBankAccountName = fromBankAccountName?.HolderName,
                    FromBankAccountCurrency = curencyBankFrom?.Code,
                    ToBankAccountId = bankTransaction?.ToBankAccountId,
                    ToBankAccountName = toBankAccountName.HolderName,
                    ToBankAccountCurrency = curencyBankTo?.Code,
                    FromValue = bankTransaction.FromValue,
                    ToValue = bankTransaction.ToValue,
                    Fee = bankTransaction.Fee,
                    TransactionDate = bankTransaction.TransactionDate,
                    Note = bankTransaction.Note,
                    NumberOfIncomingEntries = numberOfIncomingEntries,
                    LockedStatus = bankTransaction.LockedStatus,
                    CreationTime = bankTransaction.CreationTime,
                    CreationUserId = bankTransaction.CreatorUserId,
                    CreationUser = bankTransaction.CreatorUserId.HasValue ? (dicUsers.ContainsKey(bankTransaction.CreatorUserId.Value) ? dicUsers[bankTransaction.CreatorUserId.Value] : string.Empty) : string.Empty,
                    LastModifiedUser = bankTransaction.LastModifierUserId.HasValue ? (dicUsers.ContainsKey(bankTransaction.LastModifierUserId.Value) ? dicUsers[bankTransaction.LastModifierUserId.Value] : string.Empty) : string.Empty,
                    LastModifiedTime = bankTransaction.LastModificationTime,
                    LastModifiedUserId = bankTransaction.LastModifierUserId,
                    BTransactionId = bankTransaction.BTransactionId,
                    BTransactionBankNumber = bankTransaction?.BTransaction?.BankAccount.BankNumber,
                    BTransactionCurrencyId = bankTransaction?.BTransaction?.BankAccount.CurrencyId,
                    BTransactionCurrencyName = bankTransaction?.BTransaction?.BankAccount.Currency.Name,
                    BTransactionMoneyNumber = bankTransaction.BTransaction?.Money,
                    BTransactionTimeAt = bankTransaction.BTransaction?.TimeAt
                };
            }
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_LockUnlock)]
        public async Task LockBankTransaction(long banktransactionId)
        {
            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(banktransactionId);

            bankTransaction.LockedStatus = true;
            await WorkScope.UpdateAsync(bankTransaction);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_LockUnlock)]
        public async Task UnlockBankTransaction(long banktransactionId)
        {
            var bankTransaction = await WorkScope.GetAsync<BankTransaction>(banktransactionId);

            bankTransaction.LockedStatus = false;
            await WorkScope.UpdateAsync(bankTransaction);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_ExportExcel)]
        public async Task<byte[]> ExportExcel(GetAllPagingBankTransactionDto input)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var bankTransactionWS = wb.Worksheets.Add("BankTransaction");
                    var bankTransactions = await GetAllTransactions().FiltersByGetAllPagingBankTransactionDto(input).FilterByGridParam(input);

                    int currentRow = 1;
                    int stt = 0;
                    bankTransactionWS.Cell(1, 1).Value = "STT";
                    bankTransactionWS.Cell(1, 2).Value = "Tên giao dịch";
                    bankTransactionWS.Cell(1, 3).Value = "Bank gửi";
                    bankTransactionWS.Cell(1, 4).Value = "Bank nhận";
                    bankTransactionWS.Cell(1, 5).Value = "Số tiền gửi";
                    bankTransactionWS.Cell(1, 6).Value = "Số tiền nhận";
                    bankTransactionWS.Cell(1, 7).Value = "Phí giao dịch";
                    bankTransactionWS.Cell(1, 8).Value = "Ngày tạo giao dịch";
                    bankTransactionWS.Cell(1, 9).Value = "Ghi chú";
                    foreach (var bankTran in bankTransactions)
                    {
                        currentRow++;
                        stt++;
                        bankTransactionWS.Cell(currentRow, 1).Value = stt;
                        bankTransactionWS.Cell(currentRow, 2).Value = bankTran.Name;
                        bankTransactionWS.Cell(currentRow, 3).Value = bankTran.FromBankAccountName;
                        bankTransactionWS.Cell(currentRow, 4).Value = bankTran.ToBankAccountName;
                        bankTransactionWS.Cell(currentRow, 5).Value = bankTran.FromValue.ToString("N0") + " " + bankTran.FromBankAccountCurrency;
                        bankTransactionWS.Cell(currentRow, 6).Value = bankTran.ToValue.ToString("N0") + " " + bankTran.ToBankAccountCurrency;
                        string feeCurrencyName = bankTran.Fee == 0 ? "VND" : bankTran.FromBankAccountCurrency;
                        bankTransactionWS.Cell(currentRow, 7).Value = bankTran.Fee.ToString("N0") + " " + feeCurrencyName;
                        bankTransactionWS.Cell(currentRow, 8).Value = bankTran.TransactionDate;
                        bankTransactionWS.Cell(currentRow, 9).Value = bankTran.Note;
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
        public async Task<byte[]> NewExportExcel(GetAllPagingBankTransactionDto input)
        {
            var listData = await GetDataExportFile(input);
            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "BankTransaction_Template.xlsx" });
            using (ExcelPackage pck = new ExcelPackage(file))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = pck.Workbook.Worksheets[0];

                int rowIndex = 3;
                int stt = 0;
                foreach (var item in listData)
                {
                    //gan data GDNH
                    worksheet.Cells[rowIndex, 1].Value = ++stt;
                    worksheet.Cells[rowIndex, 2].Value = item.BankTransactionId;
                    worksheet.Cells[rowIndex, 3].Value = item.BankTransactionName;
                    worksheet.Cells[rowIndex, 4].Value = item.FromBankAccountName;
                    worksheet.Cells[rowIndex, 5].Value = item.ToBankAccountName;
                    worksheet.Cells[rowIndex, 6].Value = Helpers.FormatMoney(item.FromValue);
                    worksheet.Cells[rowIndex, 7].Value = item.FromCurrency;
                    worksheet.Cells[rowIndex, 8].Value = item.ToCurrency;
                    worksheet.Cells[rowIndex, 9].Value = Helpers.FormatMoney(item.ToValue);
                    worksheet.Cells[rowIndex, 10].Value = item.Fee;
                    worksheet.Cells[rowIndex, 11].Value = item.TransactionDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[rowIndex, 12].Value = item.Note;

                    //gan data request chi
                    int indexMergeCellByOutcom = 0;
                    int rowIndexOutcom = rowIndex;
                    foreach (var outc in item.OutcomingEntries)
                    {
                        ++indexMergeCellByOutcom;
                        worksheet.Cells[rowIndexOutcom, 13].Value = outc.Id;
                        worksheet.Cells[rowIndexOutcom, 14].Value = outc.Name;
                        worksheet.Cells[rowIndexOutcom, 15].Value = Helpers.FormatMoney(outc.Value);
                        worksheet.Cells[rowIndexOutcom, 16].Value = outc.CurrencyName;
                        worksheet.Cells[rowIndexOutcom, 17].Value = outc.Status;
                        worksheet.Cells[rowIndexOutcom, 18].Value = outc.OutcomingEntryType;
                        worksheet.Cells[rowIndexOutcom, 19].Value = outc.IsChiPhi;
                        worksheet.Cells[rowIndexOutcom, 20].Value = outc.ReportDate.HasValue ? outc.ReportDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        worksheet.Cells[rowIndexOutcom, 21].Value = outc.CreateTime.ToString("dd/MM/yyyy");
                        rowIndexOutcom++;
                    }

                    //gan data ghi nhan thu
                    int indexMergeCellByIncom = 0;
                    int rowIndexIncom = rowIndex;
                    foreach (var incom in item.IncomingEntries)
                    {
                        ++rowIndexIncom;
                        worksheet.Cells[rowIndexIncom, 22].Value = incom.Id;
                        worksheet.Cells[rowIndexIncom, 23].Value = incom.Name;
                        worksheet.Cells[rowIndexIncom, 24].Value = Helpers.FormatMoney(incom.Value);
                        worksheet.Cells[rowIndexIncom, 25].Value = incom.CurrencyName;
                        worksheet.Cells[rowIndexIncom, 26].Value = incom.IncomingEntryType;
                        worksheet.Cells[rowIndexIncom, 27].Value = incom.IsTinhDoanhThu;
                        worksheet.Cells[rowIndexIncom, 28].Value = incom.IsHoanTien;
                        worksheet.Cells[rowIndexIncom, 29].Value = incom.ClientName;
                        indexMergeCellByIncom++;
                    }

                    if (indexMergeCellByIncom > indexMergeCellByOutcom)
                    {
                        //TODO::Merge cells
                        //worksheet.Merge(rowIndex, 1, rowIndexIncom, 12);
                        rowIndex += indexMergeCellByIncom;
                    }
                    else
                    {
                        //TODO::Merge cells
                        //worksheet.Merge(rowIndex, 1, rowIndexOutcom, 12);
                        rowIndex += indexMergeCellByOutcom > 0 ? indexMergeCellByOutcom : 1;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    pck.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        private async Task<List<ExportDataStatisticFileDto>> GetDataExportFile(GetAllPagingBankTransactionDto input)
        {
            var query = GetAllTransactions()
                .FiltersByGetAllPagingBankTransactionDto(input)
                .ApplySearchAndFilter(input)
                .Select(x => x.Id);
            var bankTransactionIds = await query.ToListAsync();

            var qBankTransaction = WorkScope.GetAll<BankTransaction>()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Fee,
                    x.FromValue,
                    x.FromBankAccountId,
                    x.ToBankAccountId,
                    x.ToValue,
                    x.TransactionDate,
                    x.Note,
                    x.OutcomingEntryBankTransactions,
                    x.IncomingEntries
                })
                .Where(x => bankTransactionIds.Contains(x.Id));

            var qBankAccount = WorkScope.GetAll<BankAccount>()
                .Select(x => new
                {
                    x.Id,
                    x.HolderName,
                    x.BankNumber,
                    x.CurrencyId,
                    CurrencyName = x.Currency.Name
                });

            var qDataFile = from bt in qBankTransaction
                            join fba in qBankAccount on bt.FromBankAccountId equals fba.Id
                            join tba in qBankAccount on bt.ToBankAccountId equals tba.Id
                            select new ExportDataStatisticFileDto
                            {
                                BankTransactionId = bt.Id,
                                BankTransactionName = bt.Name,
                                Fee= bt.Fee,
                                FromBankAccountName = fba.HolderName,
                                ToBankAccountName = tba.HolderName,
                                FromCurrency = fba.CurrencyName,
                                FromValue = bt.FromValue,
                                ToCurrency = tba.CurrencyName,
                                ToValue = bt.ToValue,
                                TransactionDate = bt.TransactionDate,
                                Note = bt.Note,
                                OutcomingEntries = bt.OutcomingEntryBankTransactions
                                .Select(x => new ExportDataStatisticOutcomingEntryDto
                                {
                                    Id = x.OutcomingEntryId,
                                    Name = x.OutcomingEntry.Name,
                                    CurrencyName = x.OutcomingEntry.Currency.Name,
                                    CreateTime = x.OutcomingEntry.CreationTime,
                                    OutcomingEntryType = x.OutcomingEntry.OutcomingEntryType.Name,
                                    ReportDate = x.OutcomingEntry.ReportDate,
                                    Status = x.OutcomingEntry.WorkflowStatus.Name,
                                    Value = x.OutcomingEntry.Value,
                                    ExpenseType = x.OutcomingEntry.OutcomingEntryType.ExpenseType
                                }),
                                IncomingEntries = bt.IncomingEntries
                                .Select(x => new ExportDataStatisticIncomingEntryDto
                                {
                                    Id = x.Id,
                                    CurrencyName = x.Currency.Name,
                                    IncomingEntryType = x.IncomingEntryType.Name,
                                    Name = x.Name,
                                    ClientName = x.Account.Name,
                                    HoanTien = x.RelationInOutEntries.Select(x => x.IsRefund).FirstOrDefault(),
                                    RevenueCounted = x.IncomingEntryType.RevenueCounted,
                                    Value = x.Value
                                })
                            };

            return await qDataFile.ToListAsync();
        }
    }
}
