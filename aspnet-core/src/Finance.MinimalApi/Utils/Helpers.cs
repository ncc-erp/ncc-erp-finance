using System.Text.RegularExpressions;

namespace Finance.MinimalApi.Utils
{
    public class Helpers
    {
        public static ResultDetectionMoney DetectionMoney(Regex regex, string content)
        {
            var moneyString = regex.Match(content).Groups[1].Value;
            if (string.IsNullOrEmpty(moneyString))
            {
                return new ResultDetectionMoney
                {
                    ErrorMessage = "Can't Detected Money! ",
                    IsValid = false
                };
            }
            moneyString = moneyString.Replace(",", "");
            try
            {
                var moneyOfTransaction = double.Parse(moneyString.Trim());
                return new ResultDetectionMoney
                {
                    IsValid = true,
                    Result = moneyOfTransaction
                };
            }
            catch (Exception ex)
            {
                return new ResultDetectionMoney
                {
                    ErrorMessage = "Can't Converted Money! ",
                    IsValid = false,
                };
            }
        }
        public static ResultDetectionBankAccount DetectionBankNumber(Regex regex, string content)
        {
            var bankNumber = regex.Match(content).Groups[1].Value;
            if (string.IsNullOrEmpty(bankNumber))
            {
                return new ResultDetectionBankAccount
                {
                    IsValid = false,
                    ErrorMessage = "Can't Detected BankNumber",
                };
            }
            return new ResultDetectionBankAccount
            {
                IsValid = true,
                Result = bankNumber.Trim(),
            };
        }

        public static string RemoveNewLine(string content)
        {
            return content.Replace("\n", " ").Replace("\r", " ");
        }
    }

    public class ResultDetectionMoney
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public double Result { get; set; }
    }
    public class ResultDetectionBankAccount
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; }
    }
    public class BankAccountCrawl
    {
        public int? TenantId { get; set; }
        public long Id { get; set; }
        public long CurrencyId { get; set; }
    }
}
