using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Ncc
{
    public interface IPeriodResolveContributor : ITransientDependency
    {
        int? ResolvePeriodId();
    }
}
