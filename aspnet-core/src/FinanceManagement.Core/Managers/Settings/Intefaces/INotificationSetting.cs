using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface INotificationSetting
    {
        Task<string> GetEnableCrawlBTransactionNoti(int? tenantId = int.MinValue);
        Task SetEnableCrawlBTransactionNoti(bool isEnable);
        void SetNotifySetting(string platform, string notifyToChannel);
        Task SetNotifySettingAsync(string platform, string notifyToChannel);
        string GetNotifyPlatformSetting(int? tenantId);
        Task<string> GetNotifyPlatformSettingAsync(int? tenantId);
    }
}
