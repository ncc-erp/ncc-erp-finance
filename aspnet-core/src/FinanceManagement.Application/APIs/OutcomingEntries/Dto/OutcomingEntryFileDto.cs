

using Abp.AutoMapper;
using Abp.Domain.Entities;
using FinanceManagement.Enums;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    [AutoMapTo(typeof(OutcomingEntryFileDto))]
    public class OutcomingEntryFileDto : Entity<long>
    {
        public long OutcomingEntryId { get; set; }
        public string FilePath { get; set; }
        public OutcomingEntryFileStatus isAcceptFile { get; set; }
    }
}
