using FinanceManagement.Uitls;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Notifications.Mezon.Dto
{
    public class MezonMessage
    {
        public string t { set; get; }
        public List<MK> mk => MezonUtil.GetListIndexOfLinks(t);
        public List<Mentions> mentions { set; get; }

    }
    public class MK
    {
        public string type { set; get; }
        public int s { set; get; }
        public int e { set; get; }
    }
    public class Mentions
    {
        public string username { set; get; }
        public int s { set; get; }
        public int e => s + username.Length + 1;
    }
}
