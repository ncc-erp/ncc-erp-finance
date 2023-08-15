using Abp.Configuration;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;


namespace FinanceManagement.Helper
{
    public class Credential
    {
        private ISettingManager settingManager;
        private IHostingEnvironment _hostingEnvironment;
        public Credential()
        {
            this.settingManager = IocManager.Instance.Resolve<ISettingManager>();
            this._hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
        }
        /*
        public async Task<CalendarService> CredentialAsync(bool isAdmin, string user)
        {
            return await Task.Run(() =>
            {
                string[] Scopes;

                if (isAdmin)
                {
                    Scopes = new string[] {
                        CalendarService.Scope.Calendar,
                        CalendarService.Scope.CalendarEvents,

                        CalendarService.Scope.CalendarEventsReadonly
                    };
                }
                else
                {
                    Scopes = new string[] 
                    {
                        CalendarService.Scope.CalendarEventsReadonly,
                        CalendarService.Scope.Calendar,
                        CalendarService.Scope.CalendarEvents,
                    };
                }
                UserCredential userCredential;
                var filepath = Path.Combine(_hostingEnvironment.WebRootPath, "credential", "credentials.json");

                string credPath = Path.Combine(_hostingEnvironment.WebRootPath, "credential", "token.json");
                userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    //GoogleClientSecrets.Load(stream).Secrets,
                    new ClientSecrets
                    Scopes,
                    $"{user}",
                    CancellationToken.None,
                    new FileDataStore(credPath)).Result;
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = "Ims",
                });
                return service;

            });
        }*/
    }
}
