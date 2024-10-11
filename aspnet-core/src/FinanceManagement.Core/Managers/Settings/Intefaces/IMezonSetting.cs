using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface IMezonSetting : INotificationSetting
    {
        string GetNotifyMezonChannelUrl(int? tenantId = int.MinValue);
        Task<string> GetNotifyMezonChannelUrlAsync(int? tenantId = int.MinValue);
        void SetNotifyMezonChannelUrl(string channelUrl);
        Task SetNotifyMezonChannelUrlAsync(string channelUrl);
    }
}
