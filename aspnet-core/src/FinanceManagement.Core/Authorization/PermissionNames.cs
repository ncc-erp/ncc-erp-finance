using Abp.MultiTenancy;
using System.Collections.Generic;
using static FinanceManagement.Authorization.Roles.StaticRoleNames;

namespace FinanceManagement.Authorization
{
    public static class PermissionNames
    {

        public const string Dashboard = "Dashboard";
        public const string Admin = "Admin";
        public const string Directory = "Directory";
        public const string Account_Directory = "Account.Directory";
        public const string Finance = "Finance";

        //DashBoard
        public const string DashBoard_View = "DashBoard.View";
        public const string DashBoard_ExportReport = "DashBoard.ExportReport";
        public const string DashBoard_XemKhoiRequestChi = "DashBoard.XemKhoiRequestChi";
        public const string DashBoard_XemKhoiRequestChi_NhacnhoCEO = "DashBoard.XemKhoiRequestChi.NhacNhoCEO";
        public const string DashBoard_XemKhoiBienDongSoDu = "DashBoard.XemKhoiBienDongSoDu";
        public const string DashBoard_XemKhoiKhachHangTraNo = "DashBoard.XemKhoiKhachHangTraNo";
        public const string DashBoard_XemKhoiBieuDoBangThuChi = "DashBoard.KhoiBieuDoBangThuChi";
        public const string DashBoard_XemKhoiBieuDoTron = "DashBoard.XemKhoiBieuDoTron";


        //Admin
        //Tenant
        public const string Admin_Tenant = "Admin.Tenant";
        public const string Admin_Tenant_View = "Admin.Tenant.View";
        public const string Admin_Tenant_Create = "Admin.Tenant.Create";
        public const string Admin_Tenant_Edit = "Admin.Tenant.Edit";
        public const string Admin_Tenant_Delete = "Admin.Tenant.Delete";
        //User
        public const string Admin_User = "Admin.User";
        public const string Admin_User_View = "Admin.User.View";
        public const string Admin_User_Create = "Admin.User.Create";
        public const string Admin_User_Edit = "Admin.User.Edit";
        public const string Admin_User_Delete = "Admin.User.Delete";
        public const string Admin_User_ResetPassword = "Admin.User.ResetPassword";
        //Role
        public const string Admin_Role = "Admin.Role";
        public const string Admin_Role_View = "Admin.Role.View";
        public const string Admin_Role_Create = "Admin.Role.Create";
        public const string Admin_Role_Edit = "Admin.Role.Edit";
        public const string Admin_Role_Delete = "Admin.Role.Delete";

        //Workflow
        public const string Admin_Workflow = "Admin.Workflow";

        public const string Admin_Workflow_Create = "Admin.Workflow.Create";
        public const string Admin_Workflow_Delete = "Admin.Workflow.Delete";
        public const string Admin_Workflow_View = "Admin.Workflow.View";
        public const string Admin_Workflow_ViewDetail = "Admin.Workflow.ViewDetail";
        public const string Admin_Workflow_ViewDetail_View = "Admin.Workflow.ViewDetail.View";
        public const string Admin_Workflow_WorkflowDetail_Edit = "Admin.Workflow.WorkflowDetail.Edit";


        //Workflow status
        public const string Admin_WorkflowStatus = "Admin.WorkflowStatus";

        public const string Admin_WorkflowStatus_Create = "Admin.WorkflowStatus.Create";
        public const string Admin_WorkflowStatus_Edit = "Admin.WorkflowStatus.Edit";
        public const string Admin_WorkflowStatus_Delete = "Admin.WorkflowStatus.Delete";
        public const string Admin_WorkflowStatus_View = "Admin.WorkflowStatus.View";
        

        // Configuration
        public const string Admin_Configuration = "Admin.Configuration";
        public const string Admin_Configuration_View = "Admin.Configuration.View";
        public const string Admin_Configuration_EditKomuSetting = "Admin.Configuration.EditKomuSetting";
        public const string Admin_Configuration_EditGoogleSetting = "Admin.Configuration.EditGoogleSetting";
        public const string Admin_Configuration_EditSecretKey = "Admin.Configuration.EditSecretKey";
        public const string Admin_Configuration_EditLinkToRequestChiDaHoanThanh = "Admin.Configuration.EditLinkToRequestChiDaHoanThanh";
        public const string Admin_Configuration_ViewRequestChiSetting = "Admin.Configuration.ViewRequestChiSetting";
        public const string Admin_Configuration_EditRequestChiSetting = "Admin.Configuration.EditRequestChiSetting";
        public const string Admin_Configuration_EditCoTheSuaThongTinCuaKiCu = "Admin.Configuration.EditCoTheSuaThongTinCuaKiCu";

        //Line Chart setting

        public const string Admin_LineChartSetting = "Admin.LineChartSetting";
        public const string Admin_LineChartSetting_View = "Admin.LineChartSetting.View";
        public const string Admin_LineChartSetting_Create = "Admin.LineChartSetting.Create";
        public const string Admin_LineChartSetting_Edit = "Admin.LineChartSetting.Edit";
        public const string Admin_LineChartSetting_Delete = "Admin.LineChartSetting.Delete";
        public const string Admin_LineChartSetting_ActiveDeactive = "Admin.LineChartSetting.ActiveDeactive";

        //Circle Chart

        public const string Admin_CircleChart = "Admin.CircleChart";
        public const string Admin_CircleChart_View = "Admin.CircleChart.View";
        public const string Admin_CircleChart_Create = "Admin.CircleChart.Create";
        public const string Admin_CircleChart_Edit = "Admin.CircleChart.Edit";
        public const string Admin_CircleChart_Delete = "Admin.CircleChart.Delete";
        public const string Admin_CircleChart_ActiveDeactive = "Admin.CircleChart.ActiveDeactive";

         //Circle Chart Detail

        public const string Admin_CircleChart_CircleChartDetail = "Admin.CircleChart.CircleChartDetail";
        public const string Admin_CircleChart_CircleChartDetail_View = "Admin.CircleChart.CircleChartDetail.View";
        public const string Admin_CircleChart_CircleChartDetail_Create = "Admin.CircleChart.CircleChartDetail.Create";
        public const string Admin_CircleChart_CircleChartDetail_Edit = "Admin.CircleChart.CircleChartDetail.Edit";
        public const string Admin_CircleChart_CircleChartDetail_Delete = "Admin.CircleChart.CircleChartDetail.Delete";

        // Crawl
        public const string Admin_CrawlHistory = "Admin.CrawlHistory";
        public const string Admin_CrawlHistory_View = "Admin.CrawlHistory.View";

        // Auditlog
        public const string Admin_Auditlog = "Admin.Auditlog";
        public const string Admin_Auditlog_View = "Admin.Auditlog.View";


        //Directory 
        //Currency
        public const string Directory_Currency = "Directory.Currency";
        public const string Directory_Currency_Create = "Directory.Currency.Create";
        public const string Directory_Currency_Edit = "Directory.Currency.Edit";
        public const string Directory_Currency_Delete = "Directory.Currency.Delete";
        public const string Directory_Currency_View = "Directory.Currency.View";
        public const string Directory_Currency_ChangeDefaultCurrency = "Directory.Currency.ChangeDefaultCurrency";
        
        
        //Currency Convert
        public const string Directory_CurrencyConvert = "Directory.CurrencyConvert";
        public const string Directory_CurrencyConvert_Create = "Directory.CurrencyConvert.Create";
        public const string Directory_CurrencyConvert_Edit = "Directory.CurrencyConvert.Edit";
        public const string Directory_CurrencyConvert_Delete = "Directory.CurrencyConvert.Delete";
        public const string Directory_CurrencyConvert_View = "Directory.CurrencyConvert.View";
        
        //Bank
        public const string Directory_Bank = "Directory.Bank";
        public const string Directory_Bank_Create = "Directory.Bank.Create";
        public const string Directory_Bank_Edit = "Directory.Bank.Edit";
        public const string Directory_Bank_Delete = "Directory.Bank.Delete";
        public const string Directory_Bank_View = "Directory.Bank.View";

        //Account Type
        public const string Directory_AccountType = "Directory.AccountType";
        public const string Directory_AccountType_View = "Directory.AccountType.View";
        public const string Directory_AccountType_Create = "Directory.AccountType.Create";
        public const string Directory_AccountType_Edit = "Directory.AccountType.Edit";
        public const string Directory_AccountType_Delete = "Directory.AccountType.Delete";
       


        //Branches
        public const string Directory_Branch = "Directory.Branch";

        public const string Directory_Branch_Create = "Directory.Branch.Create";
        public const string Directory_Branch_Edit = "Directory.Branch.Edit";
        public const string Directory_Branch_Delete = "Directory.Branch.Delete";
        public const string Directory_Branch_View = "Directory.Branch.View";

        // Revenue
        public const string Directory_IncomingEntryType = "Directory.IncomingEntryType";
        public const string Directory_IncomingEntryType_Create = "Directory.IncomingEntryType.Create";
        public const string Directory_IncomingEntryType_Edit = "Directory.IncomingEntryType.Edit";
        public const string Directory_IncomingEntryType_Delete = "Directory.IncomingEntryType.Delete";
        public const string Directory_IncomingEntryType_View = "Directory.IncomingEntryType.View";
        
        //Expenditure
        public const string Directory_OutcomingEntryType = "Directory.OutcomingEntryType";
        public const string Directory_OutcomingEntryType_Create = "Directory.OutcomingEntryType.Create";
        public const string Directory_OutcomingEntryType_Edit = "Directory.OutcomingEntryType.Edit";
        public const string Directory_OutcomingEntryType_Delete = "Directory.OutcomingEntryType.Delete";
        public const string Directory_OutcomingEntryType_View = "Directory.OutcomingEntryType.View";



