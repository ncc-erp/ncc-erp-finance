using Abp.Modules;
using FinanceManagement.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Core.Tests
{
    [DependsOn(typeof(FinanceManagementTestModule))]
    public class FinfastCoreTestModule : AbpModule
    {
    }
}
