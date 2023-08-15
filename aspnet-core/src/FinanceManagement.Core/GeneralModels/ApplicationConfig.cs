using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class ApplicationConfig
    {
        public string ServerRootAddress { get; set; } = "";
        public string ClientRootAddress { get; set; } = "";
        public string CorsOrigins { get; set; }
    }
}
