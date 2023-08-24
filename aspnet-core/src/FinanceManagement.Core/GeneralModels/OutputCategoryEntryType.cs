using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class OutputCategoryEntryType
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public bool IsActive { get; set; }
    }
}
