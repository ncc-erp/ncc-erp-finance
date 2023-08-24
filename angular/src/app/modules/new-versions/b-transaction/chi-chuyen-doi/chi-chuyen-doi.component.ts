import { Component, Inject, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { BTransaction } from '@app/service/model/b-transaction.model';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { SelectionOutcomingEntry } from '../link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import * as _ from 'lodash';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { IncomingEntryTypeOptions } from '../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { ConversionTransactionDto, CurrencyExchangeComponent, CurrencyExchangeDialogData, DefaultIncomingEntryType, OptionIncomingEntryTypeDto } from '../currency-exchange/currency-exchange.component';
import { CurrencyService } from '@app/service/api/currency.service';
import { CurrencyDto } from '@app/modules/currency/currency.component';
import { AccountTypeEnum } from '@shared/AppEnums';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';


@Component({
  selector: 'app-chi-chuyen-doi',
  templateUrl: './chi-chuyen-doi.component.html',
  styleUrls: ['./chi-chuyen-doi.component.css']
})
export class ChiChuyenDoiComponent extends AppComponentBase implements OnInit {

  public outcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public toBankAccounts: ValueAndNameModel[];
  public fromBankAccounts: ValueAndNameModel[];
  public bTransactionMinus: BTransaction[];
  public bTransactionPlus: BTransaction[];
  public chiChuyenDoiDto : ChiChuyenDoiDto = new ChiChuyenDoiDto();

  public incomingEntryTypeTreeOptions: IncomingEntryTypeOptions;
  public linkDone = false;
  public totalBTransactionMinus: number;
  public totalBTransactionPlus: number;
  public defaultIncomingEntryTypes: number;
  public isDefaultIncomingEntryTypes: boolean;
  public currencyIdBTransactionMinus: number;
  public currencyIdBTransactionPlus: number;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<CurrencyExchangeComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: CurrencyExchangeDialogData,
    private _common: CommonService,
    public _utilities: UtilitiesService,
    private _btransaction: BtransactionService,
    private _currency: CurrencyService,
    private _configuration: AppConfigurationService,
  ) {
    super(injector);
  }


  ngOnInit(): void {
    this.setBTransaction();
    this.setDefaultIncomingEntryTypes();
    this.setBankAccountTransaction();
    this.setIncomingEntryTypes();
    this.setOutcommingEntryOptions();
  }
  setBTransaction(){
    this.bTransactionMinus = this.data.bTransactions.filter(s => s.moneyNumber < 0).sort((a, b) => a.moneyNumber - b.moneyNumber);
    this.totalBTransactionMinus = this.data.bTransactions.filter(s => s.moneyNumber < 0).reduce((a, b) => {return a - b.moneyNumber;}, 0);
    this.totalBTransactionPlus = this.data.bTransactions.filter(s => s.moneyNumber > 0).reduce((a, b) => {return a + b.moneyNumber;}, 0);
    this.bTransactionPlus = this.data.bTransactions.filter(s => s.moneyNumber > 0).sort((a, b) => b.moneyNumber - a.moneyNumber);
    this.currencyIdBTransactionMinus = this.data.bTransactions.filter(s => s.moneyNumber < 0)[0].currencyId;
    this.currencyIdBTransactionPlus = this.data.bTransactions.filter(s => s.moneyNumber > 0)[0].currencyId;
  }
  setBankAccountTransaction() {
    this.getFromBank();
    this.getToBank();
  }
  getFromBank(){
    this._common.getBankAccountOptions({currencyId: this.currencyIdBTransactionPlus, isActive: true, orderByType: AccountTypeEnum.MEDIUM} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.fromBankAccounts = response.result;
      if(response.result.length) this.chiChuyenDoiDto.fromBankAccountId = response.result[0].value;
    });
  }
  getToBank(){
    this._common.getBankAccountOptions({currencyId: this.currencyIdBTransactionMinus, isActive: true, orderByType: AccountTypeEnum.MEDIUM} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.toBankAccounts = response.result;
      if(response.result.length) this.chiChuyenDoiDto.toBankAccountId = response.result[0].value;
    });
  }
  setIncomingEntryTypes() {
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      let incomingEntryTypeOptions : IncomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: []};
      incomingEntryTypeOptions.children = response.result;
      this.incomingEntryTypeTreeOptions = incomingEntryTypeOptions;
    })
  }
  setOutcommingEntryOptions(onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved, this.currencyIdBTransactionMinus).subscribe(response => {
      if (!response.success) return;
      this.outcommingEntryOptions = response.result;
    })
  }
  incomingEntryTypeIdChange(){
    this.isDefaultIncomingEntryTypes = this.defaultIncomingEntryTypes && this.defaultIncomingEntryTypes == this.chiChuyenDoiDto.incomingEntryTypeId;
  }
  setDefaultIncomingEntryTypes(){
    this._configuration.getDefaultLoaiThuIdKhiChiChuyenDoi().subscribe(response =>{
      if(!response.success) return;
      if(response.result){
        this.chiChuyenDoiDto.incomingEntryTypeId = Number(response.result);
      }
      this.defaultIncomingEntryTypes = response.result;
      this.isDefaultIncomingEntryTypes = response.result && response.result == this.chiChuyenDoiDto.incomingEntryTypeId;
    })
  }

  process(){
    this.chiChuyenDoiDto.minusBTransactionIds = this.bTransactionMinus.map(s => s.bTransactionId);
    this.chiChuyenDoiDto.plusBTransactionIds = this.bTransactionPlus.map(s => s.bTransactionId);
    this._btransaction.checkChiChuyenDoi(this.chiChuyenDoiDto).subscribe(response => {
      if (!response.success) {
        return;
      };
      abp.message.confirm(
        "",
        "",
        (result: boolean) => {
          if (result) {
            this.link();
          }
        }
      );
    })
  }
  defaultIncomingEntryTypeChange(checked : boolean){
    this.isDefaultIncomingEntryTypes = !this.isDefaultIncomingEntryTypes;
    if (this.isDefaultIncomingEntryTypes) {
      this._configuration.setDefaultLoaiThuIdKhiChiChuyenDoi({ id : this.chiChuyenDoiDto.incomingEntryTypeId.toString()} as DefaultIncomingEntryType).subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Update default incoming entry type successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    } else {
      this._configuration.clearDefaultLoaiThuIdKhiChiChuyenDoi().subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Clear default incoming entry type successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    }
  }
  link(){
    this._btransaction.chiChuyenDoi(this.chiChuyenDoiDto)
    .subscribe(response => {
      if (!response.success) return;
      this.linkDone = true;
    })
  }
  openOutcomingEntry(){
    this.dialogRef.close({outcomingEntryId: this.chiChuyenDoiDto.outcomingEntryId, linkDone: this.linkDone});
  }
  getTiGia(){
    if(this.totalBTransactionMinus > this.totalBTransactionPlus)
      return `(1 ${this.bTransactionPlus[0].currencyName} = ${this._utilities.formatMoneyCustom(Number((this.totalBTransactionMinus*1.0/this.totalBTransactionPlus).toFixed(2)))} ${this.bTransactionMinus[0].currencyName} )`
    return `(1 ${this.bTransactionMinus[0].currencyName} = ${this._utilities.formatMoneyCustom(Number((this.totalBTransactionPlus*1.0/this.totalBTransactionMinus).toFixed(2)))} ${this.bTransactionPlus[0].currencyName} )`
  }
  isDisable(){
    if(!this.chiChuyenDoiDto.toBankAccountId) return true;
    if(!this.chiChuyenDoiDto.fromBankAccountId) return true;
    if(!this.chiChuyenDoiDto.outcomingEntryId) return true;
    if(!this.chiChuyenDoiDto.incomingEntryTypeId) return true;
    if(!this.chiChuyenDoiDto.inComingEntryName) return true;
    if(!this.toBankAccounts.some(s => s.value == this.chiChuyenDoiDto.toBankAccountId)) return true;
    if(!this.fromBankAccounts.some(s => s.value == this.chiChuyenDoiDto.fromBankAccountId)) return true;
    if(!this.outcommingEntryOptions.some(s => s.value == this.chiChuyenDoiDto.outcomingEntryId)) return true;
  }
  removeBTransactionMinus(id: number){
    if(this.bTransactionMinus.length == 1){
      this.dialogRef.close();
    }
    this.bTransactionMinus = this.bTransactionMinus.filter(s => s.bTransactionId != id);
    this.totalBTransactionMinus = this.bTransactionMinus.reduce((a, b) => {return a - b.moneyNumber;}, 0);
  }
  removeBTransactionPlus(id: number){
    if(this.bTransactionPlus.length == 1){
      this.dialogRef.close();
    }
    this.bTransactionPlus = this.bTransactionPlus.filter(s => s.bTransactionId != id);
    this.totalBTransactionPlus = this.bTransactionPlus.reduce((a, b) => {return a + b.moneyNumber;}, 0);
  }

}
export class ChiChuyenDoiDto extends ConversionTransactionDto{
  inComingEntryName: string;
}
