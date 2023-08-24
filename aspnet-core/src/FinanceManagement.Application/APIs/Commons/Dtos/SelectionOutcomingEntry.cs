using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Commons.Dtos
{
    public class SelectionOutcomingEntry : ValueAndNameDto
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string TypeName { get; set; }
        public long TypeId { get; set; }
        public double Money { get; set; }
    }
}
