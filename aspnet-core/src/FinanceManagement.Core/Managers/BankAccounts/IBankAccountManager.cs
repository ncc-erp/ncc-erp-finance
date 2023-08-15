using Abp.Dependency;
using FinanceManagement.Managers.BankAccounts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.BankAccounts
{
    public interface IBankAccountManager : ITransientDependency
    {
        Task<long> CreateBankAccount(CreateBankAccountDto input);
        Task<BankAccountInfoDto> GetBankAccountInfo(long id);
    }
}
