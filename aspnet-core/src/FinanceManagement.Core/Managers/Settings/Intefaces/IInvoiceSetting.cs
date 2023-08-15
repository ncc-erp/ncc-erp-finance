using FinanceManagement.Managers.Settings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings.Intefaces
{
    public interface IInvoiceSetting
    {
        Task<GetEspeciallyIncomingEntryTypeDto> GetDebtClientAsync();
        Task<GetEspeciallyIncomingEntryTypeDto> GetBalanceClientAsync();
        Task<GetEspeciallyIncomingEntryTypeDto> GetDeviantClientAsync();
        GetEspeciallyIncomingEntryTypeDto GetDeviantClient();
        GetEspeciallyIncomingEntryTypeDto GetDebtClient();
        GetEspeciallyIncomingEntryTypeDto GetBalanceClient();
        Task<string> GetDefaultMaLoaiThuKhachHangBonus();
        Task SetDefaultMaLoaiThuKhachHangBonus(string id);
        Task SetDeviantClientCodeAsync(string code);
        Task SetDebtClientCodeAsync(string code);
        Task SetBalanceClientCodeAsync(string code);
        Task SetValueSettingAsync(string hostSettingName, string tenantSettingName, string value);

    }
}
