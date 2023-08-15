using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Services.Project.Dto
{
    public class AuthenticateResultDto
    {
        public LoginResultDto Result { get; set; }
    }

    public class LoginResultDto
    {
        public string AccessToken { get; set; }
        public string EncryptedAccessToken { get; set; }
        public long ExpireInSeconds { get; set; }
        public long UserId { get; set; }
    }
}
