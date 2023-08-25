import { BTransactionLogModule } from './modules/new-versions/b-transaction-log/b-transaction-log.module';
import { InvoiceComponent } from './modules/invoice/invoice.component';
import { TableFilterComponent } from './../shared/table-filter/table-filter.component';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { ExpenditureListComponent } from './modules/expenditure/expenditure-list/expenditure-list.component';
import { ExpenditureComponent } from './modules/expenditure/expenditure.component';
import { CreateEditExpenditureComponent } from './modules/expenditure/create-edit-expenditure/create-edit-expenditure.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxPaginationModule } from 'ngx-pagination';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';
import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { DatePipe } from '@angular/common'
import { BrowserModule } from '@angular/platform-browser';
// tenants
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
// roles
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
// users
import { UsersComponent } from '@app/users/users.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
// layout
import { HeaderComponent } from './layout/header.component';
import { HeaderLeftNavbarComponent } from './layout/header-left-navbar.component';
import { HeaderLanguageMenuComponent } from './layout/header-language-menu.component';
import { HeaderUserMenuComponent } from './layout/header-user-menu.component';
import { FooterComponent } from './layout/footer.component';
import { SidebarComponent } from './layout/sidebar.component';
import { SidebarLogoComponent } from './layout/sidebar-logo.component';
import { SidebarUserPanelComponent } from './layout/sidebar-user-panel.component';
import { SidebarMenuComponent } from './layout/sidebar-menu.component';
import { CurrencyComponent } from './modules/currency/currency.component';
import { CreateCurrencyComponent } from './modules/currency/create-currency/create-currency.component';
import { EditCurrencyComponent } from './modules/currency/edit-currency/edit-currency.component';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { FilterComponent } from '@shared/filter/filter.component';
import { BankComponent } from './modules/bank/bank.component';
import { CreateEditBankComponent } from './modules/bank/create-edit-bank/create-edit-bank.component';
import { AccountantAccountComponent } from './modules/accountant-account/accountant-account.component';
import { CreateEditAccountantAccountComponent } from './modules/accountant-account/create-edit-accountant-account/create-edit-accountant-account.component';
import { AccountTypeComponent } from './modules/account-type/account-type.component';
import { CreateEditAccountTypeComponent } from './modules/account-type/create-edit-account-type/create-edit-account-type.component';
import { BranchComponent } from './modules/branch/branch.component';
import { CreateEditBranchComponent } from './modules/branch/create-edit-branch/create-edit-branch.component';
import { RevenueComponent } from './modules/revenue/revenue.component';
import { RevenueListComponent } from './modules/revenue/revenue-list/revenue-list.component';
import { CreateEditRevenueComponent } from './modules/revenue/create-edit-revenue/create-edit-revenue.component';
import { BankingTransactionComponent } from './modules/banking-transaction/banking-transaction.component';
import { CreateEditTransactionComponent } from './modules/banking-transaction/create-edit-transaction/create-edit-transaction.component';
import { RevenueRecordingComponent } from './modules/revenue-recording/revenue-recording.component';
import { FinanceDetailComponent } from './modules/finance-detail/finance-detail.component';
import { CreateEditRecordComponent } from './modules/finance-detail/create-edit-record/create-edit-record.component';
import { WorkFlowComponent } from './modules/work-flow/work-flow.component';
import { SocialLoginModule, SocialAuthServiceConfig, GoogleLoginProvider } from 'angularx-social-login';
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
import { BankAccountComponent } from './modules/bank-account/bank-account.component';

