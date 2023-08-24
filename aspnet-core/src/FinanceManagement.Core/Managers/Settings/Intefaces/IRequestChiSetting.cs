using FinanceManagement.Managers.Settings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface IRequestChiSetting
    {
        Task<string> GetMaLoaiChiBangLuong();
        Task<RequestChiSettingDto> GetRequestChiSetting();
        Task UpdateRequestChiSetting(RequestChiSettingDto input);
        Task<string> CheckStatusOfMaLoaiChi(string input);
    }
}
