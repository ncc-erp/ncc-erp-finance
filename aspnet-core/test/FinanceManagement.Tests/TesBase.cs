using Abp.Domain.Uow;
using Abp.Modules;
using Abp.TestBase;
using FinanceManagement.Authorization;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.EntityFrameworkCore.Seed.Host;
using FinanceManagement.EntityFrameworkCore.Seed.Tenants;
using FinanceManagement.Tests.Seeders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FinanceManagement.Tests
{
    public abstract class TesBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
        where TStartupModule : AbpModule
    {
        protected TesBase()
        {
            SeedUserData();
            SeedData();
            LoginAsHostAdmin();
        }

        public void UsingDbContext(Action<FinanceManagementDbContext> action)
        {
            using (var context = Resolve<FinanceManagementDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
        {
            using (var scope = Resolve<IServiceProvider>().CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using (var uow = uowManager.Begin())
                {
                    var result = await func();
                    await uow.CompleteAsync();
                    return result;
                }
            }
        }

        protected virtual async Task WithUnitOfWorkAsync(Func<Task> func)
        {
            using (var scope = Resolve<IServiceProvider>().CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using (var uow = uowManager.Begin())
                {
                    await func();
                    await uow.CompleteAsync();
                }
            }
        }

        private void SeedData()
        {
            UsingDbContext((context) =>
            {
                new DataSeederConsumer().Seed(context);
            });
        }

        private void SeedUserData()
        {
            UsingDbContext((context) =>
            {
                new InitialHostDbBuilder(context).Create();
                new DefaultTenantBuilder(context).Create();
            });
        }

        private void LoginAsHostAdmin()
        {
            var logInManager = Resolve<LogInManager>();
            var loginResult = logInManager.LoginAsync("admin", "123qwe").Result;
            AbpSession.UserId = loginResult.User.Id;
            AbpSession.TenantId = loginResult.User.TenantId;
        }
    }
}
