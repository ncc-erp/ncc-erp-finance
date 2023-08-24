using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinanceManagement.Services.Komu.KomuDto
{
    public class LoginJsonPrase
    {
        [JsonPropertyName("status")]
        public string status { get; set; }
        [JsonPropertyName("data")]
        public HeaderIfRequerid data { get; set; }
    }
}
