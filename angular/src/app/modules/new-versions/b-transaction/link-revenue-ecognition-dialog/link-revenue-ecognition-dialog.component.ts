import { ValueAndNameModel } from './../../../../service/model/common-DTO';
import { Component, ElementRef, Inject, Injector, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { InComingAndBTransactionDto } from '@app/service/model/b-transaction.model';
import { AppComponentBase } from '@shared/app-component-base';
import { CommonService } from '../../../../service/api/new-versions/common.service';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';
import { AccountTypeEnum } from '@shared/AppEnums';
import { CurrencyService } from '@app/service/api/currency.service';
import { CurrencyDto } from '@app/modules/currency/currency.component';

@Component({
  selector: 'app-link-revenue-ecognition-dialog',
  templateUrl: './link-revenue-ecognition-dialog.component.html',
  styleUrls: ['./link-revenue-ecognition-dialog.component.css']
})
export class LinkRevenueRecognitionAndBTransDialogComponent extends AppComponentBase implements OnInit {
  public incommingEntry: InComingAndBTransactionDto = new InComingAndBTransactionDto();
  public listOptionItem: OptionItem[] = [];
  public listOptionItemFindData: OptionItem[] = [];
  public incomingEntryTypeTreeOptions: IncomingEntryTypeOptions;
  public nameInComingEntry: string;
  public searchIncomingType: string;
  public searchBankAccount: string;
  public bankAccountOptions: ValueAndNameModel[] = [];
  public bankAccountFilters: ValueAndNameModel[] = [];
  public defaultBankAccountId: number;
  public processing: boolean;

  @ViewChildren('mat-select', { read: ElementRef }) listInput: QueryList<ElementRef> = new QueryList<ElementRef>()
  @ViewChild('linkRevenueRecognition') form: NgForm
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<LinkRevenueRecognitionAndBTransDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RevenueRecognitionDialogData,
    private _common: CommonService,
    private _btransaction: BtransactionService,
    private _currency: CurrencyService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setIncomingEntryTypes();
    if(this.isEnableMultiCurrency){
      this.setBankAccountOptionsNew();
    }else{
      this.setBankAccountOptions();
    }
  }
  setdefaultBankAccountId(){
    this._common.getDefaultBankAccountByCurrencyId(this.data.currencyId).subscribe(response => {
      if (!response.success) return;
      if(this.bankAccountOptions.find(s => s.value == response.result)){
        this.incommingEntry.fromBankAccountId = this.defaultBankAccountId = response.result;
      }else{
        this.defaultBankAccountId = 0;
      }

    })
  }
  setIncomingEntryTypes() {
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      let incomingEntryTypeOptions : IncomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: []};
      incomingEntryTypeOptions.children = response.result;
      this.incomingEntryTypeTreeOptions = incomingEntryTypeOptions;
      this.listOptionItemFindData = this.listOptionItem = this.convertTreeValueToTreeOption(incomingEntryTypeOptions, 0);
    })
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

  convertTreeValueToTreeOption(node: IncomingEntryTypeOptions, level: number){
    let optionItem: OptionItem = {id : node.item.id, name : node.item.name, hasChildren : false, class : this.getClassByLevel(level)};
    let treeOption: OptionItem[] = [];
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
  getClassByLevel(level: number): string{
    switch (level){
      case 1:
        return "level1";
      case 2:
        return "level2";
      case 3:
        return "level3";
      case 4:
        return "level4";
      default:
        return "";
    }
  }
  setBankAccountOptions(): void {
    this._common.getBankAccountByCurrency(this.data.currencyId).subscribe(response => {
      if (!response.success) return;
      this.bankAccountOptions = this.bankAccountFilters = response.result;
      this.setdefaultBankAccountId();
    })
  }
  setBankAccountOptionsNew(){
    this._common.getBankAccountOptions({currencyId: this.data.currencyId, isActive: true, orderByType: AccountTypeEnum.MEDIUM} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.bankAccountOptions = this.bankAccountFilters = response.result;
      this.setdefaultBankAccountId();
    })
  }
  filterBankAccount(): void {
    this.bankAccountFilters = this.bankAccountOptions.filter(item => item.name.trim().toLowerCase().includes(this.searchBankAccount.trim().toLowerCase()))
  }

  filterRevenue() {
    let incomingEntryTypeOptions = this.convertTreeValueToListOption(this.incomingEntryTypeTreeOptions, this.searchIncomingType);
    if(incomingEntryTypeOptions){
      this.listOptionItem = this.convertTreeValueToTreeOption(incomingEntryTypeOptions, 0);
    }
    else{
      this.listOptionItem = [];
    }
  }
  process() {
    if (this.form.invalid) {
      return;
    }
    let incomingEntryTypeName = this.listOptionItemFindData.find(s => s.id == this.incommingEntry.incomingEntryTypeId).name;
    this.incommingEntry.BTransactionId = this.data.bTransactionId;
    abp.message.confirm(
      `Tạo ghi nhận thu có <p>Name: <strong>${this.incommingEntry.name}</strong></p> <p>Loại ghi nhận thu: <strong> ${incomingEntryTypeName}</strong></p>
      Từ BĐSD <strong>#${this.data.bTransactionId} (
        <span style="color:${this.data.moneyColor};">${this.data.money}</span>
        <span style="color:${this.data.currencyColor}";> ${this.data.currencyName}</span>
      )</strong><br/>`,
      "",
      (result: boolean) => {
        if (result) {
          this.doSave();
        }
      },
      true
    );
  }
  doSave() {
    this._btransaction.createIncomingEntry(this.incommingEntry)
      .subscribe(response => {
        if (!response.success) return;
        this.createSuccess(response.result);
      }, () => { });
  }
  createSuccess(createResult: CreateResult) {
    abp.message.confirm(
      `Bạn đã tạo thành công ghi nhận thu với id: ${createResult.incomingEntryId}
      <br/>Thuộc giao dịch ngân hàng: ${createResult.bankTransactionId}
      <br/> Bạn có muốn xem chi tiết?`,
      "Done",
      (result: boolean) => {
        if (result) {
          this.dialogRef.close(createResult.bankTransactionId);
          return;
        }
        this.dialogRef.close();
      },
      { isHTML: true}
    );
  }
  bankAccountSelectOpenedChange(opened: boolean){
    if(this.bankAccountFilters.length === 0){
      this.searchBankAccount = "";
      this.filterBankAccount();
    }
  }
  incomingEntryTypeOpenedChange(opened: boolean){
    if(this.listOptionItem.length === 0){
      this.searchIncomingType = "";
      this.filterRevenue();
    }
  }
  setDefaultBankAccountIdChange(checked : boolean){
    if (!this.isDefaultBankAccount()) {
      this.setDefaultToBankAccount(this.incommingEntry.fromBankAccountId);
    } else {
      this.setDefaultToBankAccount();
    }
  }
  setDefaultToBankAccount(id? :number){
    this.processing = true;
    this._currency.defaultBankAccount({id : this.data.currencyId, defaultBankAccountId: id} as CurrencyDto)
    .subscribe(response => {
      if (!response.success) return;
      abp.notify.success("Set Default To BankAccount Success");
      if(id){
        this.getDefaultToBankAccountByCurrencyId();
      }
      this.processing = false;
    })
  }
  getDefaultToBankAccountByCurrencyId(){
    this._common.getDefaultToBankAccountByCurrencyId(this.data.currencyId).subscribe(response => {
      if (!response.success) return;
      this.incommingEntry.fromBankAccountId = this.defaultBankAccountId = response.result;
    })
  }
  isDefaultBankAccount(){
    return  this.defaultBankAccountId && this.defaultBankAccountId == this.incommingEntry.fromBankAccountId;
  }

}

export interface RevenueRecognitionDialogData {
  bTransactionId: number;
  currencyId: number;
  currencyName: string;
  currencyColor: string;
  money: string;
  moneyColor: string;
  moneyNumber: number;
}
export class CreateResult{
  bankTransactionId: number;
  incomingEntryId: number;
}
export class IncomingEntryTypeItem{
  id: number;
  name: string;
  parentId: number;

}
export class IncomingEntryTypeOptions{
  item: IncomingEntryTypeItem;
  children: IncomingEntryTypeOptions[];
}
export class OptionItem{
  id: number;
  name: string;
  class: string;
  hasChildren: boolean;
}
