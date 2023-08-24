import { Component, ElementRef, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { Utils } from '@app/service/helpers/utils';
import { BTransaction } from '@app/service/model/b-transaction.model';
import { BankAccountTransDto, ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { SelectionOutcomingEntry } from '../link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import * as _ from 'lodash';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';

@Component({
  selector: 'app-link-multi-btransaction-outcoming-entry-dialog',
  templateUrl: './link-multi-btransaction-outcoming-entry-dialog.component.html',
  styleUrls: ['./link-multi-btransaction-outcoming-entry-dialog.component.css']
})
export class LinkMultiBtransactionOutcomingEntryDialogComponent extends AppComponentBase implements OnInit {

  public outcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public backDataOutcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public bankAccountTransaction: BankAccountTransDto[];
  public linkOutcomingSalaryWithBTransactions: LinkOutcomingSalaryWithBTransactionsDto = new LinkOutcomingSalaryWithBTransactionsDto();
  public accountBankOptions: ValueAndNameModel[] = [];
  public totalMoney: number;
  public backDataAccountBankOptions: ValueAndNameModel[] = [];
  public accountOptions: ValueAndNameModel[] = [];
  public accountTypeOptions: ValueAndNameModel[] = [];
  public accountId = AppConsts.VALUE_OPTIONS_ALL;
  public searchRequestChi = "";
  public defaultToBankAccount: number;
  public accountTypeId = AppConsts.VALUE_OPTIONS_ALL;
  public linkDone = false;
  public errorMess: string;
  public searchBankAccoutName: string;
  public isDefaultToBankAccount = false;

  public isShowExchangeRate: boolean
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<LinkMultiBtransactionOutcomingEntryDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: LinkMultiBtransactionOutcomingEntryDialogData,
    private _common: CommonService,
    public _utilities: UtilitiesService,
    private _btransaction: BtransactionService,
    private _config: AppConfigurationService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.linkOutcomingSalaryWithBTransactions.bTransactionIds =  this.data.bTransactions.map(s => s.bTransactionId);
    this.setDefaultOutcomingEntry();
    if(!this.isEnableMultiCurrency){
      this.setOutcommingEntryOptionsOld();
      this.setBankAccountTransactionOld();
    }else{
      this.setOutcommingEntryOptions();
      this.setBankAccountTransaction();
    }
    this.accountTypeOptions.push(this.optionAll);
    this.accountOptions.push(this.optionAll);
    this.totalMoney = this.data.bTransactions.reduce((a, b) => {return a - b.moneyNumber;}, 0);
  }
  setDefaultBankAccount(){
    this._btransaction.getDefaultToBankAccount().subscribe(response => {
      if (!response.success) return;
      let toBankAccountId = response.result.bankAccountId;
      if(!this.bankAccountTransaction.find(s => s.bankAccountId == response.result.bankAccountId)){
        this.defaultToBankAccount = 0;
        this.isDefaultToBankAccount = false;
        return;
      }
      if(toBankAccountId != 0)
      {
        this.linkOutcomingSalaryWithBTransactions.toBankAccountId = toBankAccountId;
        this.accountTypeId = response.result.accountTypeId;
        this.accountId = response.result.accountId;
      }
      this.defaultToBankAccount = toBankAccountId;
      this.isDefaultToBankAccount = this.defaultToBankAccount && this.defaultToBankAccount == this.linkOutcomingSalaryWithBTransactions.toBankAccountId;
    })
  }
  setToBankAccountDefault(){
    this._config.setOutcomingSalary({bankAccountId : event ? this.linkOutcomingSalaryWithBTransactions.toBankAccountId.toString() : undefined } as OutcomingSalaryDto ).subscribe(response => {
      if (!response.success) return;
      this.setDefaultBankAccount();
    })
  }
  setDefaultOutcomingEntry(){
    this._btransaction.getCurrentOutcomingSalary().subscribe(response => {
      if (!response.success) return;
      this.linkOutcomingSalaryWithBTransactions.outcomingEntryId = response.result;
    })
  }
  setOutcommingEntryOptions(onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved, this.data.bTransactions[0].currencyId).subscribe(response => {
      if (!response.success) return;
      this.backDataOutcommingEntryOptions = this.outcommingEntryOptions = response.result;
      if (this.linkOutcomingSalaryWithBTransactions.outcomingEntryId && !this.backDataOutcommingEntryOptions.some(outcomingEntry => outcomingEntry.value == this.linkOutcomingSalaryWithBTransactions.outcomingEntryId)) {
        this.linkOutcomingSalaryWithBTransactions.outcomingEntryId = undefined;
      }
    })
  }
  setOutcommingEntryOptionsOld(onlyApproved = true) {
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved).subscribe(response => {
      if (!response.success) return;
      this.backDataOutcommingEntryOptions = this.outcommingEntryOptions = response.result;
      if (this.linkOutcomingSalaryWithBTransactions.outcomingEntryId && !this.backDataOutcommingEntryOptions.some(outcomingEntry => outcomingEntry.value == this.linkOutcomingSalaryWithBTransactions.outcomingEntryId)) {
        this.linkOutcomingSalaryWithBTransactions.outcomingEntryId = undefined;
      }
    })
  }
  getTypeName(){
    if(!this.linkOutcomingSalaryWithBTransactions.outcomingEntryId) return;
    return this.outcommingEntryOptions.find(s => s.value == this.linkOutcomingSalaryWithBTransactions.outcomingEntryId)?.typeName;
  }
  setBankAccountTransaction() {
    this._common.getBankAccountOptions({currencyId: this.data.bTransactions[0].currencyId, isActive: true} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.bankAccountTransaction = response.result;
      this.setDefaultBankAccount();
      this.setAllSelection(response.result);
    });
  }
  setBankAccountTransactionOld() {
    this._common.getBankAccountVNDOpstion().subscribe(response => {
      if (!response.success) return;
      this.bankAccountTransaction = response.result;
      this.setDefaultBankAccount();
      this.setAllSelection(response.result);
    });
  }
  setAllSelection(data) {
    this.setAccountType(data);
    this.setAccount(data);
    this.setBankAccount(data);
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
  searchRequestChiChange() {
    this.outcommingEntryOptions = this.backDataOutcommingEntryOptions.filter((data) =>
      data.name.trim().toLowerCase().includes(this.searchRequestChi.trim().toLowerCase().trim())
    );
  }
  onAccountTypeChange() {
    this.setAccount(this.bankAccountTransaction, this.accountTypeId);
    this.setBankAccount(this.bankAccountTransaction, this.accountTypeId);
    this.accountId = AppConsts.VALUE_OPTIONS_ALL;
    this.searchBankAccoutName = "";
    this.linkOutcomingSalaryWithBTransactions.toBankAccountId = undefined;
  }
  onAccountChange() {
    this.setBankAccount(this.bankAccountTransaction, this.accountTypeId, this.accountId);
    this.searchBankAccoutName = "";
    this.linkOutcomingSalaryWithBTransactions.toBankAccountId = undefined;
  }
  onBankAccountChange() {
    let bankAccountSelection = this.bankAccountTransaction.find(s => s.bankAccountId == this.linkOutcomingSalaryWithBTransactions.toBankAccountId);
    this.accountTypeId = bankAccountSelection.accountTypeId;
    this.setAccount(this.bankAccountTransaction, bankAccountSelection.accountTypeId);
    this.accountId = bankAccountSelection.accountId;
    this.isDefaultToBankAccount = this.defaultToBankAccount && this.defaultToBankAccount == this.linkOutcomingSalaryWithBTransactions.toBankAccountId
  }

  bankAccountSelectOpenedChange(isOpen: boolean) {
    if (this.accountBankOptions.length) return;
    this.searchBankAccoutName = "";
    this.accountBankOptions = _.cloneDeep(this.backDataAccountBankOptions)
  }
  searchBankAccoutNameChange() {
    this.accountBankOptions = this.backDataAccountBankOptions.filter(s => s.name.trim().toLowerCase().includes(this.searchBankAccoutName.trim().toLowerCase()));
  }
  removeBTransaction(btransactionId : number){
    this.data.bTransactions = this.data.bTransactions.filter(s => s.bTransactionId != btransactionId);
    this.linkOutcomingSalaryWithBTransactions.bTransactionIds = this.data.bTransactions.map(s => s.bTransactionId);
    if(this.data.bTransactions.length == 0){
      this.dialogRef.close();
    }
  }
  defaultToBankAccountChange() {
    this.isDefaultToBankAccount = !this.isDefaultToBankAccount;
    if (this.isDefaultToBankAccount) {
      this._config.setOutcomingSalary({bankAccountId : this.linkOutcomingSalaryWithBTransactions.toBankAccountId.toString() } as OutcomingSalaryDto ).subscribe(response => {
        if (!response.success) return;
        abp.notify.success("Update default to bank account successfully!");
        this.setDefaultBankAccount();
      })
    } else {
      this._config.clearDefaultToBankAccount().subscribe((response) => {
        if (!response.success) return;
        abp.notify.success("Update default to bank account successfully!");
        this.setDefaultBankAccount();
      })
    }

  }
  process(){
    this._btransaction.checkLinkOutcomingEntrySalaryWithBTransactions(this.linkOutcomingSalaryWithBTransactions).subscribe(response => {
      if (!response.success) return;
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
  link(){
    this._btransaction.linkOutcomingEntrySalaryWithBTransactions(this.linkOutcomingSalaryWithBTransactions)
    .subscribe(response => {
      if (!response.success) return;
      this.linkDone = true;
    })
  }
  openOutcomingEntry(){
    this.dialogRef.close({linkDone: this.linkDone, outcomingEntryId: this.linkOutcomingSalaryWithBTransactions.outcomingEntryId});
  }
}
export interface LinkMultiBtransactionOutcomingEntryDialogData {
  bTransactions: BTransaction[];
}
export class LinkOutcomingSalaryWithBTransactionsDto{
  bTransactionIds: number[];
  toBankAccountId: number;
  outcomingEntryId: number;
}
export class OutcomingSalaryDto{
  bankAccountId: string;
}
