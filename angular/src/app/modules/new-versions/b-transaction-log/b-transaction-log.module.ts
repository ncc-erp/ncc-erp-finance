import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BTransactionLogRoutingModule } from './b-transaction-log-routing.module';
import { ListBTransactionLogComponent } from './list-b-transaction-log/list-b-transaction-log.component';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgxMaskModule } from 'ngx-mask';


@NgModule({
  declarations: [ListBTransactionLogComponent],
  imports: [
    CommonModule,
    SharedModule,
    BTransactionLogRoutingModule,
    NgxPaginationModule,
    NgxMaskModule.forRoot(),
  ]
})
export class BTransactionLogModule { }