        //Supplier
        public const string Directory_Supplier = "Directory.Supplier";

        public const string Directory_Supplier_View = "Directory.Supplier.View";
        public const string Directory_Supplier_Create = "Directory.Supplier.Create";
        public const string Directory_Supplier_Update = "Directory.Supplier.Update";
        public const string Directory_Supplier_Delete = "Directory.Supplier.Delete";

        // Account
        //Bank Account
        public const string Account_Directory_BankAccount = "Account.Directory.BankAccount";
        public const string Account_Directory_BankAccount_View = "Account.Directory.BankAccount.View";
        public const string Account_Directory_BankAccount_Create = "Account.Directory.BankAccount.Create";
        public const string Account_Directory_BankAccount_Edit = "Account.Directory.BankAccount.Edit";
        public const string Account_Directory_BankAccount_Delete = "Account.Directory.BankAccount.Delete";
        public const string Account_Directory_BankAccount_Export = "Account.Directory.BankAccount.Export";
        public const string Account_Directory_BankAccount_LockUnlock = "Account.Directory.BankAccount.LockUnlock";
        public const string Account_Directory_BankAccount_ActiveDeactive = "Account.Directory.BankAccount.ActiveDeactive";
        public const string Account_Directory_BankAccount_ViewBankAccountDetail = "Account.Directory.BankAccount.ViewBankAccountDetail";
        public const string Account_Directory_BankAccount_EditBaseBalanace = "Account.Directory.BankAccount.EditBaseBalanace";

        //Đối tượng kế toán
        public const string Account_Directory_FinanceAccount = "Account.Directory.FinanceAccount";
        public const string Account_Directory_FinanceAccount_View = "Account.Directory.FinanceAccount.View";
        public const string Account_Directory_FinanceAccount_Create = "Account.Directory.FinanceAccount.Create";
        public const string Account_Directory_FinanceAccount_Edit = "Account.Directory.FinanceAccount.Edit";
        public const string Account_Directory_FinanceAccount_Delete = "Account.Directory.FinanceAccount.Delete";
        public const string Account_Directory_FinanceAccount_ActiveDeactive = "Account.Directory.FinanceAccount.ActiveDeactive";


        

        //Finance Management
  
        // Incoming entries
        public const string Finance_IncomingEntry = "Finance.IncomingEntry";
        public const string Finance_IncomingEntry_View = "Finance.IncomingEntry.View";
        public const string Finance_IncomingEntry_ExportExcel = "Finance.IncomingEntry.ExportExcel";

        // Outcoming entries
        public const string Finance_OutcomingEntry = "Finance.OutcomingEntry";
        public const string Finance_OutcomingEntry_View = "Finance.OutcomingEntry.View";
        public const string Finance_OutcomingEntry_ViewAllOutcomingEntryTypeFilter = "Finance.OutcomingEntry.ViewAllOutcomingEntryTypeFilter";
        public const string Finance_OutcomingEntry_ViewOnlyMe = "Finance.OutcomingEntry.ViewOnlyMe";
        public const string Finance_OutcomingEntry_Create = "Finance.OutcomingEntry.Create";
        public const string Finance_OutcomingEntry_Delete = "Finance.OutcomingEntry.Delete";
        public const string Finance_OutcomingEntry_UpdateReportDate = "Finance.OutcomingEntry.UpdateReportDate";
        public const string Finance_OutcomingEntry_UpdateRequestType = "Finance.OutcomingEntry.UpdateRequestType";
        public const string Finance_OutcomingEntry_UpdateBranch = "Finance.OutcomingEntry.UpdateBranch";
        public const string Finance_OutcomingEntry_ExportExcel = "Finance.OutcomingEntry.ExportExcel";
        public const string Finance_OutcomingEntry_ExportPdf = "Finance.OutcomingEntry.ExportPdf";
        public const string Finance_OutcomingEntry_ChangeStatus = "Finance.OutcomingEntry.ChangeStatus";
        public const string Finance_OutcomingEntry_ViewYCTD = "Finance.OutcomingEntry.ViewYCTD";
        public const string Finance_OutcomingEntry_AddInfoToYCTD = "Finance.OutcomingEntry.AddInfoToYCTD";
        public const string Finance_OutcomingEntry_EditInfoOfYCTD = "Finance.OutcomingEntry.EditInfoOfYCTD";
        public const string Finance_OutcomingEntry_DeleteInfoOfYCTD = "Finance.OutcomingEntry.DeleteInfoOfYCTD";
        public const string Finance_OutcomingEntry_RevertInfoOfYCTD = "Finance.OutcomingEntry.RevertInfoOfYCTD";
        public const string Finance_OutcomingEntry_AcceptYCTD = "Finance.OutcomingEntry.AcceptYCTD";
        public const string Finance_OutcomingEntry_RejectYCTD = "Finance.OutcomingEntry.RejectYCTD";
        public const string Finance_OutcomingEntry_Clone = "Finance.OutcomingEntry.Clone";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail = "Finance.OutcomingEntry.OutcomingEntryDetail";
        //Thông tin chung của request chi
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.View";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.Edit";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.EditRequestType";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.EditReportDate";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.AttachFile";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.AcceptFile";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.DeleteFile";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.ExportPdf";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.Clone";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOutcomingEntry = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.EditOutcomingEntry";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.YCTD";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.SendYCTD";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.EditDisscus";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.DeleteDisscus";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.EditOnlyMyDisscus";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus = "Finance.OutcomingEntry.OutcomingEntryDetail.TabGeneral.DeleteOnlyMyDisscus";
        //Thông tin chi tiết của request chi
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.View";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.Create";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.Edit";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_UpdateBranch = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.UpdateBranch";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.Delete";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive = "Finance.OutcomingEntry.OutcomingEntryDetail.TabDetailInfo.ActiveDeactive";

        //Giao dịch ngân hàng liên quan
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedBankTransaction";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedBankTransaction.View";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedBankTransaction.LinkToBTrans";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedBankTransaction.DeleteLinkToBTrans";
        //Nguồn thu liên quan
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedIncomingEntrySource";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedIncomingEntrySource.View";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedIncomingEntrySource.LinkToIncomingEntry";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry = "Finance.OutcomingEntry.OutcomingEntryDetail.TabRelatedIncomingEntrySource.DeleteLinkToIncomingEntry";

        //Nhà cung cấp
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier = "Finance.OutcomingEntry.OutcomingEntryDetail.TabSupplier";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View = "Finance.OutcomingEntry.OutcomingEntryDetail.TabSupplier.View";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier = "Finance.OutcomingEntry.OutcomingEntryDetail.TabSupplier.LinkToSupplier";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier = "Finance.OutcomingEntry.OutcomingEntryDetail.TabSupplier.CreateSupplier";
        public const string Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier = "Finance.OutcomingEntry.OutcomingEntryDetail.TabSupplier.DeleteLinkToSupplier";

        // Bank transactions
        public const string Finance_BankTransaction = "Finance.BankTransaction";
        public const string Finance_BankTransaction_View = "Finance.BankTransaction.View";
        public const string Finance_BankTransaction_Create = "Finance.BankTransaction.Create";
        public const string Finance_BankTransaction_Edit = "Finance.BankTransaction.Edit";
        public const string Finance_BankTransaction_Delete = "Finance.BankTransaction.Delete";
        public const string Finance_BankTransaction_LinkToBienDongSoDu = "Finance.BankTransaction.LinkToBienDongSoDu";
        public const string Finance_BankTransaction_ExportExcel = "Finance.OutcomingEntry.BankTransaction.ExportExcel";
        public const string Finance_BankTransaction_LockUnlock = "Finance.OutcomingEntry.BankTransaction.LockUnlock";
       
        public const string Finance_BankTransaction_ViewBienDongSoDu = "Finance.OutcomingEntry.BankTransaction.ViewBienDongSoDu";
        public const string Finance_BankTransaction_BankTransactionDetail = "Finance.BankTransaction.BankTransactionDetail";
        //Tab Bank Transaction
        public const string Finance_BankTransaction_BankTransactionDetail_TabBankTransaction = "Finance.BankTransaction.BankTransactionDetail.TabBankTransaction";
        public const string Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View = "Finance.BankTransaction.BankTransactionDetail.TabBankTransaction.View";
        public const string Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit = "Finance.BankTransaction.BankTransactionDetail.TabBankTransaction.Edit";
        //Tab Revenue Record 
        public const string Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry = "Finance.BankTransaction.BankTransactionDetail.TabIncommingEntry";
        public const string Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View = "Finance.BankTransaction.BankTransactionDetail.TabIncommingEntry.View";
        public const string Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit = "Finance.BankTransaction_BankTransactionDetail.TabIncommingEntry.Edit";

        //Tab Expenditure Request
        public const string Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry = "Finance.BankTransaction.BankTransactionDetail.TabOutcomingEntry";
        public const string Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View = "Finance.BankTransaction.BankTransactionDetail.TabOutcomingEntry.View";
       

