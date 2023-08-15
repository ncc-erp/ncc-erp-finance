using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class ImportFileOutcomingEntryDetailDto
    {
        public long OutcomingEntryId { get; set; }
        public IFormFile FileInput { get; set; }
    }
}
