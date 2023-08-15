using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinanceManagement.Services.Komu;
using FinanceManagement.Services.Project;
using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Services.Firebase;
using FinanceManagement.Services.HRM;

namespace FinanceManagement.Services
{
    /// <summary>
    /// WebServiceRegistrar is defined to add infrastructure for ERP Serivces using HttpClient 
    /// </summary>
    public static class WebServiceRegistrar
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfigurationRoot _appConfiguration)
        {
            services.AddHttpClient<IKomuService, KomuService>(options =>
             {
                 options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("KomuService:BaseAddress"));
                 options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("KomuService:SecurityCode"));
             });
            services.AddHttpClient<ProjectService>(options =>
            {
                options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("ProjectService:BaseAddress"));
                options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("ProjectService:SecurityCode"));
            });
            services.AddHttpClient<HRMService>(options =>
            {
                options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("HRMService:BaseAddress"));
                options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("HRMService:SecurityCode"));
            });
            services.AddHttpClient<FirebaseService>(options =>
             {
                 options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("Firebase:Url"));
             });
            return services;
        }
    }
}
