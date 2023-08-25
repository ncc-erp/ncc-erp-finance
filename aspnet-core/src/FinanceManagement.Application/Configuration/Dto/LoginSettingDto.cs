using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Configuration.Dto
{
    public class LoginSettingDto
    {
        public string GoogleClientId { get; set; }
        public bool EnableNormalLogin { get; set; }
    }
}
