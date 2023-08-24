using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class UpdateOutcomEntryTypeDto
    {
        public long OutcomingEntryId { get; set; }
        [Required]
        public long OutcomingEntryTypeId { get; set; }
    }
}
