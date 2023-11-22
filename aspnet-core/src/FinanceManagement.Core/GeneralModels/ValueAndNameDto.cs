using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class ValueAndNameDto
    {
        public long Value { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; }

    }
}
