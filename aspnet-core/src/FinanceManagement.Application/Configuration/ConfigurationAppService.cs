using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Runtime.Session;
using Abp.UI;
using FinanceManagement.Authorization;
using FinanceManagement.Configuration.Dto;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Managers.Settings.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceManagement.Configuration
{

    [AbpAuthorize]
    public class ConfigurationAppService : FinanceManagementAppServiceBase, IConfigurationAppService
    {
        private readonly IConfiguration _appConfiguration;

        public ConfigurationAppService(IWorkScope workScope, IConfiguration configuration) : base(workScope)
        {
            _appConfiguration = configuration;
        }

        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
        public async Task<string> GetGoogleClientAppId()
        {
            return await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.ClientAppId);
        }
        public async Task<bool> GetCanLinkWithOutcomingEnd()
        {
            return await MySettingManager.GetCanLinkWithOutComingEnd();
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_EditLinkToRequestChiDaHoanThanh)]
        public async Task SetCanLinkWithOutcomingEnd(string canLinkWithOutComingEnd)
        {
            await MySettingManager.SetCanLinkWithOutComingEnd(canLinkWithOutComingEnd.ToString());
        }
        public async Task<string> GetApplyToMultiCurrencyOutcome()
        {
            return await MySettingManager.GetApplyToMultiCurrencyOutcome();
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_EditRequestChiSetting)]
        public async Task SetApplyToMultiCurrencyOutcome(string canApplyToMultiCurrencyOutcome)
        {
            await MySettingManager.SetApplyToMultiCurrencyOutcome(canApplyToMultiCurrencyOutcome.ToString());
        }

        [AbpAuthorize(PermissionNames.Admin_Configuration_ViewRequestChiSetting)]
        public async Task<RequestChiSettingDto> GetRequestChiSetting()
        {
            return await MySettingManager.GetRequestChiSetting();
        }

        [AbpAuthorize(PermissionNames.Admin_Configuration_ViewRequestChiSetting)]
        public async Task<string> CheckStatusOfMaLoaiChi(string input)
        {
            return await MySettingManager.CheckStatusOfMaLoaiChi(input);
        }

        [AbpAuthorize(PermissionNames.Admin_Configuration_EditRequestChiSetting)]
        public async Task UpdateRequestChiSetting(RequestChiSettingDto input)
        {
            await MySettingManager.UpdateRequestChiSetting(input);
        }
        public async Task<string> GetDefaultMaLoaiThuBanNgoaiTe()
        {
            return await MySettingManager.GetDefaultMaLoaiThuBanNgoaiTe();
        }
        public async Task SetDefaultMaLoaiThuBanNgoaiTe(DefaultIncomingEntryTypeDto input)
        {
            await MySettingManager.SetDefaultMaLoaiThuBanNgoaiTe(input.Id);
        }
        public async Task ClearDefaultMaLoaiThuBanNgoaiTe()
        {
            await MySettingManager.SetDefaultMaLoaiThuBanNgoaiTe(default);
        }
        public async Task<string> GetDefaultMaLoaiThuKhachHangBonus()
        {
            return await MySettingManager.GetDefaultMaLoaiThuKhachHangBonus();
        }
        public async Task SetDefaultMaLoaiThuKhachHangBonus(DefaultIncomingEntryTypeDto input)
        {
            await MySettingManager.SetDefaultMaLoaiThuKhachHangBonus(input.Id);
        }
        public async Task ClearDefaultMaLoaiThuKhachHangBonus()
        {
            await MySettingManager.SetDefaultMaLoaiThuKhachHangBonus(default);
        }
        public async Task<string> GetDefaultMaLoaiMuaNgoaiTe()
        {
            return await MySettingManager.GetDefaultMaLoaiMuaNgoaiTe();
        }
        public async Task SetDefaultMaLoaiMuaNgoaiTe(DefaultIncomingEntryTypeDto input)
        {
            await MySettingManager.SetDefaultMaLoaiMuaNgoaiTe(input.Id);
        }
        public async Task ClearDefaultMaLoaiMuaNgoaiTe()
        {
            await MySettingManager.SetDefaultMaLoaiMuaNgoaiTe(default);
        }
        public async Task<string> GetDefaultLoaiThuIdKhiChiChuyenDoi()
        {
            return await MySettingManager.GetDefaultLoaiThuIdKhiChiChuyenDoi();
        }
        public async Task SetDefaultLoaiThuIdKhiChiChuyenDoi(DefaultIncomingEntryTypeDto input)
        {
            await MySettingManager.SetDefaultLoaiThuIdKhiChiChuyenDoi(input.Id);
        }
        public async Task ClearDefaultLoaiThuIdKhiChiChuyenDoi()
        {
            await MySettingManager.SetDefaultLoaiThuIdKhiChiChuyenDoi(default);
        }


        [AbpAuthorize(PermissionNames.Admin_Configuration)]
        public async Task<AppSettingDto> Get()
        {
            return new AppSettingDto
            {
                ClientAppId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.ClientAppId),
                SecretKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.SecretKey),
                NotifyToChannel = await MySettingManager.GetNotifyKomuChannelIdAsync(),
            };
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_EditGoogleSetting)]
        public async Task ChangeClientAppId(ClienAppDto input)
        { 
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.ClientAppId, input.ClientAppId);
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_EditSecretKey)]
        public async Task ChangeFinanceSecretKey(SecretKeyDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SecretKey, input.SecretKey);
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_EditKomuSetting)]
        public async Task ChangeNotifyKomuChannel(NotifyToChannelDto input)
        {
            await MySettingManager.SetNotifyKomuChannelIdAsync(input.NotifyToChannel);
        }


        public async Task<OutputEspecialIncomingEntryTypeDto> GetEspecialIncomingEntryType()
        {
            var debtClient = await MySettingManager.GetDebtClientAsync();
            var balanceClient = await MySettingManager.GetBalanceClientAsync();
            var deviantClient = await MySettingManager.GetDeviantClientAsync();
            return new OutputEspecialIncomingEntryTypeDto()
            {
                DebtIncomingEntryTypeCode = debtClient.Code,
                DebtIncomingEntryTypeStatus = debtClient.Status,
                BalanceIncomingEntryTypeCode = balanceClient.Code,
                BalanceIncomingEntryTypeStatus = balanceClient.Status,
                DeviantIncomingEntryTypeCode = deviantClient.Code,
                DeviantIncomingEntryTypeStatus = deviantClient.Status,
            };
        }
        [AbpAuthorize(PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang)]
        public async Task<OutputEspecialIncomingEntryTypeDto> SetEspecialIncomingEntryType(InputEspecialIncomingEntryTypeDto input)
        {
            var listEspecialIncomingEntryTypeStatus = await GetListEspecialIncomingEntryTypeStatus(input);
           
            await MySettingManager.SetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForPayingInvoice, AppSettingNames.TenantIncommingTypeCodeForPayingInvoice, input.DebtIncomingEntryTypeCode);
            await MySettingManager.SetValueSettingAsync(AppSettingNames.HostIncommingTypeCodeForClientPrePaid, AppSettingNames.TenantIncommingTypeCodeForClientPrePaid, input.BalanceIncomingEntryTypeCode);
            await MySettingManager.SetValueSettingAsync(AppSettingNames.HostIncomingTypeCodeForClientPayDeviant, AppSettingNames.TenantIncomingTypeCodeForClientPayDeviant, input.DeviantIncomingEntryTypeCode);
            
            return listEspecialIncomingEntryTypeStatus;
        }
        private async Task<OutputEspecialIncomingEntryTypeDto> GetListEspecialIncomingEntryTypeStatus(InputEspecialIncomingEntryTypeDto input)
        {
            return new OutputEspecialIncomingEntryTypeDto
            {
                DebtIncomingEntryTypeCode = input.DebtIncomingEntryTypeCode,
                DebtIncomingEntryTypeStatus = await GetIncomingEntryTypeStatus(input.DebtIncomingEntryTypeCode),
                BalanceIncomingEntryTypeCode = input.BalanceIncomingEntryTypeCode,
                BalanceIncomingEntryTypeStatus = await GetIncomingEntryTypeStatus(input.BalanceIncomingEntryTypeCode),
                DeviantIncomingEntryTypeCode = input.DeviantIncomingEntryTypeCode,
                DeviantIncomingEntryTypeStatus = await GetIncomingEntryTypeStatus(input.DeviantIncomingEntryTypeCode),
            };
        }
        private async Task<string> GetIncomingEntryTypeStatus(string IncomingEntryTypeCode)
        {
            var IncomingEntryType = await WorkScope.GetAll<IncomingEntryType>()
                .Where(s => s.Code == IncomingEntryTypeCode)
                .FirstOrDefaultAsync();
            if (IncomingEntryType == default)
                return "Not found";

            return IncomingEntryType.IsActive ? "Active" : "Inactive";
        }
        public async Task<OutcomingSalaryDto> GetOutcomingSalary()
        {
            var bankAccountId = await MySettingManager.GetOutcomingSalary();
            return new OutcomingSalaryDto()
            {
                BankAccountId = bankAccountId
            };
        }
        public async Task SetOutcomingSalary(OutcomingSalaryDto input)
        {
            await MySettingManager.SetToBankAcountFromOutcomingSalary(input.BankAccountId);
        }
        public async Task ClearDefaultToBankAccount()
        {
            await MySettingManager.SetToBankAcountFromOutcomingSalary("");
        }
        public async Task<string> GetDeviantCode()
        {
            var res = await MySettingManager.GetDeviantClientAsync();
            return res.Code;
        }
        public async Task<bool> GetAllowChangeEntityInPeriodClosed()
        {
            await Task.CompletedTask;
            return MySettingManager.GetAllowChangeEntityInPeriodClosed();   
        }
        public async Task SetAllowChangeEntityInPeriodClosed(string config)
        {
            await Task.CompletedTask;
            MySettingManager.SetAllowChangeEntityInPeriodClosed(config);
        }
        public async Task<bool> GetEnableCrawlBTransactionNoti()
        {
            var setting = await MySettingManager.GetEnableCrawlBTransactionNoti();
            return bool.Parse(setting);
        }
        public async Task SetEnableCrawlBTransactionNoti(bool isEnable)
        {
            await MySettingManager.SetEnableCrawlBTransactionNoti(isEnable);
        }
        public InternalToolConfigDto GetHrmConfig()
        {
            return new InternalToolConfigDto()
            {
                BaseAddress = _appConfiguration.GetValue<string>("HRMService:BaseAddress"),
                SecurityCode = _appConfiguration.GetValue<string>("HRMService:SecurityCode")
            };
        }
    }
}
