using Abp.Runtime.Session;
using FinanceManagement.MultiTenancy;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services.Mezon
{
    public class MezonWebService : BaseWebService, IMezonWebService
    {
        private readonly string _mezonDevChannelUrl;
        private readonly string _isNotifyToMezon;
        public MezonWebService(HttpClient httpClient, IConfiguration configuration, TenantManager tenantManager, IAbpSession abpSession) 
            : base(httpClient, tenantManager, abpSession)
        {
            _mezonDevChannelUrl = configuration.GetValue<string>("Mezon:DevModeUrl");
            _isNotifyToMezon = configuration.GetValue<string>("Mezon:EnableMezonNotification");
        }

        public void NotifyToChannel(string mezonUrl, string mezonMessage)
        {
            if (_isNotifyToMezon != "true")
            {
                _logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? mezonUrl : _mezonDevChannelUrl;
            Post(channelUrlToSend, new { type = "hook", message = mezonMessage });
        }

        public void NotifyToChannelMezon(string mezonUrl, OutcomingEntryMessageDto mezonMessage)
        {
            if (_isNotifyToMezon != "true")
            {
                _logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = mezonUrl;
            Post(channelUrlToSend, new { type = "hook", message = mezonMessage });
        }

        public async Task NotifyToChannelAsync(string mezonUrl, string mezonMessage)
        {
            if (_isNotifyToMezon != "true")
            {
                _logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? mezonUrl : _mezonDevChannelUrl;
            await PostAsync<object>(channelUrlToSend, new { type = "FINFAST", message = new { t = mezonMessage } });
        }
    }
}
