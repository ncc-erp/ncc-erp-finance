using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Castle.Core.Logging;
using FinanceManagement.EntityFrameworkCore.Seed;
using FinanceManagement.Logging;
using Microsoft.Extensions.Logging;

namespace FinanceManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(FinanceManagementCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class FinanceManagementEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }
        private LoggerFactory GetDbLoggerFactory()
        {
            return new LoggerFactory(new[] { new MyLoggerProvider(NullLogger.Instance) });
        }
        public override void PreInitialize()
        {
            var logger = GetDbLoggerFactory();
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<FinanceManagementDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        FinanceManagementDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection, logger);
                    }
                    else
                    {
                        FinanceManagementDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString, logger);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FinanceManagementEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
