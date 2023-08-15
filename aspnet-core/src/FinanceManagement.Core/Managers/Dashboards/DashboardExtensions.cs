using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.Dashboards.Dtos;
using FinanceManagement.Managers.Periods.Dtos;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Dashboards
{
    public static class DashboardExtensions
    {
        public static List<BaseDataChartDto> GetBaseDataCharts(this IEnumerable<KeyValuePairChart> data, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert)
        {
            return data
                .GroupBy(x => x.Key)
                .Select(x => new ValueYearMonthDto
                {
                    YearMonth = new YearMonthDto
                    {
                        Year = x.Key.Year,
                        Month = x.Key.Month
                    },
                    Value = x.Sum(x => x.Value),
                    ExchangeRate = dicCurrencyConvert.ContainsKey(x.Key) ? dicCurrencyConvert[x.Key] : 1
                })
                .AsEnumerable()
                .GroupBy(x => x.YearMonth.Label)
                .Select(x => new BaseDataChartDto
                {
                    Label = x.Key,
                    Value = x.Sum(s => s.TotalValue)
                })
                .ToList();
        }
        public static Dictionary<long, double> GetDictionaryComparativeCurrencyIdNotNull(this IEnumerable<CurrencyIdAndValueStatisticByCurrency> data)
        {
            return data
                .Where(x => x.CurrencyId.HasValue)
                .ToDictionary(x => x.CurrencyId.Value, x => x.Value);
        }
        public static double GetComparativeCurrencyIdNull(this IEnumerable<CurrencyIdAndValueStatisticByCurrency> data)
        {
            return data.Where(x => !x.CurrencyId.HasValue).Select(x => x.Value).FirstOrDefault();
        }
        public static void AssignInfomationForSheet(this ExcelWorksheet worksheet, GetPeriodDto periodInfo, DateTime startDate, DateTime endDate)
        {
            worksheet.Names["PeriodName"].Value = $"{periodInfo.Name} ({(periodInfo.StartDate.ToString("dd/MM/yyyy"))} - {(periodInfo.EndDate.HasValue ? periodInfo.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty)})";
            worksheet.Names["StartDate"].Value = startDate.ToString("dd/MM/yyyy");
            worksheet.Names["EndDate"].Value = endDate.ToString("dd/MM/yyyy");
        }
        public static void SetBorderRangeCells(this ExcelRange excelRange)
        {
            var border = excelRange.Style.Border;
            border.Top.Style = ExcelBorderStyle.Thin;
            border.Bottom.Style = ExcelBorderStyle.Thin;
            border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thin;
        }
    }
}
