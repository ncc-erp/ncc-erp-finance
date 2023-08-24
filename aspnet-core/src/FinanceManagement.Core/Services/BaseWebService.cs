using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using FinanceManagement.MultiTenancy;
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
        protected readonly ILogger _logger;
        public readonly IAbpSession _session;
        private readonly TenantManager _tenantManager;
        public BaseWebService(HttpClient httpClient, TenantManager tenantManager, IAbpSession abpSession)
        {
            _httpClient = httpClient;
            _logger = NullLogger.Instance;
            _tenantManager = tenantManager;
            _session = abpSession;
            AddAbpTenantNameHeaders();
        }
        protected virtual void Post(string url, object input)
        {
            var fullUrl = $"{_httpClient.BaseAddress}/{url}";
            string strInput = JsonConvert.SerializeObject(input);
            try
            {
                _logger.Info($"Post: {fullUrl} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
                _httpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
                _logger.Error($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
            }
        }
        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var fullUrl = $"{_httpClient.BaseAddress}/{url}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.Info($"Get: {url} response: { responseContent}");

                JObject responseJObj = JObject.Parse(responseContent);
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseJObj));
            }
            catch (Exception ex)
            {
                _logger.Error($"Post: {fullUrl} Error: {ex.Message}");
            }
            return default;
        }
        protected virtual async Task<T> PostAsync<T>(string url, object input)
        {
            string strInput = JsonConvert.SerializeObject(input);
            var fullUrl = $"{_httpClient.BaseAddress}/{url}";
            try
            {
                _logger.Info($"Post: {fullUrl} input: {strInput}");
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, contentString);
                string responseContent = await response.Content.ReadAsStringAsync();

                _logger.Info($"Post: {fullUrl} input: {strInput} response: {responseContent}");
                JObject responseJObj = JObject.Parse(responseContent);
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseJObj)); ;
            }
            catch (Exception e)
            {
                _logger.Error($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
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
