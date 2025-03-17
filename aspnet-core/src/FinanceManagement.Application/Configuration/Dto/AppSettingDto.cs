using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Configuration.Dto
{
    public class AppSettingDto
    {
        public bool EnableLoginGoogle { get; set; }
        public string ClientAppId { get; set; }
        public string SecretKey { get; set; }
        public string NotificationPlatform { get; set; }
        public string NotifyToChannel { get; set; }
        public bool EnableNormalLogin { get; set; }
        public bool EnableLoginMezon {  get; set; }
    }

    public class ClienAppDto
    {
        public string ClientAppId { get; set; }
    }

    public class SecretKeyDto
    {
        public string SecretKey { get; set; }
    }
    public class NotifyToChannelDto
    {
        public string NotificationPlatform { get; set; }
        public string NotifyToChannel { get; set; }
    }
}
