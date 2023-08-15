import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
import * as _ from 'lodash';
import { DefaultIncomingEntryType } from '../currency-exchange/currency-exchange.component';
import { IncomingEntryTypeOptions } from '../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { CommonService } from './../../../../service/api/new-versions/common.service';
import { PaymentInvoiceForAccount } from './../../../../service/model/b-transaction.model';
import { ValueAndNameModel } from './../../../../service/model/common-DTO';

@Component({
  selector: 'app-payment-dialog',
  templateUrl: './payment-dialog.component.html',
  styleUrls: ['./payment-dialog.component.css']
})
export class PaymentDialogComponent extends AppComponentBase implements OnInit {
  customerOptions: ValueAndNameModel[] = [];
  filteredCustomerOptions: ValueAndNameModel[] = [];
  currencyNeedConverts: CurrencyNeedConvert[] = [];
  public incomingEntryTypeOptions: IncomingEntryTypeOptions;
  public searchCustomer: string;
  public isDisable = false;
  payment: PaymentInvoiceForAccount = new PaymentInvoiceForAccount();
  public isFocusing: boolean = false;
  public lableDirectionKhachHang: LableDirection = LableDirection.Left;
  public isDefaultIncomingEntryType: boolean;
  public defaultIncomingEntryType: number;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<PaymentDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PaymentDialogData,
    private _common: CommonService,
    private _btransaction: BtransactionService,
    private _configuration: AppConfigurationService,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setCustomerOptions();
    this.getTreeIncomingEntries();
    this.setDefaultIncomingEntryTypes();
  }


  filterAccount(searchString: string) {
    let value = searchString.trim().toLowerCase();
    this.filteredCustomerOptions = this.customerOptions.filter((account) =>
      account.name.trim().toLowerCase().includes(value)
    );
  }
  getTreeIncomingEntries(){
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      this.incomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: [...response.result]};
    })
  }

  process() {
    this.isDisable = true
    this.payment.bTransactionId = this.data.bTransactionId;
    this.payment.currencyNeedConverts = this.currencyNeedConverts;
    let accountName = this.customerOptions.find((account) => account.value == this.payment.accountId).name;
    abp.message.confirm(
      `Khách hàng <strong>${accountName}</strong> thanh toán`,
      "",
      (result: boolean) => {
        if (result) {
          this.doSave();
        }
        this.isDisable = false
      },
      true
    );
  }

  doSave() {
    this._btransaction.paymentForAccount(this.payment)
      .subscribe(response => {
        if (!response.success) return;
        abp.notify.success('Updated successfully');
        this.dialogRef.close();
      }, () => { this.isDisable = false });
  }


  customerHandler() {
    this._btransaction.checkAccount(this.data.bTransactionId, this.payment.accountId)
      .subscribe(response => {
        if (!response.success) return;
        this.currencyNeedConverts = response.result;
      });
  }

  private setCustomerOptions(): void {
    this._common.getAllClient().subscribe(response => {
      if (!response.success) return;
      this.customerOptions = response.result;
      this.filteredCustomerOptions = this.customerOptions;
    })
  }

  handleSelectOpenedChange(isOpen) {
    isOpen ? this.isFocusing = true : this.isFocusing = false;
    if(this.filteredCustomerOptions.length === 0){
      this.searchCustomer = "";
      this.filteredCustomerOptions = _.cloneDeep(this.customerOptions);
    }
  }
  public focusOut(){
    this.isFocusing= false;
  }
  isBtnDisable(){
    if(this.payment.isCreateBonus){
      return !this.payment.incomingEntryName || !this.payment.incomingEntryValue || !this.payment.incomingEntryTypeId || this.isDisable;
    }
    return this.isDisable;
  }
  incomingEntryTypeIdChange(){
    this.isDefaultIncomingEntryType = this.defaultIncomingEntryType && this.defaultIncomingEntryType == this.payment.incomingEntryTypeId;
  }
  setDefaultIncomingEntryTypes(){
    this._configuration.getDefaultMaLoaiThuKhachHangBonus().subscribe(response =>{
      if(!response.success) return;
      if(response.result){
        this.payment.incomingEntryTypeId = Number(response.result);
      }
      this.defaultIncomingEntryType = response.result;
      this.isDefaultIncomingEntryType = response.result && response.result == this.payment.incomingEntryTypeId;
    })
  }

  defaultIncomingEntryTypeChange(checked : boolean){
    this.isDefaultIncomingEntryType = !this.isDefaultIncomingEntryType;
    if (this.isDefaultIncomingEntryType) {
      this._configuration.setDefaultMaLoaiThuKhachHangBonus({ id : this.payment.incomingEntryTypeId.toString()} as DefaultIncomingEntryType).subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Update default incoming entry successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    } else {
      this._configuration.clearDefaultMaLoaiThuKhachHangBonus().subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Clear default incoming entry  successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    }
  }
}

export interface PaymentDialogData {
  bTransactionId: number;
  money: number;
  currencyName: string;
}
export interface CurrencyNeedConvert {
  fromCurrencyId: number;
  fromCurrencyName: string;
  toCurrencyId: number;
  toCurrencyName: string;
  isReverseExchangeRate : boolean;
}
