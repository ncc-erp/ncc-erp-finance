using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FinanceManagement.Tests.Seeders
{
    public class DataSeederConsumer
    {
        public void Seed(FinanceManagementDbContext context)
        {
            var iseederType = typeof(ISeeder);
            var seederTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsInterface && t.IsAssignableTo(iseederType));
            foreach (var seederType in seederTypes)
            {
                var instance = (ISeeder)Activator.CreateInstance(seederType);
                instance.Seed(context);
            }
        }
    }
}
