using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using FinanceManagement.MultiTenancy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Services
{
    public class BaseWebService
    {
        protected readonly HttpClient _httpClient;
		protected readonly ILogger<BaseWebService> Logger;
		public readonly IAbpSession _session;
        private readonly TenantManager _tenantManager;
        public BaseWebService(HttpClient httpClient, TenantManager tenantManager, IAbpSession abpSession)
        {
            _httpClient = httpClient;
			Logger = IocManager.Instance.Resolve<ILogger<BaseWebService>>();
			_tenantManager = tenantManager;
            _session = abpSession;
            AddAbpTenantNameHeaders();
        }

		public void SetAuthorizationToken(string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}
		}
		protected virtual void Post(string url, object input)
        {
            var fullUrl = url.StartsWith("http") ? url : $"{_httpClient.BaseAddress}/{url}";
            string strInput = JsonConvert.SerializeObject(input);
            try
            {
				Logger.LogInformation($"Post: {fullUrl} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
                _httpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
				Logger.LogError($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
            }
        }

        protected virtual async Task<T> PostFormUrlEncodedAsync<T>(string url, Dictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            try
            {
                var response = await _httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
				Logger.LogError($"Post: {url}  Error: {ex.Message}");
            }
            return default;
        }
        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var fullUrl = url.StartsWith("http") ? url : $"{_httpClient.BaseAddress}/{url}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();
				Logger.LogInformation($"Get: {fullUrl} response: { responseContent}");

                JObject responseJObj = JObject.Parse(responseContent);
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseJObj));
            }
            catch (Exception ex)
            {
				Logger.LogError($"Post: {fullUrl} Error: {ex.Message}");
            }
            return default;
        }
        protected virtual async Task<T> PostAsync<T>(string url, object input)
        {
            string strInput = JsonConvert.SerializeObject(input);
            var fullUrl = url.StartsWith("http") ? url : $"{_httpClient.BaseAddress}/{url}";
            try
            {
				Logger.LogInformation($"Post: {fullUrl} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, contentString);
                string responseContent = await response.Content.ReadAsStringAsync();

				Logger.LogInformation($"Post: {fullUrl} input: {strInput} response: {responseContent}");
                JObject responseJObj = JObject.Parse(responseContent);
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseJObj)); ;
            }
            catch (Exception e)
            {
				Logger.LogError($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
            }

            return default;
        }
        protected virtual void AddAbpTenantNameHeaders()
        {
            if (!_session.TenantId.HasValue)
                return;
            var tenant = _tenantManager.GetById(_session.TenantId.Value);
            if (tenant == null)
                return;
            _httpClient.DefaultRequestHeaders.Add("Abp-TenantName", tenant.TenancyName);
        }
    }
}
