using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class KomuNotificationConfig
    {
        public bool EnableKomuNotification { get; set; }
        public string DevModeChannelId { get; set; }
        public bool EnableCrawlBTransactionNoti { get; set; } = false;
    }
}