        //Đối soát mới
        public const string Finance_ComparativeStatisticNew = "Finance.ComparativeStatisticNew";
        public const string Finance_ComparativeStatisticNew_View = "Finance.ComparativeStatisticNew.View";
        //Đối soát
        public const string Finance_ComparativeStatistic = "Finance.ComparativeStatistic";
        public const string Finance_ComparativeStatistic_View = "Finance.ComparativeStatistic.View";
        public const string Finance_ComparativeStatistic_ViewAllCompanyBankAccount = "Finance.ComparativeStatistic.ViewAllCompanyBankAccount";
        public const string Finance_ComparativeStatistic_ExportExcel = "Finance.ComparativeStatistic.ExportExcel";
        public const string Finance_ComparativeStatistic_ViewExplanation = "Finance.ComparativeStatistic.ViewExplanation";
        public const string Finance_ComparativeStatistic_CreateExplanation = "Finance.ComparativeStatistic.EditExplanation";
        //Biến động số dư
        public const string Finance_BĐSD = "Finance.BĐSD";
        public const string Finance_BĐSD_View = "Finance.BĐSD.View";
        public const string Finance_BĐSD_CaiDatThanhToanKhachHang = "Finance.BĐSD.CaiDatThanhToanKhachHang";
        public const string Finance_BĐSD_Create = "Finance.BĐSD.Create";
        public const string Finance_BĐSD_Edit = "Finance.BĐSD.Edit";
        public const string Finance_BĐSD_Delete = "Finance.BĐSD.Delete";
        public const string Finance_BĐSD_LinkToRequestChi = "Finance.BĐSD.LinkToRequestChi";
        public const string Finance_BĐSD_LinkToMultipleRequestChi = "Finance.BĐSD.LinkToMultipleRequestChi";
        public const string Finance_BĐSD_RemoveLinkToRequestChi = "Finance.BĐSD.RemoveLinkToRequestChi";
        public const string Finance_BĐSD_ChiLuong = "Finance.BĐSD.ChiLuong";
        public const string Finance_BĐSD_BanNgoaiTe = "Finance.BĐSD.BanNgoaiTe";
        public const string Finance_BĐSD_MuaNgoaiTe = "Finance.BĐSD.MuaNgoaiTe";
        public const string Finance_BĐSD_Import = "Finance.BĐSD.Import";
        public const string Finance_BĐSD_KhachHangThanhToan = "Finance.BĐSD.KhachHangThanhToan";
        public const string Finance_BĐSD_CreateIncomingEntry = "Finance.BĐSD.CreateIncomingEntry";
        public const string Finance_BĐSD_CreateMultiIncomingEntry = "Finance.BĐSD.CreateMultiIncomingEntry";
        public const string Finance_BĐSD_ChiChuyenDoi = "Finance.BĐSD.ChiChuyenDoi";
        //Invoice
        public const string Finance_Invoice = "Finance.Invoice";

        public const string Finance_Invoice_View = "Finance.Invoice.View";
        public const string Finance_Invoice_Export_Report = "Finance.Invoice.Export.Report";
        public const string Finance_Invoice_Create = "Finance.Invoice.Create";
        public const string Finance_Invoice_Update = "Finance.Invoice.Update";
        public const string Finance_Invoice_Delete = "Finance.Invoice.Delete";
        public const string Finance_Invoice_AutoPay= "Finance.Invoice.AutoPay";
        public const string Finance_Invoice_EditNote = "Finance.Invoice.EditNote";
        public const string Finance_Invoice_EditStatusInvoice = "Finance.Invoice.EditStatusInvoice";
        public const string Finance_Invoice_KhachHangTraKenhTien = "Finance.Invoice.KhachHangTraKenhTien";
        //Period

        public const string Finance_Period = "Finance.Period";
        public const string Finance_Period_View = "Finance.Period.View";
        public const string Finance_Period_Create = "Finance.Period.Create";
        public const string Finance_Period_Edit = "Finance.Period.Edit";
        public const string Finance_Period_CloseAndCreate = "Finance.Period.CloseAndCreate";

        

       
    }

