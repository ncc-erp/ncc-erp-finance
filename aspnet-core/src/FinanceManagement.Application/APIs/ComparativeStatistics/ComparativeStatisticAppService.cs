using Abp.Authorization;
using FinanceManagement.APIs.ComparativeStatistics.Dto;
using FinanceManagement.APIs.Explanations.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.ComparativeStatistics
{
    [AbpAuthorize]
    public class ComparativeStatisticAppService : FinanceManagementAppServiceBase
    {
        public ComparativeStatisticAppService(IWorkScope workScope) : base(workScope)
        {
        }

        //[AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_Create)]
        private async Task<object> Update(long comparativeStatisticId)
        {
            var bankAccountCompanies = await (from ba in WorkScope.GetAll<BankAccount>()
                                              join acc in WorkScope.GetAll<Account>().Include(x => x.AccountType) on ba.AccountId equals acc.Id
                                              where acc.AccountType.Code == Constants.ACCOUNT_TYPE_COMPANY
                                              select new
                                              {
                                                  BankAccountId = ba.Id,
                                              }).ToListAsync();
            var currentBankAccounts = await WorkScope.GetAll<Explanation>().Where(x => x.ComparativeStatisticId == comparativeStatisticId).Select(x => x.BankAccountId).ToListAsync();

            var newBankAccounts = bankAccountCompanies.Where(x => !currentBankAccounts.Contains(x.BankAccountId));

            foreach (var bankAccount in newBankAccounts)
            {
                //insert real explan
                var realExplan = new ExplanationDto
                {
                    BankAccountId = bankAccount.BankAccountId,
                    BankAccountExplanation = string.Empty,
                    Type = ExplanationType.Real,
                    ComparativeStatisticId = comparativeStatisticId
                };
                await WorkScope.InsertAsync(ObjectMapper.Map<Explanation>(realExplan));
                //insert convert explan
                var convertExplan = new ExplanationDto
                {
                    BankAccountId = bankAccount.BankAccountId,
                    BankAccountExplanation = string.Empty,
                    Type = ExplanationType.Convert,
                    ComparativeStatisticId = comparativeStatisticId
                };
                await WorkScope.InsertAsync(ObjectMapper.Map<Explanation>(convertExplan));
            }
            return newBankAccounts;
        }
        [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_View)]
        public async Task<ComparativeStatisticDTO> GetComparativeStatistics(DateTime? startDate, DateTime? endDate)
        {
            var isExistComparativeStatistic = await WorkScope.GetAll<ComparativeStatistic>()
                .AnyAsync(x => (!startDate.HasValue || x.StartDate.Value.Date == startDate.Value.Date) && (!endDate.HasValue || x.EndDate.Value.Date == endDate.Value.Date));
            if (!isExistComparativeStatistic)
            {
                var comparativeStatistic = new ComparativeStatisticDTO
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    DifferentExplanation = string.Empty
                };
                await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<ComparativeStatistic>(comparativeStatistic));
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            var currentComparativeStatistic = await WorkScope.GetAll<ComparativeStatistic>()
                .Where(x => (!startDate.HasValue || x.StartDate.Value.Date == startDate.Value.Date) 
                && (!endDate.HasValue || x.EndDate.Value.Date == endDate.Value.Date))
                .FirstOrDefaultAsync();
            await Update(currentComparativeStatistic.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
            var result = await WorkScope.GetAll<ComparativeStatistic>()
                .Where(x => !startDate.HasValue || x.StartDate.Value.Date == startDate.Value.Date)
                .Where(x => !endDate.HasValue || x.EndDate.Value.Date == endDate.Value.Date)
                .Select(x => new ComparativeStatisticDTO
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    DifferentExplanation = x.DifferentExplanation
                }).FirstOrDefaultAsync();
            return result;
        }
    }


}
