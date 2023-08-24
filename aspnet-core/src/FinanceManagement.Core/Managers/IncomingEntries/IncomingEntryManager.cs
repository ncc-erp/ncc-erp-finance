using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.IncomingEntries
{
    public class IncomingEntryManager : DomainManager, IIncomingEntryManager
    {
        public IncomingEntryManager(IWorkScope ws) : base(ws)
        {
        }
        public async Task<long> CreateIncomingEntry(CreateIncomingEntryDto input)
        {
            var id = await _ws.InsertAndGetIdAsync(ObjectMapper.Map<IncomingEntry>(input));
            await CurrentUnitOfWork.SaveChangesAsync();
            return id;
        }
    }
}
