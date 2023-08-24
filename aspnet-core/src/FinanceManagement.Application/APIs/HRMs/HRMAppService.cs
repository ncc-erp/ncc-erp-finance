using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.BankAccounts.Dto;
using FinanceManagement.APIs.Banks.Dto;
using FinanceManagement.APIs.HRMs.Dto;
using FinanceManagement.APIs.OutcomingEntryDetails.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinanceManagement.Authorization.Roles.StaticRoleNames;

namespace FinanceManagement.APIs.HRMs
{
    public class HRMAppService : FinanceManagementAppServiceBase
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public HRMAppService(IWorkScope workScope) : base(workScope)
        {
            //_httpContextAccessor = httpContextAccessor;
        }

        [AbpAllowAnonymous]
        [NccAuth]
        public async Task<CreateOutcomingEntryHRMDto> CreateOucomingEntryByHRM(CreateOutcomingEntryHRMDto input)
        {
            //if (!CheckSecurityCode())
            //{
            //    throw new UserFriendlyException("SecretKey does not match!");
            //}

            var initialWorkflowStatus = WorkScope.GetAll<WorkflowStatus>().FirstOrDefault(ws => ws.Code == Constants.WORKFLOW_STATUS_START);
            var initalOutcomingEntryType = WorkScope.GetAll<OutcomingEntryType>().FirstOrDefault(oet => oet.Code == Constants.OUTCOMING_ENTRY_TYPE_SALARY);
            var accountTypeID = WorkScope.GetAll<AccountType>().FirstOrDefault(x => x.Code == Constants.ACCOUNT_TYPE_COMPANY);
            var initalAccountId = WorkScope.GetAll<Account>().FirstOrDefault(a => a.AccountTypeId == accountTypeID.Id && a.Default == true);
            var account = WorkScope.GetAll<Account>();
            if (initialWorkflowStatus == null)
            {
                throw new UserFriendlyException("Workflow status with code [START] doesn't exist");
            }
            if (initalOutcomingEntryType == null)
            {
                throw new UserFriendlyException("OutcomingEntryType with code [SALARY] doesn't exist");
            }
            if (accountTypeID == null)
            {
                throw new UserFriendlyException("AccountType with code [COMPANY] doesn't exist or not default");
            }
            var currencyId = await WorkScope.GetAll<Currency>().Where(x => x.Code == Constants.CURRENCY_VND).Select(x => x.Id).FirstOrDefaultAsync();
            input.CurrencyId = currencyId;
            input.OutcomingEntryTypeId = initalOutcomingEntryType.Id;
            input.WorkflowStatusId = initialWorkflowStatus.Id;
            input.AccountId = initalAccountId.Id;
            input.Value = input.Detail.Sum(x => x.Total);
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntry>(input));

            foreach (var item in input.Detail)
            {
                var detail = new OutcomingEntryDetail
                {
                    OutcomingEntryId = input.Id,
                    Name = item.Name,
                    AccountId = account.Where(x => x.Code == item.UserCode).Select(x => x.Id).FirstOrDefault(),
                    Quantity = 1,
                    UnitPrice = item.UnitPrice,
                    Total = item.Total
                };
                await WorkScope.InsertAndGetIdAsync(detail);
            }

            return input;
        }

        [AbpAllowAnonymous]
        [NccAuth]
        public async Task<CreateAccountAndBankAccountDto> CreateBankAndBankAccountByHRM(CreateAccountAndBankAccountDto input)
        {
            //if (!CheckSecurityCode())
            //{
            //    throw new UserFriendlyException("SecretKey does not match!");
            //}

            var currency = await WorkScope.GetAll<Currency>().FirstOrDefaultAsync(x => x.Code == Constants.CURRENCY_VND);
            if (currency == null)
                throw new UserFriendlyException($"Currency code {Constants.CURRENCY_VND} not exist");

            var bank = await WorkScope.GetAll<Bank>().FirstOrDefaultAsync(x => x.Code == Constants.BANK_TCB);
            if (bank == null)
                throw new UserFriendlyException($"Bank code '{Constants.BANK_TCB}' not exist");

            var accountType = await WorkScope.GetAll<AccountType>().FirstOrDefaultAsync(x => x.Code == Constants.ACCOUNT_TYPE_EMPLOYEE);
            if (accountType == null)
                throw new UserFriendlyException($"AccountType with Code '{Constants.ACCOUNT_TYPE_EMPLOYEE}' not exist");

            var account = new Account
            {
                AccountTypeId = accountType.Id,
                Name = input.Name,
                Code = input.Code,
                Default = false
            };

            input.Id = await WorkScope.InsertAndGetIdAsync(account);

            var bankAccount = new BankAccount
            {
                HolderName = input.HolderName,
                BankNumber = input.BankNumber,
                AccountId = input.Id,
                CurrencyId = currency.Id,
                BankId = bank.Id,
                Amount = 0
            };
            await WorkScope.InsertAndGetIdAsync(bankAccount);

            return input;
        }

        //private bool CheckSecurityCode()
        //{
        //    var secretCode = SettingManager.GetSettingValue(AppSettingNames.SecretKey);
        //    var header = _httpContextAccessor.HttpContext.Request.Headers;
        //    Logger.Info("secretCode:" + secretCode.Substring(0, 5));            
        //    var securityCodeHeader = header["X-Secret-Key"].ToString();
        //    Logger.Info("securityCodeHeader:" + securityCodeHeader.Substring(0, 5));
        //    if (secretCode == securityCodeHeader)
        //        return true;
        //    return false;
        //}
    }
}
