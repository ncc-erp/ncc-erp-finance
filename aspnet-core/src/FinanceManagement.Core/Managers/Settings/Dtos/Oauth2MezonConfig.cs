using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.Settings.Dtos
{
	public class Oauth2MezonConfig
	{
		public string Client_Id { set; get; }
		public string Client_Secret { set; get; }
		public string Grant_Type { get; set; }
		public string Redirect_URI { get; set; }
		public string Url_Oauth2Mezon { get; set; }
		public string Url_UserInfo { get; set; }
		public bool EnableLoginMezon { get; set; }
	}

	public class ConfigEnableLogin
	{
		public bool EnableNormalLogin { get; set; }
		public bool EnableLoginMezon { set; get; }
		public bool EnableLoginGoogle { get; set; }
	}
}