    public class GrantPermissionRoles
    {
        public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>()
        {
            {
                Host.Admin,
                new List<string>()
                {

                    PermissionNames.Dashboard,
                    PermissionNames.Admin,
                    PermissionNames.Directory,
                    PermissionNames.Account_Directory,
                    PermissionNames.Finance,

                    //DashBoard
                    PermissionNames.DashBoard_View,
                    PermissionNames.DashBoard_XemKhoiRequestChi,
                    PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO,
                    PermissionNames.DashBoard_XemKhoiBienDongSoDu,
                    PermissionNames.DashBoard_XemKhoiKhachHangTraNo,
                    PermissionNames.DashBoard_XemKhoiBieuDoBangThuChi,
                    PermissionNames.DashBoard_XemKhoiBieuDoTron,
                    PermissionNames.DashBoard_ExportReport,
                    
                    //Admin
                    //Tenant
                    PermissionNames.Admin_Tenant,
                    PermissionNames.Admin_Tenant_View,
                    PermissionNames.Admin_Tenant_Create,
                    PermissionNames.Admin_Tenant_Edit,
                    PermissionNames.Admin_Tenant_Delete,
                    //User
                    PermissionNames.Admin_User,
                    PermissionNames.Admin_User_View,
                    PermissionNames.Admin_User_Create,
                    PermissionNames.Admin_User_Edit,
                    PermissionNames.Admin_User_Delete,
                    PermissionNames.Admin_User_ResetPassword,
                    //Role
                    PermissionNames.Admin_Role,
                    PermissionNames.Admin_Role_View,
                    PermissionNames.Admin_Role_Create,
                    PermissionNames.Admin_Role_Edit,
                    PermissionNames.Admin_Role_Delete,

                    //Workflow
                    PermissionNames.Admin_Workflow,
                    PermissionNames.Admin_Workflow_Create,
                    PermissionNames.Admin_Workflow_Delete,
                    PermissionNames.Admin_Workflow_View,
                    PermissionNames.Admin_Workflow_ViewDetail,
                    PermissionNames.Admin_Workflow_WorkflowDetail_Edit,

                    //Workflow status
                    PermissionNames.Admin_WorkflowStatus,
                    PermissionNames.Admin_WorkflowStatus_Create,
                    PermissionNames.Admin_WorkflowStatus_Edit,
                    PermissionNames.Admin_WorkflowStatus_Delete,
                    PermissionNames.Admin_WorkflowStatus_View,

                    //Configuration
                    PermissionNames.Admin_Configuration,
                    PermissionNames.Admin_Configuration_View,
                    PermissionNames.Admin_Configuration_EditKomuSetting,
                    PermissionNames.Admin_Configuration_EditGoogleSetting,
                    PermissionNames.Admin_Configuration_EditSecretKey,
                    PermissionNames.Admin_Configuration_EditLinkToRequestChiDaHoanThanh,
                    PermissionNames.Admin_Configuration_ViewRequestChiSetting,
                    PermissionNames.Admin_Configuration_EditRequestChiSetting,
                    PermissionNames.Admin_Configuration_EditCoTheSuaThongTinCuaKiCu,

                    PermissionNames.Admin_CrawlHistory,
                    PermissionNames.Admin_CrawlHistory_View,

                    //LineChartSetting
                    PermissionNames.Admin_LineChartSetting,
                    PermissionNames.Admin_LineChartSetting_View,
                    PermissionNames.Admin_LineChartSetting_Create,
                    PermissionNames.Admin_LineChartSetting_Edit,
                    PermissionNames.Admin_LineChartSetting_Delete,
                    PermissionNames.Admin_LineChartSetting_ActiveDeactive,

                    //CircleChart
                    PermissionNames.Admin_CircleChart,
                    PermissionNames.Admin_CircleChart_View,
                    PermissionNames.Admin_CircleChart_Create,
                    PermissionNames.Admin_CircleChart_Edit,
                    PermissionNames.Admin_CircleChart_Delete,
                    PermissionNames.Admin_CircleChart_ActiveDeactive,

                    //CircleChartDetail
                    PermissionNames.Admin_CircleChart_CircleChartDetail,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_View,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Create,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Edit,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Delete,

                    // Crawl
                    PermissionNames.Admin_CrawlHistory,
                    PermissionNames.Admin_CrawlHistory_View,
                    
                    // Auditlog
                    PermissionNames.Admin_Auditlog,
                    PermissionNames.Admin_Auditlog_View,

                     //Supplier
                    PermissionNames.Directory_Supplier,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,

                    // Directory
                    //Currency
                    PermissionNames.Directory_Currency,
                    PermissionNames.Directory_Currency_Create,
                    PermissionNames.Directory_Currency_Delete,
                    PermissionNames.Directory_Currency_Edit,
                    PermissionNames.Directory_Currency_View,
                    PermissionNames.Directory_Currency_ChangeDefaultCurrency,
                    //Currency convert
                    PermissionNames.Directory_CurrencyConvert,
                    PermissionNames.Directory_CurrencyConvert_Create,
                    PermissionNames.Directory_CurrencyConvert_Delete,
                    PermissionNames.Directory_CurrencyConvert_Edit,
                    PermissionNames.Directory_CurrencyConvert_View,
                    //Bank
                    PermissionNames.Directory_Bank,
                    PermissionNames.Directory_Bank_Create,
                    PermissionNames.Directory_Bank_Edit,
                    PermissionNames.Directory_Bank_Delete,
                    PermissionNames.Directory_Bank_View,
                    //Account type
                    PermissionNames.Directory_AccountType,
                    PermissionNames.Directory_AccountType_Create,
                    PermissionNames.Directory_AccountType_Edit,
                    PermissionNames.Directory_AccountType_Delete,
                    PermissionNames.Directory_AccountType_View,
                    //Incoming entry
                    PermissionNames.Directory_IncomingEntryType,
                    PermissionNames.Directory_IncomingEntryType_Create,
                    PermissionNames.Directory_IncomingEntryType_Edit,
                    PermissionNames.Directory_IncomingEntryType_Delete,
                    PermissionNames.Directory_IncomingEntryType_View,

                    //Outcoming entry
                    PermissionNames.Directory_OutcomingEntryType,
                    PermissionNames.Directory_OutcomingEntryType_Create,
                    PermissionNames.Directory_OutcomingEntryType_Edit,
                    PermissionNames.Directory_OutcomingEntryType_Delete,
                    PermissionNames.Directory_OutcomingEntryType_View,
                    //Supplier
                    PermissionNames.Directory_Supplier ,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,
                    //Branch
                    PermissionNames.Directory_Branch,
                    PermissionNames.Directory_Branch_Create,
                    PermissionNames.Directory_Branch_Edit,
                    PermissionNames.Directory_Branch_Delete,
                    PermissionNames.Directory_Branch_View,

                    //Quản lý tài khoản
                    PermissionNames.Account_Directory_BankAccount,
                    PermissionNames.Account_Directory_BankAccount_View,
                    PermissionNames.Account_Directory_BankAccount_Create,
                    PermissionNames.Account_Directory_BankAccount_Edit,
                    PermissionNames.Account_Directory_BankAccount_Delete,
                    PermissionNames.Account_Directory_BankAccount_Export,
                    PermissionNames.Account_Directory_BankAccount_LockUnlock,
                    PermissionNames.Account_Directory_BankAccount_ActiveDeactive,
                    PermissionNames.Account_Directory_BankAccount_ViewBankAccountDetail,
                    PermissionNames.Account_Directory_BankAccount_EditBaseBalanace,
                    PermissionNames.Account_Directory_FinanceAccount,
                    PermissionNames.Account_Directory_FinanceAccount_View,
                    PermissionNames.Account_Directory_FinanceAccount_Create,
                    PermissionNames.Account_Directory_FinanceAccount_Edit,
                    PermissionNames.Account_Directory_FinanceAccount_Delete,
                    PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive,

                    //Quản lý tài chính
                     
                    //IncomingEntry
                    PermissionNames.Finance_IncomingEntry,
                    PermissionNames.Finance_IncomingEntry_View,
                    PermissionNames.Finance_IncomingEntry_ExportExcel,

                    //OutcomingEntry
                    PermissionNames.Finance_OutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_View,
                    PermissionNames.Finance_OutcomingEntry_Create,
                    PermissionNames.Finance_OutcomingEntry_Delete,
                    PermissionNames.Finance_OutcomingEntry_ViewAllOutcomingEntryTypeFilter,
                    PermissionNames.Finance_OutcomingEntry_ViewOnlyMe,
                    PermissionNames.Finance_OutcomingEntry_UpdateReportDate,
                    PermissionNames.Finance_OutcomingEntry_UpdateRequestType,
                    PermissionNames.Finance_OutcomingEntry_UpdateBranch,
                    PermissionNames.Finance_OutcomingEntry_ExportExcel,
                    PermissionNames.Finance_OutcomingEntry_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_ChangeStatus,
                    PermissionNames.Finance_OutcomingEntry_ViewYCTD,
                    PermissionNames.Finance_OutcomingEntry_AddInfoToYCTD,
                    PermissionNames.Finance_OutcomingEntry_EditInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_DeleteInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_RevertInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_AcceptYCTD,
                    PermissionNames.Finance_OutcomingEntry_RejectYCTD,
                    PermissionNames.Finance_OutcomingEntry_Clone,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_UpdateBranch,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry,
                    
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier,

                    //Bank Transaction
                    PermissionNames.Finance_BankTransaction,
                    PermissionNames.Finance_BankTransaction_View,
                    PermissionNames.Finance_BankTransaction_Create,
                    PermissionNames.Finance_BankTransaction_Delete,
                    PermissionNames.Finance_BankTransaction_ExportExcel,
                    PermissionNames.Finance_BankTransaction_LinkToBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_LockUnlock,
                    PermissionNames.Finance_BankTransaction_ViewBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View,
                                    

                    //Đối soát mới
                    PermissionNames.Finance_ComparativeStatisticNew,
                    PermissionNames.Finance_ComparativeStatisticNew_View,
                    //Đối soát
                    PermissionNames.Finance_ComparativeStatistic,
                    PermissionNames.Finance_ComparativeStatistic_View,
                    PermissionNames.Finance_ComparativeStatistic_ViewAllCompanyBankAccount,
                    PermissionNames.Finance_ComparativeStatistic_ViewExplanation,
                    PermissionNames.Finance_ComparativeStatistic_CreateExplanation,

                    //Biến động số dư
                    PermissionNames.Finance_BĐSD,
                    PermissionNames.Finance_BĐSD_View,
                    PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang,
                    PermissionNames.Finance_BĐSD_Create,
                    PermissionNames.Finance_BĐSD_Edit,
                    PermissionNames.Finance_BĐSD_Delete,
                    PermissionNames.Finance_BĐSD_LinkToRequestChi,
                    PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi,
                    PermissionNames.Finance_BĐSD_RemoveLinkToRequestChi,
                    PermissionNames.Finance_BĐSD_ChiLuong,
                    PermissionNames.Finance_BĐSD_BanNgoaiTe,
                    PermissionNames.Finance_BĐSD_MuaNgoaiTe,
                    PermissionNames.Finance_BĐSD_Import,
                    PermissionNames.Finance_BĐSD_KhachHangThanhToan,
                    PermissionNames.Finance_BĐSD_CreateIncomingEntry,
                    PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry,
                    PermissionNames.Finance_BĐSD_ChiChuyenDoi,
                    //invoice
                    PermissionNames.Finance_Invoice,
                    PermissionNames.Finance_Invoice_View,
                    PermissionNames.Finance_Invoice_Export_Report,
                    PermissionNames.Finance_Invoice_Create,
                    PermissionNames.Finance_Invoice_Update,
                    PermissionNames.Finance_Invoice_Delete,
                    PermissionNames.Finance_Invoice_AutoPay,
                    PermissionNames.Finance_Invoice_EditNote,
                    PermissionNames.Finance_Invoice_EditStatusInvoice,
                    PermissionNames.Finance_Invoice_KhachHangTraKenhTien,


                    //period
                    PermissionNames.Finance_Period,
                    PermissionNames.Finance_Period_Create,
                    PermissionNames.Finance_Period_View,
                    PermissionNames.Finance_Period_CloseAndCreate,
                    PermissionNames.Finance_Period_Edit,
                    
    
                }
            },
            {
                Tenants.CEO,
                new List<string>()
                {

                    PermissionNames.Dashboard,
                    PermissionNames.Admin,
                    PermissionNames.Directory,
                    PermissionNames.Account_Directory,
                    PermissionNames.Finance,

                    //DashBoard
                    PermissionNames.DashBoard_View,
                    PermissionNames.DashBoard_XemKhoiRequestChi,
                    PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO,
                    PermissionNames.DashBoard_XemKhoiBienDongSoDu,
                    PermissionNames.DashBoard_XemKhoiKhachHangTraNo,
                    PermissionNames.DashBoard_XemKhoiBieuDoBangThuChi,
                    PermissionNames.DashBoard_XemKhoiBieuDoTron,
                    PermissionNames.DashBoard_ExportReport,
                    
                    //Admin
                    //Tenant
                    PermissionNames.Admin_Tenant,
                    PermissionNames.Admin_Tenant_View,
                    PermissionNames.Admin_Tenant_Create,
                    PermissionNames.Admin_Tenant_Edit,
                    PermissionNames.Admin_Tenant_Delete,
                    //User
                    PermissionNames.Admin_User,
                    PermissionNames.Admin_User_View,
                    PermissionNames.Admin_User_Create,
                    PermissionNames.Admin_User_Edit,
                    PermissionNames.Admin_User_Delete,
                    PermissionNames.Admin_User_ResetPassword,
                    //Role
                    PermissionNames.Admin_Role,
                    PermissionNames.Admin_Role_View,
                    PermissionNames.Admin_Role_Create,
                    PermissionNames.Admin_Role_Edit,
                    PermissionNames.Admin_Role_Delete,

                    //Workflow
                    PermissionNames.Admin_Workflow,
                    PermissionNames.Admin_Workflow_Create,
                    PermissionNames.Admin_Workflow_Delete,
                    PermissionNames.Admin_Workflow_View,
                    PermissionNames.Admin_Workflow_ViewDetail,
                    PermissionNames.Admin_Workflow_ViewDetail_View,
                    PermissionNames.Admin_Workflow_WorkflowDetail_Edit,

                    //Workflow status
                    PermissionNames.Admin_WorkflowStatus,
                    PermissionNames.Admin_WorkflowStatus_Create,
                    PermissionNames.Admin_WorkflowStatus_Edit,
                    PermissionNames.Admin_WorkflowStatus_Delete,
                    PermissionNames.Admin_WorkflowStatus_View,

                    //Configuration
                    PermissionNames.Admin_Configuration,
                    PermissionNames.Admin_Configuration_View,
                    PermissionNames.Admin_Configuration_EditKomuSetting,
                    PermissionNames.Admin_Configuration_EditGoogleSetting,
                    PermissionNames.Admin_Configuration_EditSecretKey,
                    PermissionNames.Admin_Configuration_EditLinkToRequestChiDaHoanThanh,
                    PermissionNames.Admin_Configuration_EditCoTheSuaThongTinCuaKiCu,

                    PermissionNames.Admin_CrawlHistory,
                    PermissionNames.Admin_CrawlHistory_View,

                    //LineChartSetting
                    PermissionNames.Admin_LineChartSetting,
                    PermissionNames.Admin_LineChartSetting_View,
                    PermissionNames.Admin_LineChartSetting_Create,
                    PermissionNames.Admin_LineChartSetting_Edit,
                    PermissionNames.Admin_LineChartSetting_Delete,
                    PermissionNames.Admin_LineChartSetting_ActiveDeactive,

                    //CircleChart
                    PermissionNames.Admin_CircleChart,
                    PermissionNames.Admin_CircleChart_View,
                    PermissionNames.Admin_CircleChart_Create,
                    PermissionNames.Admin_CircleChart_Edit,
                    PermissionNames.Admin_CircleChart_Delete,
                    PermissionNames.Admin_CircleChart_ActiveDeactive,

                    //CircleChartDetail
                    PermissionNames.Admin_CircleChart_CircleChartDetail,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_View,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Create,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Edit,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Delete,

                    // Crawl
                    PermissionNames.Admin_CrawlHistory,
                    PermissionNames.Admin_CrawlHistory_View,

                    // Auditlog
                    PermissionNames.Admin_Auditlog,
                    PermissionNames.Admin_Auditlog_View,

                     //Supplier
                    PermissionNames.Directory_Supplier,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,

                    // Directory
                    //Currency
                    PermissionNames.Directory_Currency,
                    PermissionNames.Directory_Currency_Create,
                    PermissionNames.Directory_Currency_Delete,
                    PermissionNames.Directory_Currency_Edit,
                    PermissionNames.Directory_Currency_View,
                    PermissionNames.Directory_Currency_ChangeDefaultCurrency,
                    //Currency convert
                    PermissionNames.Directory_CurrencyConvert,
                    PermissionNames.Directory_CurrencyConvert_Create,
                    PermissionNames.Directory_CurrencyConvert_Delete,
                    PermissionNames.Directory_CurrencyConvert_Edit,
                    PermissionNames.Directory_CurrencyConvert_View,
                    //Bank
                    PermissionNames.Directory_Bank,
                    PermissionNames.Directory_Bank_Create,
                    PermissionNames.Directory_Bank_Edit,
                    PermissionNames.Directory_Bank_Delete,
                    PermissionNames.Directory_Bank_View,
                    //Account type
                    PermissionNames.Directory_AccountType,
                    PermissionNames.Directory_AccountType_Create,
                    PermissionNames.Directory_AccountType_Edit,
                    PermissionNames.Directory_AccountType_Delete,
                    PermissionNames.Directory_AccountType_View,
                    //Incoming entry
                    PermissionNames.Directory_IncomingEntryType,
                    PermissionNames.Directory_IncomingEntryType_Create,
                    PermissionNames.Directory_IncomingEntryType_Edit,
                    PermissionNames.Directory_IncomingEntryType_Delete,
                    PermissionNames.Directory_IncomingEntryType_View,

                    //Outcoming entry
                    PermissionNames.Directory_OutcomingEntryType,
                    PermissionNames.Directory_OutcomingEntryType_Create,
                    PermissionNames.Directory_OutcomingEntryType_Edit,
                    PermissionNames.Directory_OutcomingEntryType_Delete,
                    PermissionNames.Directory_OutcomingEntryType_View,
                    //Supplier
                    PermissionNames.Directory_Supplier ,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,
                    //Branch
                    PermissionNames.Directory_Branch,
                    PermissionNames.Directory_Branch_Create,
                    PermissionNames.Directory_Branch_Edit,
                    PermissionNames.Directory_Branch_Delete,
                    PermissionNames.Directory_Branch_View,

                    //Quản lý tài khoản
                    PermissionNames.Account_Directory_BankAccount,
                    PermissionNames.Account_Directory_BankAccount_View,
                    PermissionNames.Account_Directory_BankAccount_Create,
                    PermissionNames.Account_Directory_BankAccount_Edit,
                    PermissionNames.Account_Directory_BankAccount_Delete,
                    PermissionNames.Account_Directory_BankAccount_Export,
                    PermissionNames.Account_Directory_BankAccount_LockUnlock,
                    PermissionNames.Account_Directory_BankAccount_ActiveDeactive,
                    PermissionNames.Account_Directory_BankAccount_ViewBankAccountDetail,
                    PermissionNames.Account_Directory_FinanceAccount,
                    PermissionNames.Account_Directory_FinanceAccount_View,
                    PermissionNames.Account_Directory_FinanceAccount_Create,
                    PermissionNames.Account_Directory_FinanceAccount_Edit,
                    PermissionNames.Account_Directory_FinanceAccount_Delete,
                    PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive,

                    //Quản lý tài chính
                     
                    //IncomingEntry
                    PermissionNames.Finance_IncomingEntry,
                    PermissionNames.Finance_IncomingEntry_View,
                    PermissionNames.Finance_IncomingEntry_ExportExcel,

                    //OutcomingEntry
                    PermissionNames.Finance_OutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_View,
                    PermissionNames.Finance_OutcomingEntry_Create,
                    PermissionNames.Finance_OutcomingEntry_Delete,
                    PermissionNames.Finance_OutcomingEntry_ViewOnlyMe,
                    PermissionNames.Finance_OutcomingEntry_UpdateReportDate,
                    PermissionNames.Finance_OutcomingEntry_UpdateRequestType,
                    PermissionNames.Finance_OutcomingEntry_UpdateBranch,
                    PermissionNames.Finance_OutcomingEntry_ExportExcel,
                    PermissionNames.Finance_OutcomingEntry_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_ChangeStatus,
                    PermissionNames.Finance_OutcomingEntry_Clone,
                    PermissionNames.Finance_OutcomingEntry_ViewYCTD,
                    PermissionNames.Finance_OutcomingEntry_AddInfoToYCTD,
                    PermissionNames.Finance_OutcomingEntry_EditInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_DeleteInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_RevertInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_AcceptYCTD,
                    PermissionNames.Finance_OutcomingEntry_RejectYCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier,

                    //Bank Transaction
                    PermissionNames.Finance_BankTransaction,
                    PermissionNames.Finance_BankTransaction_View,
                    PermissionNames.Finance_BankTransaction_Create,
                    PermissionNames.Finance_BankTransaction_Delete,
                    PermissionNames.Finance_BankTransaction_ExportExcel,
                    PermissionNames.Finance_BankTransaction_LinkToBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_LockUnlock,
                    PermissionNames.Finance_BankTransaction_ViewBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View,


                    //Đối soát mới
                    PermissionNames.Finance_ComparativeStatisticNew,
                    PermissionNames.Finance_ComparativeStatisticNew_View,
                    //Đối soát
                    PermissionNames.Finance_ComparativeStatistic,
                    PermissionNames.Finance_ComparativeStatistic_View,
                    PermissionNames.Finance_ComparativeStatistic_ViewAllCompanyBankAccount,
                    PermissionNames.Finance_ComparativeStatistic_ViewExplanation,
                    PermissionNames.Finance_ComparativeStatistic_CreateExplanation,

                    //Biến động số dư
                    PermissionNames.Finance_BĐSD,
                    PermissionNames.Finance_BĐSD_View,
                    PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang,
                    PermissionNames.Finance_BĐSD_Create,
                    PermissionNames.Finance_BĐSD_Edit,
                    PermissionNames.Finance_BĐSD_Delete,
                    PermissionNames.Finance_BĐSD_LinkToRequestChi,
                    PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi,
                    PermissionNames.Finance_BĐSD_RemoveLinkToRequestChi,
                    PermissionNames.Finance_BĐSD_ChiLuong,
                    PermissionNames.Finance_BĐSD_Import,
                    PermissionNames.Finance_BĐSD_KhachHangThanhToan,
                    PermissionNames.Finance_BĐSD_CreateIncomingEntry,
                    PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry,
                    PermissionNames.Finance_BĐSD_ChiChuyenDoi,
                    //invoice
                    PermissionNames.Finance_Invoice,
                    PermissionNames.Finance_Invoice_View,
                    PermissionNames.Finance_Invoice_Export_Report,
                    PermissionNames.Finance_Invoice_Create,
                    PermissionNames.Finance_Invoice_Update,
                    PermissionNames.Finance_Invoice_Delete,
                    PermissionNames.Finance_Invoice_AutoPay,
                    PermissionNames.Finance_Invoice_EditNote,
                    PermissionNames.Finance_Invoice_EditStatusInvoice,
                    PermissionNames.Finance_Invoice_KhachHangTraKenhTien,


                    //period
                    PermissionNames.Finance_Period,
                    PermissionNames.Finance_Period_Create,
                    PermissionNames.Finance_Period_View,
                    PermissionNames.Finance_Period_CloseAndCreate,
                    PermissionNames.Finance_Period_Edit,


                }
            },
            {
                Tenants.Accountant,
                new List<string>()
                {

                    PermissionNames.Dashboard,
                    PermissionNames.Admin,
                    PermissionNames.Directory,
                    PermissionNames.Account_Directory,
                    PermissionNames.Finance,

                    //DashBoard
                    PermissionNames.DashBoard_View,
                    PermissionNames.DashBoard_XemKhoiRequestChi,
                    PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO,
                    PermissionNames.DashBoard_XemKhoiBienDongSoDu,
                    PermissionNames.DashBoard_XemKhoiKhachHangTraNo,
                    PermissionNames.DashBoard_XemKhoiBieuDoBangThuChi,
                    PermissionNames.DashBoard_XemKhoiBieuDoTron,
                    PermissionNames.DashBoard_ExportReport,
                    
                    //Admin
                    //Tenant
                    //User
                    PermissionNames.Admin_User,
                    PermissionNames.Admin_User_View,
                    PermissionNames.Admin_User_Create,

                    //Role
                    PermissionNames.Admin_Role,
                    PermissionNames.Admin_Role_View,

                    //Workflow
                    PermissionNames.Admin_Workflow,
                    PermissionNames.Admin_Workflow_View,
                    PermissionNames.Admin_Workflow_ViewDetail,
                    PermissionNames.Admin_Workflow_ViewDetail_View,

                    //Workflow status
                    PermissionNames.Admin_WorkflowStatus,
                    PermissionNames.Admin_WorkflowStatus_View,

                    //LineChartSetting
                    PermissionNames.Admin_LineChartSetting,
                    PermissionNames.Admin_LineChartSetting_View,
                    PermissionNames.Admin_LineChartSetting_Create,
                    PermissionNames.Admin_LineChartSetting_Edit,
                    PermissionNames.Admin_LineChartSetting_Delete,
                    PermissionNames.Admin_LineChartSetting_ActiveDeactive,

                    //CircleChart
                    PermissionNames.Admin_CircleChart,
                    PermissionNames.Admin_CircleChart_View,
                    PermissionNames.Admin_CircleChart_Create,
                    PermissionNames.Admin_CircleChart_Edit,
                    PermissionNames.Admin_CircleChart_Delete,
                    PermissionNames.Admin_CircleChart_ActiveDeactive,

                    //CircleChartDetail
                    PermissionNames.Admin_CircleChart_CircleChartDetail,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_View,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Create,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Edit,
                    PermissionNames.Admin_CircleChart_CircleChartDetail_Delete,

                    // Crawl
                    PermissionNames.Admin_CrawlHistory,
                    PermissionNames.Admin_CrawlHistory_View,

                    // Auditlog
                    PermissionNames.Admin_Auditlog,
                    PermissionNames.Admin_Auditlog_View,

                     //Supplier
                    PermissionNames.Directory_Supplier,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,

                    // Directory
                    //Currency
                    PermissionNames.Directory_Currency,
                    PermissionNames.Directory_Currency_Create,
                    PermissionNames.Directory_Currency_Delete,
                    PermissionNames.Directory_Currency_Edit,
                    PermissionNames.Directory_Currency_View,
                    PermissionNames.Directory_Currency_ChangeDefaultCurrency,
                    //Currency convert
                    PermissionNames.Directory_CurrencyConvert,
                    PermissionNames.Directory_CurrencyConvert_Create,
                    PermissionNames.Directory_CurrencyConvert_Delete,
                    PermissionNames.Directory_CurrencyConvert_Edit,
                    PermissionNames.Directory_CurrencyConvert_View,
                    //Bank
                    PermissionNames.Directory_Bank,
                    PermissionNames.Directory_Bank_Create,
                    PermissionNames.Directory_Bank_Edit,
                    PermissionNames.Directory_Bank_Delete,
                    PermissionNames.Directory_Bank_View,
                    //Account type
                    PermissionNames.Directory_AccountType,
                    PermissionNames.Directory_AccountType_Create,
                    PermissionNames.Directory_AccountType_Edit,
                    PermissionNames.Directory_AccountType_Delete,
                    PermissionNames.Directory_AccountType_View,
                    //Incoming entry
                    PermissionNames.Directory_IncomingEntryType,
                    PermissionNames.Directory_IncomingEntryType_Create,
                    PermissionNames.Directory_IncomingEntryType_Edit,
                    PermissionNames.Directory_IncomingEntryType_Delete,
                    PermissionNames.Directory_IncomingEntryType_View,

                    //Outcoming entry
                    PermissionNames.Directory_OutcomingEntryType,
                    PermissionNames.Directory_OutcomingEntryType_Create,
                    PermissionNames.Directory_OutcomingEntryType_Edit,
                    PermissionNames.Directory_OutcomingEntryType_Delete,
                    PermissionNames.Directory_OutcomingEntryType_View,
                    //Supplier
                    PermissionNames.Directory_Supplier ,
                    PermissionNames.Directory_Supplier_View,
                    PermissionNames.Directory_Supplier_Create,
                    PermissionNames.Directory_Supplier_Update,
                    PermissionNames.Directory_Supplier_Delete,
                    //Branch
                    PermissionNames.Directory_Branch,
                    PermissionNames.Directory_Branch_Create,
                    PermissionNames.Directory_Branch_Edit,
                    PermissionNames.Directory_Branch_Delete,
                    PermissionNames.Directory_Branch_View,

                    //Quản lý tài khoản
                    PermissionNames.Account_Directory_BankAccount,
                    PermissionNames.Account_Directory_BankAccount_View,
                    PermissionNames.Account_Directory_BankAccount_Create,
                    PermissionNames.Account_Directory_BankAccount_Edit,
                    PermissionNames.Account_Directory_BankAccount_Delete,
                    PermissionNames.Account_Directory_BankAccount_Export,
                    PermissionNames.Account_Directory_BankAccount_LockUnlock,
                    PermissionNames.Account_Directory_BankAccount_ActiveDeactive,
                    PermissionNames.Account_Directory_BankAccount_ViewBankAccountDetail,
                    PermissionNames.Account_Directory_FinanceAccount,
                    PermissionNames.Account_Directory_FinanceAccount_View,
                    PermissionNames.Account_Directory_FinanceAccount_Create,
                    PermissionNames.Account_Directory_FinanceAccount_Edit,
                    PermissionNames.Account_Directory_FinanceAccount_Delete,
                    PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive,

                    //Quản lý tài chính
                     
                    //IncomingEntry
                    PermissionNames.Finance_IncomingEntry,
                    PermissionNames.Finance_IncomingEntry_View,
                    PermissionNames.Finance_IncomingEntry_ExportExcel,

                    //OutcomingEntry
                    PermissionNames.Finance_OutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_View,
                    PermissionNames.Finance_OutcomingEntry_Create,
                    PermissionNames.Finance_OutcomingEntry_Delete,
                    PermissionNames.Finance_OutcomingEntry_ViewOnlyMe,
                    PermissionNames.Finance_OutcomingEntry_UpdateReportDate,
                    PermissionNames.Finance_OutcomingEntry_UpdateRequestType,
                    PermissionNames.Finance_OutcomingEntry_UpdateBranch,
                    PermissionNames.Finance_OutcomingEntry_ExportExcel,
                    PermissionNames.Finance_OutcomingEntry_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_ChangeStatus,
                    PermissionNames.Finance_OutcomingEntry_Clone,
                    PermissionNames.Finance_OutcomingEntry_ViewYCTD,
                    PermissionNames.Finance_OutcomingEntry_AddInfoToYCTD,
                    PermissionNames.Finance_OutcomingEntry_EditInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_DeleteInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_RevertInfoOfYCTD,
                    PermissionNames.Finance_OutcomingEntry_RejectYCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier,

                    //Bank Transaction
                    PermissionNames.Finance_BankTransaction,
                    PermissionNames.Finance_BankTransaction_View,
                    PermissionNames.Finance_BankTransaction_Create,
                    PermissionNames.Finance_BankTransaction_Delete,
                    PermissionNames.Finance_BankTransaction_ExportExcel,
                    PermissionNames.Finance_BankTransaction_LinkToBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_LockUnlock,
                    PermissionNames.Finance_BankTransaction_ViewBienDongSoDu,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit,

                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry,
                    PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View,
                                    

                    //Đối soát mới
                    PermissionNames.Finance_ComparativeStatisticNew,
                    PermissionNames.Finance_ComparativeStatisticNew_View,
                    //Đối soát
                    PermissionNames.Finance_ComparativeStatistic,
                    PermissionNames.Finance_ComparativeStatistic_View,
                    PermissionNames.Finance_ComparativeStatistic_ViewAllCompanyBankAccount,
                    PermissionNames.Finance_ComparativeStatistic_ViewExplanation,
                    PermissionNames.Finance_ComparativeStatistic_CreateExplanation,

                    //Biến động số dư
                    PermissionNames.Finance_BĐSD,
                    PermissionNames.Finance_BĐSD_View,
                    PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang,
                    PermissionNames.Finance_BĐSD_Create,
                    PermissionNames.Finance_BĐSD_Edit,
                    PermissionNames.Finance_BĐSD_Delete,
                    PermissionNames.Finance_BĐSD_LinkToRequestChi,
                    PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi,
                    PermissionNames.Finance_BĐSD_RemoveLinkToRequestChi,
                    PermissionNames.Finance_BĐSD_ChiLuong,
                    PermissionNames.Finance_BĐSD_Import,
                    PermissionNames.Finance_BĐSD_KhachHangThanhToan,
                    PermissionNames.Finance_BĐSD_CreateIncomingEntry,
                    PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry,
                    PermissionNames.Finance_BĐSD_ChiChuyenDoi,
                    //invoice
                    PermissionNames.Finance_Invoice,
                    PermissionNames.Finance_Invoice_View,
                    PermissionNames.Finance_Invoice_Export_Report,
                    PermissionNames.Finance_Invoice_Create,
                    PermissionNames.Finance_Invoice_Update,
                    PermissionNames.Finance_Invoice_Delete,
                    PermissionNames.Finance_Invoice_AutoPay,
                    PermissionNames.Finance_Invoice_EditNote,
                    PermissionNames.Finance_Invoice_EditStatusInvoice,
                    PermissionNames.Finance_Invoice_KhachHangTraKenhTien,


                    //period
                    PermissionNames.Finance_Period,
                    PermissionNames.Finance_Period_Create,
                    PermissionNames.Finance_Period_View,
                    PermissionNames.Finance_Period_CloseAndCreate,
                    PermissionNames.Finance_Period_Edit,


                }
            },
            {
                Tenants.Requester,
                new List<string>()
                {
                    PermissionNames.Finance,

                    //Quản lý tài chính
                     

                    //OutcomingEntry
                    PermissionNames.Finance_OutcomingEntry,
                    PermissionNames.Finance_OutcomingEntry_Create,
                    PermissionNames.Finance_OutcomingEntry_ViewOnlyMe,
                    PermissionNames.Finance_OutcomingEntry_ChangeStatus,
                    PermissionNames.Finance_OutcomingEntry_Clone,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View,

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View,
                   

                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier,
                    PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier,
                }
            }
        };

