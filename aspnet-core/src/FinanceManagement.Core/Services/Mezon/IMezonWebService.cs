using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml;

namespace FinanceManagement.Services.Mezon
{
    public interface IMezonWebService
    {
        void NotifyToChannel(string mezonUrl, string mezonMessage);
        void NotifyToChannelMezon(string mezonUrl, OutcomingEntryMessageDto mezonMessage);
        Task NotifyToChannelAsync(string komuMessage, string channelId);
    }
}
