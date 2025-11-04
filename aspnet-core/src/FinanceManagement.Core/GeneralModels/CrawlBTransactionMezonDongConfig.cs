using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class CrawlBTransactionMezonDongConfig
    {
        public int IntervalMilisecond { get; set; } = 600000;
        public string BaseAddress { get; set; }
        public bool EnableCrawlBTransactionMezonDong { get; set; }
        public int DaysToLookBack { get; set; } = 30;
    }
}
