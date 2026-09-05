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
            var accountMatch = Regex.Match(
                input,
                @"(?:Tai khoan Thanh toan|TK)[^\d]*(\d+)"
            );

            if (accountMatch.Success)
                info.AccountNumber = accountMatch.Groups[1].Value;


            // 2. Tìm biến động số dư
            //
            // Hỗ trợ:
            // + VND 23,464,309
            // - VND 10,000,000
            // So tien GD:+10,951.96
            // So tien GD:-10,000,000
            // So tien GD:+USD 1,123.00
            // So tien GD:-USD 1,123.00
            //
            var transactionMatch = Regex.Match(
                input,
                @"(?:^|\n)([+-])\s*(?:VND|USD)?\s*([\d,]+(?:\.\d+)?)|So tien GD:\s*([+-])\s*(?:VND|USD)?\s*([\d,]+(?:\.\d+)?)"
            );

            if (transactionMatch.Success)
            {
                string rawAmount = "";

                // Dạng:
                // + VND 23,464,309
                // + USD 1,123.00
                if (transactionMatch.Groups[1].Success &&
                    transactionMatch.Groups[2].Success)
                {
                    rawAmount =
                        transactionMatch.Groups[1].Value +
                        transactionMatch.Groups[2].Value;
                }
                // Dạng:
                // So tien GD:-9,868,504
                // So tien GD:+USD 1,123.00
                else if (transactionMatch.Groups[3].Success &&
                        transactionMatch.Groups[4].Success)
                {
                    rawAmount =
                        transactionMatch.Groups[3].Value +
                        transactionMatch.Groups[4].Value;
                }

                rawAmount = rawAmount.Replace(",", "");

                if (double.TryParse(
                    rawAmount,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double amount))
                {
                    info.TransactionAmount = amount;
                }
            }


            // 3. Tìm số dư
            //
            // Hỗ trợ:
            // So du:2,527.39
            // So du: VND 942,390,864
            // So du:USD 1,107,321.16
            //
            var balanceMatch = Regex.Match(
                input,
                @"So du:\s*(?:VND|USD)?\s*([\d,]+(?:\.\d+)?)"
            );

            if (balanceMatch.Success)
            {
                string balanceRaw =
                    balanceMatch.Groups[1].Value.Replace(",", "");

                if (double.TryParse(
                    balanceRaw,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double balance))
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
