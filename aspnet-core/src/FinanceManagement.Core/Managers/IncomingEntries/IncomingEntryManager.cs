using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.IncomingEntries
{
    public class IncomingEntryManager : DomainManager, IIncomingEntryManager
    {
        public IncomingEntryManager(IWorkScope ws) : base(ws)
        {
        }
        public async Task<long> CreateIncomingEntry(CreateIncomingEntryDto input)
        {
            var id = await _ws.InsertAndGetIdAsync(ObjectMapper.Map<IncomingEntry>(input));
            await CurrentUnitOfWork.SaveChangesAsync();
            return id;
        }
        public IQueryable<IncomingEntryDto> BuildIncomingQuery()
        {
            var query = (from ie in _ws.GetAll<IncomingEntry>().Include(x => x.IncomingEntryType).OrderByDescending(x => x.CreationTime)
                             //join cc in WorkScope.GetAll<CurrencyConvert>() on ie.CurrencyId equals cc.CurrencyId into ccs
                             //from cc in ccs.DefaultIfEmpty()
                         join bt in _ws.GetAll<BankTransaction>()
                              on ie.BankTransactionId equals bt.Id
                         join ba in _ws.GetAll<BankAccount>() on bt.FromBankAccountId equals ba.Id
                         join a in _ws.GetAll<Account>().Include(x => x.AccountType) on ba.AccountId equals a.Id
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
                             ClientAccountId = a.AccountType.Code == FinanceManagementConsts.ACCOUNT_TYPE_CLIENT ? ba.AccountId : default,
                             ClientName = a.AccountType.Code == FinanceManagementConsts.ACCOUNT_TYPE_CLIENT ? ba.Account.Name : null,
                             CreationUserId = ie.CreatorUserId,
                             CreationTime = ie.CreationTime,
                             LastModifiedTime = ie.LastModificationTime,
                             LastModifiedUserId = ie.LastModifierUserId,
                             RevenueCounted = ie.IncomingEntryType.RevenueCounted
                         }).OrderByDescending(x => x.Date);
            return query;
        }
    }
}
