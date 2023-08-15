import { AuditlogComponent } from './modules/auditlog/auditlog.component';
import { BTransactionComponent } from './modules/new-versions/b-transaction/b-transaction.component';
import { FinanceReviewComponent } from './modules/finance-review/finance-review.component';
import { TransactionTabComponent } from './modules/invoice/invoice-detail/transaction-tab/transaction-tab.component';
import { InvoiceDetailComponent } from './modules/invoice/invoice-detail/invoice-detail.component';
import { InvoiceComponent } from './modules/invoice/invoice.component';
import { AdminSettingComponent } from './modules/admin-setting/admin-setting.component';
import { SupplierComponent } from './modules/expenditure-request-detail/supplier/supplier.component';
import { RelevantTabComponent } from './modules/expenditure-request-detail/relevant-tab/relevant-tab.component';
import { ExpenditureRequestDetailComponent } from './modules/expenditure-request-detail/expenditure-request-detail.component';
import { ExpenditureRequestComponent } from './modules/expenditure-request/expenditure-request.component';
import { FinanceDetailComponent } from './modules/finance-detail/finance-detail.component';
import { ExpenditureComponent } from './modules/expenditure/expenditure.component';
import { RevenueComponent } from './modules/revenue/revenue.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { CurrencyComponent } from './modules/currency/currency.component';
import { BankComponent } from './modules/bank/bank.component';
import { AccountantAccountComponent } from './modules/accountant-account/accountant-account.component';
import { BranchComponent } from './modules/branch/branch.component';
import { AccountTypeComponent } from './modules/account-type/account-type.component';
import { BankingTransactionComponent } from './modules/banking-transaction/banking-transaction.component';
import { RevenueRecordingComponent } from './modules/revenue-recording/revenue-recording.component';
import { BankAccountComponent } from './modules/bank-account/bank-account.component';
import { BankAccountDetailComponent } from './modules/bank-account/bank-account-detail/bank-account-detail.component';

import { WorkFlowComponent } from './modules/work-flow/work-flow.component';
import { WorkFlowDetailComponent } from './modules/work-flow/work-flow-detail/work-flow-detail.component';
import { MainTabComponent } from './modules/expenditure-request-detail/main-tab/main-tab.component';
import { DetailTabComponent } from './modules/expenditure-request-detail/detail-tab/detail-tab.component';

