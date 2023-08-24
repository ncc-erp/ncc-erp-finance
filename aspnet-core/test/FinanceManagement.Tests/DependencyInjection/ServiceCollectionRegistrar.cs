using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Abp.Dependency;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Identity;
using Microsoft.Extensions.Logging;
using FinanceManagement.Logging;
using Castle.Core.Logging;

namespace FinanceManagement.Tests.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            services.AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<FinanceManagementDbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseLazyLoadingProxies()
            .UseLoggerFactory(GetDbLoggerFactory());
            //.UseInternalServiceProvider(serviceProvider);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<FinanceManagementDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }
        private static LoggerFactory GetDbLoggerFactory()
        {
            return new LoggerFactory(new[] { new MyLoggerProvider(NullLogger.Instance) });
        }
    }
}
