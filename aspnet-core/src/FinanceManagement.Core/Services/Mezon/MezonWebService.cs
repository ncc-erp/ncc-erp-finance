using Abp.Runtime.Session;
using FinanceManagement.MultiTenancy;
using FinanceManagement.Notifications.Mezon.Dto;
using FinanceManagement.Services.Mezon.Dto;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IConfiguration _configuration;
        public MezonWebService(HttpClient httpClient, IConfiguration configuration, TenantManager tenantManager, IAbpSession abpSession) 
            : base(httpClient, tenantManager, abpSession)
        {
            _mezonDevChannelUrl = configuration.GetValue<string>("Mezon:DevModeUrl");
            _isNotifyToMezon = configuration.GetValue<string>("Mezon:EnableMezonNotification");
            _configuration = configuration;
        }

		public async Task<AuthOauth2Mezon> GetTokenForOauth2Mezon(string code)
		{
			var url = _configuration.GetValue<string>("Oauth2Mezon:Url_Oauth2Mezon");
			var urlInfo = _configuration.GetValue<string>("Oauth2Mezon:Url_UserInfo");
			var client_id = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
			var client_secret = _configuration.GetValue<string>("Oauth2Mezon:Client_Secret");
			var grant_type = _configuration.GetValue<string>("Oauth2Mezon:Grant_Type");
			var redirect_uri = _configuration.GetValue<string>("Oauth2Mezon:Redirect_URI");

			var formData = new Dictionary<string, string>
			{
			   { "client_id", client_id },
					   { "client_secret", client_secret },
					   { "grant_type", grant_type },
					   { "redirect_uri", redirect_uri },
					   { "code", code }
			 };

			var response = await PostFormUrlEncodedAsync<TokenResponse>(url, formData);
			SetAuthorizationToken(response.AccessToken);

			var infoAuth = await PostAsync<AuthOauth2Mezon>(urlInfo, null);

			return infoAuth;
		}

		public void NotifyToChannel(string mezonUrl, string mezonMessage)
        {
            if (_isNotifyToMezon != "true")
            {
				Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? mezonUrl : _mezonDevChannelUrl;
            Post(channelUrlToSend, new { type = "FINFAST", message = new {username="Finfast", t = mezonMessage } });
        }

        public async Task NotifyToChannelAsync(string mezonUrl, string mezonMessage)
        {
            if (_isNotifyToMezon != "true")
            {
				Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? mezonUrl : _mezonDevChannelUrl;
            await PostAsync<object>(channelUrlToSend, new { type = "FINFAST", message = new { t = mezonMessage } });
        }
        public void NotifyToChannelMezon(MezonMessage message, string channelId)
        {
            if (_isNotifyToMezon != "true")
            {
				Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? channelId : _mezonDevChannelUrl;
            Post(channelIdToSend, new { type = "hook", message = message });
        }
        public async Task NotifyToChannelMezonAsync(MezonMessage message, string channelId)
        {
            if (_isNotifyToMezon != "true")
            {
				Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? channelId : _mezonDevChannelUrl;
            await PostAsync<object>(channelIdToSend, new { type = "hook", message = message });
        }
    }
}
