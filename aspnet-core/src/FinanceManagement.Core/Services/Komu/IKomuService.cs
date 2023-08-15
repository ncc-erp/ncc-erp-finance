using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Komu
{
    public interface IKomuService
    {
        void NotifyToChannel(string komuMessage, string channelId);
        Task NotifyToChannelAsync(string komuMessage, string channelId);
    }
}
