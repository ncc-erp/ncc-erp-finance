using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceManagement.Helper
{

    public static class CrawlBTransactionHelper
    {

        public static BTransInfo ExtractBTransaction(string input)
        {
            var info = new BTransInfo();

            // 1. Tìm số tài khoản
            var accountMatch = Regex.Match(input, @"(?:Tai khoan Thanh toan|TK)[^\d]*(\d+)");
            if (accountMatch.Success)
                info.AccountNumber = accountMatch.Groups[1].Value;

            // 2. Tìm biến động số dư
            var transactionMatch = Regex.Match(input, @"(?:^|\n)([+-])\s*VND\s*([\d,]+(?:\.\d+)?)|So tien GD:([+-][\d,]+(?:\.\d+)?)");
            if (transactionMatch.Success)
            {
                string rawAmount = "";
                if (transactionMatch.Groups[1].Success && transactionMatch.Groups[2].Success)
                {
                    // Dạng: "+ VND 23,464,309"
                    rawAmount = transactionMatch.Groups[1].Value + transactionMatch.Groups[2].Value;
                }
                else if (transactionMatch.Groups[3].Success)
                {
                    // Dạng: "So tien GD:-9,868,504"
                    rawAmount = transactionMatch.Groups[3].Value;
                }

                // Chuyển sang double
                rawAmount = rawAmount.Replace(",", "");
                if (double.TryParse(rawAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    info.TransactionAmount = amount;
                }
            }

            // 3. Tìm số dư
            var balanceMatch = Regex.Match(input, @"So du:\s*(?:VND\s*)?([\d,]+(?:\.\d+)?)");
            if (balanceMatch.Success)
            {
                string balanceRaw = balanceMatch.Groups[1].Value.Replace(",", "");
                if (double.TryParse(balanceRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out double balance))
                {
                    info.Balance = balance;
                }
            }

            return info;
        }

    }
    public class BTransInfo
    {
        public string AccountNumber { get; set; }
        public double TransactionAmount { get; set; }
        public double Balance { get; set; }
    }

    

}
