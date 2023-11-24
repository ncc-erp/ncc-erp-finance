using FinanceManagement.Enums;
using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Dashboards.Dtos
{
    public class BaoCaoChungDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public double TongThu { get; set; }
        public string TongThuFormat => Helpers.FormatMoney(TongThu);
        public double TongThuThuc { get; set; }
        public string TongThuThucFormat => Helpers.FormatMoney(TongThuThuc);
        public double TongChi { get; set; }
        public string TongChiFormat => Helpers.FormatMoney(TongChi);
        public double TongChiThuc { get; set; }
        public string TongChiThucFormat => Helpers.FormatMoney(TongChiThuc);
        public double Du => TongThu - TongChi;
        public string DuFormat => Helpers.FormatMoney(Du);
        public double DuThuc => TongThuThuc - TongChiThuc;
        public string DuThucFormat => Helpers.FormatMoney(DuThuc);
        public double ChiKhongThuc => TongChi - TongChiThuc;
        public string ChiKhongThucFormat => Helpers.FormatMoney(ChiKhongThuc);
        public double ThuKhongThuc => TongThu - TongThuThuc;
        public string ThuKhongThucFormat => Helpers.FormatMoney(ThuKhongThuc);
        public double ChenhLech => Du - DuThuc;
        public string ChenhLechFormat => Helpers.FormatMoney(ChenhLech);
    }
    public class GetThongTinRequestChi
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DetailName { get; set; }
        public double Total { get; set; }
        public string TotalFormat => Helpers.FormatMoney(Total);
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public DateTime? ReportDate { get; set; }
        public double ExchangeRate { get; set; }
        public string ExchangeRateFromat => Helpers.FormatMoney(ExchangeRate);
        public double TotalVND => Total * ExchangeRate;
        public string TotalVNDFormat => Helpers.FormatMoney(TotalVND);
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long OutcomingEntryTypeId { get; set; }
        public string OutcomingEntryType { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string LaChiPhi => ExpenseType == ExpenseType.REAL_EXPENSE ? "YES" : "NO";
        public IEnumerable<GetThongTinRequestChi> Details { get; set; }
    }
    public class GetThongTinChiTheoChiNhanh
    {
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public double TotalVND { get; set; }
        public string TotalVNDFormat => Helpers.FormatMoney(TotalVND);
    }
    public class BaoCaoThuDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string MonthYear => Month.HasValue ? $"{Month}/{Year}" : string.Empty;
        public DateTime? TransactionDate { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney(Value);
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double ExchangeRate { get; set; }
        public string ExchangeRateFormat => Helpers.FormatMoney(ExchangeRate);
        public double TotalVND => Value * ExchangeRate;
        public string TotalVNDFormat => Helpers.FormatMoney(TotalVND);
        public long IncomingEntryTypeId { get; set; }
        public string IncomingEntryType { get; set; }
        public bool IsDoanhThu { get; set; }
        public string TinhDoanhThu => IsDoanhThu ? "YES" : "NO";
        public long BankTransactionId { get; set; }
    }

    public class BaoCaoDto
    {
        public BaoCaoThuDto BaoCaoThu { get; set; }
        public GetThongTinRequestChi BaoCaoChi { get; set; }
    }
}
