using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class ResultComparativeStatisticByCurrencyDto
    {
        public List<ComparativeStatisticByCurrencyDto> Statistics { get; set; }
        public double TotalToVND => Statistics.Sum(x => x.DuTheoSaoKe * x.ExchangeRate);
        public string TotalToVNDFormat => Helpers.FormatMoney(TotalToVND);
        public object ExchangeRates => Statistics.Select(x => new {x.TienTe, x.ExchangeRate, x.ExchangeRateFormat}).ToList();
    }
    public class ComparativeStatisticByCurrencyDto
    {
        public string TienTe { get; set; }
        public double DuDauKi { get; set; }
        public string DuDauKiFormat => Helpers.FormatMoney(DuDauKi);
        public double ThuSo { get; set; }
        public string ThuSoFormat => Helpers.FormatMoney(ThuSo);
        public double ThuSaoKe { get; set; }
        public string ThuSaoKeFormat => Helpers.FormatMoney(ThuSaoKe);
        public double ChenhLechThu => ThuSo - ThuSaoKe;
        public string ChenhLechThuFormat => Helpers.FormatMoney(ChenhLechThu);
        public double ChiSo { get; set; }
        public string ChiSoFormat => Helpers.FormatMoney(ChiSo);
        public double ChiSaoKe { get; set; }
        public string ChiSaoKeFormat => Helpers.FormatMoney(ChiSaoKe);
        public double HoanTien { get; set; }
        public string HoanTienFormat => Helpers.FormatMoney(HoanTien);
        public double ChenhLechChi => ChiSo + HoanTien - ChiSaoKe;
        public string ChenhLechChiFormat => Helpers.FormatMoney(ChenhLechChi);
        public double DuTheoSo => ThuSo - ChiSo - HoanTien + DuDauKi;
        public string DuTheoSoFormat => Helpers.FormatMoney(DuTheoSo);
        public double DuTheoSaoKe => ThuSaoKe - ChiSaoKe + DuDauKi;
        public string DuTheoSaoKeFormat => Helpers.FormatMoney(DuTheoSaoKe);
        public double ChenhLech => DuTheoSo - DuTheoSaoKe;
        public string ChenhLechFormat => Helpers.FormatMoney(ChenhLech);
        public double ExchangeRate { get; set; }
        public string ExchangeRateFormat => Helpers.FormatMoney(ExchangeRate);
        public bool IsActive { get; set; }
        public bool IsShow => IsActive || ThuSo != 0 || ThuSaoKe != 0 || ChiSaoKe != 0 || ChiSo != 0 || HoanTien != 0 || DuDauKi != 0;
        public bool IsEditBaseBalance => ThuSo == 0 && ThuSaoKe == 0 && ChiSaoKe != 0 && ChiSo != 0 && HoanTien != 0;
    }
    public class CurrencyIdAndValueStatisticByCurrency
    {
        public long? CurrencyId { get; set; }
        public double Value { get; set; }
    }
}
