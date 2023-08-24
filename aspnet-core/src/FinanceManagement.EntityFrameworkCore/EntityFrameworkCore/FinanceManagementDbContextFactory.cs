using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using FinanceManagement.Configuration;
using FinanceManagement.Web;
using Microsoft.Extensions.Logging;
using FinanceManagement.Logging;
using Castle.Core.Logging;

namespace FinanceManagement.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class FinanceManagementDbContextFactory : IDesignTimeDbContextFactory<FinanceManagementDbContext>
    {
        private LoggerFactory GetDbLoggerFactory()
        {
            return new LoggerFactory(new[] { new MyLoggerProvider(NullLogger.Instance) });
        }
        public FinanceManagementDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FinanceManagementDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
            var logger = GetDbLoggerFactory();

            FinanceManagementDbContextConfigurer.Configure(builder, configuration.GetConnectionString(FinanceManagementConsts.ConnectionStringName), logger);

            return new FinanceManagementDbContext(builder.Options, null, null);
        }
    }
}
