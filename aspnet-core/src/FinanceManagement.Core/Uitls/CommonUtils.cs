using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Uitls
{
    public static class CommonUtils
    {
        public static string GetCurrencyColor(string currency)
        {
            if(currency == "USD")
            {
                return "blue";
            }
            return "black";
        }
    }
}
