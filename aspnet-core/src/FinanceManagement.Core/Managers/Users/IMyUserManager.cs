using Abp.Dependency;
using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Users
{
    public interface IMyUserManager : IDomainService, ITransientDependency
    {
        Task<Dictionary<long, string>> GetDictionaryUserAudited(IEnumerable<long> userIds);
    }
}
