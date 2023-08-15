using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BankAccounts.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.BankAccounts
{
    public class BankAccountManager : DomainManager, IBankAccountManager
    {
        public BankAccountManager(IWorkScope ws) : base(ws)
        {
        }

        public async Task<long> CreateBankAccount(CreateBankAccountDto input)
        {
            return await _ws.InsertAndGetIdAsync(ObjectMapper.Map<BankAccount>(input));
        }
        public async Task<BankAccountInfoDto> GetBankAccountInfo(long id)
        {
            return await _ws.GetAll<BankAccount>()
                .Select(s => new BankAccountInfoDto
                {
                    Id = s.Id,
                    CurrencyId = s.CurrencyId,
                    BaseBalance = s.BaseBalance,
                    CurrencyCode = s.Currency.Code,
                    CurrencyName = s.Currency.Name,
                    HolderName = s.HolderName,
                    AccountTypeEnum = s.Account.Type
                })
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
