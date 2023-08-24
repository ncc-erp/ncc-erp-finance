using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class ApplicationConfig
    {
        public string ServerRootAddress { get; set; } = "http://stg-api-finfast.nccsoft.vn/";
        public string ClientRootAddress { get; set; } = "http://stg-finfast.nccsoft.vn/";
        public string CorsOrigins { get; set; }
    }
}
