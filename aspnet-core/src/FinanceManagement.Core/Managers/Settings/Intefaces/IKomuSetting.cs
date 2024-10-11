using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface IKomuSetting : INotificationSetting
    {
        string GetNotifyKomuChannelId(int? tenantId = int.MinValue);
        Task<string> GetNotifyKomuChannelIdAsync(int? tenantId = int.MinValue);
        void SetNotifyKomuChannelId(string komuChannelId);
        Task SetNotifyKomuChannelIdAsync(string komuChannelId);
    }
}
