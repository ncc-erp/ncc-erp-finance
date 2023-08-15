import { Component, ElementRef, Inject, Injector, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { BankAccountTransDto, ValueAndNameModel } from '@app/service/model/common-DTO';
import { TranslateService } from '@ngx-translate/core';
import { AppComponentBase } from '@shared/app-component-base';
import { LinkExpenditureAndBTransDto } from '../list-btransaction/list-btransaction.component';
import { CommonService } from '../../../../service/api/new-versions/common.service';
import { AppConsts } from '@shared/AppConsts';
import { Utils } from '@app/service/helpers/utils';
import * as _ from 'lodash';
import { SelectionOutcomingEntry } from '../link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';
import { LinkExpenditureResultComponent } from './link-expenditure-result/link-expenditure-result.component';

@Component({
  selector: 'app-link-expenditure-dialog',
  templateUrl: './link-expenditure-dialog.component.html',
  styleUrls: ['./link-expenditure-dialog.component.css']
})
export class LinkExpenditureAndBTransDialogComponent extends AppComponentBase implements OnInit {
  public outcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public backDataOutcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public bankAccountTransaction: BankAccountTransDto[];
  public linkExpenditureAndBTrans: LinkExpenditureAndBTransDto = new LinkExpenditureAndBTransDto();
  public accountBankOptions: ValueAndNameModel[] = [];
  public backDataAccountBankOptions: ValueAndNameModel[] = [];
  public accountOptions: ValueAndNameModel[] = [];
  public accountTypeOptions: ValueAndNameModel[] = [];
  public accountId = AppConsts.VALUE_OPTIONS_ALL;
  public searchRequestChi = "";
  public accountTypeId = AppConsts.VALUE_OPTIONS_ALL;
  private defaultAccoutTypeName = undefined;
  public errorMess: string;
  public searchBankAccoutName: string;
  public isShowExchangeRate: boolean = false;
  public canLinkWithOutComingEnd: boolean = true;
  public checkboxEnd = false;
  public checkCurrencyLink: CheckCurrencyLinkOutcomToBTransaction;
  @ViewChildren('mat-select', { read: ElementRef }) listInput: QueryList<ElementRef> = new QueryList<ElementRef>();
  @ViewChild('linkOutcomingEntry') form: NgForm;
  @ViewChild('searchOutcoming') searchOutcoming: ElementRef;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<LinkExpenditureAndBTransDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LinkExpenditureAndBTransDialogData,
    private settingService: AppConfigurationService,
    private _common: CommonService,
    private _btransaction: BtransactionService,
    private translate: TranslateService,
    private dialog: MatDialog
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.accountTypeOptions.push(this.optionAll);
    this.accountOptions.push(this.optionAll);
    this.linkExpenditureAndBTrans.exchangeRate = AppConsts.EXCHANGE_RATE_DEFAULT;
    if(!this.isEnableMultiCurrency){
      this.checkCurrency();
      this.setOutcommingEntryOptionsOld();
      this.setBankAccountTransactionOld();
    }else{
      this.setOutcommingEntryOptions(this.data.currencyId);
      this.setBankAccountTransaction();
    }
    this.getConfigLinkOutComing();
  }

  setOutcommingEntryOptions(currencyId, onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved, currencyId).subscribe(response => {
      if (!response.success) return;
      this.outcommingEntryOptions = response.result;
      this.backDataOutcommingEntryOptions = response.result;
      if (this.linkExpenditureAndBTrans.outcomingEntryId && !this.backDataOutcommingEntryOptions.some(outcomingEntry => outcomingEntry.value == this.linkExpenditureAndBTrans.outcomingEntryId)) {
        this.linkExpenditureAndBTrans.outcomingEntryId = undefined;
      }
    })
  }
  setOutcommingEntryOptionsOld(onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved).subscribe(response => {
      if (!response.success) return;
      this.outcommingEntryOptions = response.result;
      this.backDataOutcommingEntryOptions = response.result;
      if (this.linkExpenditureAndBTrans.outcomingEntryId && !this.backDataOutcommingEntryOptions.some(outcomingEntry => outcomingEntry.value == this.linkExpenditureAndBTrans.outcomingEntryId)) {
        this.linkExpenditureAndBTrans.outcomingEntryId = undefined;
      }
    })
  }
  bankAccountSelectOpenedChange(isOpen: boolean) {
    if (this.accountBankOptions.length) return;
    this.searchBankAccoutName = "";
    this.accountBankOptions = _.cloneDeep(this.backDataAccountBankOptions)
  }
  setBankAccountTransaction() {
    this._common.getBankAccountOptions({currencyId: this.data.currencyId, isActive: true} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.bankAccountTransaction = response.result;
      this.setAllSelection(response.result);
    });
  }
  setBankAccountTransactionOld() {
    this._common.getBankAccountVNDOpstion().subscribe(response => {
      if (!response.success) return;
      this.bankAccountTransaction = response.result;
      this.setAllSelection(response.result);
    });
  }
  setAllSelection(data) {
    this.setAccountType(data);
    this.setAccount(data);
    this.setBankAccount(data);
    this.setdefaultAccountType();
  }
  setdefaultAccountType() {
    //
    this.translate.get('menu3.m3_child7').subscribe((res: string) => { this.defaultAccoutTypeName = res; });
    const defaultAccountType = this.bankAccountTransaction.find(s => s.accountTypeName == this.defaultAccoutTypeName);
    if (defaultAccountType && this.defaultAccoutTypeName) {
      this.accountTypeId = defaultAccountType.accountTypeId;
      this.setAccount(this.bankAccountTransaction, this.accountTypeId);
      this.setBankAccount(this.bankAccountTransaction, this.accountTypeId, this.accountId);
    }
    else {
      this.accountTypeId = AppConsts.VALUE_OPTIONS_ALL;
    }
    this.accountId = AppConsts.VALUE_OPTIONS_ALL;
  }
  checkCurrency() {
    this._btransaction.checkCurrencyLinkOutcomingEntryWithBTransaction(this.data.bTransactionId)
      .subscribe(response => {
        if (!response.success) return;
        //this.isShowExchangeRate = !response.result;
        this.checkCurrencyLink = response.result;
      }, () => { });
  }
  setAccountType(datas: BankAccountTransDto[]) {
    this.accountTypeOptions = [];
    this.accountTypeOptions.push(this.optionAll);
    datas.forEach(element => {
      const data = { value: element.accountTypeId, name: element.accountTypeName } as ValueAndNameModel;
      if (!this.accountTypeOptions.find(s => s.value == data.value))
        this.accountTypeOptions.push(data);
    });
  }
  setAccount(datas: BankAccountTransDto[], accountTypeId = AppConsts.VALUE_OPTIONS_ALL) {
    this.accountOptions = [];
    this.accountOptions.push(this.optionAll);
    datas.forEach(element => {
      if (Utils.isSelectedOptionsAll(accountTypeId) || element.accountTypeId == accountTypeId) {
        const data = { value: element.accountId, name: element.accountName } as ValueAndNameModel;
        if (!this.accountOptions.find(s => s.value == data.value))
          this.accountOptions.push(data);
      }
    });
  }
  setBankAccount(datas: BankAccountTransDto[], accountTypeId = AppConsts.VALUE_OPTIONS_ALL, accountId = AppConsts.VALUE_OPTIONS_ALL) {
    this.accountBankOptions = [];
    datas.forEach(element => {
      if ((Utils.isSelectedOptionsAll(accountTypeId) || element.accountTypeId == accountTypeId) &&
        (Utils.isSelectedOptionsAll(accountId) || element.accountId == accountId)) {
        const data = { value: element.bankAccountId, name: element.name } as ValueAndNameModel;
        if (!this.accountBankOptions.find(s => s.value == data.value))
          this.accountBankOptions.push(data);
      }
    });
    this.backDataAccountBankOptions = this.accountBankOptions;
  }
  onAccountTypeChange() {
    this.setAccount(this.bankAccountTransaction, this.accountTypeId);
    this.setBankAccount(this.bankAccountTransaction, this.accountTypeId);
    this.accountId = AppConsts.VALUE_OPTIONS_ALL;
    this.searchBankAccoutName = "";
    this.linkExpenditureAndBTrans.toBankAccountId = undefined;
  }
  onAccountChange() {
    this.setBankAccount(this.bankAccountTransaction, this.accountTypeId, this.accountId);
    this.searchBankAccoutName = "";
    this.linkExpenditureAndBTrans.toBankAccountId = undefined;
  }
  onBankAccountChange() {
    let bankAccountSelection = this.bankAccountTransaction.find(s => s.bankAccountId == this.linkExpenditureAndBTrans.toBankAccountId);
    this.accountTypeId = bankAccountSelection.accountTypeId;
    this.setAccount(this.bankAccountTransaction, bankAccountSelection.accountTypeId);
    this.accountId = bankAccountSelection.accountId;
  }
  searchRequestChiChange() {
    this.outcommingEntryOptions = this.backDataOutcommingEntryOptions.filter((data) =>
      data.name.trim().toLowerCase().includes(this.searchRequestChi.toLowerCase().trim())
    );
  }
  requestChiSelectOpenedChange(isOpen: boolean) {
    if(isOpen){
      this.searchOutcoming.nativeElement.focus();
      this.searchRequestChiChange();
    }
    else{
      this.outcommingEntryOptions = _.cloneDeep(this.backDataOutcommingEntryOptions);
    }
    if(this.outcommingEntryOptions.length){
      return;
    }
    this.searchRequestChi = "";
    this.outcommingEntryOptions = _.cloneDeep(this.backDataOutcommingEntryOptions);
  }
  searchBankAccoutNameChange() {
    this.accountBankOptions = this.backDataAccountBankOptions.filter(s => s.name.trim().toLowerCase().includes(this.searchBankAccoutName.trim().toLowerCase()));
  }
  validateBankAccout() {
    let bankAccountSelection = this.bankAccountTransaction.find(s => s.accountId == this.accountId &&
      s.accountTypeId == this.accountTypeId && s.bankAccountId == this.linkExpenditureAndBTrans.toBankAccountId)
    if (bankAccountSelection) {
      return true;
    }
    bankAccountSelection = this.bankAccountTransaction.find(s => s.accountId == this.accountId &&
      s.accountTypeId == this.accountTypeId);
    if (!bankAccountSelection) {
      this.accountId = undefined;
    }
    this.linkExpenditureAndBTrans.toBankAccountId = undefined;
    return false
  }
  process() {
    if (this.form.invalid && !this.validateBankAccout()) {
      return;
    }
    this.linkExpenditureAndBTrans.BTransactionId = this.data.bTransactionId;
    let bankAccount = this.bankAccountTransaction?.find(s => s.bankAccountId == this.linkExpenditureAndBTrans.toBankAccountId)?.bankAccountName;
    let message = `Link biến động số dư
    <strong> #${this.linkExpenditureAndBTrans.BTransactionId}
    (
      <span style="color:${this.data.moneyColor};">${this.data.money}</span>
      <span style="color:${this.data.currencyColor}";> ${this.data.currencyName}</span>
    )</strong>
    <br/>tới request chi: <strong>${this.outcommingEntryOptions.find(s => s.value == this.linkExpenditureAndBTrans.outcomingEntryId).name}</strong>
    <br/>bank nhận: <strong>${bankAccount}</strong>
    `

    let ref = this.dialog.open(LinkExpenditureResultComponent, {
      width: "700px",
      data: {
        message : message,
        item: this.linkExpenditureAndBTrans
      }
    })
    ref.afterClosed().subscribe(rs =>{
      if(rs){
        this.dialogRef.close();
      }
    })

  }

  checkboxEndChange() {
    this.setOutcommingEntryOptions(!this.checkboxEnd);
  }
  getConfigLinkOutComing(){
    this.settingService.getCanLinkWithOutComingEnd().subscribe(response => {
      if(!response.success) return;
        this.canLinkWithOutComingEnd = response.result;
    })
  }
}
export interface LinkExpenditureAndBTransDialogData {
  bTransactionId: number;
  currencyName: string;
  currencyColor: string;
  money: string;
  moneyColor: string;
  currencyId: number;
}
export interface CheckCurrencyLinkOutcomToBTransaction{
  isDifferent: boolean;
  currencyCode: string;
}
