using Abp.Dependency;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.IncomingEntries
{
    public interface IIncomingEntryManager : ITransientDependency
    {
        Task<long> CreateIncomingEntry(CreateIncomingEntryDto input);
        IQueryable<IncomingEntryDto> BuildIncomingQuery();
    }

}
