import { AppConsts, BANK_TRANSCATION_DATE_TIME_OPTIONS, OPTION_ALL } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { InputFilterDto } from "./../../../shared/filter/filter.component";
import { CreateEditTransactionComponent } from "./create-edit-transaction/create-edit-transaction.component";
import { Router, ActivatedRoute } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import {
  PagedListingComponentBase,
  PagedRequestDto,
  tableHeaderDto,
} from "@shared/paged-listing-component-base";
import {
  Component,
  ElementRef,
  Injector,
  OnInit,
  ViewChild,
  HostListener,
} from "@angular/core";
import { TransactionService } from "../../service/api/transaction.service";
import { catchError, finalize } from "rxjs/operators";
import { isNgTemplate } from "@angular/compiler";
import * as FileSaver from "file-saver";
import {
  LinkBTransactionDialogComponent,
  LinkBTransactionDialogData,
} from "./link-BTransaction-dialog/link-b-transaction-dialog.component";
import { Time } from "@angular/common";
import * as moment from "moment";
import { CurrencyService } from "@app/service/api/currency.service";
import { CurrencyConvertDto } from "../currency/currency.component";
import { AccountTypeEnum, BankTransactionFilterDateTimeType, CurrencyColor, DateSelectorEnum } from "@shared/AppEnums";
import { IOption } from '@shared/components/custome-select/custome-select.component';
import { Utils } from '@app/service/helpers/utils';
import { DateTimeSelector } from '@shared/date-selector/date-selector.component';

