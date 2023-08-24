using Abp.UI;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Uitls;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceManagement.Helper
{
    public static class Helpers
    {
        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(
            this IEnumerable<T> collection,
            Func<T, K> id_selector,
            Func<T, K> parent_id_selector,
            K root_id = default(K)
        )
        {
            foreach (var c in collection.Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), root_id)))
            {
                yield return new TreeItem<T>
                {
                    Item = c,
                    Children = collection.GenerateTree(id_selector, parent_id_selector, id_selector(c))
                };
            }
        }
        public static T EnsureNotNull<T>(ref T field) where T : new()
        {
            if (field == null)
                field = new T();

            return field;
        }

        public static T Lock<T>(object lockObj, Func<T> f)
        {
            lock (lockObj) { return f(); };
        }

        public static T GetOrCreate<T>(ref T value, object lockObject, Func<T> factory) where T : class
        {
            if (value != null)
                return value;

            lock (lockObject)
            {
                if (value != null)
                    return value;

                value = factory();
                return value;
            }
        }

        public static T FailWith<T>(string message)
        {
            return FailWith<T>(new Exception(message));
        }
        public static T FailWith<T>(Exception exception)
        {
            throw exception;
        }
        public static string GetEnumName<T>(T t) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T must be enumerated type.");
            return Enum.GetName(typeof(T), t);
        }
        public static List<T> GetListEnum<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T must be enumerated type.");
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
        public static List<ValueAndNameDto> ListBTransactionStatuses = new List<ValueAndNameDto>
        {
            new ValueAndNameDto{ Value = BTransactionStatus.PENDING.GetHashCode(), Name = BTransactionStatus.PENDING.ToString()},
            new ValueAndNameDto{ Value = BTransactionStatus.DONE.GetHashCode(), Name = BTransactionStatus.DONE.ToString()},
        };
        public static List<ValueAndNameDto> ListRevenueStatuses = new List<ValueAndNameDto>
        {
            new ValueAndNameDto{ Value = RevenueManagedStatus.Not_Yet.GetHashCode(), Name = "Chưa trả"},
            new ValueAndNameDto{ Value = RevenueManagedStatus.Paid_Part.GetHashCode(), Name = "Đã trả một phần"},
            new ValueAndNameDto{ Value = RevenueManagedStatus.Done.GetHashCode(), Name = "Hoàn thành"},
            new ValueAndNameDto{ Value = RevenueManagedStatus.Not_Paid.GetHashCode(), Name = "Không trả"},
            new ValueAndNameDto{ Value = RevenueManagedStatus.Only_Paid_Part.GetHashCode(), Name = "Chỉ trả một phần"},
        };
        public static List<ValueAndNameDto> ListInvoiceStatuses = new List<ValueAndNameDto>
        {
            new ValueAndNameDto{ Value = NInvoiceStatus.CHUA_TRA.GetHashCode(), Name = "Chưa trả"},
            new ValueAndNameDto{ Value = NInvoiceStatus.TRA_1_PHAN.GetHashCode(), Name = "Đã trả một phần"},
            new ValueAndNameDto{ Value = NInvoiceStatus.HOAN_THANH.GetHashCode(), Name = "Hoàn thành"},
            new ValueAndNameDto{ Value = NInvoiceStatus.KHONG_TRA.GetHashCode(), Name = "Không trả"},
            new ValueAndNameDto{ Value = NInvoiceStatus.CHI_TRA_1_PHAN.GetHashCode(), Name = "Chỉ trả một phần"},
        };
        public static string StripHTMLContent(string content)
        {
            var pass1 = Regex.Replace(content, @"<[^>]+>|&nbsp;", " ");
            var pass2 = Regex.Replace(pass1, @"\s{2,}", " ");
            return Regex.Replace(pass2, @"\n", " ");
        }
        public static string StripHtmlContentForHashtag(string content)
        {
            var a = content.Replace(AllRegex.RemoveHtmltags, " ");

            var b = a.Replace(AllRegex.Remove2Space, " ");
            return b;
        }
        public static string RemoveNewLine(string content)
        {
            return content.Replace("\r\n", " ").Replace("\n", " ");
        }
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
                var money = double.Parse(moneyString.Trim());
                return new ResultDetectionMoney
                {
                    IsValid = true,
                    Result = money
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
        public static ResultConvertMoney ParseMoney(string money)
        {
            money = money.Replace(",", "");
            try
            {
                var pMoney = double.Parse(money.Trim());
                return new ResultConvertMoney
                {
                    IsValid = true,
                    Result = pMoney
                };
            }
            catch (Exception ex)
            {
                return new ResultConvertMoney();
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
        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp).AddHours(7);
        }
        public static ResultConvertTimestamp ConvertTimestampToLong(string timestamp)
        {
            try
            {
                var newTimestamp = long.Parse(timestamp.Trim());
                return new ResultConvertTimestamp
                {
                    IsValid = true,
                    Result = newTimestamp
                };
            }
            catch (Exception ex)
            {
                return new ResultConvertTimestamp
                {
                    ErrorMessage = "Can't Converted Timestamp!",
                    IsValid = false,
                };
            }
        }
        public static string FormatMoneyVND(double money)
        {
            return string.Format("{0:#,##0}", money);
        }
        public static string FormatMoney(double money)
        {
            return string.Format("{0:#,##0.##}", money);
        }
        public static string FormatMoney8PartAfterDot(double money)
        {
            return string.Format("{0:#,##0.########}", money);
        }
        public static string FormatMoney4PartAfterDot(double money)
        {
            return string.Format("{0:#,##0.####}", money);
        }
        public static double RoundMoneyToEven(double money)
        {
            return Math.Round(money, 0, MidpointRounding.ToEven);
        }

        public static string GetNameBankTransaction(InputGetNameBankTransaction input)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"TK {input.BankNumber}");
            string plusOrSub = string.Empty;
            if (input.Money > 0) plusOrSub = "+";
            sb.AppendLine($"So tien GD: {plusOrSub}{string.Format("{0:#,##0}", input.Money)} {input.CurrencyName} luc {DateTimeUtils.FormatDateTime(input.TimeAt)}");
            return sb.ToString();
        }
        public static string GetContentSendNotifyKomu(
            string outcomingEntryName,
            string outcomingEntryTypeCode,
            string branchName,
            string createdByName,
            DateTime creationTime
        )
        {
            var message = new StringBuilder();
            message.AppendLine("```");
            message.AppendLine($"Nội dung: {outcomingEntryName}");
            message.AppendLine($"Loại: {outcomingEntryTypeCode}");
            message.AppendLine($"Chi nhánh: {branchName}");
            message.AppendLine($"Tạo bởi: {createdByName} lúc {DateTimeUtils.FormatDateTime(creationTime)}");
            message.AppendLine("```");
            return message.ToString();
        }
        public static string GetSubContentSendNotifyKomu(
            string verifier,
            string statusCode,
            long outcomingEntryId,
            string money,
            string currencyCode
        )
        {
            return $"{verifier} ***[{statusCode}]*** request chi **#{outcomingEntryId}** số tiền **{money} {currencyCode}**";
        }
        public static string GetContentSalarySendNotifyKomu(
            long outcomingEntryId,
            string outcomingEntryName,
            string money,
            string statusCode
        )
        {
            return $"**[HRM Tool]** created request chi **#{outcomingEntryId} {outcomingEntryName}** số tiền **{money} VND** ({statusCode})";
        }

        public static string GetContentTeamBuildingSendNotifyKomu(
            long outcomingEntryId,
            string outcomingEntryName,
            string money,
            string statusCode
        )
        {
            return $"**[Timesheet Tool]** created request chi **#{outcomingEntryId} {outcomingEntryName}** số tiền **{money} VND** ({statusCode})";
        }

        /// <summary>
        /// Lấy nội dung thông báo yêu cầu thay đổi khi send
        /// </summary>
        /// <param name="verifier"></param>
        /// <param name="outcomingEntryId"></param>
        /// <param name="outcomingEntryName"></param>
        /// <param name="money"></param>
        /// <param name="oldMoney"></param>
        /// <param name="statusCode"></param>
        /// <param name="url"></param>
        /// <param name="reason"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public static string GetContentRequestChangePendingCEO(
            string verifier,
            long outcomingEntryId,
            string outcomingEntryName,
            double money,
            double oldMoney,
            string statusCode,
            string url,
            string reason,
            string currencyCode = FinanceManagementConsts.VND_CURRENCY_NAME,
            string oldCurrencyCode = FinanceManagementConsts.VND_CURRENCY_NAME
        )
        {
            return $"{verifier} ***[{statusCode}]*** **YÊU CẦU THAY ĐỔI** request chi **#{outcomingEntryId} {outcomingEntryName}** số tiền từ **{FormatMoneyVND(oldMoney)} {oldCurrencyCode}** => **{FormatMoneyVND(money)} {currencyCode}** "
                + (string.IsNullOrEmpty(reason) ? "" : $"``` Lý do thay đổi:\n {reason} ```")
                + (string.IsNullOrEmpty(reason) ? "\n" : "") + 
                $"{url}";
        }
        /// <summary>
        /// Lấy nội dung thông báo yêu cầu thay đổi khi đồng ý
        /// </summary>
        /// <param name="verifier"></param>
        /// <param name="outcomingEntryId"></param>
        /// <param name="outcomingEntryName"></param>
        /// <param name="money"></param>
        /// <param name="oldMoney"></param>
        /// <param name="statusCode"></param>
        /// <param name="url"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public static string GetContentRequestChangeApprove(
            string verifier,
            long outcomingEntryId,
            string outcomingEntryName,
            double money,
            double oldMoney,
            string statusCode,
            string url,
            string currencyCode = FinanceManagementConsts.VND_CURRENCY_NAME,
            string oldCurrencyCode = FinanceManagementConsts.VND_CURRENCY_NAME
        )
        {
            return $"{verifier} ***[{statusCode}]*** **YÊU CẦU THAY ĐỔI** request chi **#{outcomingEntryId} {outcomingEntryName}** số tiền từ **{FormatMoneyVND(oldMoney)} {oldCurrencyCode}** => **{FormatMoneyVND(money)} {currencyCode}**\n {url}";
        }
        /// <summary>
        /// Lấy nội dung thông báo yêu cầu thay đổi khi từ chối
        /// </summary>
        /// <param name="verifier"></param>
        /// <param name="outcomingEntryId"></param>
        /// <param name="outcomingEntryName"></param>
        /// <param name="money"></param>
        /// <param name="oldMoney"></param>
        /// <param name="statusCode"></param>
        /// <param name="url"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public static string GetContentRequestChangeReject(
            string verifier,
            long outcomingEntryId,
            string outcomingEntryName,
            double money,
            double oldMoney,
            string statusCode,
            string url,
            string currencyCode = FinanceManagementConsts.VND_CURRENCY_NAME,
            string oldCurrencyCode = FinanceManagementConsts.VND_CURRENCY_NAME
        )
        {
            return $"{verifier} ***[{statusCode}]*** **YÊU CẦU THAY ĐỔI** request chi **#{outcomingEntryId} {outcomingEntryName}** số tiền từ **{FormatMoneyVND(oldMoney)} {oldCurrencyCode}** => **{FormatMoneyVND(money)} {currencyCode}**\n {url}";
        }
        public static string NumberToText(double inputNumber)
        {
            try
            {
                if (inputNumber == 0)
                {
                    return "Không đồng";
                }
                string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
                bool isNegative = false;

                // -12345678.3445435 => "-12345678"
                string sNumber = inputNumber.ToString("#");
                double number = Convert.ToDouble(sNumber);
                if (number < 0)
                {
                    number = -number;
                    sNumber = number.ToString();
                    isNegative = true;
                }


                int ones, tens, hundreds;

                int positionDigit = sNumber.Length;   // last -> first

                string result = " ";


                if (positionDigit == 0)
                    result = unitNumbers[0] + result;
                else
                {
                    // 0:       ###
                    // 1: nghìn ###,###
                    // 2: triệu ###,###,###
                    // 3: tỷ    ###,###,###,###
                    int placeValue = 0;

                    while (positionDigit > 0)
                    {
                        // Check last 3 digits remain ### (hundreds tens ones)
                        tens = hundreds = -1;
                        ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                            if (positionDigit > 0)
                            {
                                hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                                positionDigit--;
                            }
                        }

                        if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                            result = placeValues[placeValue] + result;

                        placeValue++;
                        if (placeValue > 3) placeValue = 1;

                        if ((ones == 1) && (tens > 1))
                            result = "một " + result;
                        else
                        {
                            if ((ones == 5) && (tens > 0))
                                result = "lăm " + result;
                            else if (ones > 0)
                                result = unitNumbers[ones] + " " + result;
                        }
                        if (tens < 0)
                            break;
                        else
                        {
                            if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                            if (tens == 1) result = "mười " + result;
                            if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                        }
                        if (hundreds < 0) break;
                        else
                        {
                            if ((hundreds > 0) || (tens > 0) || (ones > 0))
                                result = unitNumbers[hundreds] + " trăm " + result;
                        }
                        result = " " + result;
                    }
                }
                result = result.Trim();
                if (isNegative) result = "Âm " + result;
                result = result + " đồng";
                result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.ToLower());
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Get Information File Template
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public static FileInfo GetInfoFileTemplate(params string[] pathFile)
        {
            var fileInfo = new FileInfo(Path.Combine(pathFile));
            if (!fileInfo.Exists)
            {
                throw new UserFriendlyException("Không tìm thấy file mẫu");
            }
            return fileInfo;
        }

        public static IEnumerable<ExcelRangeBase> GetCellHaveContents(this ExcelWorksheet excelWorksheet, string[] content)
        {
            return excelWorksheet.Cells.Where(s => s.Value != default && content.Contains(s.Value.ToString()));
        }
        public static ExcelRangeBase GetCellHaveContent(this ExcelWorksheet excelWorksheet, string content)
        {
            return excelWorksheet.Cells.FirstOrDefault(s => s.Value != default && s.Value.ToString().Equals(content));
        }
        public static ExcelRangeBase GetCellHaveContentOrDefault(this IEnumerable<ExcelRangeBase> excelRangeBases, string content)
        {
            return excelRangeBases.FirstOrDefault(s => s.Value != default && s.Value.ToString().Equals(content));
        }
        public static ExcelRangeBase GetCellHaveContent(this IEnumerable<ExcelRangeBase> excelRangeBases, string content)
        {
            var excelRangeBase = excelRangeBases.FirstOrDefault(s => s.Value != default && s.Value.ToString().Equals(content));
            if (excelRangeBase == default) throw new UserFriendlyException($"Không tìm thấy ô có giá trị: {content}");
            return excelRangeBase;
        }

    }
}