        public class SystemPermission
        {
            public string Name { get; set; }
            public MultiTenancySides MultiTenancySides { get; set; }
            public string DisplayName { get; set; }
            public bool IsConfiguration { get; set; }
            public List<SystemPermission> Childrens { get; set; }

            public static List<SystemPermission> ListPermissions = new List<SystemPermission>()
            {
                new SystemPermission{ Name =  PermissionNames.Dashboard, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Trang chủ" },
                new SystemPermission{ Name =  PermissionNames.Admin, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin" },
                new SystemPermission{ Name =  PermissionNames.Directory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản trị danh mục" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý tài khoản" },
                new SystemPermission{ Name =  PermissionNames.Finance, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý tài chính" },
                //DashBoard
                new SystemPermission{ Name =  PermissionNames.DashBoard_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem trang chủ" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_ExportReport, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export báo cáo" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối request chi" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Nhắc nhở CEO" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBienDongSoDu, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biến động số dư" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiKhachHangTraNo, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối khách hàng trả nợ" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBieuDoBangThuChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biểu đồ / bảng thu chi" },
                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBieuDoTron, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biểu đồ tròn" },

                //Admin
                //Tenant
                new SystemPermission{ Name =  PermissionNames.Admin_Tenant, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tenant" },
                new SystemPermission{ Name =  PermissionNames.Admin_Tenant_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                //User
                new SystemPermission{ Name =  PermissionNames.Admin_User, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "User" },
                new SystemPermission{ Name =  PermissionNames.Admin_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_User_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_User_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Admin_User_ResetPassword, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đặt lại mật khẩu" },
                //Role
                new SystemPermission{ Name =  PermissionNames.Admin_Role, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Role" },
                new SystemPermission{ Name =  PermissionNames.Admin_Role_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_Role_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_Role_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_Role_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                //Workflow
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Work Flow" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_ViewDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_ViewDetail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết" },
                new SystemPermission{ Name =  PermissionNames.Admin_Workflow_WorkflowDetail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },

                //Workflow status
                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Trạng thái Work Flow" },
                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //Configutation
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Config" },
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditKomuSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Komu Setting" },
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditGoogleSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Google Setting" },
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditSecretKey, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Secret Key" },
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditLinkToRequestChiDaHoanThanh,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết/ Bỏ liên kết đến request chi đã hoàn thành"},
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ViewRequestChiSetting,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem request chi setting"},
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditRequestChiSetting,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa request chi setting"},
                new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditCoTheSuaThongTinCuaKiCu,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa cho phép sửa thông tin trong kì cũ"},
                
                //Line Chart Setting
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Line Chart setting" },
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                
                //Circle Chart
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Circle Chart setting" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },

                //Circle Chart Detail
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Circle Chart Detail setting" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                
                //Lịch sử Crawl
                new SystemPermission{ Name =  PermissionNames.Admin_CrawlHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lịch sử Crawl giao dịch" },
                new SystemPermission{ Name =  PermissionNames.Admin_CrawlHistory_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                // Auditlog
                new SystemPermission{ Name =  PermissionNames.Admin_Auditlog, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Auditlog" },
                new SystemPermission{ Name =  PermissionNames.Admin_Auditlog_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                //Currency
                new SystemPermission{ Name =  PermissionNames.Directory_Currency, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tiền tệ" },

                new SystemPermission{ Name =  PermissionNames.Directory_Currency_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_Currency_ChangeDefaultCurrency, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi tiền tệ mặc định" },
                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //CurrencyConvert
                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tỉ giá" },
                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //Bank
                new SystemPermission{ Name =  PermissionNames.Directory_Bank, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bank" },
                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Directory_Bank_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },

                //AccountType
                new SystemPermission{ Name =  PermissionNames.Directory_AccountType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại đối tượng" },
                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //IncomingEntryType
                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại thu" },
                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //outcomingEntryType
                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại chi" },
                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                //Branches
                new SystemPermission{ Name =  PermissionNames.Directory_Branch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi nhánh" },
                new SystemPermission{ Name =  PermissionNames.Directory_Branch_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                //Supplier
                new SystemPermission{ Name =  PermissionNames.Directory_Supplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Nhà cung cấp" },
                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Update, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                //Account
                //Bank account
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tài khoản ngân hàng" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_LockUnlock, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lock/Unlock" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Export, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_ViewBankAccountDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết tài khoản ngân hàng" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_EditBaseBalanace, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cập nhật số dư đầu kì" },
                //Đối tượng kế toán
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối tượng kế toán" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                //IncomingEntries
                new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Ghi nhận thu" },
                new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },

                //OutcomingEntry
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý Request chi" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewAllOutcomingEntryTypeFilter, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lọc theo tất cả loại chi"},
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewOnlyMe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉ xem các request chi mình tạo ra" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateReportDate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa ngày báo cáo" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateRequestType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa loại yêu cầu" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateBranch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa chi nhánh" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất file excel" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ExportPdf, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất phiếu chi pdf" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ChangeStatus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi trạng thái theo workflow (gửi duyệt, từ chối, duyệt, thực thi)" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Clone, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Clone" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_AddInfoToYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thêm mới thông tin chi tiết cho YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_EditInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa thông tin chi tiết của YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_DeleteInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa thông tin chi tiết của YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_RevertInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Revert thông tin chi tiết của YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_AcceptYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chấp nhận YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_RejectYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Từ chối YCTĐ" },
                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết request chi" },
                // Tab thông tin chung
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab thông tin chung"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa loại yêu cầu"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa ngày báo cáo"},
                
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Clone"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Yêu cầu thay đổi"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Sửa/Gửi YCTĐ"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Thêm/Chỉnh sửa tất cả thảo luận"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xuất phiếu chi pdf"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa tất cả thảo luận"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Đính kèm file"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Accept file"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa file đính kèm"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Thêm/Chỉnh sửa thảo luận của bản thân"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa thảo luận của bản thân"},
                                        
                //Tab thông tin chi tiết
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab thông tin chi tiết"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem thông tin"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tạo mới"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_UpdateBranch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa chi nhánh"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Đã trả/Chưa trả"},
                
                // Tab các giao dịch ngân hàng liên quan
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab giao dịch ngân hàng liên quan"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới giao dịch ngân hàng"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới giao dịch ngân hàng"},

                
                // Tab các nguồn thu liên quan
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab nguồn thu liên quan"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem thông tin"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới nguồn thu"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới nguồn thu"},
                //Tab nhà cung cấp
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab nhà cung cấp"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới nhà cung cấp"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tạo mới nhà cung cấp"},
                new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới nhà cung cấp"},
                
                //BankTransactions
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Giao dịch ngân hàng" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_LinkToBienDongSoDu, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Link đến biến động số dư" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_LockUnlock, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lock/UnLock" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết giao dịch ngân hàng" },
                
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab giao dịch ngân hàng" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab ghi nhận thu" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab request chi" },
                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                
                 
                //Đối soát mới
                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatisticNew, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối soát mới" },
                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatisticNew_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                //Đối soát
                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối soát" },
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_ViewAllCompanyBankAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả các tài khoản ngân hàng của công ty" },
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_ViewExplanation, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết giải trình"},
                                new SystemPermission{ Name = PermissionNames.Finance_ComparativeStatistic_CreateExplanation, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thêm giải trình"},

                //Biến động số dư
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Biến động số dư"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cài đặt thanh toán khách hàng"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_LinkToRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết đến một request chi"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết đến nhiều request chi"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_RemoveLinkToRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thu hồi liên kết đến request chi"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_ChiLuong, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Link tới request chi"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_BanNgoaiTe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bán ngoại tệ"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_MuaNgoaiTe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Mua ngoại tệ"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Import, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_KhachHangThanhToan, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khách hàng thanh toán"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CreateIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới 1 ghi nhận thu"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới nhiều ghi nhận thu"},
                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_ChiChuyenDoi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi chuyển đổi"},
                //Invoice
                new SystemPermission{ Name =  PermissionNames.Finance_Invoice, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khoản phải thu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới (khoản phải thu)" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Export_Report, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất báo cáo công nợ"},
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_AutoPay, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Auto trả nợ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_KhachHangTraKenhTien, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khách hàng trả kênh tiền" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Update, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa khoản phải thu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa khoản phải thu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_EditNote, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa ghi chú" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_EditStatusInvoice, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa trạng thái khoản phải thu" },
                // Period
                new SystemPermission{Name = PermissionNames.Finance_Period, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Kỳ kế toán"},
                new SystemPermission{Name = PermissionNames.Finance_Period_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả"},
                new SystemPermission{Name = PermissionNames.Finance_Period_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới kỳ kế toán đầu tiên"},
                new SystemPermission{Name = PermissionNames.Finance_Period_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa"},
                new SystemPermission{Name = PermissionNames.Finance_Period_CloseAndCreate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đóng kì hiện tại và tạo mới"},
               


            };

            public static List<SystemPermission> TreePermissions = new List<SystemPermission>()
            {
                 new SystemPermission{ Name =  PermissionNames.Dashboard, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Home",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.DashBoard_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem trang chủ" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_ExportReport, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export báo cáo" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối request chi" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiRequestChi_NhacnhoCEO, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Nhắc nhở CEO" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBienDongSoDu, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biến động số dư" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiKhachHangTraNo, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối khách hàng trả nợ" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBieuDoBangThuChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biểu đồ / bảng thu chi" },
                                new SystemPermission{ Name =  PermissionNames.DashBoard_XemKhoiBieuDoTron, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem khối biểu đồ tròn" },
                            }
                        },
                new SystemPermission { Name =  PermissionNames.Admin, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin",
                    Childrens = new List<SystemPermission>() {
                        new SystemPermission { Name = PermissionNames.Admin_Tenant, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Tenant" ,
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name = PermissionNames.Admin_Tenant_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name = PermissionNames.Admin_Tenant_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name = PermissionNames.Admin_Tenant_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name = PermissionNames.Admin_Tenant_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },
                        new SystemPermission { Name = PermissionNames.Admin_User, MultiTenancySides = MultiTenancySides.Host, DisplayName = "User" , 
                            Childrens = new List<SystemPermission>()
                            {
         
                                new SystemPermission{ Name =  PermissionNames.Admin_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Admin_User_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Admin_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Admin_User_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Admin_User_ResetPassword, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đặt lại mật khẩu" },
                            }
                            
                        },
                        new SystemPermission { Name = PermissionNames.Admin_Role, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Role" , 
                            Childrens = new List < SystemPermission >() {
                                new SystemPermission{ Name =  PermissionNames.Admin_Role_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Admin_Role_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Admin_Role_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Admin_Role_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },
                        new SystemPermission { Name = PermissionNames.Admin_Workflow, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Workflow",
                            Childrens = new List<SystemPermission>()
                            {

                                 new SystemPermission{ Name =  PermissionNames.Admin_Workflow_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Workflow_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Workflow_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Workflow_ViewDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Admin_Workflow_ViewDetail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết" },
                                        new SystemPermission{ Name =  PermissionNames.Admin_Workflow_WorkflowDetail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                    }
                                 
                                 },

                            }
                        },
                        new SystemPermission { Name =  PermissionNames.Admin_WorkflowStatus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Trạng thái",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Admin_WorkflowStatus_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },
                        
                        new SystemPermission{ Name =  PermissionNames.Admin_Configuration, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Configuration",
                            Childrens = new List<SystemPermission>()
                            {
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditKomuSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Komu Setting" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditGoogleSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Google Setting" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditSecretKey, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa Secret Key" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditLinkToRequestChiDaHoanThanh, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết/ Bỏ liên kết đến request chi đã hoàn thành"},
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ViewRequestChiSetting,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem request chi setting"},
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditRequestChiSetting,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa request chi setting"},
                                 new SystemPermission{ Name =  PermissionNames.Admin_Configuration_EditCoTheSuaThongTinCuaKiCu,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa cho phép sửa thông tin trong kì cũ"}
                            }
                        },
                       new SystemPermission{ Name =  PermissionNames.Admin_CrawlHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Crawl history",
                            Childrens = new List<SystemPermission>()
                            {
                                 new SystemPermission{ Name =  PermissionNames.Admin_CrawlHistory_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                            }
                        },
                       new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Line Chart setting",
                            Childrens = new List<SystemPermission>()
                            {
                                 new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_LineChartSetting_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },

                            }
                        },
                       new SystemPermission{ Name =  PermissionNames.Admin_CircleChart, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Circle Chart setting",
                            Childrens = new List<SystemPermission>()
                            {
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                                 new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Circle Chart Detail setting",
                                     Childrens = new List<SystemPermission>()
                                     {
                                          new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                          new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                          new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                          new SystemPermission{ Name =  PermissionNames.Admin_CircleChart_CircleChartDetail_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },

                                     }
                                 }
                            }
                        },
                       new SystemPermission{ Name =  PermissionNames.Admin_Auditlog, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Audtilog",
                            Childrens = new List<SystemPermission>()
                            {
                                 new SystemPermission{ Name =  PermissionNames.Admin_Auditlog_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },

                            }
                        },
                    }
                },
                new SystemPermission { Name =  PermissionNames.Directory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản trị danh mục",
                    Childrens = new List<SystemPermission>() {
                        new SystemPermission{ Name =  PermissionNames.Directory_Currency, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tiền tệ",
                            Childrens = new List<SystemPermission>()
                            {

                                new SystemPermission{ Name =  PermissionNames.Directory_Currency_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Currency_ChangeDefaultCurrency, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi tiền tệ mặc định" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Currency_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tỉ giá",
                            Childrens = new List<SystemPermission>()
                            {

                                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_CurrencyConvert_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Directory_Bank, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Ngân hàng",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_Bank_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Bank_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },  
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Directory_AccountType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại đối tượng",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_AccountType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },

                        new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại thu",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_IncomingEntryType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Loại chi",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_OutcomingEntryType_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },

                        new SystemPermission{ Name =  PermissionNames.Directory_Branch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi nhánh",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_Branch_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Branch_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                        },

                        
                        new SystemPermission{ Name =  PermissionNames.Directory_Supplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Nhà cung cấp",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Update, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Directory_Supplier_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                            }
                         },
                    }
                },
                 new SystemPermission { Name =  PermissionNames.Account_Directory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý tài khoản",
                    Childrens = new List<SystemPermission>() {
                         new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tài khoản ngân hàng",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_LockUnlock, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lock/Unlock" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_Export, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_ViewBankAccountDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết tài khoản ngân hàng" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_BankAccount_EditBaseBalanace, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sửa số dư đầu kì"},
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối tượng kế toán",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active/Deactive" },
                            }
                        },
                    }
                },
                new SystemPermission { Name =  PermissionNames.Finance, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý tài chính",
                    Childrens = new List<SystemPermission>() {
                         new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Ghi nhận thu",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_IncomingEntry_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },

                            }
                         },
                         new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quản lý request chi",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewAllOutcomingEntryTypeFilter, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lọc theo tất cả loại chi" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewOnlyMe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉ xem các request chi mình tạo ra" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateReportDate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa ngày báo cáo" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateRequestType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa loại yêu cầu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_UpdateBranch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa chi nhánh" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất file excel" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ExportPdf, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất phiếu chi pdf" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ChangeStatus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi trạng thái theo workflow (gửi duyệt, từ chối, duyệt, thực thi)" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_Clone, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Clone" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_ViewYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem YCTĐ" },

                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_AddInfoToYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thêm mới thông tin chi tiết cho YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_EditInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa thông tin chi tiết của YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_DeleteInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa thông tin chi tiết của YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_RevertInfoOfYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Revert thông tin chi tiết của YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_AcceptYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chấp nhận YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_RejectYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Từ chối YCTĐ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết request chi" ,
                                    Childrens =  new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tab thông tin chung",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa loại yêu cầu"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa ngày báo cáo"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Clone"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_YCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Yêu cầu thay đổi"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_SendYCTD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Sửa/Gửi YCTĐ"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Thêm/Chỉnh sửa tất cả thảo luận"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Thêm/Chỉnh sửa thảo luận của bản thân"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa thảo luận của bản thân"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa tất cả thảo luận"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xuất phiếu chi pdf"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Đính kèm file"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Accept file"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa file đính kèm"},
                                    }
                                },

                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab thông tin chi tiết",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tạo mới"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_UpdateBranch, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Chỉnh sửa chi nhánh"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Thay đổi trạng thái Đã trả"},
                                        
                                    }
                                },

                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab giao dịch ngân hàng liên quan",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới giao dịch ngân hàng"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới giao dịch ngân hàng"},

                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab nguồn thu liên quan",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem thông tin"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới nguồn thu"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới nguồn thu"},
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab nhà cung cấp",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xem thông tin"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Liên kết tới nhà cung cấp"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Tạo mới nhà cung cấp"},
                                        new SystemPermission{ Name = PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Xóa liên kết tới nhà cung cấp"},
                                    }
                                },
                                    },


                                },

                                
                            }


                         },
                         new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Giao dịch ngân hàng",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_ExportExcel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export file" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_LinkToBienDongSoDu, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Link đến biến động số dư" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_LockUnlock, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Lock/UnLock" },
                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi tiết giao dịch ngân hàng" ,
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab giao dịch ngân hàng" ,
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa" },
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab ghi nhận thu" ,
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sửa loại thu" },
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab request chi" ,
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Finance_BankTransaction_BankTransactionDetail_TabOutcomingEntry_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem" },
                                            }
                                        },

                                    }
                                },
                            }
                        },