import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CreateEditBankAccountComponent } from './modules/bank-account/create-edit-bank-account/create-edit-bank-account.component';
import { BankAccountDetailComponent } from './modules/bank-account/bank-account-detail/bank-account-detail.component';
import { CreateEditWorkFlowComponent } from './modules/work-flow/create-edit-work-flow/create-edit-work-flow.component';
import { WorkFlowDetailComponent } from './modules/work-flow/work-flow-detail/work-flow-detail.component';
import { ExpenditureRequestComponent } from './modules/expenditure-request/expenditure-request.component';
import { CreateEditRequestComponent } from './modules/expenditure-request/create-edit-request/create-edit-request.component';
import { CheckWarningCreateRequestComponent} from './modules/expenditure-request/check-warning-create-request/check-warning-create-request.component';
import { ExpenditureRequestDetailComponent } from './modules/expenditure-request-detail/expenditure-request-detail.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { CreateRequestDetailComponent } from './modules/expenditure-request-detail/create-request-detail/create-request-detail.component';
import { LinkTransactionComponent } from './modules/expenditure-request-detail/link-transaction/link-transaction.component';
import { CreateMultiTransactionComponent } from './modules/expenditure-request-detail/create-multi-transaction/create-multi-transaction.component';
import { MainTabComponent } from './modules/expenditure-request-detail/main-tab/main-tab.component';
import { RelevantTabComponent } from './modules/expenditure-request-detail/relevant-tab/relevant-tab.component';
import { DetailTabComponent } from './modules/expenditure-request-detail/detail-tab/detail-tab.component';


