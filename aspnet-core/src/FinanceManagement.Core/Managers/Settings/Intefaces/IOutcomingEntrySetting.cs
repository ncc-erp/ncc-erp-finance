using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface IOutcomingEntrySetting
    {
        Task<string> GetOutcomingSalary();
        Task<bool> GetCanLinkWithOutComingEnd();
        Task<string> GetDefaultMaLoaiThuBanNgoaiTe();
        Task<string> GetDefaultMaLoaiMuaNgoaiTe();
        Task<string> GetDefaultLoaiThuIdKhiChiChuyenDoi();
        Task SetDefaultLoaiThuIdKhiChiChuyenDoi(string id);
        Task SetDefaultMaLoaiMuaNgoaiTe(string id);
        Task SetDefaultMaLoaiThuBanNgoaiTe(string id);
        Task SetCanLinkWithOutComingEnd(string id);
        Task SetToBankAcountFromOutcomingSalary(string toBankAcountFromOutcomingSalary);
        Task<string> GetApplyToMultiCurrencyOutcome();
        Task SetApplyToMultiCurrencyOutcome(string canApplyToMultiCurrencyOutcome);
    }
}