import { RevenuesTabComponent } from './modules/expenditure-request-detail/revenues-tab/revenues-tab.component';
import { SupplierListComponent } from './modules/supplier-list/supplier-list.component';
import { StatusComponent } from './modules/status/status.component';
import { GeneralTabComponent } from './modules/invoice/invoice-detail/general-tab/general-tab.component';
import { ProjectTimesheetComponent } from './modules/invoice/invoice-detail/project-timesheet/project-timesheet.component';
import { RevenueManagedComponent } from './modules/revenue-managed/revenue-managed.component';
import { RequestChangeDialogComponent } from './modules/expenditure-request-detail/request-change-dialog/request-change-dialog.component';
import { RequestChangeMainTabComponent } from './modules/expenditure-request-detail/request-change-dialog/request-change-main-tab/request-change-main-tab.component';
import { PeriodComponent } from './modules/period/period.component';
import { LineChartSettingComponent } from './modules/line-chart-setting/line-chart-setting.component';
import { FinanceReviewOldComponent } from './modules/finance-review-old/finance-review-old.component';
import { CurrencyConvertComponent } from './modules/currency-convert/currency-convert.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent,canActivate: [AppRouteGuard] ,data: { permission: 'Dashboard' }},
                    { path: 'users', component: UsersComponent, data: { permission: 'Admin.User' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Admin.Role' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Admin.Tenant' }, canActivate: [AppRouteGuard] },
                    { path: 'workFlow', component: WorkFlowComponent, canActivate: [AppRouteGuard] },
                    { path: 'lineChartSetting', component: LineChartSettingComponent, canActivate: [AppRouteGuard] },
                    { path: 'auditlog', component: AuditlogComponent, data: { permission: 'Admin.Auditlog' }, canActivate: [AppRouteGuard] },
                    { path: 'workFlowDetail', component: WorkFlowDetailComponent, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent },
                    { path: 'update-password', component: ChangePasswordComponent },
                    { path: 'currencies', component: CurrencyComponent, canActivate: [AppRouteGuard] },
                    { path: 'currency-convert', component:CurrencyConvertComponent, canActivate: [AppRouteGuard]},
                    { path: 'banks', component: BankComponent, canActivate: [AppRouteGuard] },
                    { path: 'branches', component: BranchComponent, canActivate: [AppRouteGuard] },
                    { path: 'accountant-account', component: AccountantAccountComponent, canActivate: [AppRouteGuard] },
                    { path: 'accountType', component: AccountTypeComponent, canActivate: [AppRouteGuard] },
                    { path: "incomingType", component: RevenueComponent, canActivate: [AppRouteGuard] },
                    { path: "outcomingType", component: ExpenditureComponent, canActivate: [AppRouteGuard] },
                    { path: "bank-transaction", component: BankingTransactionComponent, canActivate: [AppRouteGuard] },
                    { path: "revenue-record", component: RevenueRecordingComponent, canActivate: [AppRouteGuard] },
                    { path: "detail", component: FinanceDetailComponent, canActivate: [AppRouteGuard] },
                    { path: "setting", component: AdminSettingComponent, canActivate: [AppRouteGuard] },

                    { path: 'bank-account', component: BankAccountComponent },
                    { path: "bank-account/detail", component: BankAccountDetailComponent, canActivate: [AppRouteGuard] },
                    { path: "expenditure-request", component: ExpenditureRequestComponent, canActivate: [AppRouteGuard] },
                    { path: "supplierList", component: SupplierListComponent, canActivate: [AppRouteGuard] },
                    { path: "supplier", component: SupplierComponent, canActivate: [AppRouteGuard] },
                    { path: "status", component: StatusComponent, canActivate: [AppRouteGuard] },
                    {
                        path: "requestDetail", component: ExpenditureRequestDetailComponent, canActivate: [AppRouteGuard],
                        children: [{
                            path: "main",
                            component: MainTabComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: "detail",
                            component: DetailTabComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: 'relevant-transaction',
                            component: RelevantTabComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: 'revenues-transaction',
                            component: RevenuesTabComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: 'supplier',
                            component: SupplierComponent,
                            canActivate: [AppRouteGuard]
                        }
                        ]
                    },
                    {
                        path: "invoiceDetail", component: InvoiceDetailComponent, canActivate: [AppRouteGuard],
                        children: [{
                            path: "invoice-general",
                            component: GeneralTabComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: "project-timesheet",
                            component: ProjectTimesheetComponent,
                            canActivate: [AppRouteGuard]
                        },
                        {
                            path: "transaction-tab",
                            component: TransactionTabComponent,
                            canActivate: [AppRouteGuard]
                        }
                        ]
                    }
                    ,
                    {
                        path: "bankAccountDetail", component: BankAccountDetailComponent, canActivate: [AppRouteGuard]
                    },
                    {
                        path: "invoice", component: InvoiceComponent, canActivate: [AppRouteGuard]
                    },
                    { path: "finance-review", component: FinanceReviewComponent, canActivate: [AppRouteGuard] },
                    { path: "finance-statistic-old", component: FinanceReviewOldComponent, canActivate: [AppRouteGuard] },
                    { path: "revenue-managed", component: RevenueManagedComponent, canActivate: [AppRouteGuard] },
                    { path: "btransaction", loadChildren: () => import('app/modules/new-versions/b-transaction/b-transaction.module').then(m => m.BTransactionModule) },
                    { path: "nrevenue", loadChildren: () => import('app/modules/new-versions/n-revenue/n-revenue.module').then(m => m.NRevenueModule) },
                    { path: "b-transaction-log", loadChildren: () => import('app/modules/new-versions/b-transaction-log/b-transaction-log.module').then(m => m.BTransactionLogModule) },
                    { path: 'period', component: PeriodComponent, canActivate: [AppRouteGuard]}
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
