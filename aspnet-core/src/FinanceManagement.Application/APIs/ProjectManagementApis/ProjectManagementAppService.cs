using Abp.Authorization;
using Abp.Configuration;
using Abp.UI;
using FinanceManagement.APIs.Accounts.Dto;
using FinanceManagement.APIs.BankAccounts.Dto;
using FinanceManagement.APIs.ProjectApis.Dto;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.ProjectManagementApis
{
    public class ProjectManagementAppService : FinanceManagementAppServiceBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISettingManager _settingManager;

        public ProjectManagementAppService(IHttpContextAccessor httpContextAccessor, ISettingManager settingManager, IWorkScope workScope) : base(workScope)
        {
            _httpContextAccessor = httpContextAccessor;
            _settingManager = settingManager;
        }

        //[AbpAllowAnonymous]
        //[HttpPost]
        //public async Task<List<CreateInvoiceDto>> CreateInvoice(List<CreateInvoiceDto> input)
        //{
        //    if (!CheckSecurityCode())
        //    {
        //        throw new UserFriendlyException("SecretKey does not match!");
        //    }

        //    foreach (var item in input)
        //    {
        //        var invoice = new Invoice
        //        {
        //            Name = item.Name,
        //            ClientName = item.ClientName,
        //            Project = item.Project,
        //            TimeAt = DateTime.Now,
        //            AccountCode = item.AccountCode,
        //            TotalPrice = item.TotalPrice,
        //            Status = item.Status,
        //            Note = item.Note
        //        };
        //        item.Id = await WorkScope.InsertAndGetIdAsync(invoice);

        //        foreach (var detail in item.Detail)
        //        {
        //            var invoiceDetail = new InvoiceDetail
        //            {
        //                InvoiceId = item.Id,
        //                ProjectName = detail.ProjectName,
        //                FileId = detail.LinkFile != null ? detail.FileId : 0,
        //                FileName = detail.LinkFile
        //            };
        //            await WorkScope.InsertAndGetIdAsync(invoiceDetail);
        //        }
        //    }
        //    return input;
        //}

        private bool CheckSecurityCode()
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.SecretKey);
            var header = _httpContextAccessor.HttpContext.Request.Headers;
            var securityCodeHeader = header["X-Secret-Key"];
            if (secretCode == securityCodeHeader)
                return true;
            return false;
        }


        private async Task<long> GetIdAccountType()
        {
            var query = await WorkScope.GetAll<AccountType>().Where(x => x.Code == Enums.AccountTypeEnum.CLIENT.ToString()).Select(s => s.Id).FirstOrDefaultAsync();
            if (query != default)
            {
                return query;
            }
            return await WorkScope.GetAll<AccountType>().Select(s => s.Id).FirstOrDefaultAsync();
        }

        private long GetIdCurrency()
        {
           return WorkScope.GetAll<Currency>().Select(s => s.Id).FirstOrDefault();
        }

        private long GetIdBank()
        {
            return WorkScope.GetAll<Bank>().Select(s => s.Id).FirstOrDefault();
        }

        [HttpPost]
        public async Task<string> CreateAccount(AccountDto input)
        {
            if (!CheckSecurityCode())
            {
                return "<p style='color:#dc3545'>Fail! SecretKey does not match! in <b>FINFAST TOOL</b></p>";
            }
            var isExistName = await WorkScope.GetAll<Account>().AnyAsync(s => s.Name == input.Name);
            if (isExistName)
                return string.Format("<p style='color:#dc3545'>Fail! Account name <b>{0}</b> already exist in <b>FINFAST TOOL</b></p>", input.Name);

            var isExistCode = await WorkScope.GetAll<Account>().AnyAsync(s => s.Code == input.Code);
            if (isExistCode)
                return string.Format("<p style='color:#dc3545'>Fail! Account code <b>{0}</b> already exist in <b>FINFAST TOOL</b></p>", input.Code);
            input.Default = false;
            input.AccountTypeId = await GetIdAccountType();
            input.Type = Enums.AccountTypeEnum.CLIENT;
            var item = ObjectMapper.Map<Account>(input);
            var accountId = await WorkScope.InsertAndGetIdAsync(item);
            var bankAccountDto = new BankAccountDto
            {
                AccountId = accountId,
                HolderName = input.Code,
                BankNumber = "1",
                Amount = 0,
                BaseBalance = 0,
                BankId = GetIdBank(),
                CurrencyId = GetIdCurrency(),
            };
            await CreateBankAccount(bankAccountDto);
            return string.Format("<p style='color:#28a745'>Create account name <b>{0}</b> in <b>FINFAST TOOL</b> successful!</p>", input.Name);
        }

        private async Task<BankAccountDto> CreateBankAccount(BankAccountDto input)
        {
            var bankAccountId = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<BankAccount>(input));
            var bankAccount = await WorkScope.GetAsync<BankAccount>(bankAccountId);
            bankAccount.BankNumber = bankAccountId.ToString();
            CurrentUnitOfWork.SaveChanges();
            return input;
        }
    }
}
