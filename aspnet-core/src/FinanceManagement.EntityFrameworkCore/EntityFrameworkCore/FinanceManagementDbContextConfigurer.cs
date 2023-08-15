using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManagement.EntityFrameworkCore
{
    public static class FinanceManagementDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FinanceManagementDbContext> builder, string connectionString, LoggerFactory logger)
        {
            builder.UseSqlServer(connectionString);
            builder.UseLoggerFactory(logger);
        }

        public static void Configure(DbContextOptionsBuilder<FinanceManagementDbContext> builder, DbConnection connection, LoggerFactory logger)
        {
            builder.UseSqlServer(connection);
            builder.UseLoggerFactory(logger);
        }
    }
}
