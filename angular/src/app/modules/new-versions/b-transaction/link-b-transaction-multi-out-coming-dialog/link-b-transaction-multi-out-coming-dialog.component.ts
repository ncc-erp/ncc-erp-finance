import { Component, ElementRef, Inject, Injector, OnInit, ViewChild } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatSelect } from "@angular/material/select";
import { BtransactionService } from "@app/service/api/new-versions/btransaction.service";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { Utils } from "@app/service/helpers/utils";
import * as _ from 'lodash';
import {
  BankAccountTransDto,
  ValueAndNameModel,
} from "@app/service/model/common-DTO";
import { TranslateService } from "@ngx-translate/core";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/AppConsts";
import { FilterBankAccount } from "@app/service/model/bank-account.dto";

@Component({
  selector: "app-link-b-transaction-multi-out-coming-dialog",
  templateUrl: "./link-b-transaction-multi-out-coming-dialog.component.html",
  styleUrls: ["./link-b-transaction-multi-out-coming-dialog.component.css"],
})
export class LinkBTransactionMultiOutComingDialogComponent
  extends AppComponentBase
  implements OnInit {
  @ViewChild("matSelect") matSelect: MatSelect;
  public outcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public backDataOutcommingEntryOptions: SelectionOutcomingEntry[] = [];
  public listOutcommingEntrySelected: SelectionOutcomingEntry[] = [];
  public searchOutComming: string;
  public linkMultipleOutcomingEntryWithBTransactionDto: LinkMultipleOutcomingEntryWithBTransactionDto =
    new LinkMultipleOutcomingEntryWithBTransactionDto();
  public errorMess: string;
  @ViewChild('inputSearchRequestChi') inputSearchRequestChi: ElementRef;
  @ViewChild('inputSearchBankAccount') inputSearchBankAccount: ElementRef;

  public outcomingEntryIds: number[];
  public bankAccountTransaction: BankAccountTransDto[];
  public accountBankOptions: ValueAndNameModel[] = [];
  public backDataAccountBankOptions: ValueAndNameModel[] = [];
  public accountOptions: ValueAndNameModel[] = [];
  public accountTypeOptions: ValueAndNameModel[] = [];
  public accountId = AppConsts.VALUE_OPTIONS_ALL;
  public searchRequestChi: string;
  public accountTypeId = AppConsts.VALUE_OPTIONS_ALL;
  private defaultAccoutTypeName = undefined;
  public searchBankAccoutName: string;
  public isShowExchangeRate: boolean = false;
  public totalSelectedOutcome: number = 0
  public isValidLinkOutcome:boolean = false

  public checkboxEnd = false;

  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<LinkBTransactionMultiOutComingDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: LinkBTransactionMultiOutComingDialogData,
    private _common: CommonService,
    private translate: TranslateService,
    private _btransaction: BtransactionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setDefaultPayload();
    this.getOutcomingEntry();
    this.accountTypeOptions.push(this.optionAll);
    this.accountOptions.push(this.optionAll);
    if(!this.isEnableMultiCurrency){
      this.checkCurrency();
    }
    this.setBankAccountSelection();
  }
  setDefaultPayload() {
    this.linkMultipleOutcomingEntryWithBTransactionDto.exchangeRate =
      AppConsts.EXCHANGE_RATE_DEFAULT;
    this.linkMultipleOutcomingEntryWithBTransactionDto.bTransactionId =
      this.data.bTransactionId;
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds = [];
  }
  checkCurrency() {
    this._btransaction
      .checkCurrencyLinkOutcomingEntryWithBTransaction(this.data.bTransactionId)
      .subscribe(
        (response) => {
          if (!response.success) return;
          this.isShowExchangeRate = !response.result;
        },
        () => { }
      );
  }
  setBankAccountSelection() {
    if(this.isEnableMultiCurrency){
      this.setBankAccountTransaction();
      return;
    }
    this.setBankAccountTransactionOld();

  }
  setBankAccountTransaction() {
    this._common.getBankAccountOptions({currencyId: this.data.currencyId, isActive: true} as FilterBankAccount).subscribe((response) => {
      if (!response.success) return;
      this.bankAccountTransaction = response.result;
      this.setAllSelection(response.result);
    });
  }
  setBankAccountTransactionOld() {
    this._common.getBankAccountVNDOpstion().subscribe((response) => {
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
    this.translate.get("menu3.m3_child7").subscribe((res: string) => {
      this.defaultAccoutTypeName = res;
    });
    const defaultAccountType = this.bankAccountTransaction.find(
      (s) => s.accountTypeName == this.defaultAccoutTypeName
    );
    if (defaultAccountType && this.defaultAccoutTypeName) {
      this.accountTypeId = defaultAccountType.accountTypeId;
      this.setAccount(this.bankAccountTransaction, this.accountTypeId);
      this.setBankAccount(
        this.bankAccountTransaction,
        this.accountTypeId,
        this.accountId
      );
    } else {
      this.accountTypeId = AppConsts.VALUE_OPTIONS_ALL;
    }
    this.accountId = AppConsts.VALUE_OPTIONS_ALL;
  }
  setAccountType(datas: BankAccountTransDto[]) {
    this.accountTypeOptions = [];
    this.accountTypeOptions.push(this.optionAll);
    datas.forEach((element) => {
      const data = {
        value: element.accountTypeId,
        name: element.accountTypeName,
      } as ValueAndNameModel;
      if (!this.accountTypeOptions.find((s) => s.value == data.value))
        this.accountTypeOptions.push(data);
    });
  }
  setAccount(
    datas: BankAccountTransDto[],
    accountTypeId = AppConsts.VALUE_OPTIONS_ALL
  ) {
    this.accountOptions = [];
    this.accountOptions.push(this.optionAll);
    datas.forEach((element) => {
      if (
        Utils.isSelectedOptionsAll(accountTypeId) ||
        element.accountTypeId == accountTypeId
      ) {
        const data = {
          value: element.accountId,
          name: element.accountName,
        } as ValueAndNameModel;
        if (!this.accountOptions.find((s) => s.value == data.value))
          this.accountOptions.push(data);
      }
    });
  }
  outcommingSelectOpenedChange(isOpen: boolean) {
    if (isOpen) {
      this.inputSearchRequestChi.nativeElement.focus();
    }
    if (this.outcommingEntryOptions.length) return;

    const searchOutComming = this.backDataOutcommingEntryOptions.filter(
      (item) =>
        !this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.some(
          (outcomming) => outcomming == item.value
        )
    );

    if (!searchOutComming.length) return;

    this.searchOutComming = "";
    this.outcommingEntryOptions = _.cloneDeep(searchOutComming);
  }
  searchBankAccoutNameChange() {
    this.accountBankOptions = this.backDataAccountBankOptions.filter(s => s.name.trim().toLowerCase().includes(this.searchBankAccoutName.trim().toLowerCase()));
  }
  bankAccountSelectOpenedChange(isOpen: boolean) {
    if (isOpen) {
      this.inputSearchBankAccount.nativeElement.focus();
    }
    if (this.accountBankOptions.length) return;
    this.searchBankAccoutName = "";
    this.accountBankOptions = _.cloneDeep(this.backDataAccountBankOptions);
  }
  setBankAccount(
    datas: BankAccountTransDto[],
    accountTypeId = AppConsts.VALUE_OPTIONS_ALL,
    accountId = AppConsts.VALUE_OPTIONS_ALL
  ) {
    this.accountBankOptions = [];
    datas.forEach((element) => {
      if (
        (Utils.isSelectedOptionsAll(accountTypeId) ||
          element.accountTypeId == accountTypeId) &&
        (Utils.isSelectedOptionsAll(accountId) ||
          element.accountId == accountId)
      ) {
        const data = {
          value: element.bankAccountId,
          name: element.name,
        } as ValueAndNameModel;
        if (!this.accountBankOptions.find((s) => s.value == data.value))
          this.accountBankOptions.push(data);
      }
    });
    this.backDataAccountBankOptions = this.accountBankOptions;
  }
  getOutcomingEntry(onlyApproved = true) {
    this.outcommingEntryOptions = [];
    this._common.getApprovedAndEndOutcomingEntry(onlyApproved, this.isEnableMultiCurrency?this.data.currencyId:undefined).subscribe((response) => {
      if (!response.success) return;
      this.backDataOutcommingEntryOptions = this.outcommingEntryOptions = response.result;
      this.setListOutcommingEntrySelected();
    });
  }
  searchOutCommingChange() {
    this.outcommingEntryOptions = this.backDataOutcommingEntryOptions.filter(
      (item) =>
        item.name.trim()
          .toLowerCase()
          .includes(this.searchOutComming.trim().toLocaleLowerCase()) &&
        !this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.some(
          (outcomming) => outcomming == item.value
        )
    );
  }
  selectAll() {
    this.outcomingEntryIds = this.outcommingEntryOptions.map(
      (s) => s.value
    ) as number[];
  }
  clearAll() {
    let listOutCommingFilter = this.outcommingEntryOptions.map(
      (s) => s.value
    ) as number[];
    if (this.outcomingEntryIds) {
      this.outcomingEntryIds = this.outcomingEntryIds.map((item) => {
        if (!listOutCommingFilter.some((outcomming) => item == outcomming))
          return item;
      });
    }
  }
  cancelSelecttion() {
    this.searchOutComming = "";
    this.outcomingEntryIds = [];
    this.matSelect.close();
  }
  setoutcommingEntryOptions() {
    this.outcommingEntryOptions = this.backDataOutcommingEntryOptions.filter(
      (outcomming) =>
        !this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.some(outcomingEntryId => outcomingEntryId == outcomming.value)
    );
  }
  clickOKBtn() {
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.push(
      ...this.outcomingEntryIds
    );
    this.cancelSelecttion();
    this.setListOutcommingEntrySelected();
  }
  cancelSelectOutComming(id: number) {
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds =
      this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.filter(
        (item) => item != id
      );
    this.setListOutcommingEntrySelected();
  }
  cancelAllSelectOutComming() {
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds = [];
    this.setListOutcommingEntrySelected();
    this.outcomingEntryIds = [];
  }
  setListOutcommingEntrySelected() {
    this.outcomingEntryIds = [];
    this.listOutcommingEntrySelected =
      this.backDataOutcommingEntryOptions.filter((outcoming) =>
        this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.some(
          (item) => item == outcoming.value
        )
      );
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds =
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds
    .filter(outcomingEntryId =>
            this.listOutcommingEntrySelected.some(outcommingEntrySelected =>
              outcommingEntrySelected.value == outcomingEntryId)
          );
    this.totalSelectedOutcome = 0
    this.totalSelectedOutcome = this.listOutcommingEntrySelected.reduce((sum, item) => {
        return sum += item.money
    }, 0)

    this.isValidLinkOutcome = Math.abs(Number(this.data.moneyValue)) === Math.abs(this.totalSelectedOutcome)

    this.setoutcommingEntryOptions();
    this.resetErrorMess();
  }
  onAccountTypeChange() {
    this.setAccount(this.bankAccountTransaction, this.accountTypeId);
    this.setBankAccount(this.bankAccountTransaction, this.accountTypeId);
    this.accountId = AppConsts.VALUE_OPTIONS_ALL;
    this.searchBankAccoutName = "";
    this.linkMultipleOutcomingEntryWithBTransactionDto.toBankAccountId =
      undefined;
  }
  onAccountChange() {
    this.setBankAccount(
      this.bankAccountTransaction,
      this.accountTypeId,
      this.accountId
    );
    this.searchBankAccoutName = "";
    this.linkMultipleOutcomingEntryWithBTransactionDto.toBankAccountId =
      undefined;
  }
  onBankAccountChange() {
    let bankAccountSelection = this.bankAccountTransaction.find(
      (s) =>
        s.bankAccountId ==
        this.linkMultipleOutcomingEntryWithBTransactionDto.toBankAccountId
    );
    this.accountTypeId = bankAccountSelection.accountTypeId;
    this.setAccount(
      this.bankAccountTransaction,
      bankAccountSelection.accountTypeId
    );
    this.accountId = bankAccountSelection.accountId;
    this.resetErrorMess();
  }
  resetErrorMess() {
    this.errorMess = "";
  }
  process() {
    if (this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds) {
      abp.message.confirm(
        `Xác nhận`,
        "",
        (result: boolean) => {
          if (result) {
            this.doSave();
          }
        },
        true
      );
    }
  }
  doSave() {
    this._btransaction
      .linkMultipleOutcomingEntryWithBTransaction(
        this.linkMultipleOutcomingEntryWithBTransactionDto
      )
      .subscribe((response) => {
        if (!response.result.success) {
          this.errorMess = response.result.errorMessage;
        } else {
          this.errorMess = "";
          this.afterSave(response.result.bankTransactionId);
        }
      });
  }

  afterSave(bankTransactionId: number) {
    abp.message.confirm(
      `Bạn có muốn xem chi tiết`,
      "Done",
      (result: boolean) => {
        if (result) {
          this.dialogRef.close(bankTransactionId);
          return;
        } else {
          this.dialogRef.close();
        }
      },
      true
    );
  }
  addRequestChi(outcomming: SelectionOutcomingEntry) {
    this.outcommingEntryOptions = this.outcommingEntryOptions.filter(s => s !== outcomming);
    this.linkMultipleOutcomingEntryWithBTransactionDto.outcomingEntryIds.push(
      ...this.outcomingEntryIds
    );
    this.listOutcommingEntrySelected.push(outcomming);
    this.setListOutcommingEntrySelected();
    this.searchOutCommingChange();
  }
  checkboxEndChange() {
    this.getOutcomingEntry(!this.checkboxEnd);
  }
}

export interface LinkBTransactionMultiOutComingDialogData {
  bTransactionId: number;
  currencyName: string;
  currencyColor: string;
  currencyId: number;
  money: string;
  moneyColor: string;
  moneyValue?:number
}
export class LinkMultipleOutcomingEntryWithBTransactionDto {
  bTransactionId: number;
  toBankAccountId: number;
  outcomingEntryIds: number[];
  exchangeRate: number;
}
export class OutcomingEntrie {
  id: number;
  name: string;
  isWaning: boolean;
}
export class SelectionOutcomingEntry extends ValueAndNameModel {
  statsCode: string;
  statsName: string;
  typeName: string;
  typeId: number;
  money:number;
}
