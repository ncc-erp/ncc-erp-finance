import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NRevenueRoutingModule } from './n-revenue-routing.module';
import { NRevenueComponent } from './n-revenue.component';
import { ListNRevenueComponent } from './list-n-revenue/list-n-revenue.component';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CreateEditNRevenueComponent } from './create-edit-n-revenue/create-edit-n-revenue.component';
import { EditStatusNRevenueComponent } from './edit-status-n-revenue/edit-status-n-revenue.component';
import { EditNoteNRevenueComponent } from './edit-note-n-revenue/edit-note-n-revenue.component';
import { AutoPaymentDebtDialogComponent } from './auto-payment-debt-dialog/auto-payment-debt-dialog.component';
import { NgxMaskModule } from 'ngx-mask';
import { CreateNRevenueByAccountComponent } from './create-n-revenue-by-account/create-n-revenue-by-account.component';
import { ClientPayDeviantDialogComponent } from './client-pay-deviant-dialog/client-pay-deviant-dialog.component';

@NgModule({
  declarations: [NRevenueComponent, ListNRevenueComponent, CreateEditNRevenueComponent, EditStatusNRevenueComponent, EditNoteNRevenueComponent, AutoPaymentDebtDialogComponent, CreateNRevenueByAccountComponent, ClientPayDeviantDialogComponent],
  imports: [
    CommonModule,
    NRevenueRoutingModule,
    SharedModule,
    NgxPaginationModule,
    NgxMaskModule.forRoot(),
  ]
})
export class NRevenueModule { }
