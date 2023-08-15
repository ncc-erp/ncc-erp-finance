using FinanceManagement.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FinanceManagement.Services.Komu.KomuDto;
using FinanceManagement.MultiTenancy;
using Abp.Runtime.Session;

namespace FinanceManagement.Services.Komu
{
    public class KomuService : BaseWebService, IKomuService
    {
        private readonly string _channelIdDevMode;
        private readonly string _isNotifyToKomu;

        public KomuService(HttpClient httpClient, IConfiguration configuration, TenantManager tenantManager, IAbpSession abpSession) 
            : base(httpClient, tenantManager, abpSession)
        {
            _channelIdDevMode = configuration.GetValue<string>("KomuService:DevModeChannelId");
            _isNotifyToKomu = configuration.GetValue<string>("KomuService:EnableKomuNotification");
        }
        public async Task<ulong?> GetKomuUserId(string userName)
        {
            var komuUser = await PostAsync<KomuUserDto>(KomuUrlConstant.KOMU_USERID, new { username = userName });
            if (komuUser != null)
                return komuUser.KomuUserId;

            return default;
        }

        public void NotifyToChannel(string komuMessage, string channelId)
        {
            if (_isNotifyToKomu != "true")
            {
                _logger.Info("_isNotifyToKomu=" + _isNotifyToKomu + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_channelIdDevMode) ? channelId : _channelIdDevMode;
            Post(KomuUrlConstant.KOMU_CHANNELID, new { message = komuMessage, channelid = channelIdToSend });
        }
        public async Task NotifyToChannelAsync(string komuMessage, string channelId)
        {
            if (_isNotifyToKomu != "true")
            {
                _logger.Info("_isNotifyToKomu=" + _isNotifyToKomu + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_channelIdDevMode) ? channelId : _channelIdDevMode;
            await PostAsync<object>(KomuUrlConstant.KOMU_CHANNELID, new { message = komuMessage, channelid = channelIdToSend });
        }
    }
}
