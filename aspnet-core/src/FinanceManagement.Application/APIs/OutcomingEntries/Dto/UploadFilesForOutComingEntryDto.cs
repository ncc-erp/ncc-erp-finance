
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class UploadFilesForOutComingEntryDto
    {
        public long OutcomingEntryId { get; set; }
        public IFormFile file { get; set; }
    }
}
