using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FinanceManagement.Helper
{
    public static class CrawlMezonTransactionHelper
    {
        public static MezonTransInfo ExtractMezonTransaction(string input)
        {
            var info = new MezonTransInfo();

            try
            {
                // Tách phần message và metadata
                var parts = input.Split(new[] { "|#Meta:" }, StringSplitOptions.None);

                if (parts.Length > 1)
                {
                    // Phần metadata
                    var metaParts = parts[1].Split(';');

                    foreach (var metaPart in metaParts)
                    {
                        var keyValue = metaPart.Split('=');
                        if (keyValue.Length != 2) continue;

                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();

                        switch (key)
                        {
                            case "Hash":
                                info.Hash = value;
                                break;
                            case "Sender":
                                info.Sender = value;
                                break;
                            case "Receiver":
                                info.Receiver = value;
                                break;
                            case "Money":
                                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double money))
                                    info.TransactionAmount = money;
                                break;
                            case "Timestamp":
                                if (long.TryParse(value, out long timestamp))
                                    info.Timestamp = timestamp;
                                break;
                            case "BlockNumber":
                                if (long.TryParse(value, out long blockNumber))
                                    info.BlockNumber = blockNumber;
                                break;
                            case "Wallet":
                                info.WalletAddress = value;
                                break;
                        }
                    }
                }

                // Lưu message gốc (phần dễ đọc)
                info.ReadableMessage = parts[0];
            }
            catch (Exception)
            {
                // Return empty info if parsing fails
            }

            return info;
        }
    }

    public class MezonTransInfo
    {
        public string Hash { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public double TransactionAmount { get; set; }
        public long Timestamp { get; set; }
        public long BlockNumber { get; set; }
        public string WalletAddress { get; set; }
        public string ReadableMessage { get; set; }  // Message dễ đọc
    }
}
