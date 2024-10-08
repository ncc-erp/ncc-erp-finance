using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public interface IMezonWebService
    {
        void NotifyToChannel(string mezonUrl, string mezonMessage);
        Task NotifyToChannelAsync(string komuMessage, string channelId);
    }
}
