import { BTransactionComponent } from './b-transaction.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BTransactionRoutingModule } from './b-transaction-routing.module';
import { ListBtransactionComponent } from './list-btransaction/list-btransaction.component';
import { SharedModule } from '../../../../shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { PaymentDialogComponent } from './payment-dialog/payment-dialog.component';
import { SettingPaymentDialogComponent } from './setting-payment-dialog/setting-payment-dialog.component';
import { NgxMaskModule } from 'ngx-mask';
import { CreateEditBTransactionComponent } from './create-edit-b-transaction/create-edit-b-transaction.component';
import { LinkBTransactionMultiOutComingDialogComponent } from './link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import { ImportFileComponent } from './import-file/import-file.component';
import { DialogResultImportFileComponent } from './dialog-result-import-file/dialog-result-import-file.component';
import { LinkMultiBtransactionOutcomingEntryDialogComponent } from './link-multi-btransaction-outcoming-entry-dialog/link-multi-btransaction-outcoming-entry-dialog.component';
import { RollbackLinkOutcomingEntryComponent } from './rollback-link-outcoming-entry/rollback-link-outcoming-entry.component';
import { CurrencyExchangeComponent } from './currency-exchange/currency-exchange.component';
import { BuyForeignCurrencyComponent } from './buy-foreign-currency/buy-foreign-currency.component';
import { CreateMultiIncomingEntryComponent } from './create-multi-incoming-entry/create-multi-incoming-entry.component';
import { SelectionTreeIncomingEntryTypeComponent } from './create-multi-incoming-entry/selection-tree-incoming-entry-type/selection-tree-incoming-entry-type.component';
import { FormsModule } from '@angular/forms';
import { ChiChuyenDoiComponent } from './chi-chuyen-doi/chi-chuyen-doi.component';
import { LinkExpenditureResultComponent } from './link-expenditure-dialog/link-expenditure-result/link-expenditure-result.component';
import { RollbackClientPaidComponent } from './rollback-client-paid/rollback-client-paid.component';

@NgModule({
  declarations: [BTransactionComponent, ListBtransactionComponent, PaymentDialogComponent, SettingPaymentDialogComponent, CreateEditBTransactionComponent, LinkBTransactionMultiOutComingDialogComponent, ImportFileComponent, DialogResultImportFileComponent, LinkMultiBtransactionOutcomingEntryDialogComponent, RollbackLinkOutcomingEntryComponent, CurrencyExchangeComponent, BuyForeignCurrencyComponent, CreateMultiIncomingEntryComponent, ChiChuyenDoiComponent, LinkExpenditureResultComponent, RollbackClientPaidComponent],
  imports: [
    CommonModule,
    BTransactionRoutingModule,
    SharedModule,
    NgxPaginationModule,
    NgxMaskModule.forRoot(),
    FormsModule
  ],
  exports:[

  ]
})
export class BTransactionModule { }
