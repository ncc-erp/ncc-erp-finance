using Abp.Runtime.Session;
using Abp.UI;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Settings.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings
{
    public class MySettingManager : DomainManager, IMySettingManager
    {
        private readonly IAbpSession _session;

        public MySettingManager(IWorkScope ws, IAbpSession session) : base(ws)
        {
            _session = session;
        }
        public async Task<GetEspeciallyIncomingEntryTypeDto> GetDebtClientAsync()
        {
            var incommingTypeCodeForPayingInvoice = await GetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForPayingInvoice, AppSettingNames.TenantIncommingTypeCodeForPayingInvoice);

            return await GetEspeciallyIncomingEntryType(incommingTypeCodeForPayingInvoice);
        }
        public async Task<GetEspeciallyIncomingEntryTypeDto> GetBalanceClientAsync()
        {
            var incommingTypeCodeForClientPrePaid = await GetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForClientPrePaid, AppSettingNames.TenantIncommingTypeCodeForClientPrePaid);

            return await GetEspeciallyIncomingEntryType(incommingTypeCodeForClientPrePaid);
        }
        public async Task<GetEspeciallyIncomingEntryTypeDto> GetDeviantClientAsync()
        {
            var incommingTypeCodeForClientPayDeviant = await GetValueSettingAsync(AppSettingNames.HostIncomingTypeCodeForClientPayDeviant, AppSettingNames.TenantIncomingTypeCodeForClientPayDeviant);

            return await GetEspeciallyIncomingEntryType(incommingTypeCodeForClientPayDeviant);
        }
        private async Task<GetEspeciallyIncomingEntryTypeDto> GetEspeciallyIncomingEntryType(string incommingTypeCode)
        {
            if (string.IsNullOrEmpty(incommingTypeCode))
                return new GetEspeciallyIncomingEntryTypeDto();

            var incomingEntryType = await _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == incommingTypeCode)
                .FirstOrDefaultAsync();

            if (incomingEntryType == null)
                return new GetEspeciallyIncomingEntryTypeDto()
                {
                    Code = incommingTypeCode,
                    Status = "Not found"
                };
            else
                return new GetEspeciallyIncomingEntryTypeDto()
                {
                    Code = incommingTypeCode,
                    Id = incomingEntryType.Id,
                    Status = incomingEntryType.IsActive ? "Active" : "Inactive"
                };
        }
        public GetEspeciallyIncomingEntryTypeDto GetDeviantClient()
        {
            var incommingTypeCodeForClientPayDeviant = GetValueSetting(AppSettingNames.HostIncomingTypeCodeForClientPayDeviant, AppSettingNames.TenantIncomingTypeCodeForClientPayDeviant);

            if (string.IsNullOrEmpty(incommingTypeCodeForClientPayDeviant))
                return new GetEspeciallyIncomingEntryTypeDto();

            var incomingEntryTypeId = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == incommingTypeCodeForClientPayDeviant)
                .Select(s => s.Id)
                .FirstOrDefault();

            return new GetEspeciallyIncomingEntryTypeDto()
            {
                Code = incommingTypeCodeForClientPayDeviant,
                Id = incomingEntryTypeId
            };
        }
        public GetEspeciallyIncomingEntryTypeDto GetDebtClient()
        {
            var incommingTypeCodeForPayingInvoice = GetValueSetting(AppSettingNames.HostIncommingTypeCodeForPayingInvoice, AppSettingNames.TenantIncommingTypeCodeForPayingInvoice);

            if (string.IsNullOrEmpty(incommingTypeCodeForPayingInvoice))
                return new GetEspeciallyIncomingEntryTypeDto();

            var incomingEntryTypeId = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == incommingTypeCodeForPayingInvoice)
                .Select(s => s.Id)
                .FirstOrDefault();

            return new GetEspeciallyIncomingEntryTypeDto()
            {
                Code = incommingTypeCodeForPayingInvoice,
                Id = incomingEntryTypeId
            };
        }
        public GetEspeciallyIncomingEntryTypeDto GetBalanceClient()
        {
            var incommingTypeCodeForClientPrePaid = GetValueSetting(AppSettingNames.HostIncommingTypeCodeForClientPrePaid, AppSettingNames.TenantIncommingTypeCodeForClientPrePaid);

            if (string.IsNullOrEmpty(incommingTypeCodeForClientPrePaid))
                return new GetEspeciallyIncomingEntryTypeDto();

            var incomingEntryTypeId = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == incommingTypeCodeForClientPrePaid)
                .Select(s => s.Id)
                .FirstOrDefault();

            return new GetEspeciallyIncomingEntryTypeDto()
            {
                Code = incommingTypeCodeForClientPrePaid,
                Id = incomingEntryTypeId
            };
        }
        public async Task SetDebtClientCodeAsync(string debtCode)
        {
            var incommingTypeCodeForPayingInvoice = await _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == debtCode)
                .Select(s => s.Code)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(incommingTypeCodeForPayingInvoice))
                throw new UserFriendlyException("Not Found IncommingTypeCodeForPayingInvoice!");

            await SetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForPayingInvoice, AppSettingNames.TenantIncommingTypeCodeForPayingInvoice, incommingTypeCodeForPayingInvoice);
        }
        public async Task SetBalanceClientCodeAsync(string balanceCode)
        {
            var incommingTypeCodeForClientPrePaid = await _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == balanceCode)
                .Select(s => s.Code)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(incommingTypeCodeForClientPrePaid))
                throw new UserFriendlyException("Not Found IncommingTypeCodeForClientPrePaid!");

            await SetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForClientPrePaid, AppSettingNames.TenantIncommingTypeCodeForClientPrePaid, balanceCode);
        }
        public async Task SetDeviantClientCodeAsync(string code)
        {
            var incommingTypeCodeForClientPayDeviant = await _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Code == code)
                .Select(s => s.Code)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(incommingTypeCodeForClientPayDeviant))
                throw new UserFriendlyException("Not Found IncommingTypeCodeForClientPayDeviant!");

            await SetValueSettingAsync(AppSettingNames.HostIncomingTypeCodeForClientPayDeviant, AppSettingNames.TenantIncomingTypeCodeForClientPayDeviant, incommingTypeCodeForClientPayDeviant);
        }

        public async Task<bool> GetCanLinkWithOutComingEnd()
        {
            var can = await GetValueSettingAsync(AppSettingNames.HostCanLinkWithOutComingEnd, AppSettingNames.TenantCanLinkWithOutComingEnd);
            return bool.Parse(can);
        }
        public async Task SetCanLinkWithOutComingEnd(string canLinkWithOutComingEnd)
        {
            await SetValueSettingAsync(AppSettingNames.HostCanLinkWithOutComingEnd, AppSettingNames.TenantCanLinkWithOutComingEnd, canLinkWithOutComingEnd);
        }

        public async Task<string> GetApplyToMultiCurrencyOutcome()
        {
            return await GetValueSettingAsync(AppSettingNames.HostCanApplyMutltiCurrencyOutcome, AppSettingNames.TenantCanApplyMultiCurrencyOutcome);
        }
        public async Task SetApplyToMultiCurrencyOutcome(string canApplyToMultiCurrencyOutcome)
        {
            await SetValueSettingAsync(AppSettingNames.HostCanApplyMutltiCurrencyOutcome, AppSettingNames.TenantCanApplyMultiCurrencyOutcome, canApplyToMultiCurrencyOutcome);
        }
        public async Task<string> GetMaLoaiChiBangLuong()
        {
            return await GetValueSettingAsync(AppSettingNames.HostMaLoaiChiBangLuong, AppSettingNames.TenantMaLoaiChiBangLuong);
        }
        public async Task<RequestChiSettingDto> GetRequestChiSetting()
        {
            return new RequestChiSettingDto
            {
                CanApplyMutltiCurrencyOutcome = await GetValueSettingAsync(AppSettingNames.HostCanApplyMutltiCurrencyOutcome, AppSettingNames.TenantCanApplyMultiCurrencyOutcome),
                MaLoaiChiBangLuong = await GetValueSettingAsync(AppSettingNames.HostMaLoaiChiBangLuong, AppSettingNames.TenantMaLoaiChiBangLuong)
            };
        }

        public async Task<string> CheckStatusOfMaLoaiChi(string input)
        {
            if (!input.HasValue())
            {
                return "Not found";
            }

            var result = await _ws.GetAll<OutcomingEntryType>()
                .Where(x => x.Code.ToLower() == input.ToLower())
                .Select(x => new { x.IsActive })
                .FirstOrDefaultAsync();

            if (result == default)
                return "Not found";
            else if (result.IsActive)
                return "Active";
            else
                return "Inactive";
        }
        public async Task UpdateRequestChiSetting(RequestChiSettingDto input)
        {
            await SetValueSettingAsync(AppSettingNames.HostCanApplyMutltiCurrencyOutcome, AppSettingNames.TenantCanApplyMultiCurrencyOutcome, input.CanApplyMutltiCurrencyOutcome);
            await SetValueSettingAsync(AppSettingNames.HostMaLoaiChiBangLuong, AppSettingNames.TenantMaLoaiChiBangLuong, input.MaLoaiChiBangLuong);
        }
        public async Task<string> GetDefaultMaLoaiThuBanNgoaiTe()
        {
            return await GetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdWhenBanNgoaiTe, AppSettingNames.TenantDefaultLoaiThuIdWhenBanNgoaiTe);
        }
        public async Task SetDefaultMaLoaiThuBanNgoaiTe(string id)
        {
            await SetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdWhenBanNgoaiTe, AppSettingNames.TenantDefaultLoaiThuIdWhenBanNgoaiTe, id);
        }
        public async Task<string> GetDefaultMaLoaiThuKhachHangBonus()
        {
            return await GetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdKhachHangBonus, AppSettingNames.TenantDefaultLoaiThuIdKhachHangBonus);
        }
        public async Task SetDefaultMaLoaiThuKhachHangBonus(string id)
        {
            await SetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdKhachHangBonus, AppSettingNames.TenantDefaultLoaiThuIdKhachHangBonus, id);
        }
        public async Task<string> GetDefaultMaLoaiMuaNgoaiTe()
        {
            return await GetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdWhenMuaNgoaiTe, AppSettingNames.TenantDefaultLoaiThuIdWhenMuaNgoaiTe);
        }
        public async Task SetDefaultMaLoaiMuaNgoaiTe(string id)
        {
            await SetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdWhenMuaNgoaiTe, AppSettingNames.TenantDefaultLoaiThuIdWhenMuaNgoaiTe, id);
        }
        public async Task<string> GetOutcomingSalary()
        {
            return await GetValueSettingAsync(AppSettingNames.HostToBankAcountFromOutcomingSalary, AppSettingNames.TenantToBankAcountFromOutcomingSalary);
        }
        public async Task SetToBankAcountFromOutcomingSalary(string toBankAcountFromOutcomingSalary)
        {
            await SetValueSettingAsync(AppSettingNames.HostToBankAcountFromOutcomingSalary, AppSettingNames.TenantToBankAcountFromOutcomingSalary, toBankAcountFromOutcomingSalary);
        }

        public string GetNotifyKomuChannelId(int? tenantId = int.MinValue)
        {
            return GetValueSetting(AppSettingNames.HostNotifyToChannel, AppSettingNames.TenantNotifyChannel, tenantId);
        }

        public async Task<string> GetNotifyKomuChannelIdAsync(int? tenantId = int.MinValue)
        {
            return await GetValueSettingAsync(AppSettingNames.HostNotifyToChannel, AppSettingNames.TenantNotifyChannel);
        }

        public void SetNotifyKomuChannelId(string komuChannelId)
        {
            SetValueSetting(AppSettingNames.HostNotifyToChannel, AppSettingNames.TenantNotifyChannel, komuChannelId);
        }

        public async Task SetNotifyKomuChannelIdAsync(string komuChannelId)
        {
            await SetValueSettingAsync(AppSettingNames.HostNotifyToChannel, AppSettingNames.TenantNotifyChannel, komuChannelId);
        }

        public async Task<string> GetEnableCrawlBTransactionNoti(int? tenantId = int.MinValue)
        {
          return await GetValueSettingAsync(AppSettingNames.HostEnableCrawlBTransactionNoti, AppSettingNames.TenantEnableCrawlBTransactionNoti, tenantId);
        }

        public async Task SetEnableCrawlBTransactionNoti(bool isEnable)
        {
            await SetValueSettingAsync(AppSettingNames.HostEnableCrawlBTransactionNoti, AppSettingNames.TenantEnableCrawlBTransactionNoti, isEnable.ToString());
        }

        public async Task<string> GetDefaultLoaiThuIdKhiChiChuyenDoi()
        {
            return await GetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdKhiChiChuyenDoi, AppSettingNames.TenantDefaultLoaiThuIdKhiChiChuyenDoi);
        }
        public async Task SetDefaultLoaiThuIdKhiChiChuyenDoi(string id)
        {
            await SetValueSettingAsync(AppSettingNames.HostDefaultLoaiThuIdKhiChiChuyenDoi, AppSettingNames.TenantDefaultLoaiThuIdKhiChiChuyenDoi, id);
        }
        public bool GetAllowChangeEntityInPeriodClosed()
        {
            var config = GetValueSetting(AppSettingNames.HostAllowChangeEntityInPeriodClosed, AppSettingNames.TenantAllowChangeEntityInPeriodClosed);
            if(bool.TryParse(config, out bool result))
            {
                return result;
            }
            return false;
        }
        public void SetAllowChangeEntityInPeriodClosed(string config)
        {
            SetValueSetting(AppSettingNames.HostAllowChangeEntityInPeriodClosed, AppSettingNames.TenantAllowChangeEntityInPeriodClosed, config);
        }
        #region set, get setting into database for multi-tenancy
        private async Task<string> GetValueSettingAsync(string hostSettingName, string tenantSettingName, int? tenantId = int.MinValue)
        {
            var currentTenant = _session.TenantId;
            if(tenantId != int.MinValue)
            {
                currentTenant = tenantId;
            }

            if (currentTenant.HasValue)
                return await SettingManager.GetSettingValueForTenantAsync(tenantSettingName, currentTenant.Value);

            return await SettingManager.GetSettingValueForApplicationAsync(hostSettingName);
        }
        private string GetValueSetting(string hostSettingName, string tenantSettingName, int? tenantId = int.MinValue)
        {
            var currentTenant = _session.TenantId;
            if (tenantId != int.MinValue)
            {
                currentTenant = tenantId;
            }

            if (currentTenant.HasValue)
                return SettingManager.GetSettingValueForTenant(tenantSettingName, currentTenant.Value);

            return SettingManager.GetSettingValueForApplication(hostSettingName);
        }
        private void SetValueSetting(string hostSettingName, string tenantSettingName, string value)
        {
            if (_session.TenantId.HasValue)
            {
                SettingManager.ChangeSettingForTenant(_session.TenantId.Value, tenantSettingName, value);
            }
            else
            {
                SettingManager.ChangeSettingForApplication(hostSettingName, value);
            }
        }
        public async Task SetValueSettingAsync(string hostSettingName, string tenantSettingName, string value)
        {
            if (_session.TenantId.HasValue)
            {
                await SettingManager.ChangeSettingForTenantAsync(_session.TenantId.Value, tenantSettingName, value);
            }
            else
            {
                await SettingManager.ChangeSettingForApplicationAsync(hostSettingName, value);
            }
        }
        #endregion
    }
}
