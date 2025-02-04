using FinanceManagement.Notifications.Mezon.Dto;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceManagement.Uitls
{
    public class MezonUtil
    {
        public static List<MK> GetListIndexOfLinks(string message)
        {
            string pattern = @"https?://[^\s]+";
            List<MK> links = new List<MK>();
            MatchCollection matches = Regex.Matches(message, pattern);
            var tmp = 0;
            foreach (Match match in matches)
            {
                links.Add(new MK
                {
                    type = "lk",
                    s = match.Index,
                    e = match.Index + match.Length,
                });
                tmp = match.Index + match.Length - 1;
            }
            var pos = message.IndexOf(".");
            if (pos == -1)
            {
                links.Add(new MK
                {
                    type = "t",
                    s = pos,
                    e = message.Length,
                });
                return links;
            }
            tmp = tmp == 0 ? pos : tmp;
            links.Add(new MK
            {
                type = "t",
                s = tmp,
                e = message.Length ,
            });
            return links;
        }
    }
}
