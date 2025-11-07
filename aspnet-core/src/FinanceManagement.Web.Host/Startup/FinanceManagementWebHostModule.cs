using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using FinanceManagement.Configuration;
using FinanceManagement.GeneralModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FinanceManagement.Web.Host.Startup
{
    [DependsOn(
       typeof(FinanceManagementWebCoreModule))]
    public class FinanceManagementWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public FinanceManagementWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.TenantIdResolveKey = "Abp-TenantId";
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FinanceManagementWebHostModule).GetAssembly());
        }
        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            if (FinfastStatics.EnableFirebaseService)
            {
                workManager.Add(IocManager.Resolve<CrawlBTransactionBackgroundWorker>());
            }
            if (MMNConfigManager.EnableCrawlBTransactionMezonDong)
            {
                workManager.Add(IocManager.Resolve<CrawlBTransactionMezonDongBackgroundWorker>());
            }
        }
    }
}
