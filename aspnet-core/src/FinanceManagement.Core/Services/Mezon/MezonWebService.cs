using Abp.Runtime.Session;
using FinanceManagement.MultiTenancy;
using FinanceManagement.Notifications.Mezon.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public class MezonWebService : BaseWebService
    {
        private readonly string _mezonDevChannelUrl;
        private readonly string _isNotifyToMezon;
        public MezonWebService(HttpClient httpClient, IConfiguration configuration, TenantManager tenantManager, IAbpSession abpSession) 
            : base(httpClient, tenantManager, abpSession)
        {
            _mezonDevChannelUrl = configuration.GetValue<string>("Mezon:DevModeUrl");
            _isNotifyToMezon = configuration.GetValue<string>("Mezon:EnableMezonNotification");
        }

        public void NotifyToChannelMezon(MezonMessage message, string channelId)
        {
            if (_isNotifyToMezon != "true")
            {
                _logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? channelId : _mezonDevChannelUrl;
            Post(channelIdToSend, new { type = "hook", message = message });
        }
        public async Task NotifyToChannelMezonAsync(MezonMessage message, string channelId)
        {
            if (_isNotifyToMezon != "true")
            {
                _logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? channelId : _mezonDevChannelUrl;
            await PostAsync<object>(channelIdToSend, new { type = "hook", message = message });
        }
    }
}
