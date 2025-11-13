using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement
{
    public class CrawlMezonDongConfig
    {
        public static bool EnableCrawlBTransactionMezonDong { get; set; } = true;
        public static int IntervalSecond { get; set; } = 60;
        public static string BaseAddress { get; set; }
        public static int Limit { get; set; } = 100;

        public static void Load(IConfiguration config)
        {
            var section = config.GetSection("CrawlBTransactionMezonDong");

            EnableCrawlBTransactionMezonDong = section.GetValue<bool?>("EnableCrawlB") ?? true;
            IntervalSecond = section.GetValue<int?>("IntervalSecond") ?? 60;
            BaseAddress = section.GetValue<string>("BaseAddress");
            Limit = section.GetValue<int?>("Limit") ?? 100;
        }
    }
}
