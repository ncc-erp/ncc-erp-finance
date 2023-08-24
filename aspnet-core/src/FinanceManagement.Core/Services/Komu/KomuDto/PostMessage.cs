using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinanceManagement.Services.Komu.KomuDto
{
    public class PostMessage
    {
        public string channel { get; set; }
        public string text { get; set; }
        [JsonPropertyName("alias")]
        public string alias { get; set; }
        public List<attachment> attachments { get; set; }
    }
    public class attachment
    {
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonProperty(PropertyName = "title_link")]
        public string titlelink { get; set; }
    }
    public class HeaderIfRequerid
    {
        [JsonPropertyName("authToken")]
        public string AuthToken { get; set; }
        [JsonPropertyName("userId")]
        public string userId { get; set; }
    }
}
