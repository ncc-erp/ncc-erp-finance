using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class UpdateReportDateDto
    {
        public long OutcomingEntryId { get; set; }
        [Required]
        public DateTime ReportDate { get; set; }
    }
}
