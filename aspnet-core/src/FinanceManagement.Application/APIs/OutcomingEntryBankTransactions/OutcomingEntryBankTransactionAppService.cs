using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.GetOutcomingEntries.Dto;
using FinanceManagement.APIs.OutcomingEntries.Dto;
using FinanceManagement.APIs.OutcomingEntryBankTransactions.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.OutcomingEntryBankTransactions
{
    [AbpAuthorize]
    public class OutcomingEntryBankTransactionAppService : FinanceManagementAppServiceBase
    {
        public OutcomingEntryBankTransactionAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry)]
        public async Task<List<GetOutcomingEntryDto>> GetAllOutcomingEntryByTransaction(long bankTransactionId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var requester = WorkScope.GetAll<User>();
                var obt = await WorkScope.GetAll<OutcomingEntryBankTransaction>().Where(x => x.BankTransactionId == bankTransactionId).Select(s => s.OutcomingEntryId).ToListAsync();
                var oed = WorkScope.GetAll<OutcomingEntryDetail>().Where(x => obt.Contains(x.OutcomingEntryId));

                var query = WorkScope.GetAll<OutcomingEntry>().Where(x => obt.Contains(x.Id))
                                  .Select(x => new GetOutcomingEntryDto
                                  {
                                      Id = x.Id,
                                      OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                                      OutcomingEntryTypeCode = x.OutcomingEntryType.Code,
                                      OutcomingEntryTypeName = x.OutcomingEntryType.Name,
                                      ExpenseType = x.OutcomingEntryType.ExpenseType,
                                      Requester = requester.FirstOrDefault(r => r.Id == x.CreatorUserId).Name,
                                      Name = x.Name,
                                      AccountId = x.AccountId,
                                      AccountName = x.Account.Name,
                                      BranchId = x.BranchId,
                                      BranchName = x.Branch.Name,
                                      CurrencyId = x.CurrencyId,
                                      CurrencyName = x.Currency.Code,
                                      WorkflowStatusId = x.WorkflowStatusId,
                                      WorkflowStatusName = x.WorkflowStatus.Name,
                                      WorkflowStatusCode = x.WorkflowStatus.Code,
                                      Value = x.Value,
                                  });

                return await query.ToListAsync();
            }
        }

        [HttpPost]
        //[AbpAuthorize(PermissionNames.Finance_BankTransaction_SaveMultipleRequest)]
        public async Task<List<OutcomingEntryBankTransactionDto>> SaveMultipleRequest(List<OutcomingEntryBankTransactionDto> input)
        {
            foreach(var item in input)
            {
                await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntryBankTransaction>(item));

                var relationInOut = await WorkScope.GetAll<RelationInOutEntry>()
                                        .Where(x => x.OutcomingEntryId == item.OutcomingEntryId && x.IncomingEntry.BankTransactionId == 0).FirstOrDefaultAsync();
                var outcoming = await WorkScope.GetAsync<OutcomingEntry>(item.OutcomingEntryId);

                if(relationInOut != null)
                {
                    var incoming = await WorkScope.GetAsync<IncomingEntry>(relationInOut.IncomingEntryId);
                    incoming.BankTransactionId = item.BankTransactionId;
                    incoming.Value = outcoming.Value;
                    await WorkScope.UpdateAsync(incoming);
                }
            }

            return input;
        }

        [HttpGet]
       // [AbpAuthorize(PermissionNames.Finance_BankTransaction_GetAllOutcomingEntryByStatusEND)]
        public async Task<List<GetOutcomingEntryDto>> GetAllOutcomingEntryByStatusEND(long bankTransactionId)
        {
            var obt = await WorkScope.GetAll<OutcomingEntryBankTransaction>().Where(x => x.BankTransactionId == bankTransactionId).ToListAsync();

            var oed = WorkScope.GetAll<OutcomingEntryDetail>();
            var query = WorkScope.GetAll<OutcomingEntry>().Where(x => x.WorkflowStatus.Code == Constants.WORKFLOW_STATUS_END)
                        .Where(x => !obt.Select(y => y.OutcomingEntryId).Contains(x.Id))
                        .Select(x => new GetOutcomingEntryDto
                        {
                            Id = x.Id,
                            OutcomingEntryTypeId = x.OutcomingEntryTypeId,
                            OutcomingEntryTypeCode = x.OutcomingEntryType.Code,
                            OutcomingEntryTypeName = x.OutcomingEntryType.Name,
                            Name = x.Name,
                            AccountId = x.AccountId,
                            AccountName = x.Account.Name,
                            BranchId = x.BranchId,
                            BranchName = x.Branch.Name,
                            CurrencyId = x.CurrencyId,
                            CurrencyName = x.Currency.Code,
                            Value = oed.Where(y => y.OutcomingEntryId == x.Id).Sum(x => x.Total),
                            WorkflowStatusId = x.WorkflowStatusId,
                            WorkflowStatusName = x.WorkflowStatus.Name,
                            WorkflowStatusCode = x.WorkflowStatus.Code,
                        });

            return await query.ToListAsync();
        }
    }
}
