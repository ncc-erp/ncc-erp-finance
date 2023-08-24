using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.Authorization.Users;
using FinanceManagement.MultiTenancy;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using Microsoft.Extensions.Logging;
using FinanceManagement.Logging;
using Castle.Core.Logging;
using System.Threading.Tasks;
using System.Threading;
using Abp.Events.Bus.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FinanceManagement.GeneralModels;
using Abp.Extensions;
using System.Linq;
using Abp;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using FinanceManagement.Ncc;
using Abp.Dependency;
using static FinanceManagement.Authorization.Roles.StaticRoleNames;
using FinanceManagement.Managers.Settings;
using DocumentFormat.OpenXml.Vml.Office;
using Abp.UI;

namespace FinanceManagement.EntityFrameworkCore
{
    public class FinanceManagementDbContext : AbpZeroDbContext<Tenant, Role, User, FinanceManagementDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyConvert> CurrencyConverts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<IncomingEntry> IncomingEntries { get; set; }
        public DbSet<IncomingEntryType> IncomingEntryTypes { get; set; }
        public DbSet<OutcomingEntryType> OutcomingEntryTypes { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
        public DbSet<WorkflowStatusTransition> WorkflowStatusTransitions { get; set; }
        public DbSet<WorkflowStatusTransitionPermission> WorkflowStatusTransitionPermissions { get; set; }
        public DbSet<OutcomingEntry> OutcomingEntries { get; set; }
        public DbSet<OutcomingEntryDetail> OutcomingEntryDetails { get; set; }
        public DbSet<OutcomingEntryBankTransaction> OutcomingEntryBankTransaction { get; set; }
        public DbSet<UserOutcomingType> UserOutcomingType { get; set; }
        public DbSet<RelationInOutEntry> RelationInOutEntry { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<OutcomingEntrySupplier> OutcomingEntrySupplier { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceBankTransaction> InvoiceBankTransactions { get; set; }
        public DbSet<OutcomingEntryFile> OutcomingEntryFiles { get; set; }
        public DbSet<RevenueManaged> RevenueManageds { get; set; }
        public DbSet<RevenueManagedFile> RevenueManagedFiles { get; set; }
        public DbSet<ComparativeStatistic> ComparativeStatistics { get; set; }
        public DbSet<Explanation> Explanations { get; set; }
        public DbSet<FinanceReviewExplain> FinanceReviewExplains { get; set; }
        public DbSet<LineChart> LineCharts { get; set; }
        public DbSet<LineChartSetting> LineChartSettings { get; set; }

        #region new version
        public DbSet<BTransaction> BTransactions { get; set; }
        public DbSet<BTransactionLog> BTransactionLogs { get; set; }
        public DbSet<OutcomingEntryStatusHistory> OutcomingEntryStatusHistories { get; set; }
        public DbSet<TempOutcomingEntry> TempOutcomingEntries { get; set; }
        public DbSet<TempOutcomingEntryDetail> TempOutcomingEntryDetails { get; set; }  
        public DbSet<Period> Periods { get; set; }
        public DbSet<PeriodBankAccount> PeriodBankAccounts { get; set; }
        #endregion
        protected readonly IPeriodResolveContributor _periodResolveContributor;
        protected IMySettingManager _settingManager { get; set; }
        public FinanceManagementDbContext(
            DbContextOptions<FinanceManagementDbContext> options,
            IPeriodResolveContributor periodResolveContributor,
            IMySettingManager settingManager)
            : base(options)
        {
            _periodResolveContributor = periodResolveContributor;
            _settingManager = settingManager;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(GetDbLoggerFactory());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BTransactionLog>()
                .HasIndex(p => p.Key);
        }
        #region add logger
        private LoggerFactory GetDbLoggerFactory()
        {
            return new LoggerFactory(new[] { new MyLoggerProvider(NullLogger.Instance) });
        }
        #endregion

        #region customize data filters and apply concepts
        protected virtual int? CurrentPeriodId => GetCurrentPeriodIdOrNull();
        protected virtual bool IsHavePeriodFilterEnabled
        {
            get
            {
                ICurrentUnitOfWorkProvider currentUnitOfWorkProvider = CurrentUnitOfWorkProvider;
                if (currentUnitOfWorkProvider == null)
                {
                    return false;
                }

                return currentUnitOfWorkProvider.Current?.IsFilterEnabled(nameof(IMustHavePeriod)) == true;
            }
        }
        protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
        {
            if (typeof(IMustHavePeriod).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return base.ShouldFilterEntity<TEntity>(entityType);
        }
        protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        {
            var expression = base.CreateFilterExpression<TEntity>();
            if (typeof(IMustHavePeriod).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> expression5 = (TEntity e) => !IsHavePeriodFilterEnabled || !CurrentPeriodId.HasValue || CurrentPeriodId == 0 || ((IMustHavePeriod)e).PeriodId == CurrentPeriodId;
                expression = ((expression == null) ? expression5 : CombineExpressions(expression, expression5));
            }
            return expression;
        }
        protected override void ApplyAbpConcepts(EntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            base.ApplyAbpConcepts(entry, userId, changeReport);
            if(entry.Entity is IMustHavePeriod && IsHavePeriodFilterEnabled)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        CheckAllowChangeEntityByPeriod();
                        CheckAndSetMustHavePeriodIdProperty(entry.Entity, userId);
                        break;
                    case EntityState.Modified:
                        //TODO::implement new logic if need
                        CheckAllowChangeEntityByPeriod();
                        break;
                    case EntityState.Deleted:
                        //TODO::implement new logic if need
                        CheckAllowChangeEntityByPeriod();
                        break;
                }
            }
        }
        private void CheckAllowChangeEntityByPeriod()
        {
            var activePeriodId = GetPeriodOnActive();
            var isAllowChangeEntityInPeriodClosed = _settingManager.GetAllowChangeEntityInPeriodClosed();
            if((CurrentPeriodId != activePeriodId) && !isAllowChangeEntityInPeriodClosed)
            {
                throw new UserFriendlyException("Bạn cần bật config trong [Admin > Cài đặt][Cho phép thay đổi dữ liệu trong kì đã được đóng]");
            }
        }
        private void CheckAndSetMustHavePeriodIdProperty(object entityAsObj, long? userId)
        {
            IMayHaveTenant mayHaveTenant = entityAsObj.As<IMayHaveTenant>();
            if (!mayHaveTenant.TenantId.HasValue)
            {
                mayHaveTenant.TenantId = GetCurrentTenantIdOrNull();
            }
            base.SetCreationAuditProperties(entityAsObj, userId);
            if (SuppressAutoSetTenantId || !(entityAsObj is IMustHavePeriod))
            {
                return;
            }
            IMustHavePeriod mustHavePeriod = entityAsObj.As<IMustHavePeriod>();
            if (mustHavePeriod.PeriodId <= 0)
            {
                if (!CurrentPeriodId.HasValue || CurrentPeriodId.Value == 0)
                {
                    return;
                }
                mustHavePeriod.PeriodId = CurrentPeriodId.Value;
            }
        }
        private int? GetCurrentPeriodIdOrNull()
        {
            try
            {
                var periodId = _periodResolveContributor.ResolvePeriodId();
                if(periodId.HasValue)
                    return periodId;
               
                var currentPeriodId = GetPeriodOnActive();
                return currentPeriodId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private int GetPeriodOnActive()
        {
            var tenantId = GetCurrentTenantIdOrNull();
            return Periods.Where(x => x.IsActive && x.TenantId == tenantId).Select(x => x.Id).FirstOrDefault();
        }
        #endregion
    }
}
