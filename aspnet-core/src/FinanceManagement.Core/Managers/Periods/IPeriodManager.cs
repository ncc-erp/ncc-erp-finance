using Abp.Dependency;
using Abp.Domain.Services;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.Periods.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Periods
{
    public interface IPeriodManager : ITransientDependency
    {
        Task<int> Create(CreatePeriodAndPeriodBankAccountDto input);
        Task<Period> CreatePeriod(FormPeriod input);
        Task CreatePeriodBankAccount(CreatePeriodBankAccountDto input, int periodId = default);
        Task AssignPeriodIdForTheFirstTime(Period period, List<CreatePeriodBankAccountTheFirstTime> input);
        Task Update(UpdatePeriodDto input);
        Task UpdateBaseBalancePeriodBankAccount(long bankAccountId, double baseBalance);
        Task<GetPeriodHaveDetail> Get(int id);
        IQueryable<GetPeriodHaveDetail> IQGetPeriodHaveDetail();
        IQueryable<GetPeriodDto> IQGetPeriod();
        Task<bool> IsTheFirstRecord();
        Task<PreviewClosePeriodDto> PreviewBeforeWhenClosePeriod();
        Task<PreviewClosePeriodByBankAccountDto> CheckDiffRealBalanceAndBTransaction(PreviewClosePeriodByBankAccountDto input);
        Task ClosePeriod();
        Task<Period> GetCurrentPeriod();
        Task<int> GetCurrentPeriodId();
        Task<int> CountPeriod();
    }
}
