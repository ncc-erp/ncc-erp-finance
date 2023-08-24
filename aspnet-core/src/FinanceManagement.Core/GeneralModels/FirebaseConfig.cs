using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class FirebaseConfig
    {
        public int IntervalMilisecond { get; set; } = 600000;
        public bool RunFirebaseBackgroundService { get; set; }
        public string SecretKey { get; set; }
        public string Url { get; set; }
    }
}
