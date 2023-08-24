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
import { FilterBankAccount } from '@app/service/model/bank-account.dto';

@Component({
  selector: 'app-buy-foreign-currency',
  templateUrl: './buy-foreign-currency.component.html',
  styleUrls: ['./buy-foreign-currency.component.css']
})
export class BuyForeignCurrencyComponent extends AppComponentBase implements OnInit {


  public outcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public backDataOutcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public toBankAccountTransaction: ValueAndNameModel[];
  public fromBankAccountTransaction: ValueAndNameModel[];
  public bTransactionMinus: BTransaction[];
  public bTransactionPlus: BTransaction[];
  public tempFromBankAccountTransaction: ValueAndNameModel[];
  public tempToBankAccountTransaction: ValueAndNameModel[];
  public conversionTransactionDto : ConversionTransactionDto = new ConversionTransactionDto();
  public searchRequestChi = "";
  public searchIncomingEntryType = "";
  public incomingEntryTypeTreeOptions: IncomingEntryTypeOptions;
  public incomingEntryTypes: OptionIncomingEntryTypeDto[] = [];
  public searchToBankAccoutName = "";
  public searchFromBankAccoutName = "";
  public linkDone = false;
  public isShowExchangeRate: boolean;
  public totalBTransactionMinus: number;
  public totalBTransactionPlus: number;
  public defaultIncomingEntryTypes: number;
  public isDefaultIncomingEntryTypes: boolean;
  public processing: boolean;
  public isDefaultFromBankAccountId: boolean;
  public isDefaultToBankAccountId: boolean;
  public defaultFromBankAccountId: number;
  public defaultToBankAccountId: number;

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
    this.bTransactionMinus = this.data.bTransactions.filter(s => s.moneyNumber < 0).sort((a, b) => a.moneyNumber - b.moneyNumber);
    this.totalBTransactionMinus = this.data.bTransactions.filter(s => s.moneyNumber < 0).reduce((a, b) => {return a - b.moneyNumber;}, 0);
    this.totalBTransactionPlus = this.data.bTransactions.filter(s => s.moneyNumber > 0).reduce((a, b) => {return a + b.moneyNumber;}, 0);
    this.bTransactionPlus = this.data.bTransactions.filter(s => s.moneyNumber > 0).sort((a, b) => b.moneyNumber - a.moneyNumber);
    if(this.isEnableMultiCurrency){
      this.getFromBankAcocunt();
      this.getToBankAcocunt();
    }else{
      this.setBankAccountTransaction();
    }
    this.setDefaultBankAccount();
    this.setDefaultIncomingEntryTypes();
    this.setIncomingEntryTypes();
    this.setOutcommingEntryOptions();

  }
  incomingEntryTypeIdChange(){
    this.isDefaultIncomingEntryTypes = this.defaultIncomingEntryTypes && this.defaultIncomingEntryTypes == this.conversionTransactionDto.incomingEntryTypeId;
  }
  setDefaultIncomingEntryTypes(){
    this._configuration.getDefaultMaLoaiMuaNgoaiTe().subscribe(response =>{
      if(!response.success) return;
      if(response.result){
        this.conversionTransactionDto.incomingEntryTypeId = Number(response.result);
      }
      this.defaultIncomingEntryTypes = response.result;
      this.isDefaultIncomingEntryTypes = response.result && response.result == this.conversionTransactionDto.incomingEntryTypeId;
    })
  }
  toBankAccountSelectChange(){
    this.isDefaultToBankAccountId = this.defaultToBankAccountId && this.defaultToBankAccountId == this.conversionTransactionDto.toBankAccountId;
  }
  fromBankAccountSelectChange(){
    this.isDefaultFromBankAccountId = this.defaultFromBankAccountId && this.defaultFromBankAccountId == this.conversionTransactionDto.fromBankAccountId;
  }
  setDefaultBankAccount(){
    this.getDefaultFromBankAccountByCurrencyIdWhenBuy();
    this.getDefaultToBankAccountByCurrencyIdWhenBuy();
  }
  getDefaultFromBankAccountByCurrencyIdWhenBuy(){
    this._common.getDefaultFromBankAccountByCurrencyIdWhenBuy(this.bTransactionPlus[0].currencyId).subscribe(response => {
      if (!response.success) return;
      this.conversionTransactionDto.fromBankAccountId = this.defaultFromBankAccountId = response.result;
      this.isDefaultFromBankAccountId = this.defaultFromBankAccountId && this.defaultFromBankAccountId == this.conversionTransactionDto.fromBankAccountId;
    })
  }
  getDefaultToBankAccountByCurrencyIdWhenBuy(){
    this._common.getDefaultToBankAccountByCurrencyIdWhenBuy(this.bTransactionPlus[0].currencyId).subscribe(response => {
      if (!response.success) return;
      this.conversionTransactionDto.toBankAccountId = this.defaultToBankAccountId = response.result;
      this.isDefaultToBankAccountId = this.defaultToBankAccountId && this.defaultToBankAccountId == this.conversionTransactionDto.toBankAccountId;
    })
  }
  setOutcommingEntryOptions(onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved, this.bTransactionMinus[0].currencyId).subscribe(response => {
      if (!response.success) return;
      this.backDataOutcommingEntryOptions = this.outcommingEntryOptions = response.result;
      if (this.conversionTransactionDto.outcomingEntryId && !this.backDataOutcommingEntryOptions.some(outcomingEntry => outcomingEntry.value == this.conversionTransactionDto.outcomingEntryId)) {
        this.conversionTransactionDto.outcomingEntryId = undefined;
      }
    })
  }
  setDefaultToBankAccount(id? :number){
    this.processing = true;
    this._currency.defaultToBankAccountIdWhenBuy({id : this.bTransactionPlus[0].currencyId, defaultToBankAccountIdWhenBuy: id} as CurrencyDto)
    .subscribe(response => {
      if (!response.success) return;
      abp.notify.success("Set Default To BankAccount When Buy Success");
      if(id){
        this.getDefaultToBankAccountByCurrencyIdWhenBuy();
      }
      this.processing = false;
    })
  }
  setDefaultFromBankAccount(id? :number){
    this.processing = true;
    this._currency.defaultFromBankAccountIdWhenBuy({id : this.bTransactionPlus[0].currencyId, defaultFromBankAccountIdWhenBuy: id} as CurrencyDto)
    .subscribe(response => {
      if (!response.success) return;
      abp.notify.success("Set Default From BankAccount When Buy Success");
      if(id)
      {
        this.getDefaultFromBankAccountByCurrencyIdWhenBuy();
      }
      this.processing = false;
    })
  }

  close(){
    this.dialogRef.close({outcomingEntryId: this.conversionTransactionDto.outcomingEntryId, linkDone: this.linkDone});
  }
  setIncomingEntryTypes() {
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      let incomingEntryTypeOptions : IncomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: []};
      incomingEntryTypeOptions.children = response.result;
      this.incomingEntryTypeTreeOptions = incomingEntryTypeOptions;
      this.incomingEntryTypes = this.convertTreeValueToTreeOption(incomingEntryTypeOptions, -1);
    })
  }
  convertTreeValueToTreeOption(node: IncomingEntryTypeOptions, level: number){
    let optionItem: OptionIncomingEntryTypeDto = {id : node.item.id, name : node.item.name, hasChildren : false, space : level};
    let treeOption: OptionIncomingEntryTypeDto[] = [];
    if(node.children.length > 0)
    {
      optionItem.hasChildren = true;
      treeOption.push(optionItem);
      node.children.forEach(childNode => {
        treeOption.push(...this.convertTreeValueToTreeOption(childNode, level + 1));
      })
    }
    else{
      treeOption.push(optionItem);
    }
    return treeOption;
  }
  getTypeName(){
    if(!this.conversionTransactionDto.outcomingEntryId) return;
    return this.outcommingEntryOptions.find(s => s.value == this.conversionTransactionDto.outcomingEntryId)?.typeName;
  }
  setBankAccountTransaction() {
    this._common.getAllBankAccount().subscribe(response => {
      if (!response.success) return;
      this.fromBankAccountTransaction = this.toBankAccountTransaction = this.tempFromBankAccountTransaction= this.tempToBankAccountTransaction = response.result;
    });
  }
  getToBankAcocunt(){
    this._common.getBankAccountOptions({currencyId: this.data.bTransactions.find(s => s.moneyNumber < 0).currencyId, isActive: true}as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.toBankAccountTransaction = this.tempToBankAccountTransaction = response.result;
    });
  }
  getFromBankAcocunt(){
    this._common.getBankAccountOptions({currencyId: this.data.bTransactions.find(s => s.moneyNumber > 0).currencyId, isActive: true}as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.fromBankAccountTransaction = this.tempFromBankAccountTransaction = response.result;
    });
  }
  searchRequestChiChange() {
    this.outcommingEntryOptions = this.backDataOutcommingEntryOptions.filter((data) =>
      data.name.trim().toLowerCase().includes(this.searchRequestChi.toLowerCase().trim())
    );
  }
  outcomingEntrySelectOpenedChange(isOpen: boolean) {
    if(!isOpen){
      this.outcommingEntryOptions = _.cloneDeep(this.backDataOutcommingEntryOptions)
    }else{
      this.searchRequestChiChange();
    }

  }
  toBankAccountSelectOpenedChange(isOpen: boolean) {
    if(!isOpen){
      this.toBankAccountTransaction = _.cloneDeep(this.tempToBankAccountTransaction)
    }else{
      this.searchToBankAccoutNameChange();
    }

  }
  fromBankAccountSelectOpenedChange(isOpen: boolean) {
    if(!isOpen){
      this.fromBankAccountTransaction = _.cloneDeep(this.tempFromBankAccountTransaction)
    }else{
      this.searchFromBankAccoutNameChange();
    }

  }
  searchToBankAccoutNameChange() {
    this.toBankAccountTransaction = this.tempToBankAccountTransaction.filter(s => s.name.trim().toLowerCase().includes(this.searchToBankAccoutName.trim().toLowerCase()));
  }
  searchFromBankAccoutNameChange(){
    this.fromBankAccountTransaction = this.tempFromBankAccountTransaction.filter(s => s.name.trim().toLowerCase().includes(this.searchFromBankAccoutName.trim().toLowerCase()));
  }
  dropMinus(event: CdkDragDrop<BTransaction[]>) {
    moveItemInArray(this.bTransactionMinus, event.previousIndex, event.currentIndex);
  }
  dropPositives(event: CdkDragDrop<BTransaction[]>) {
    moveItemInArray(this.bTransactionPlus, event.previousIndex, event.currentIndex);
  }

  process(){
    this.conversionTransactionDto.minusBTransactionIds = this.bTransactionMinus.map(s => s.bTransactionId);
    this.conversionTransactionDto.plusBTransactionIds = this.bTransactionPlus.map(s => s.bTransactionId);
    this._btransaction.checkMuaNgoaiTe(this.conversionTransactionDto).subscribe(response => {
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
      this._configuration.setDefaultMaLoaiMuaNgoaiTe({ id : this.conversionTransactionDto.incomingEntryTypeId.toString()} as DefaultIncomingEntryType).subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Update default incoming entry successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    } else {
      this._configuration.clearDefaultMaLoaiMuaNgoaiTe().subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Clear default incoming entry  successfully!");
        this.setDefaultIncomingEntryTypes();
      })
    }
  }
  setDefaultToBankAccountIdChange(checked : boolean){
    this.isDefaultToBankAccountId = !this.isDefaultToBankAccountId;
    if (this.isDefaultToBankAccountId) {
      this.setDefaultToBankAccount(this.conversionTransactionDto.toBankAccountId);
    } else {
      this.setDefaultToBankAccount();
    }
  }
  setDefaultFromBankAccountIdChange(checked : boolean){
    this.isDefaultFromBankAccountId = !this.isDefaultFromBankAccountId;
    if (this.isDefaultFromBankAccountId) {
      this.setDefaultFromBankAccount(this.conversionTransactionDto.fromBankAccountId);
    } else {
      this.setDefaultFromBankAccount();
    }
  }
  link(){
    this._btransaction.muaNgoaiTe(this.conversionTransactionDto)
    .subscribe(response => {
      if (!response.success) return;
      this.linkDone = true;
    })
  }
  openOutcomingEntry(){
    this.dialogRef.close(this.conversionTransactionDto.outcomingEntryId);
  }
  isDisable(){
    if(this.processing) return true;
    if(!this.conversionTransactionDto.toBankAccountId) return true;
    if(!this.conversionTransactionDto.fromBankAccountId) return true;
    if(!this.conversionTransactionDto.outcomingEntryId) return true;
    if(!this.conversionTransactionDto.incomingEntryTypeId) return true;
    if(!this.toBankAccountTransaction.some(s => s.value == this.conversionTransactionDto.toBankAccountId)) return true;
    if(!this.fromBankAccountTransaction.some(s => s.value == this.conversionTransactionDto.fromBankAccountId)) return true;
    if(!this.outcommingEntryOptions.some(s => s.value == this.conversionTransactionDto.outcomingEntryId)) return true;
  }
  incomingEntryTypeOpenedChange(isOpen: boolean) {
    if(!isOpen){
      this.incomingEntryTypes = this.convertTreeValueToTreeOption(this.incomingEntryTypeTreeOptions, -1);
    }else{
      this.searchIncomingEntryTypeChange();
    }

  }
  searchIncomingEntryTypeChange(){
    let incomingEntryTypeOptions = this.convertTreeValueToListOption(this.incomingEntryTypeTreeOptions, this.searchIncomingEntryType);
    if(incomingEntryTypeOptions){
      this.incomingEntryTypes = this.convertTreeValueToTreeOption(incomingEntryTypeOptions, -1);
    }
    else{
      this.incomingEntryTypes = [];
    }
  }
  convertTreeValueToListOption(node: IncomingEntryTypeOptions, searchInTree: string) : IncomingEntryTypeOptions{
    let nodeClone : IncomingEntryTypeOptions = JSON.parse(JSON.stringify(node));
    if(nodeClone.item.name.trim().toLowerCase().indexOf(searchInTree.trim().toLowerCase()) != -1){
      return nodeClone;
    }

    if(!nodeClone.children){
      return null;
    }

    let returnChild: IncomingEntryTypeOptions[] = [];

    node.children.forEach(childNode => {
      let p = this.convertTreeValueToListOption(childNode, searchInTree);
      if(p){
        returnChild.push(p);
      }
    });

    if(returnChild.length > 0){
      nodeClone.children = returnChild;
      return nodeClone;
    }

    return null;
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
