using Abp.Modules;
using FinanceManagement.Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Application.Test
{
    [DependsOn(typeof(FinfastCoreTestModule))]
    public class FinfastApplicationTestModule : AbpModule
    {
    }
}
