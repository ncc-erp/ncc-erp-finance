using Abp.Dependency;
using FinanceManagement.Managers.BankTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.BankTransactions
{
    public interface IBankTransactionManager : ITransientDependency
    {
        Task<long> CreateBankTransaction(CreateBankTransactionDto input);
        Task CheckLinkBankTransactionToBTransaction(long bankTransactionId, long bTransactionId);
    }
}
