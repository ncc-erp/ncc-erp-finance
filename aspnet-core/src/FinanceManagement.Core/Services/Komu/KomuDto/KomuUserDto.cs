using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Services.Komu.KomuDto
{
    public class KomuUserDto
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("userid")]
        public ulong? KomuUserId { get; set; }
    }
}