import { CreateEditStatusComponent } from './modules/work-flow/create-edit-status/create-edit-status.component';
import { CreateEditTransitionComponent } from './modules/work-flow/create-edit-transition/create-edit-transition.component';
import { EditTransitionComponent } from './modules/work-flow/edit-transition/edit-transition.component';
import { RevenuesTabComponent } from './modules/expenditure-request-detail/revenues-tab/revenues-tab.component';
import { LinkRevenuesComponent } from './modules/expenditure-request-detail/link-revenues/link-revenues.component';
import { LinkSupplierComponent } from './modules/expenditure-request-detail/supplier/link-supplier/link-supplier.component';
import { SupplierComponent } from './modules/expenditure-request-detail/supplier/supplier.component';
import { SupplierListComponent } from './modules/supplier-list/supplier-list.component';
import { CreateEditSupplierComponent } from './modules/supplier-list/create-edit-supplier/create-edit-supplier.component';
import { StatusComponent } from './modules/status/status.component';
import { CreateEditDialogStatusComponent } from './modules/status/create-edit-dialog-status/create-edit-dialog-status.component';
import { CurrencyMaskModule } from "ng2-currency-mask";
import { NgxMaskModule } from 'ngx-mask';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { ClickOutsideModule } from 'ng-click-outside';
import { LinkToExpenditureRequestComponent } from './modules/finance-detail/link-to-expenditure-request/link-to-expenditure-request.component';
import { AdminSettingComponent } from './modules/admin-setting/admin-setting.component';
import { ImportRequestFileComponent } from './modules/expenditure-request-detail/main-tab/import-request-file/import-request-file.component';
import { CreateEditInvoiceComponent } from './modules/invoice/create-edit-invoice/create-edit-invoice.component';
import { InvoiceDetailComponent } from './modules/invoice/invoice-detail/invoice-detail.component';
import { GeneralTabComponent } from './modules/invoice/invoice-detail/general-tab/general-tab.component';
import { ProjectTimesheetComponent } from './modules/invoice/invoice-detail/project-timesheet/project-timesheet.component';
import { TransactionTabComponent } from './modules/invoice/invoice-detail/transaction-tab/transaction-tab.component';
import { ExportReportComponent } from './home/export-report/export-report.component';
import { FinanceReviewComponent } from './modules/finance-review/finance-review.component';
import { ReviewExplainComponent } from './modules/finance-review/review-explain/review-explain.component';
import { ViewReviewExplainComponent } from './modules/finance-review/view-review-explain/view-review-explain.component';
import { RevenueManagedComponent } from './modules/revenue-managed/revenue-managed.component';
import { CreateEditRevenuemanagedComponent } from './modules/revenue-managed/create-edit-revenuemanaged/create-edit-revenuemanaged.component';
import { UploadFileDialogComponent } from './modules/revenue-managed/upload-file-dialog/upload-file-dialog.component';
import { BTransactionLogComponent } from './modules/new-versions/b-transaction-log/b-transaction-log.component';
import { LinkBTransactionDialogComponent } from './modules/banking-transaction/link-BTransaction-dialog/link-b-transaction-dialog.component';
import { LinkExpenditureAndBTransDialogComponent } from './modules/new-versions/b-transaction/link-expenditure-dialog/link-expenditure-dialog.component';
import { LinkRevenueRecognitionAndBTransDialogComponent } from './modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { TempModeTableComponent } from './modules/expenditure-request-detail/detail-tab/temp-mode/temp-mode-table/temp-mode-table.component';
import { NormalDetailTableComponent } from './modules/expenditure-request-detail/detail-tab/normal-detail/normal-detail-table/normal-detail-table.component';
import { RequestChangeDialogComponent } from './modules/expenditure-request-detail/request-change-dialog/request-change-dialog.component';
import { RequestChangeMainTabComponent } from './modules/expenditure-request-detail/request-change-dialog/request-change-main-tab/request-change-main-tab.component';
import { PeriodComponent } from './modules/period/period.component';
import { CreateEditPeriodComponent } from './modules/period/create-edit-period/create-edit-period.component';
import { HeaderPeriodComponent } from './layout/header-period.component';
import {CloseAndCreatePeriodComponent} from './modules/period/close-and-create-period/close-and-create-period.component';
import { CurrencyConvertComponent } from './modules/currency-convert/currency-convert.component';
import { CreateEditCurrencyConvertComponent } from './modules/currency-convert/create-edit-currency-convert/create-edit-currency-convert.component';
import { LineChartSettingComponent } from './modules/line-chart-setting/line-chart-setting.component';
import { CreateEditLineChartSettingComponent } from './modules/line-chart-setting/create-edit-line-chart-setting/create-edit-line-chart-setting.component';
import { SelectionTreeComponent } from './modules/line-chart-setting/create-edit-line-chart-setting/selection-tree/selection-tree.component';
import { FinanceReviewOldComponent } from './modules/finance-review-old/finance-review-old.component';
import { CurrencyExchangeComponent } from './modules/new-versions/b-transaction/currency-exchange/currency-exchange.component';
import { EditReportDateComponent } from './modules/expenditure-request/edit-report-date/edit-report-date.component';
import { EditOutcomingTypeComponent } from './modules/expenditure-request/edit-outcoming-type/edit-outcoming-type.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { BTransactionModule } from './modules/new-versions/b-transaction/b-transaction.module';
import { UpdateImcomeTypeComponent } from './modules/finance-detail/update-imcome-type/update-imcome-type.component';
import { CloneRequestComponent } from './modules/expenditure-request/clone-request/clone-request.component';
import { AuditlogComponent } from './modules/auditlog/auditlog.component';
import { ActiveCompanyBankAccountComponent } from './modules/bank-account/active-company-bank-account/active-company-bank-account.component';
import {UpdateBaseBalanaceComponent} from './modules/bank-account/bank-account-detail/update-base-balanace/update-base-balanace.component';
import { ImportDetailComponent } from './modules/expenditure-request-detail/detail-tab/normal-detail/import-detail/import-detail.component';
import { UpdateBranchComponent } from './modules/expenditure-request-detail/main-tab/update-branch/update-branch.component';
import { DetailBaocaoThuComponent } from './home/detail-baocao-thu/detail-baocao-thu.component';
import { DetailBaocaoChiComponent } from './home/detail-baocao-chi/detail-baocao-chi.component';
import { DetailNhanvienNoComponent } from './home/detail-nhanvien-no/detail-nhanvien-no.component';
import { AppConsts } from '@shared/AppConsts';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    // tenants
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
    // layout
    HeaderComponent,
    HeaderLeftNavbarComponent,
    HeaderLanguageMenuComponent,
    HeaderUserMenuComponent,
    FooterComponent,
    SidebarComponent,
    SidebarLogoComponent,
    SidebarUserPanelComponent,
    SidebarMenuComponent,
    CurrencyComponent,
    CreateCurrencyComponent,
    EditCurrencyComponent,
    FilterComponent,
    TableFilterComponent,
    BankComponent,
    CreateEditBankComponent,
    AccountantAccountComponent,
    CreateEditAccountantAccountComponent,
    AccountTypeComponent,
    CreateEditAccountTypeComponent,
    BranchComponent,
    CreateEditBranchComponent,
    RevenueComponent,
    RevenueListComponent,
    CreateEditRevenueComponent,
    CreateEditExpenditureComponent,
    ExpenditureComponent,
    ExpenditureListComponent,
    BankingTransactionComponent,
    CreateEditTransactionComponent,
    RevenueRecordingComponent,
    FinanceDetailComponent,
    BankAccountComponent,
    CreateEditBankAccountComponent,
    CreateEditBranchComponent,
    BranchComponent,
    BankAccountDetailComponent,

    CreateEditRecordComponent,
    WorkFlowComponent,

    CreateEditWorkFlowComponent,

    WorkFlowDetailComponent,

    ExpenditureRequestComponent,

    CreateEditRequestComponent,

    CheckWarningCreateRequestComponent,

    ExpenditureRequestDetailComponent,

    CreateRequestDetailComponent,

    CreateEditStatusComponent,

    CreateEditTransitionComponent,

    EditTransitionComponent,

    LinkTransactionComponent,

    CreateMultiTransactionComponent,

    MainTabComponent,

    RelevantTabComponent,

    DetailTabComponent,

    RevenuesTabComponent,

    LinkRevenuesComponent,

    SupplierComponent,

    LinkSupplierComponent,
    SupplierListComponent,
    CreateEditSupplierComponent,
    StatusComponent,
    CreateEditDialogStatusComponent,
    LinkToExpenditureRequestComponent,
    AdminSettingComponent,
    ImportRequestFileComponent,
    InvoiceComponent,
    CreateEditInvoiceComponent,
    InvoiceDetailComponent,
    GeneralTabComponent,
    ProjectTimesheetComponent,
    TransactionTabComponent,
    ExportReportComponent,
    FinanceReviewComponent,
    ReviewExplainComponent,
    ViewReviewExplainComponent,
    RevenueManagedComponent,
    CreateEditRevenuemanagedComponent,
    UploadFileDialogComponent,
    BTransactionLogComponent,
    LinkBTransactionDialogComponent,
    LinkExpenditureAndBTransDialogComponent,
    LinkRevenueRecognitionAndBTransDialogComponent,
    TempModeTableComponent,
    NormalDetailTableComponent,
    RequestChangeDialogComponent,
    RequestChangeMainTabComponent,
    PeriodComponent,
    CreateEditPeriodComponent,
    HeaderPeriodComponent,
    CloseAndCreatePeriodComponent,
    LineChartSettingComponent,
    CreateEditLineChartSettingComponent,
    SelectionTreeComponent,
    FinanceReviewOldComponent,
    CurrencyConvertComponent,
    CreateEditCurrencyConvertComponent,
    EditReportDateComponent,
    EditOutcomingTypeComponent,
    UpdateImcomeTypeComponent,
    CloneRequestComponent,
    ActiveCompanyBankAccountComponent,
    AuditlogComponent,

    UpdateBaseBalanaceComponent,

    ImportDetailComponent,

    UpdateBranchComponent,

    DetailBaocaoThuComponent,

    DetailBaocaoChiComponent,

    DetailNhanvienNoComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ModalModule.forChild(),
    BsDropdownModule,
    CollapseModule,
    TabsModule,
    AppRoutingModule,
    ServiceProxyModule,
    SharedModule,
    NgxPaginationModule,
    MatSelectModule,
    MatFormFieldModule,
    Ng2SearchPipeModule,
    SocialLoginModule,
    CurrencyMaskModule,
    AngularEditorModule,
    ClickOutsideModule,
    BTransactionLogModule,
    NgxMatSelectSearchModule,
    NgxMaskModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      }
    }),
    BTransactionModule

  ],
  exports: [
    TranslateModule,
  ],
  providers: [
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: true,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(
              AppConsts.googleClientId
            ),
          },
        ],
      } as SocialAuthServiceConfig,
    },
    {
      provide: MAT_DATE_LOCALE, useValue: 'en-GB'
    },
    DatePipe
  ],
  entryComponents: [
    // tenants
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ResetPasswordDialogComponent,
    //bank
    CreateEditBankComponent,
    //
    BankAccountComponent,
    CreateEditBankAccountComponent,
  ],
})
export class AppModule { }
