using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BankTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace FinanceManagement.Managers.BankTransactions
{
    public class BankTransactionManager : DomainManager, IBankTransactionManager
    {
        public BankTransactionManager(IWorkScope ws) : base(ws)
        {
        }

        public async Task CheckLinkBankTransactionToBTransaction(long bankTransactionId, long bTransactionId)
        {
            var bankTransactionInfo = await _ws.GetAll<BankTransaction>()
                .Where(s => s.Id == bankTransactionId)
                .Select(s => new
                {
                    s.FromValue,
                    s.ToValue,
                    s.BTransactionId
                })
                .FirstOrDefaultAsync();

            var bTransactionInfo = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == bTransactionId)
                .Select(s => s.Money)
                .FirstOrDefaultAsync();
        }

        public async Task<long> CreateBankTransaction(CreateBankTransactionDto input)
        {
            var id = await _ws.InsertAndGetIdAsync(ObjectMapper.Map<BankTransaction>(input));
            await CurrentUnitOfWork.SaveChangesAsync();
            return id;
        }
    }
}
