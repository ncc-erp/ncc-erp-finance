using Abp.UI;
using FinanceManagement.APIs.HRMs.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Notifications;
using FinanceManagement.Notifications.Komu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.HRMs
{
    public class HRMv2AppService : FinanceManagementAppServiceBase
    {
        private readonly IKomuNotification _komuNotification;
        private readonly ICommonManager _commonManager;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        public HRMv2AppService(IWorkScope workScope, IKomuNotification komuNotification, ICommonManager commonManager, IOutcomingEntryManager outcomingEntryManager) : base(workScope)
        {
            _komuNotification = komuNotification;
            _commonManager = commonManager;
            _outcomingEntryManager = outcomingEntryManager;
        }

        [NccAuth]
        public async Task CreateOucomingEntryByHRM(InputCreateOutcomeEntryFormHrmDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var workflowStatusApprovedId = await _commonManager.GetWorkflowStatusApprovedId();
                if (workflowStatusApprovedId == default)
                {
                    throw new UserFriendlyException("Workflow status with code [APPROVED] doesn't exist");
                }

                var outcomingEntryTypeSalaryId = await _commonManager.GetOutcomingEntryTypeSalaryId();
                if (outcomingEntryTypeSalaryId == default)
                {
                    throw new UserFriendlyException("OutcomingEntryType doesn't exist");
                }

                var accountCompanyId = await _commonManager.GetDefaultAccountCompanyId();
                if (accountCompanyId == default)
                {
                    throw new UserFriendlyException("Can't find default account with type company");
                }

                var currencyVNDId = await _commonManager.GetCurrencyVNDId();
                if (currencyVNDId == default)
                {
                    throw new UserFriendlyException("Can't find currency VND");
                }

                var branchList = WorkScope.GetAll<Branch>().Select(x => new {
                    x.Id,
                    x.Code
                }).ToList();

                var dicBranches = branchList.ToDictionary(x => x.Code, x => x.Id);
                long branchCTYId = 0;

                if (dicBranches.ContainsKey(FinanceManagementConsts.BRANCH_CODE_CTY))
                {
                    branchCTYId = dicBranches[FinanceManagementConsts.BRANCH_CODE_CTY];
                }
                else
                {
                    branchCTYId = branchList.Select(x => x.Id).FirstOrDefault();
                }

                var newOutcomingEntry = new OutcomingEntry
                {
                    Name = input.Name,
                    AccountId = accountCompanyId,
                    CurrencyId = currencyVNDId,
                    OutcomingEntryTypeId = outcomingEntryTypeSalaryId,
                    WorkflowStatusId = workflowStatusApprovedId,
                    Value = input.Details.Sum(x => x.UnitPrice),
                    BranchId = branchCTYId,
                };
                var newOutcomeEntryId = await WorkScope.InsertAndGetIdAsync(newOutcomingEntry);

                await _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
                {
                    OutcomingEntryId = newOutcomingEntry.Id,
                    Value = newOutcomingEntry.Value,
                    WorkflowStatusId = workflowStatusApprovedId,
                });

                foreach (var item in input.Details)
                {
                    var detail = new OutcomingEntryDetail
                    {
                        OutcomingEntryId = newOutcomeEntryId,
                        Name = item.Name,
                        AccountId = accountCompanyId,
                        Quantity = 1,
                        UnitPrice = item.UnitPrice,
                        Total = item.UnitPrice,
                        BranchId = dicBranches.ContainsKey(item.BranchCode) ? dicBranches[item.BranchCode] : default,
                    };
                    await WorkScope.InsertAsync(detail);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                _komuNotification.NotifySalary(newOutcomeEntryId);
            }
        }

        [NccAuth]
        public async Task<List<string>> ValidPayrollFromHrm(InputValidCreateOutcomeEntry input)
        {
            var failList = new List<string>();

            var existOutcomeEntry = WorkScope.GetAll<OutcomingEntry>()
                .Where(x => x.Name == input.PayrollName)
                .FirstOrDefault();

            if (existOutcomeEntry != default)
            {
                failList.Add($"request chi: <strong>{input.PayrollName}</strong> đã tồn tại bên finfast");
            }

            var branchCodes = await WorkScope.GetAll<Branch>()
                .Select(x => x.Code)
                .ToListAsync();

            var duplicateCodes = branchCodes.GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key);

            foreach (var duplicate in duplicateCodes)
            {
                failList.Add($"branch code <strong>{duplicate}</strong> bị trùng bên finfast");
            }

            foreach (var code in input.BranchCodes)
            {
                if (!branchCodes.Contains(code))
                {
                    failList.Add($"branch code <strong>{code}</strong> không tồn tại bên finfast");
                }
            }

            return failList;
        }

        [NccAuth]
        public async Task<CreateAccountAndBankAccountDto> CreateBankAndBankAccountByHRM(CreateAccountAndBankAccountDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
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
                    Type = AccountTypeEnum.EMPLOYEE,
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
        }
    }
}
