using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class MezonNotificationConfig
    {
        public bool EnableMezonNotification { get; set; }
        public string DevModeUrl { get; set; }
        public bool EnableCrawlBTransactionNoti { get; set; } = false;
    }
}
