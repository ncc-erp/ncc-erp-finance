using System;
using System.Threading.Tasks;
using Abp.UI;
using FinanceManagement.APIs.TimesheetTools.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Commons;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Notifications.Komu;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using System.Collections.Generic;

namespace FinanceManagement.APIs.TimesheetTool
{
    public class TimesheetToolAppService : FinanceManagementAppServiceBase
    {
        private readonly IKomuNotification _komuNotification;
        private readonly ICommonManager _commonManager;
        private readonly IOutcomingEntryManager _outcomingEntryManager;

        public TimesheetToolAppService(IWorkScope workScope, IKomuNotification komuNotification, ICommonManager commonManager, IOutcomingEntryManager outcomingEntryManager) : base(workScope)
        {
            _komuNotification = komuNotification;
            _commonManager = commonManager;
            _outcomingEntryManager = outcomingEntryManager;
        }

        [NccAuth]
        public async Task CreateOutcomingEntryByTimesheet(InputCreateOutcomingEntryFromTimesheetDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                //name
                if (string.IsNullOrEmpty(input.Name.Trim()))
                {
                    throw new UserFriendlyException("Name of Outcome Entry can't be null");
                }
                else if (_commonManager.IsOutcomingEntryNameExist(input.Name))
                {
                    throw new UserFriendlyException($"Outcome Entry [{input.Name}] existed in Finfast");
                }

                //Money
                if (input.Money <= 0)
                {
                    throw new UserFriendlyException("Value of Outcome Entry can't be negative");
                }

                //status START   
                var workflowStatusStartId = await _commonManager.GetWorkflowStatusStartId();
                if (workflowStatusStartId == default)
                {
                    throw new UserFriendlyException("Workflow status with code [START] doesn't exist");
                }

                //type of OutcomeEntry
                var outComingEntryTypeTeamBuildingId = await _commonManager.GetOutcomingEntryTypeTeamBuildingId();
                if (outComingEntryTypeTeamBuildingId == default)
                {
                    throw new UserFriendlyException("OutcomingEntryType with code [team_building] doesn't exist");
                }

                //company
                var accountCompanyId = await _commonManager.GetDefaultAccountCompanyId();
                if (accountCompanyId == default)
                {
                    throw new UserFriendlyException("Can't find default account with type company");
                }

                //currency
                var currencyVNDId = await _commonManager.GetCurrencyVNDId();
                if (currencyVNDId == default)
                {
                    throw new UserFriendlyException("Can't find currency VND");
                }

                //branch
                var branchList = await WorkScope.GetAll<Branch>()
                                    .Select(x => new { x.Code, x.Id })
                                    .ToListAsync();

                var branchCTYId = branchList.Where(x => x.Code == FinanceManagementConsts.BRANCH_CODE_CTY)
                                            .Select(x => x.Id)
                                            .FirstOrDefault();
                if (branchCTYId == default)
                {
                    throw new UserFriendlyException($"Branch {FinanceManagementConsts.BRANCH_CODE_CTY} doesn't exist in Finfast");
                }

                var duplicateBranchCode = branchList.GroupBy(x => x.Code)
                                                    .Where(x => x.Count() > 1)
                                                    .Select(x => x.Key)
                                                    .FirstOrDefault();
                if (duplicateBranchCode != default)
                {
                    throw new UserFriendlyException($"Branch {duplicateBranchCode} duplicate in Finfast");
                }


                var newOutcomingEntry = new OutcomingEntry
                {
                    Name = input.Name,
                    AccountId = accountCompanyId,
                    CurrencyId = currencyVNDId,
                    OutcomingEntryTypeId = outComingEntryTypeTeamBuildingId,
                    WorkflowStatusId = workflowStatusStartId,
                    Value = input.Money,
                    BranchId = branchCTYId
                };
                var newOutcomingEntryId = await WorkScope.InsertAndGetIdAsync(newOutcomingEntry);

                await _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
                {
                    OutcomingEntryId = newOutcomingEntry.Id,
                    Value = newOutcomingEntry.Value,
                    WorkflowStatusId = workflowStatusStartId,
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                _komuNotification.NotifyTeamBuilding(newOutcomingEntryId);
            }
        }
    }
}

