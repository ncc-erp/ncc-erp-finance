using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement
{
    public class MMNConfigManager
    {
        public static bool EnableCrawlBTransactionMezonDong { get; set; } = true;
        public static int IntervalMilisecond { get; set; } = 600000;
        public static string BaseAddress { get; set; }

        public static void Load(IConfiguration config)
        {
            var section = config.GetSection("CrawlBTransactionMezonDong");

            EnableCrawlBTransactionMezonDong = section.GetValue<bool?>("EnableCrawlBTransactionMezonDong") ?? true;
            IntervalMilisecond = section.GetValue<int?>("IntervalMilisecond") ?? 600000;
            BaseAddress = section.GetValue<string>("BaseAddress");
        }
    }
}
