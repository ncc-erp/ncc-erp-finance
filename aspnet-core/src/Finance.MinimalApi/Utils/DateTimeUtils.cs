using Abp.Timing;

namespace Finance.MinimalApi.Utils
{
    public class DateTimeUtils
    {
        // All now function use Clock.Provider.Now
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }
    }
}
