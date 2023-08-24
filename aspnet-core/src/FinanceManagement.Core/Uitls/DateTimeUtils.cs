using Abp.Timing;
using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Uitls
{
    public class DateTimeUtils
    {
        // All now function use Clock.Provider.Now
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }
        public static string FormatDateTime(DateTime date)
        {
            return date.ToString("dd-MM-yyyy HH:mm");
        }
        public static ResultConvertDateTime ParseDateTime(string dateTime)
        {
            try
            {
                var pDateTime = DateTime.Parse(dateTime);
                return new ResultConvertDateTime
                {
                    IsValid = true,
                    Result = pDateTime,
                };
            }
            catch (Exception ex)
            {
                return new ResultConvertDateTime();
            }
        }
        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return date.AddDays(1 - date.Day).Date;
        }
        public static DateTime FirstDayOfCurrentyMonth()
        {
            return FirstDayOfMonth(GetNow());
        }
        public static string GetMonthYearLabelChart(DateTime date)
        {
            return date.ToString("MM-yyyy");
        }
        public static List<string> GetMonthYearLabelChartFromDate(DateTime startDate, DateTime endDate)
        {
            var result = new List<string>();
            var date = startDate;
            while (date <= endDate)
            {
                result.Add(GetMonthYearLabelChart(date));
                date = date.AddMonths(1);
            }
            return result;
        }
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}
