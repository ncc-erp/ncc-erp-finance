using Abp.Domain.Repositories;
using Abp.UI;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BTransactions;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.Periods.Dtos;
using FinanceManagement.Uitls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Periods
{
    public class PeriodManager : DomainManager, IPeriodManager
    {
        private readonly IRepository<Period, int> _periodRepo;
        private readonly IRepository<User, long> _userRepo;
        private readonly IRepository<PeriodBankAccount, long> _periodBankAccountRepo;
        private readonly IBTransactionManager _bTransactionManager;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        public PeriodManager(
            IWorkScope ws,
            IRepository<Period, int> periodRepo,
            IRepository<User, long> userRepo,
            IRepository<PeriodBankAccount, long> periodBankAccountRepo,
            IBTransactionManager bTransactionManager,
            IOutcomingEntryManager outcomingEntryManager
        ) : base(ws)
        {
            _periodRepo = periodRepo;
            _periodBankAccountRepo = periodBankAccountRepo;
            _bTransactionManager = bTransactionManager;
            _outcomingEntryManager = outcomingEntryManager;
            _userRepo = userRepo;
        }
        public async Task<int> Create(CreatePeriodAndPeriodBankAccountDto input)
        {
            var period = await CreatePeriod(new FormPeriod
            {
                IsActive = true,
                Name = input.Name,
                StartDate = DateTimeUtils.GetNow()
            });

            var bankAccountIds = input.PeriodBankAccounts.Select(x => x.BankAccountId).ToList();
            var bankAccountIdsActive = await _ws.GetAll<PeriodBankAccount>()
                .Where(x => bankAccountIds.Contains(x.BankAccountId) && x.IsActive)
                .Select(x => x.BankAccountId)
                .ToListAsync();

            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                foreach (var item in input.PeriodBankAccounts)
                {
                    if (bankAccountIdsActive.Contains(item.BankAccountId))
                    {
                        await CreatePeriodBankAccount(item, period.Id);
                    }
                }
            }
            //await UpdateBaseBalanceBankAccount(input.PeriodBankAccounts);

            await CurrentUnitOfWork.SaveChangesAsync();
            return period.Id;
        }
        private async Task UpdateBaseBalanceBankAccount(List<CreatePeriodBankAccountDto> input)
        {
            var dicBankAccountIdToBaseBalance = input.ToDictionary(x => x.BankAccountId, x => x.BaseBalance);
            (await _ws.GetAll<BankAccount>()
                .Where(x => dicBankAccountIdToBaseBalance.Keys.Contains(x.Id))
                .ToListAsync()
            ).ForEach(item => item.BaseBalance = dicBankAccountIdToBaseBalance.ContainsKey(item.Id) ? dicBankAccountIdToBaseBalance[item.Id] : item.BaseBalance);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<Period> CreatePeriod(FormPeriod input)
        {
            var period = await _periodRepo.InsertAsync(ObjectMapper.Map<Period>(input));
            period.TenantId = CurrentUnitOfWork.GetTenantId();
            await CurrentUnitOfWork.SaveChangesAsync();
            return period;
        }
        public async Task CreatePeriodBankAccount(CreatePeriodBankAccountDto input, int periodId = default)
        {
            var periodBankAccount = await _periodBankAccountRepo.InsertAsync(ObjectMapper.Map<PeriodBankAccount>(input));
            periodBankAccount.TenantId = CurrentUnitOfWork.GetTenantId();
            if (periodId != default)
            {
                periodBankAccount.PeriodId = periodId;
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task AssignPeriodIdForTheFirstTime(Period period, List<CreatePeriodBankAccountTheFirstTime> input)
        {
            _ws.GetAll<BTransaction>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<OutcomingEntry>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<IncomingEntry>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<BankTransaction>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<TempOutcomingEntry>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<TempOutcomingEntryDetail>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<RelationInOutEntry>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<OutcomingEntryBankTransaction>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<OutcomingEntryDetail>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<OutcomingEntrySupplier>().ToList().ForEach(item => item.PeriodId = period.Id);
            _ws.GetAll<OutcomingEntryStatusHistory>().ToList().ForEach(item => item.PeriodId = period.Id);

            period.IsActive = true;

            await CurrentUnitOfWork.SaveChangesAsync();

            var dicTotalMoneyBTransaction = await _ws.GetAll<BTransaction>()
            .Select(x => new
            {
                x.BankAccountId,
                x.Money
            })
            .GroupBy(x => x.BankAccountId)
            .Select(x => new { x.Key, Total = x.Sum(s => s.Money) })
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Key, x => x.Total);

            foreach (var dto in input)
            {
                var periodBankAccountDto = new CreatePeriodBankAccountDto();

                if (dicTotalMoneyBTransaction.ContainsKey(dto.BankAccountId))
                    periodBankAccountDto.BaseBalance = dto.CurrentBalance - dicTotalMoneyBTransaction[dto.BankAccountId];
                else
                    periodBankAccountDto.BaseBalance = dto.CurrentBalance;

                periodBankAccountDto.BankAccountId = dto.BankAccountId;
                      
                await CreatePeriodBankAccount(periodBankAccountDto);
            }
        }
        public async Task Update(UpdatePeriodDto input)
        {
            var currentPeriod = await GetCurrentPeriod();
            ObjectMapper.Map(input, currentPeriod);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task UpdateBaseBalancePeriodBankAccount(long bankAccountId, double baseBalance)
        {
            using (CurrentUnitOfWork.EnableFilter(nameof(IMustHavePeriod)))
            {
                var hasBTransaction = await _bTransactionManager.HasBTransaction();
                if (hasBTransaction)
                    return;

                var periodBankAccount = await _periodBankAccountRepo.GetAll()
                    .Where(x => x.BankAccountId == bankAccountId)
                    .FirstOrDefaultAsync();

                if (periodBankAccount == null)
                    return;
                periodBankAccount.BaseBalance = baseBalance;

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
        public async Task<GetPeriodHaveDetail> Get(int id)
        {
            return await IQGetPeriodHaveDetail()
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public IQueryable<GetPeriodHaveDetail> IQGetPeriodHaveDetail()
        {
            return _periodRepo.GetAll()
                .Select(x => new GetPeriodHaveDetail
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive,
                    PeriodBankAccounts = x.PeriodBankAccounts
                    .Select(s => new GetPeriodBankAccount
                    {
                        Id = s.Id,
                        BankAccountId = s.BankAccountId,
                        BankAccountName = s.BankAccount.HolderName,
                        BankAccountNumber = s.BankAccount.BankNumber,
                        BaseBalance = s.BaseBalance,
                    })
                });
        }
        public IQueryable<GetPeriodDto> IQGetPeriod()
        {
            var users = _userRepo.GetAll();

            return _periodRepo.GetAll()
                .Select(x => new GetPeriodDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive,
                    CreationTime = x.CreationTime,
                    CreateByUserName = users.Where(s => s.Id == x.CreatorUserId).Select(s => s.Name).FirstOrDefault()
                });
        }
        public async Task<PreviewClosePeriodDto> PreviewBeforeWhenClosePeriod()
        {
            var result = new PreviewClosePeriodDto();

            result.CountBTransactionDiffDoneStatus = await _bTransactionManager.CountBTransactionPendingStatus();

            result.OutcomingEntryInfos = await _outcomingEntryManager.GetOutcomingEntryNotEndForClosePeriod();

            result.BankAccountInfos = await _ws.GetAll<PeriodBankAccount>()
                .Where(x => x.IsActive)
                .Select(x => new PreviewClosePeriodByBankAccountDto
                {
                    BankAccountId = x.BankAccountId,
                    BankAccountName = x.BankAccount.HolderName,
                    BankNumber = x.BankAccount.BankNumber,
                    IsActive = x.IsActive
                }).ToListAsync();

            return result;
        }
        public async Task<PreviewClosePeriodByBankAccountDto> CheckDiffRealBalanceAndBTransaction(PreviewClosePeriodByBankAccountDto input)
        {
            if (!input.CurrentBalance.HasValue)
                throw new UserFriendlyException("Vui lòng nhập số dư hiện tại của tài khoản!");

            var totalMoneyBTransaction = await _ws.GetAll<BTransaction>()
                .Where(x => x.BankAccountId == input.BankAccountId)
                .SumAsync(x => x.Money);

            var baseBalanceByBankAccount = await _periodBankAccountRepo.GetAll()
                .Where(x => x.BankAccountId == input.BankAccountId)
                .Select(x => new { x.BaseBalance, x.PeriodId })
                .FirstOrDefaultAsync();

            input.BalanceByBTransaction = baseBalanceByBankAccount.BaseBalance + totalMoneyBTransaction;

            return input;
        }
        public async Task ClosePeriod()
        {
            var countBTransactionDiffDoneStatus = await _bTransactionManager.CountBTransactionPendingStatus();
            if (countBTransactionDiffDoneStatus > 0)
                throw new UserFriendlyException("Không thể đóng kì khi tồn tại Biến động số dư chưa DONE");

            var countOutcomingEntryDiffEndStatus = await _outcomingEntryManager.CountOutcomingEntryNotEndForClosePeriod();
            if (countOutcomingEntryDiffEndStatus > 0)
                throw new UserFriendlyException("Không thể đóng kì khi tồn tại Request chi chưa THỰC THI");

            var currentPeriod = await GetCurrentPeriod();
            //close period
            currentPeriod.IsActive = false;
            currentPeriod.EndDate = DateTimeUtils.GetNow();

            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<bool> IsTheFirstRecord()
        {
            return !(await _periodRepo
                .GetAll()
                .AsNoTracking()
                .AnyAsync());
        }
        public async Task<Period> GetCurrentPeriod()
        {
            return await _periodRepo.GetAll()
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetCurrentPeriodId()
        {
            return await _periodRepo.GetAll()
                .Where(x => x.IsActive)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<int> CountPeriod()
        {
            return await _periodRepo.GetAll().CountAsync();
        }
    }
}
