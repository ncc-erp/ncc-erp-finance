using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Editions;
using FinanceManagement.Entities;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Admin_Tenant)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IWorkScope _ws;
        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IWorkScope ws)
            : base(repository)
        {
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _ws = ws;
        }


        public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
        {
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter

            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            if (input.IsClone)
            {
                //get currency
                var currencies = await _ws.GetAll<Currency>()
                    .AsNoTracking()
                    .ToListAsync();
                //get bank
                var banks = await _ws.GetAll<Bank>()
                    .AsNoTracking()
                    .ToListAsync();
                //get loai doi tuong
                var accountTypes = await _ws.GetAll<AccountType>()
                    .AsNoTracking()
                    .ToListAsync();
                //loai thu
                var incomingEntryTypes = await _ws.GetAll<IncomingEntryType>()
                    .AsNoTracking()
                    .ToListAsync();
                //loai chi
                var outcomingEntryTypes = await _ws.GetAll<OutcomingEntryType>()
                    .AsNoTracking()
                    .ToListAsync();
                //workflow 
                var workflowStatuses = await _ws.GetAll<WorkflowStatus>()
                    .AsNoTracking()
                    .ToListAsync();
                var workflows = await _ws.GetAll<Workflow>()
                    .AsNoTracking()
                    .ToListAsync();
                var workflowTransitions = await _ws.GetAll<WorkflowStatusTransition>()
                    .AsNoTracking()
                    .ToListAsync();
                var workflowTransitionPermission = await _ws.GetAll<WorkflowStatusTransitionPermission>()
                    .AsNoTracking()
                    .ToListAsync();

                using (CurrentUnitOfWork.SetTenantId(tenant.Id))
                {
                    //clone currency
                    foreach (var item in currencies)
                    {
                        item.Id = 0;
                        var newCurrencyId = await _ws.InsertAndGetIdAsync(item);
                        await _ws.InsertAsync(new CurrencyConvert
                        {
                            CurrencyId = newCurrencyId
                        });
                    }

                    //banks
                    foreach (var item in banks)
                    {
                        item.Id = 0;
                        await _ws.InsertAsync(item);
                    }

                    //clone loai doi tuong
                    foreach (var item in accountTypes)
                    {
                        item.Id = 0;
                        await _ws.InsertAsync(item);
                    }

                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    var dicWorkflow = await CloneWorkflow(workflowStatuses, workflows, workflowTransitions, workflowTransitionPermission, adminRole.Id);

                    //clone loai thu
                    await CloneIncomingEntryTypes(incomingEntryTypes, new Dictionary<long, long>(), null);

                    //clone loai chi
                    await CloneOutcomingEntryTypes(outcomingEntryTypes, new Dictionary<long, long>(), null, dicWorkflow);
                    await CreateRoleAndAddPermission(tenant.Id);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }

            return MapToEntityDto(tenant);
        }
        private async Task<Dictionary<long, long>> CloneWorkflow(
            List<WorkflowStatus> workflowStatuses,
            List<Workflow> workflows,
            List<WorkflowStatusTransition> transitions,
            List<WorkflowStatusTransitionPermission> wPermission,
            int roleId)
        {
            var dicWorkflowStatuses = new Dictionary<long, long>();
            foreach (var workflowStatus in workflowStatuses)
            {
                var newWorkflowStatus = new WorkflowStatus();
                newWorkflowStatus.Code = workflowStatus.Code;
                newWorkflowStatus.Name = workflowStatus.Name;
                var newId = await _ws.InsertAndGetIdAsync(newWorkflowStatus);

                dicWorkflowStatuses[workflowStatus.Id] = newId;
            }
            //workflow
            var dicWorkflow = new Dictionary<long, long>();
            foreach (var workflow in workflows)
            {
                var newWorkflow = new Workflow();
                newWorkflow.Name = workflow.Name;
                var newWorkflowId = await _ws.InsertAndGetIdAsync(newWorkflow);
                dicWorkflow[workflow.Id] = newWorkflowId;

                var workflowTrans = transitions.Where(x => x.WorkflowId == workflow.Id);
                foreach (var transition in workflowTrans)
                {
                    var newTransition = new WorkflowStatusTransition();
                    newTransition.WorkflowId = newWorkflowId;
                    newTransition.FromStatusId = dicWorkflowStatuses[transition.FromStatusId];
                    newTransition.ToStatusId = dicWorkflowStatuses[transition.ToStatusId];
                    newTransition.Name = transition.Name;
                    var newWorkflowTransId = await _ws.InsertAndGetIdAsync(newTransition);

                    var vPermissions = wPermission.Where(x => x.TransitionId == transition.Id);
                    foreach (var vPermission in vPermissions)
                    {
                        var workflowPermision = new WorkflowStatusTransitionPermission();
                        workflowPermision.RoleId = roleId;
                        workflowPermision.TransitionId = newWorkflowTransId;

                        await _ws.InsertAndGetIdAsync(workflowPermision);
                    }
                }
            }
            return dicWorkflow;
        }
        private async Task CloneIncomingEntryTypes(List<IncomingEntryType> input, Dictionary<long, long> dics, long? rootId)
        {
            var childrens = input.Where(x => x.ParentId == rootId);
            foreach (var item in childrens)
            {
                var oldId = item.Id;
                if (rootId.HasValue && dics.ContainsKey(rootId.Value))
                {
                    item.ParentId = dics[rootId.Value];
                }
                item.Id = 0;
                await _ws.InsertAsync(item);
                await CurrentUnitOfWork.SaveChangesAsync();
                dics[oldId] = item.Id;

                await CloneIncomingEntryTypes(input, dics, oldId);
            }
        }
        private async Task CloneOutcomingEntryTypes(
            List<OutcomingEntryType> input, 
            Dictionary<long, long> dics, 
            long? rootId,
            Dictionary<long, long> dicWorkflow
        )
        {
            var childrens = input.Where(x => x.ParentId == rootId);
            foreach (var item in childrens)
            {
                var oldId = item.Id;
                if (rootId.HasValue && dics.ContainsKey(rootId.Value))
                {
                    item.ParentId = dics[rootId.Value];
                }
                item.Id = 0;
                item.WorkflowId = dicWorkflow.ContainsKey(item.WorkflowId) ? dicWorkflow[item.WorkflowId] : item.WorkflowId;
                await _ws.InsertAsync(item);
                await CurrentUnitOfWork.SaveChangesAsync();
                dics[oldId] = item.Id;

                await CloneOutcomingEntryTypes(input, dics, oldId, dicWorkflow);
            }
        }
        protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        private async Task CreateRoleAndAddPermission(int? tenantId)
        {
            var roleSeeds = new List<string>() { StaticRoleNames.Tenants.CEO,
                                                StaticRoleNames.Tenants.Accountant,
                                                StaticRoleNames.Tenants.Requester};

            foreach (var roleSeed in roleSeeds)
            {
                var input = new Role
                {
                    TenantId = tenantId,
                    Name = roleSeed,
                    DisplayName = roleSeed,
                    IsStatic = false
                };

                var role = ObjectMapper.Map<Role>(input);
                role.SetNormalizedName();

                CheckErrors(await _roleManager.CreateAsync(role));

                var grantedPermissionsByRole = GrantPermissionRoles.PermissionRoles
                                                .Where(x => x.Key == roleSeed)
                                                .FirstOrDefault()
                                                .Value;
                if (grantedPermissionsByRole != null)
                {
                    var grantedPermissions = PermissionManager
                        .GetAllPermissions()
                        .Where(p => grantedPermissionsByRole.Contains(p.Name))
                        .ToList();
                    await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
                }
            }


        }
    }
}

