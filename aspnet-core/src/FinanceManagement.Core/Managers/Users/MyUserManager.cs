using FinanceManagement.Authorization.Users;
using FinanceManagement.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Users
{
    public class MyUserManager : DomainManager, IMyUserManager
    {
        public MyUserManager(IWorkScope ws) : base(ws)
        {
        }
        public async Task<Dictionary<long, string>> GetDictionaryUserAudited(IEnumerable<long> userIds)
        {
            var dicUsers = await _ws.GetAll<User>()
                .Where(s => userIds.Contains(s.Id))
                .Select(s => new { s.Id, s.Name })
                .AsNoTracking()
                .ToDictionaryAsync(s => s.Id, s => s.Name);
            return dicUsers;
        }
    }
}
