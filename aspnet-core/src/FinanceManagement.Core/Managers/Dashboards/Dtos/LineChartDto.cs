using FinanceManagement.Helper;
using FinanceManagement.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class ResultChartDto
    {
        public IEnumerable<string> Labels { get; set; }
        public List<NewChartDto> Charts { get; set; } = new List<NewChartDto>();
    }
    public class NewChartDto
    {
        public string Name { get; set; }
        public ChartStyleDto ItemStyle { get; set; }
        public string Type { get; set; }
        public List<double> Data { get; set; }
        public string Total => Helpers.FormatMoney(Data.Sum());
        public int BarGap => 0;
        public string BarMaxWidth => "80";
    }
    public class CurrencyYearMonthDto
    {
        public long? CurrencyId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public override bool Equals(object obj)
        {
            var key = obj as CurrencyYearMonthDto;

            if (key == null)
                return false;

            return CurrencyId.Equals(key.CurrencyId) &&
                   Year.Equals(key.Year) && 
                   Month.Equals(key.Month);
        }
        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = (int)2166136261;

                hash = hash * 16777619 ^ CurrencyId.GetHashCode();
                hash = hash * 16777619 ^ Year.GetHashCode();
                hash = hash * 16777619 ^ Month.GetHashCode();
                return hash;
            }
        }

    }
    public class ValueYearMonthDto
    {
        public YearMonthDto YearMonth { get; set; }
        public double Value { get; set; }
        public double ExchangeRate { get; set; }
        public double TotalValue => Value * ExchangeRate;
    }
    public class YearMonthDto 
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Label => DateTimeUtils.GetMonthYearLabelChart(new DateTime(Year, Month, 1));
    } 
    public class ChartStyleDto
    {
        public string Color { get; set; }
    }
    public class BaseDataChartDto
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }
    public class KeyValuePairChart
    {
        public CurrencyYearMonthDto Key { get; set; }
        public double Value { get; set; }
    }
}
