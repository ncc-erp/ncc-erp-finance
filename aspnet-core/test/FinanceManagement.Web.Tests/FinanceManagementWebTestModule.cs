using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace FinanceManagement.Web.Tests
{
    [DependsOn(
        typeof(FinanceManagementWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class FinanceManagementWebTestModule : AbpModule
    {
        public FinanceManagementWebTestModule(FinanceManagementEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FinanceManagementWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(FinanceManagementWebMvcModule).Assembly);
        }
    }
}