                         new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatisticNew, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối soát mới",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatisticNew_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                            }

                        },
                        new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đối soát",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_ViewAllCompanyBankAccount, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả các tài khoản ngân hàng của công ty" },
                                new SystemPermission{ Name =  PermissionNames.Finance_ComparativeStatistic_ViewExplanation, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết giải trình"},
                                new SystemPermission{ Name = PermissionNames.Finance_ComparativeStatistic_CreateExplanation, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thêm giải trình"},
                            }
                        },


                        new SystemPermission{ Name = PermissionNames.Finance_BĐSD, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Biến động số dư", 
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CaiDatThanhToanKhachHang, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cài đặt thanh toán khách hàng"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_LinkToRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết đến một request chi"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_LinkToMultipleRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Liên kết đến nhiều request chi"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_RemoveLinkToRequestChi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thu hổi liên kết đến request chi"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_ChiLuong, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Link tới request chi"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_BanNgoaiTe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bán ngoại tệ"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_MuaNgoaiTe, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Mua ngoại tệ"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_Import, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_KhachHangThanhToan, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khách hàng thanh toán"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CreateIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới 1 ghi nhận thu"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_CreateMultiIncomingEntry, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới nhiều ghi nhận thu"},
                                new SystemPermission{ Name = PermissionNames.Finance_BĐSD_ChiChuyenDoi, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chi chuyển đổi"},
                            }

                        },
                          new SystemPermission{ Name =  PermissionNames.Finance_Invoice, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khoản phải thu",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới (khoản phải thu)" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Export_Report, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xuất báo cáo công nợ"},
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_AutoPay, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Auto trả nợ" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_KhachHangTraKenhTien, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Khách hàng trả kênh tiền" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Update, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa khoản phải thu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa khoản phải thu" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_EditNote, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa ghi chú" },
                                new SystemPermission{ Name =  PermissionNames.Finance_Invoice_EditStatusInvoice, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa trạng thái khoản phải thu" },
                                
                            }
                          },

                           new SystemPermission{Name = PermissionNames.Finance_Period, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Kỳ kế toán",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{Name = PermissionNames.Finance_Period_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem tất cả"},
                                    new SystemPermission{Name = PermissionNames.Finance_Period_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tạo mới kỳ kế toán đầu tiên"},
                                    new SystemPermission{Name = PermissionNames.Finance_Period_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Chỉnh sửa"},
                                    new SystemPermission{Name = PermissionNames.Finance_Period_CloseAndCreate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Đóng kì hiện tại và tạo mới"},

                                }
                           },
                    }
                },
            };
        }
    }
}