@Component({
  selector: "app-banking-transaction",
  templateUrl: "./banking-transaction.component.html",
  styleUrls: ["./banking-transaction.component.css"],
})
export class BankingTransactionComponent
  extends PagedListingComponentBase<BankTransactionDto>
  implements OnInit
{
  @ViewChild("dropdownFilter") dropdownFilter: ElementRef;
  @ViewChild("inputSearchFromBank") inputSearchFromBank: ElementRef;
  @ViewChild("inputSearchToBank") inputSearchToBank: ElementRef;

  Finance_BankTransaction_Create =
    PERMISSIONS_CONSTANT.Finance_BankTransaction_Create;
  Finance_BankTransaction_Delete =
    PERMISSIONS_CONSTANT.Finance_BankTransaction_Delete;
  Finance_BankTransaction_ExportExcel =
    PERMISSIONS_CONSTANT.Finance_BankTransaction_ExportExcel;

  transactionList: any;
  sortDrirect: number = 0;
  transDate: string = "";
  iconSort: string = "";
  iconCondition: string = "transactionDate";
  requestParam: PagedRequestDto;
  searchWithDateTime = {} as DateTimeSelector;
  defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
  TABLE_NAME = "transactionFilter";
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    {
      propertyName: "name",
      displayName: "filterBy.Name",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "fromBankAccountName",
      displayName: "filterBy.SentBank",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "toBankAccountName",
      displayName: "filterBy.ReceivedBank",
      comparisions: [0, 6, 7, 8],
    },

    {
      propertyName: "fromValue",
      displayName: "filterBy.FromValue",
      comparisions: [0, 1, 3],
    },
    {
      propertyName: "toValue",
      displayName: "filterBy.Tovalue",
      comparisions: [0, 1, 3],
    },
    {
      propertyName: "fee",
      displayName: "filterBy.Fee",
      comparisions: [0, 1, 3],
    },
    {
      propertyName: "createDate",
      displayName: "Ngày tạo",
      comparisions: [0, 1, 3],
      filterType: 1,
    },
    {
      propertyName: "transactionDate",
      displayName: "Ngày giao dịch",
      comparisions: [0, 1, 3],
      filterType: 1,
    },
  ];
  showHeader = false;
  tableHeader: tableHeaderDto[] = [
    { name: "Check All", value: true, fieldName: "checkAll" },
    { name: "STT", value: true, fieldName: "stt" },
    { name: "Name", value: true, fieldName: "name" },
    { name: "BTransactionInfo", value: true, fieldName: "bTransactionInfo" },
    { name: "From bank", value: true, fieldName: "fromBankAccountName" },
    { name: "To bank", value: true, fieldName: "toBankAccountName" },
    { name: "Value", value: true, fieldName: "value" },
    //{ name: 'From value', value: true, fieldName: 'fromValue' },
    //{ name: 'To value', value: true, fieldName: 'toValue' },
    { name: "Fee", value: true, fieldName: "fee" },
    { name: "Updated At", value: true, fieldName: "updatedAt" },
    { name: "Sent date", value: true, fieldName: "transactionDate" },
    { name: "Note", value: true, fieldName: "note" },
  ];

  fromBankOptions: BankDto[] = [];
  toBankOptions: BankDto[] = [];
  searchId: number;
  searchMoney: number;
  searchFee: number;
  searchFromBank: string;
  searchToBank: string;
  selectedFromBank: number[] = [];
  selectedToBank: number[] = [];
  fromCurrency: number[] = [];
  toCurrency: number[] = [];
  comparisionDate: number;
  searchDate = new Date();

  CurrencyColor = CurrencyColor;
  BANK_TRANSCATION_DATE_TIME_OPTIONS = BANK_TRANSCATION_DATE_TIME_OPTIONS;
  optionDate: BankTransactionFilterDateTimeType = BankTransactionFilterDateTimeType.NO_FILTER;
  BankTransactionFilterDateTimeTypeNO_FILTER = BankTransactionFilterDateTimeType.NO_FILTER

  protected list(
    request: GetAllPagingBankTransactionDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.filterItems = [];
    request.fromCurrencyIds = this.fromCurrency;
    request.toCurrencyIds = this.toCurrency;
    request.sort = this.transDate;
    request.sortDirection = this.sortDrirect;
    this.requestParam = request;
    request.id = this.searchId;
    request.fromMoney = this.searchMoney;
    request.fee = this.searchFee;
    request.fromBankAccounts = this.selectedFromBank;
    request.toBankAccounts = this.selectedToBank;

    request.filterDateTime = {
      dateTimeType: this.optionDate ? this.optionDate : BankTransactionFilterDateTimeType.NO_FILTER,
      fromDate: this.searchWithDateTime.fromDate?.format("YYYY/MM/DD"),
      toDate: this.searchWithDateTime.toDate?.format("YYYY/MM/DD")
    };
    if(this.defaultDateFilterType == DateSelectorEnum.ALL){
      request.filterDateTime.fromDate = undefined;
      request.filterDateTime.toDate = undefined;
    }

    this.service
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((data) => {
        this.transactionList = data.result.items.map((transaction) => {
          transaction.displayName = transaction.name.replace("\n", "<br/>");
          return transaction;
        });
        this.showPaging(data.result, pageNumber);
      });
  }

  listCurrency: CurrencyConvertDto[];

  constructor(
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private service: TransactionService,
    private currencyService: CurrencyService,
    injector: Injector
  ) {
    super(injector);
    const id = route.snapshot.queryParamMap.get("id");
    if (id) {
      this.searchId = Number(id);
    }
    this.applyUrlFilters()
  }

  ngOnInit(): void {
    this.subscriptions.push(
    AppConsts.periodId.asObservable().subscribe(() => this.refresh()))
    this.setTableHeader(this.tableHeader, this.TABLE_NAME);
    this.getAllBankAccount();
    this.currencyService.GetAllForDropdown().subscribe((data) => {
      this.listCurrency = data.result.map(s => {return {value: s.id, name: s.name}});
    });
    this.sortDate("transactionDate");
  }

  getAllBankAccount() {
    this.service.getAllFromBankAccount().subscribe((data) => {
      this.fromBankOptions = data.result;
    });
    this.service.getAllToBankAccount().subscribe((data) => {
      this.toBankOptions = data.result;
    });
  }
  getOptionName(bankInfo: BankDto){
    return `${bankInfo.bankAccountName}(${bankInfo.bankAccountCurrency}) [${bankInfo.bankAccountTypeCode}]`;
  }

  turnOffPermission = false;
  sortDate(data) {
    if (this.iconCondition !== data) {
      this.sortDrirect = -1;
    }
    this.iconCondition = data;
    this.transDate = data;
    this.sortDrirect++;
    if (this.sortDrirect > 1) {
      this.transDate = "";
      this.iconSort = "";
      this.sortDrirect = -1;
    }
    if (this.sortDrirect == 1) {
      this.iconSort = "fas fa-sort-amount-down";
    } else if (this.sortDrirect == 0) {
      this.iconSort = "fas fa-sort-amount-up";
    } else {
      this.iconSort = "fas fa-sort";
    }
    this.refresh();
  }
  showOrHideHeader(): void {
    this.showHeader = !this.showHeader;
    if (this.showHeader) {
      this.turnOffPermission = false;
    }
  }
  getBTRansactionInfo(transaction) {
    return (
      "#" +
      transaction.bTransactionId +
      " " +
      (transaction.bTransactionMoneyNumber > 0 ? "+" : "") +
      transaction.bTransactionMoney +
      transaction.bTransactionCurrencyName +
      "<br/>TK " +
      transaction.bTransactionBankNumber
    );
  }
  showBTransaction(bTRansactionid: number) {
    // if (this.permission.isGranted(this.Finance_OutcomingEntry_ViewDetail)) {
    //   this.router.navigate(['app/btransaction'], {
    //     queryParams: {
    //       id: bTRansactionid,
    //     }
    //   })
    // }
    this.router.navigate(["app/btransaction"], {
      queryParams: {
        id: bTRansactionid,
      },
    });
  }
  turnOff(): void {
    if (this.turnOffPermission) {
      this.showHeader = false;
    }
    this.turnOffPermission = true;
  }

  handleClearFilter() {
    this.optionDate = BankTransactionFilterDateTimeType.NO_FILTER;
    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.comparisionDate = undefined;
    this.searchText = "";
    this.searchDate = new Date();
    this.searchId = undefined;
    this.selectedFromBank = [];
    this.selectedToBank = [];
    this.searchMoney = undefined;
    this.searchFee = undefined;
    this.fromCurrency = [];
    this.toCurrency = [];
    this.setParamToUrl();
    this.refresh();
  }

  changSelection(item): void {
    if (item.fieldName == "checkAll") {
      this.tableHeader.forEach((e) => {
        e.value = item.value;
      });
    } else {
      this.tableHeader[0].value = this.tableHeader
        .slice(1, this.tableHeader.length)
        .some((h) => h.value == false)
        ? false
        : true;
    }

    localStorage.setItem("transactionFilter", JSON.stringify(this.tableHeader));
  }
  showDialog(command: String, transaction: any): void {
    let bankTransaction = {} as BankTransactionDto;
    if (command == "edit") {
      bankTransaction = {
        accountTypeCode: transaction.accountTypeCode,
        fromBankAccountId: transaction.fromBankAccountId,
        fromBankAccountName: transaction.fromBankAccountName,
        toBankAccountName: transaction.toBankAccountName,
        toBankAccountId: transaction.toBankAccountId,
        fromValue: transaction.fromValue,
        toValue: transaction.toValue,
        fromBankAccountTypeCode: transaction.fromBankAccountTypeCode,
        toBankAccountTypeCode: transaction.toBankAccountTypeCode,
        fee: transaction.fee,
        transactionDate: transaction.transactionDate,
        note: transaction.note,
        fromBankAccountCurrency: transaction.fromBankAccountCurrency,
        toBankAccountCurrency: transaction.toBankAccountCurrency,
        id: transaction.id,
        name: transaction.name,
      };
    }

    this.dialog.open(CreateEditTransactionComponent, {
      data: {
        item: bankTransaction,
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }

  setParamToUrl() {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        id: this.searchId,
        searchText: this.searchText,
      },
      queryParamsHandling: "merge",
    });
  }

  onFromBankSelect(ids: number[]) {
    this.selectedFromBank = ids;
    if(this.selectedFromBank.length === 0){
      this.onPageFilter('fromBankAccounts', OPTION_ALL)
    }
    else{
      this.onPageFilter('fromBankAccounts', this.selectedFromBank)
    }
  }

  onToBankSelect(ids: number[]) {
    this.selectedToBank = ids;
    if(this.selectedToBank.length === 0){
      this.onPageFilter('toBankAccounts', OPTION_ALL)
    }
    else{
      this.onPageFilter('toBankAccounts', this.selectedToBank)
    }
  }

  handleChangeOptionDate(value: number) {
    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.onPageFilter('optionDate', value)
  }

  handleChangeDateComparision(value){
    this.setFilterToUrl('comparisionDate', value)
  }

  createTransaction() {
    this.showDialog("create", {});
  }
  editTransaction(transaction: BankTransactionDto) {
    this.showDialog("edit", transaction);
  }

  delete(item: BankTransactionDto): void {
    abp.message.confirm(
      "Delete transaction " + item.name + "?",
      "",
      (result: boolean) => {
        if (result) {
          this.service
            .delete(item.id)
            .pipe(catchError(this.service.handleError))
            .subscribe(() => {
              abp.notify.success("Deleted transaction " + item.name);
              this.refresh();
            });
        }
      }
    );
  }

  showDetail(id) {
      this.router.navigate(["/app/detail"], {
        queryParams: {
          index: 0,
          id: id,
        },
      });
  }
  lockBankTransction(transction) {
    abp.message.confirm(
      "Lock transaction: " + transction.name + "?",
      ``,
      (result: boolean) => {
        if (result) {
          this.service.lockBankTransaction(transction.id).subscribe((rs) => {
            abp.notify.success("Locked transaction: " + transction.name);
            this.refresh();
          });
        }
      }
    );
  }
  unlockBankTransaction(transction) {
    abp.message.confirm(
      "Unlock transaction: " + transction.name + "?",
      "",
      (result: boolean) => {
        if (result) {
          this.service.unlockBankTransaction(transction.id).subscribe((rs) => {
            abp.notify.success("Unlocked transaction: " + transction.name);
            this.refresh();
          });
        }
      }
    );
  }
  downloadFile() {
    this.service.exportExcel(this.requestParam).subscribe((data) => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8",
      });
      FileSaver.saveAs(file, `Giao dịch ngân hàng.xlsx`);
    });
  }
  newDownloadFile(){
    this.service.exportNewExcel(this.requestParam).subscribe((data) => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8",
      });
      FileSaver.saveAs(file, `Giao dịch ngân hàng.xlsx`);
    });
  }

  linkBTransaction(transaction: DetailBankTransactionDto): void {
    //TODO: show dialog payment for customer
    const linkBTransactionDialogComponent = this.dialog.open(
      LinkBTransactionDialogComponent,
      {
        width: "900px",
        data: {
          bankTransactionId: transaction.id,
          title: transaction.name,
        } as LinkBTransactionDialogData,
      }
    );

    linkBTransactionDialogComponent.afterClosed().subscribe((result) => {
      this.refresh();
      //TODO: handle result
    });
  }

  async onSearchId(){
    await this.onResetFilter()
    this.searchText = ""
    this.searchMoney = null
    this.onPageFilter('id', this.searchId)
  }

  onFilterFromCurrency(ids: number[]){
    this.fromCurrency = ids;
    if(this.fromCurrency.length === 0){
      this.onPageFilter('fromCurrencyIds', OPTION_ALL)
    }
    else{
      this.onPageFilter('fromCurrencyIds', this.fromCurrency)
    }
  }

  onFilterToCurrency(ids: number[]){
    this.toCurrency = ids;
    if(this.toCurrency.length === 0){
      this.onPageFilter('toCurrencyIds', OPTION_ALL)
    }
    else{
      this.onPageFilter('toCurrencyIds', this.toCurrency)
    }
  }

  async onSearch(){
    this.searchId = null
    this.searchMoney = null

    await this.onResetFilter()
    this.getDataPage(1)
  }

  async onSearchMoney(){
    await this.onResetFilter()
    this.searchText = ""
    this.searchId = null
    this.onPageFilter('fromMoney', this.searchMoney)
  }

  applyUrlFilters(){
    var querySnapshot = this.route.snapshot.queryParams
      this.searchId = querySnapshot['id'] ? querySnapshot['id'] : null;
      this.searchMoney = querySnapshot['fromMoney'] ? querySnapshot['fromMoney'] : null;
      this.optionDate = querySnapshot['optionDate'] ? Number(querySnapshot['optionDate']) : BankTransactionFilterDateTimeType.NO_FILTER;
      this.comparisionDate = querySnapshot['comparisionDate'] ?  Utils.toNumber(querySnapshot['comparisionDate']) : null;
      this.selectedFromBank = querySnapshot['fromBankAccounts'] ? JSON.parse(querySnapshot['fromBankAccounts']) : [];
      this.selectedToBank = querySnapshot['toBankAccounts'] ? JSON.parse(querySnapshot['toBankAccounts']) : [];
      this.fromCurrency = querySnapshot['fromCurrencyIds'] ? JSON.parse(querySnapshot['fromCurrencyIds']) : [];
      this.toCurrency = querySnapshot['toCurrencyIds'] ? JSON.parse(querySnapshot['toCurrencyIds']) : [];

      let dateFilterParam = querySnapshot['dateFilter'] ? JSON.parse(querySnapshot['dateFilter']) : {} as DateTimeSelector;

    if (dateFilterParam.dateType) {
      this.searchWithDateTime = dateFilterParam
      this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate)
      this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate)
      this.defaultDateFilterType = this.searchWithDateTime.dateType
    }
}
  isShowLinkToBDSDBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BankTransaction_LinkToBienDongSoDu);
  }
  isAllowRoutingToDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BankTransaction_BankTransactionDetail);
  }
  isShowLockUnlockBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BankTransaction_LockUnlock);
  }
  isAllowRoutingToBDSD(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD)
  }
  async onResetFilter() {
    this.optionDate = BankTransactionFilterDateTimeType.NO_FILTER;
    this.comparisionDate = null
    this.selectedFromBank = []
    this.selectedToBank = []
    this.fromCurrency = []
    this.toCurrency = []

    this.resetQueryParams(['id', 'fromMoney', 'optionDate', 'comparisionDate', 'fromBankAccounts', 'toBankAccounts', 'fromCurrencyIds', 'toCurrencyIds', 'dateFilter'])
  }

  onDateChange(event) {
    let data = event;
    if(data.dateType == DateSelectorEnum.ALL){
      data.fromDate = undefined;
      data.toDate = undefined;
    }
    this.searchWithDateTime = data;
    this.defaultDateFilterType = data.dateType;
    this.searchWithDateTime.dateType = data.dateType
    let cloneDate = { ...data }
    cloneDate.fromDate = moment(cloneDate.fromDate)
    cloneDate.toDate = moment(cloneDate.toDate)

    this.onPageFilter('dateFilter', cloneDate)
  }
}
export class BankTransactionDto {
  fromBankAccountId: number;
  fromBankAccountName: string;
  fromBankAccountTypeCode: string;
  toBankAccountTypeCode: string;
  toBankAccountName: string;
  toBankAccountId: number;
  fromValue: number;
  toValue: number;
  fee: number;
  accountTypeCode: string;
  transactionDate: string;
  note: string;
  fromBankAccountCurrency: string;
  toBankAccountCurrency: string;
  id: number;
  name: string;
  lockedStatus?: boolean;
}
export class DetailBankTransactionDto {
  id: number;
  name: string;
  fromBankAccountId: number;
  fromBankAccountName: string;
  fromBankAccountCurrency: string;
  fromBankAccountTypeCode: string;
  toBankAccountId: number;
  toBankAccountName: string;
  toBankAccountCurrency: string;
  toBankAccountTypeCode: string;
  fromValue: number;
  toValue: number;
  fee: number;
  transactionDate: Time;
  note: string;
  numberOfIncomingEntries: number;
  createDate: Date;
  lockedStatus: boolean;
  bTransactionId: number;
  isWarning: boolean;
  displayName: string;
}

export interface BankDto {
  bankAccountId: number;
  bankAccountName: string;
  bankAccountCurrency: string;
  bankAccountTypeCode: string;
  accountTypeEnum: AccountTypeEnum;
  name: string;
  value: number;
}

export class GetAllPagingBankTransactionDto extends PagedRequestDto {
  id: number;
  fromMoney: number;
  toMoney: number;
  fromCurrencyIds: number[];
  toCurrencyIds: number[];
  fromBankAccounts: number[];
  toBankAccounts: number[];
  fee: number;
  filterDateTime: {
    dateTimeType: BankTransactionFilterDateTimeType;
    fromDate: string;
    toDate: string;
  };
}
