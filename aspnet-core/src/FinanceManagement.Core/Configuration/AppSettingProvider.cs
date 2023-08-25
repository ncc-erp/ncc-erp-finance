using System.Collections.Generic;
using Abp.Configuration;

namespace FinanceManagement.Configuration
{
    /// <summary>
    /// AppSettingProvider: use mechanism Cache (Key-Value) or save on the DB, it's priority save on the DB if not exist on the DB 
    /// it will take value on the mem-cache
    /// using Multi-tenancy with scope
    /// Scope: Application(for host), Tenant (for Tenant), Application | Tenant (priority host -> apply host to tenancy)
    /// </summary>
    public class AppSettingProvider : SettingProvider
    {
        public static string RegexSTKDetectionValue = @"TK ([\d]+)(?=[^\d])";
        public static string RegexMoneyDetectionValue = @"GD:([0-9.,+-]+)(?=[^\d.,+-])";
        public static string RegexRemainMoneyDetectionValue = @"du:([0-9.,+-]+)(?=[^\d.,+-])";
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.EnableNormalLogin,"True",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.ClientAppId,"ClientAppId",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SecretKey, "SecretKey", scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.UserBot,"UserBot",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.PasswordBot,"PasswordBot",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.KomuUrl,"KomuUrl",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.KomuSecretCode,"KomuSecretCode",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FinanceUrl,"FinanceUrl",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectUri,"ProjectUri",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectUser,"admin",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectPassword,"ProjectPassword",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostNotifyToChannel,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantNotifyChannel,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostIncommingTypeCodeForPayingInvoice,FinanceManagementConsts.RevenueClientCode,scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.HostIncommingTypeCodeForClientPrePaid,FinanceManagementConsts.BalanceClientCode,scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantIncommingTypeCodeForPayingInvoice,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TenantIncommingTypeCodeForClientPrePaid,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.RegexSTKDetection,RegexSTKDetectionValue,scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.RegexMoneyDetection,RegexMoneyDetectionValue,scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.RegexRemainMoneyDetection, RegexRemainMoneyDetectionValue, scopes: SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FirebaseScretKey,"FirebaseScretKey",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FirebaseUrl,"FirebaseUrl",scopes:SettingScopes.Application |SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostIncomingTypeCodeForClientPayDeviant,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantIncomingTypeCodeForClientPayDeviant,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostToBankAcountFromOutcomingSalary,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantToBankAcountFromOutcomingSalary,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostCanLinkWithOutComingEnd,"false",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantCanLinkWithOutComingEnd,"false",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostDefaultLoaiThuIdWhenBanNgoaiTe,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantDefaultLoaiThuIdWhenBanNgoaiTe,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostDefaultLoaiThuIdWhenMuaNgoaiTe,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantDefaultLoaiThuIdWhenMuaNgoaiTe,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostCanApplyMutltiCurrencyOutcome,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantCanApplyMultiCurrencyOutcome,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostMaLoaiChiBangLuong,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantMaLoaiChiBangLuong,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostDefaultLoaiThuIdKhachHangBonus,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantDefaultLoaiThuIdKhachHangBonus,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostDefaultLoaiThuIdKhiChiChuyenDoi,"",scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantDefaultLoaiThuIdKhiChiChuyenDoi,"",scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostAllowChangeEntityInPeriodClosed, "false", scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantAllowChangeEntityInPeriodClosed, "false", scopes:SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.HostEnableCrawlBTransactionNoti, "false", scopes:SettingScopes.Application),
                new SettingDefinition(AppSettingNames.TenantEnableCrawlBTransactionNoti, "false", scopes:SettingScopes.Tenant),
            };
        }
    }
}
