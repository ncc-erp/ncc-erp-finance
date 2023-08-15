using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using FinanceManagement.Authorization;

namespace FinanceManagement
{
    [DependsOn(
        typeof(FinanceManagementCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class FinanceManagementApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<FinanceManagementAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FinanceManagementApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
