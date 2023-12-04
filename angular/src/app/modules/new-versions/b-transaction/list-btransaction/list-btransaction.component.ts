import { RollbackLinkOutcomingEntryComponent } from "./../rollback-link-outcoming-entry/rollback-link-outcoming-entry.component";
import { ImportBTransactionResult } from "./../../../../service/model/b-transaction.model";
import {
  PaymentDialogComponent,
  PaymentDialogData,
} from "./../payment-dialog/payment-dialog.component";
import { MatDialog } from "@angular/material/dialog";
import { UtilitiesService } from "./../../../../service/api/new-versions/utilities.service";
import { CommonService } from "./../../../../service/api/new-versions/common.service";
import {
  BTransactionStatus,
  CommandDialog,
  ExpressionEnum,
} from "./../../../../../shared/AppEnums";
import { DateSelectorEnum } from "@shared/AppEnums";
import { BtransactionService } from "./../../../../service/api/new-versions/btransaction.service";
import { Injector, ViewChild, ViewChildren } from "@angular/core";
import {
  FilterDto,
  PagedListingComponentBase,
  PagedRequestDto,
} from "../../../../../shared/paged-listing-component-base";
import { Component, OnInit } from "@angular/core";
import { BTransaction } from "../../../../service/model/b-transaction.model";
import { AppConsts, OPTION_ALL } from "@shared/AppConsts";
import { ValueAndNameModel } from "@app/service/model/common-DTO";
import { Utils } from "../../../../service/helpers/utils";
import { EComparisor } from "@app/modules/revenue-managed/revenue-managed.component";
import {
  DateFormat,
  DateTimeSelector,
} from "@shared/date-selector/date-selector.component";
import { IFilterDateTimeParam } from "../../../../service/interfaces/filter-date.interface";
import { EspecialIncomingEntryTypeDto, SettingPaymentDialogComponent } from "../setting-payment-dialog/setting-payment-dialog.component";
import { CreateEditBTransactionComponent } from "../create-edit-b-transaction/create-edit-b-transaction.component";
import { ActivatedRoute } from "@angular/router";
import {
  LinkExpenditureAndBTransDialogComponent,
  LinkExpenditureAndBTransDialogData,
} from "../link-expenditure-dialog/link-expenditure-dialog.component";
import {
  LinkRevenueRecognitionAndBTransDialogComponent,
  RevenueRecognitionDialogData,
} from "../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import {
  LinkBTransactionMultiOutComingDialogComponent,
  LinkBTransactionMultiOutComingDialogData,
} from "../link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component";
import { ImportFileComponent } from "../import-file/import-file.component";
import { DialogResultImportFileComponent } from "../dialog-result-import-file/dialog-result-import-file.component";
import {
  LinkMultiBtransactionOutcomingEntryDialogComponent,
  LinkMultiBtransactionOutcomingEntryDialogData,
} from "../link-multi-btransaction-outcoming-entry-dialog/link-multi-btransaction-outcoming-entry-dialog.component";
import {
  CurrencyExchangeComponent,
  CurrencyExchangeDialogData,
} from "../currency-exchange/currency-exchange.component";
import { BuyForeignCurrencyComponent } from "../buy-foreign-currency/buy-foreign-currency.component";
import { CreateMultiIncomingEntryComponent } from "../create-multi-incoming-entry/create-multi-incoming-entry.component";
import { ChiChuyenDoiComponent } from "../chi-chuyen-doi/chi-chuyen-doi.component";
import * as moment from "moment";
import { AppConfigurationService } from "@app/service/api/app-configuration.service";
import { MatMenuTrigger } from "@angular/material/menu";
import * as FileSaver from "file-saver";
import { RollbackClientPaidComponent } from "../rollback-client-paid/rollback-client-paid.component";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";

@Component({
  selector: "app-list-btransaction",
  templateUrl: "./list-btransaction.component.html",
  styleUrls: ["./list-btransaction.component.css"],
})
export class ListBtransactionComponent
  extends PagedListingComponentBase<BTransaction>
  implements OnInit {
  btransactions: BTransactionAndCheck[] = [];
  defaultDateFilterType = DateSelectorEnum.ALL;

  accountOptions: ValueAndNameModel[] = [];
  transactionStatusOptions: ValueAndNameModel[] = [];
  transactionStatusOptionsPendding: any = BTransactionStatus.PENDING;
  searchDetail = {
    bankAccountId: AppConsts.VALUE_OPTIONS_ALL,
    bTransactionStatus: this.transactionStatusOptionsPendding,
  };

  expressionOptions: ValueAndNameModel[] = AppConsts.ExpressionOptions;
  slExpression: number = ExpressionEnum.EQUAL;
  searchMoney: any = {
    fromMoney: "",
    toMoney: "",
  };
  public readonly SORT_DIRECTION = ESortDirection;
  public currentSortField: string = DEFAULT_VALUES.SORT_FIELD;
  public currentSortOption: number = ESortDirection.ASCENDING;
  public tooltipChiChuyenDoi = "Chi chuyển đổi thực hiện các chức năng bao gồm:"
    + "\n+ Bán ngoại tệ (Chi ngoại tệ & nhận tiền default)"
    + "\n+ Mua ngoại tệ (Chi tiền default & nhận về ngoại tệ)"
    + "\n+ Gửi tiết kiệm (Chi tiền default & nhận tiền default)"
    + "\n+ Chuyển tiền nội bộ (Chuyển tiền giữa các tài khoản ngân hàng của công ty)"
    + "\nChú ý: Chọn các biến động số dư âm và dương tương ứng";
  public readonly SORTABLE_FIELD = {
    Money: "moneyNumber",
    TimeAt: "timeAt",
  };
  searchWithDateTime = {} as DateTimeSelector;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu5;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.btransaction;
  queryParams;

  id?: number | string;
  @ViewChildren(MatMenuTrigger) menuTrigger: any;
  constructor(
    injector: Injector,
    private _btransaction: BtransactionService,
    private _common: CommonService,
    public _utilities: UtilitiesService,
    public dialog: MatDialog,
    private _settingService: AppConfigurationService,
    private route: ActivatedRoute,
    private translate: TranslateService
  ) {
    super(injector);
    this.id = this.route.snapshot.queryParamMap.get("id");
    this.getBTranByStatus();
    this.applyUrlFilters()
  }
  ngOnInit(): void {
    if (this.id) {
      this.searchDetail = {
        bankAccountId: AppConsts.VALUE_OPTIONS_ALL,
        bTransactionStatus: AppConsts.VALUE_OPTIONS_ALL,
      };
    }
    this.setAccountOptions();
    this.setBTransactionStatusOptions();
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.refresh();
      }));
    this.getEspecialIncomingEntryType();
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
}

onLangChange(){
  this.translate.get("menu.menu5").subscribe((res: string) => {
    this.routeTitleFirstLevel = res;
    this.updateBreadCrumb();
  });
  this.translate.get("menu5.m5_child8").subscribe((res: string) => {
    this.title = res;
    this.updateBreadCrumb();
  });
}

updateBreadCrumb() {
  let queryParamsString = this.queryParams.toString();
  this.listBreadCrumb = [
    { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
    { name: ' <i class="fas fa-chevron-right"></i> ' },
    { name: this.title , url: this.routeUrlSecondLevel + (queryParamsString ? '?' + queryParamsString : '')}
  ];
}
  getEspecialIncomingEntryType() {
    this._settingService.getEspecialIncomingEntryType()
      .subscribe(async response => {
        if (!response.success) return;
        if (!response.result.balanceIncomingEntryTypeCode || !response.result.debtIncomingEntryTypeCode || !response.result.deviantIncomingEntryTypeCode) {
          await abp.message.warn("Setting khách hàng thanh toán chưa đầy đủ", "Warning");
          this.settingPayment();
        }
      });
  }
  onChangeFilter() {
    // this.id = "";
    this.getFirstPage();
  }
  payment(transaction: BTransaction): void {
    //TODO: show dialog payment for customer
    const paymentDialog = this.dialog.open(PaymentDialogComponent, {
      width: "700px",
      data: {
        bTransactionId: transaction.bTransactionId,
        money: transaction.moneyNumber,
        currencyName: transaction.currencyName,
      } as PaymentDialogData,
    });

    paymentDialog.afterClosed().subscribe((result) => {
      this.refresh();
      //TODO: handle result
    });
  }
  linkOutcomingEntryWithBTransaction(transaction: BTransaction): void {
    //TODO: show dialog
    const linkOutcomingEntryWithBTransactionDialog = this.dialog.open(
      LinkExpenditureAndBTransDialogComponent,
      {
        width: "950px",
        data: {
          bTransactionId: transaction.bTransactionId,
          currencyName: transaction.currencyName,
          currencyColor: this._utilities.getColorByCurrency(
            transaction.currencyName
          ),
          currencyId: transaction.currencyId,
          money:
            transaction.moneyNumber > 0
              ? "+" + transaction.money
              : transaction.money,
          moneyColor: transaction.moneyNumber > 0 ? "black" : "red",
          //title: "#" + transaction.bTransactionId.toString() + " (<strong> " + transaction.money + "</strong><strong>" + transaction.currencyName + "</strong>)"
        } as LinkExpenditureAndBTransDialogData,
      }
    );

    linkOutcomingEntryWithBTransactionDialog
      .afterClosed()
      .subscribe((outcomingEntryId) => {
        if (outcomingEntryId) {
          this.showOutcomingEntry(outcomingEntryId);
          return;
        }
        this.refresh();
        //TODO: handle result
      });
  }
  linkMultitiOutcomingEntry(transaction: BTransaction): void {
    //TODO: show dialog
    const linkBTransactionMultiOutComingDialog = this.dialog.open(
      LinkBTransactionMultiOutComingDialogComponent,
      {
        width: "1100px",
        data: {
          bTransactionId: transaction.bTransactionId,
          currencyName: transaction.currencyName,
          currencyColor: this._utilities.getColorByCurrency(
            transaction.currencyName
          ),
          currencyId: transaction.currencyId,
          money:
            transaction.moneyNumber > 0
              ? "+" + transaction.money
              : transaction.money,
          moneyValue: transaction.moneyNumber,
          moneyColor: transaction.moneyNumber > 0 ? "black" : "red",
        } as LinkBTransactionMultiOutComingDialogData,
      }
    );

    linkBTransactionMultiOutComingDialog
      .afterClosed()
      .subscribe((outcomingEntryId) => {
        if (outcomingEntryId) {
          this.showDetailOutComming(outcomingEntryId);
          return;
        }
        this.refresh();
        //TODO: handle result
      });
  }
  revenueRecognition(transaction: BTransaction): void {
    const revenueRecognitionDialog = this.dialog.open(
      LinkRevenueRecognitionAndBTransDialogComponent,
      {
        width: "800px",
        data: {
          bTransactionId: transaction.bTransactionId,
          currencyName: transaction.currencyName,
          currencyColor: this._utilities.getColorByCurrency(
            transaction.currencyName
          ),
          money:
            transaction.moneyNumber > 0
              ? "+" + transaction.money
              : transaction.money,
          moneyColor: transaction.moneyNumber > 0 ? "black" : "red",
          //title: "#" + transaction.bTransactionId.toString() + " (" + transaction.money + transaction.currencyName + ")",
          currencyId: transaction.currencyId,
          moneyNumber: transaction.moneyNumber
        } as RevenueRecognitionDialogData,
      }
    );

    revenueRecognitionDialog.afterClosed().subscribe((id) => {
      if (id) {
        this.showRevenue(id);
        return;
      }
      this.refresh();
      //TODO: handle result
    });
  }

  showRevenue(id) {
    this.router.navigate(["/app/detail"], {
      queryParams: {
        index: 1,
        id: id,
      },
    });
    // if (this.permission.isGranted(this.Finance_IncomingEntry_ViewDetail)) {
    //   this.router.navigate(["/app/detail"], {
    //     queryParams: {
    //       index: 1,
    //       id: id,
    //     },
    //   });
    // }
  }
  //Chưa check quyền
  showOutcomingEntry(id) {
    this.router.navigate(["/app/requestDetail/relevant-transaction"], {
      queryParams: {
        index: 2,
        id: id,
      },
    });
    // if (this.permission.isGranted(this.Finance_IncomingEntry_ViewDetail)) {
    //   this.router.navigate(["/app/detail"], {
    //     queryParams: {
    //       index: 1,
    //       id: id,
    //     },
    //   });
    // }
  }
  showDetailOutComming(id) {
    this.router.navigate(["/app/detail/"], {
      queryParams: {
        index: 3,
        id: id,
      },
    });
    // if (this.permission.isGranted(this.Finance_IncomingEntry_ViewDetail)) {
    //   this.router.navigate(["/app/detail"], {
    //     queryParams: {
    //       index: 1,
    //       id: id,
    //     },
    //   });
    // }
  }

  onDateChange($event): void {
    this.searchWithDateTime = $event;
    this.defaultDateFilterType = $event.dateType;
    this.searchWithDateTime.dateType = $event.dateType
    let cloneDate = { ...$event }
    cloneDate.fromDate = moment(cloneDate.fromDate).format("YYYY-MM-DD")
    cloneDate.toDate = moment(cloneDate.toDate).format("YYYY-MM-DD")

    this.onPageFilter('dateFilter', cloneDate)
  }

  private setAccountOptions(): void {
    this._common.getBankAccountInBTransaction().subscribe((response) => {
      if (!response.success) return;

      this.accountOptions = response.result
    });
  }
  private setBTransactionStatusOptions(): void {
    this._common.getBTransactionStatus().subscribe((response) => {
      if (!response.success) return;

      this.transactionStatusOptions = response.result
    });
  }
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const payload = this.getRequestParams(request);
    if (this.currentSortField) {
      payload.sort = this.currentSortField;
      payload.sortDirection = this.currentSortOption;
    }
    this._btransaction
      .getAllTransactionPaging(payload, this.id)
      .subscribe((response) => {
        if (!response.success) return;

        this.btransactions = response.result.items;
        this.totalItems = response.result.totalCount;
        this.isTableLoading = false;
      });
  }
  private getRequestParams(
    request: PagedRequestDto
  ): BTransactionPagedRequestDto {
    const payload = { ...request } as BTransactionPagedRequestDto;
    const filterItems: FilterDto[] = [];

    for (const property in this.searchDetail) {
      if (!Utils.isSelectedOptionsAll(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: EComparisor.EQUAL,
        };
        filterItems.push(filterObj);
      }
    }
    payload.filterItems = filterItems;

    if (!Utils.isSelectedOptionsAll(this.slExpression)) {
      payload.filterMoneyParam = {
        type: this.slExpression,
        fromValue: this.searchMoney.fromMoney,
        toValue: this.searchMoney.toMoney,
      } as IFilterMoneyParam;
    }

    if (this.searchWithDateTime?.dateType !== DateSelectorEnum.ALL) {
      // this.id = "";
      payload.filterDateTimeParam = {
        fromDate: this.searchWithDateTime?.fromDate?.format(
          DateFormat.YYYY_MM_DD
        ),
        toDate: this.searchWithDateTime?.toDate?.format(DateFormat.YYYY_MM_DD),
      } as IFilterDateTimeParam;
    }

    return payload;
  }
  protected delete(entity: BTransaction): void {
    this.menuTrigger._results.forEach((item) => item.closeMenu());
    abp.message.confirm(`Delete Transaction with Id = ${entity.bTransactionId} ?`, "", (result) => {
      if (result) {
        this._btransaction.delete(entity.bTransactionId).subscribe(() => {
          abp.notify.success("Transaction successfully");
          this.refresh();
        });
      }
    });
  }

  rollbackLinkOutcomingEntry(id: number): void {
    const _ref = this.dialog.open(RollbackLinkOutcomingEntryComponent, {
      data: id,
      width: "1100px",
      disableClose: true,
    });

    _ref.afterClosed().subscribe((result) => {
      if (!result) return;
      this.refresh();
    });
  }

  rollBackClientPaid(id: any): void {
    const _ref = this.dialog.open(RollbackClientPaidComponent, {
      data: id,
      width: "1300px",
      disableClose: true,
    });

    _ref.afterClosed().subscribe((result) => {
      if (!result) return;
      this.refresh();
    });
  }

  settingPayment() {
    let dialogRef = this.dialog.open(SettingPaymentDialogComponent, {
      data: {},
      width: "700px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.refresh();
      }
    });
  }

  createTransaction() {
    this.showCreateEditTransactionDialog(CommandDialog.CREATE);
  }
  editTransaction(btransaction: BTransaction) {
    this.showCreateEditTransactionDialog(CommandDialog.EDIT, btransaction);
  }

  openDialogSelectFile() {
    const _ref = this.dialog.open(ImportFileComponent, {
      width: "500px",
    });
    _ref.afterClosed().subscribe((res: ImportBTransactionResult) => {
      if (res) {
        this.dialog.open(DialogResultImportFileComponent, {
          data: res,
          width: "500px",
        });
      }
    });
  }

  private showCreateEditTransactionDialog(
    command: CommandDialog,
    item?: BTransaction
  ): void {
    let bTransaction = {} as CreateEditBTransactionDto;
    if (command == CommandDialog.EDIT) {
      bTransaction = {
        bTransactionId: item.bTransactionId,
        bankAccountId: item.bankAccountId,
        money: item.moneyNumber,
        timeAt: item.timeAt.toString(),
        note: item.note,
      };
    }
    //console.log(bTransaction)
    let dialogRef = this.dialog.open(CreateEditBTransactionComponent, {
      data: {
        command: command,
        item: bTransaction,
      },
      width: "700px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getDataPage(AppConsts.FIRST_PAGE);
      }
    });
  }
  sort(fieldName: string) {
    if (fieldName !== this.currentSortField) {
      this.currentSortField = fieldName;
      this.currentSortOption = ESortDirection.ASCENDING;
    } else {
      if (this.currentSortOption === ESortDirection.DESCENDING)
        this.currentSortOption = ESortDirection.ASCENDING;
      else this.currentSortOption = ESortDirection.DESCENDING;
    }
    this.getDataPage(AppConsts.FIRST_PAGE);
  }

  getBTranByStatus() {
    let status = this.route.snapshot.queryParamMap.get("status");
    if (status) {
      this.searchDetail.bTransactionStatus = status == "DONE"
        ? BTransactionStatus.DONE
        : BTransactionStatus.PENDING
    }
  }
  isShowBtnMenu(item: BTransaction) {
    return (
      this.isShowBtnKhachHangThanhToan(item) ||
      this.isShowBtnRequestChi(item) ||
      this.isShowBtnGhiNhanThu(item) ||
      this.isShowBtnEdit(item) ||
      this.isShowBtnDelete(item) ||
      this.isShowBtnRollBackBTransaction(item) ||
      this.isShowBtnRollbackClientPaid(item)
    );
  }
  isShowBtnKhachHangThanhToan(item: BTransaction) {
    return (
      item.bTransactionStatus == BTransactionStatus.PENDING &&
      item.moneyNumber > 0
    );
  }
  isShowBtnRollBackBTransaction(item: BTransaction) {
    return (
      item.bTransactionStatus == BTransactionStatus.DONE && item.moneyNumber < 0
    );
  }
  isShowBtnRollbackClientPaid(item: BTransaction) {
    return (
      item.bTransactionStatus == BTransactionStatus.DONE && item.moneyNumber >= 0
    );
  }
  isShowBtnRequestChi(item: BTransaction) {
    return (
      item.bTransactionStatus == BTransactionStatus.PENDING &&
      item.moneyNumber < 0
    );
  }
  isShowBtnGhiNhanThu(item: BTransaction) {
    return (
      item.bTransactionStatus == BTransactionStatus.PENDING &&
      item.moneyNumber > 0
    );
  }
  isShowBtnEdit(item: BTransaction) {
    return item.bTransactionStatus == BTransactionStatus.PENDING;
  }
  isShowBtnDelete(item: BTransaction) {
    return item.bTransactionStatus == BTransactionStatus.PENDING;
  }
  isShowBtnChiLuong() {
    return true;
  }
  isDisableBtnChiLuong() {
    return !this.btransactions.filter((s) => s.check).length;
  }
  openDialogChiLuong() {
    this.checkChiLuong();
  }
  isDisableBtnBanNgoaiTe() {
    return !this.btransactions.filter((s) => s.check).length;
  }
  isDisableBtnMuaNgoaiTe() {
    return !this.btransactions.filter((s) => s.check).length;
  }
  openDialogBanNgoaiTe() {
    this.checkBanNgoaiTe();
  }
  openDialogMuaNgoaiTe() {
    this.checkMuaNgoaiTe();
  }
  isDisableBtnChiChuyenDoi() {
    return !this.btransactions.filter((s) => s.check).length;
  }
  openDialogChiChuyenDoi() {
    const bTransactions = this.btransactions.filter((s) => s.check);
    const bTransactionMinus = bTransactions.filter(s => s.moneyNumber < 0);
    const bTransactionPlus = bTransactions.filter(s => s.moneyNumber > 0);
    if (bTransactionPlus.length == 0) {
      abp.message.warn("Phải có biến động số dư DƯƠNG");
      return;
    }
    if (bTransactionMinus.length == 0) {
      abp.message.warn("Phải có biến động số dư ÂM");
      return;
    }
    if (!this.checkCurrency(bTransactionPlus)) {
      abp.message.warn("Các biến động số dư DƯƠNG phải cùng loại tiền");
      return;
    }
    if (!this.checkCurrency(bTransactionMinus)) {
      abp.message.warn("Các biến động số dư ÂM phải cùng loại tiền");
      return;
    }
    const chiChuyenDoiComponent = this.dialog.open(
      ChiChuyenDoiComponent,
      {
        width: "1200px",
        data: {
          bTransactions: bTransactions,
        } as CurrencyExchangeDialogData,
        disableClose: true,
      }
    );

    chiChuyenDoiComponent
      .afterClosed()
      .subscribe((rs) => {
        if (rs.linkDone) {
          if (rs.outcomingEntryId) {
            this.showOutcomingEntry(rs.outcomingEntryId);
            return
          }
        }
        this.refresh()
      });
  }

  checkChiLuong() {
    const btransactions = this.btransactions.filter((s) => s.check);
    if (btransactions.some((s) => s.moneyNumber > 0)) {
      abp.message.error("Không thể chi lương từ các biến động số dư dương");
      return;
    }
    if (!this.checkCurrency(btransactions)) {
      abp.message.warn("Các biến động số dư phải cùng loại tiền");
      return;
    }
    const linkMultiBtransactionOutcomingEntryDialogComponent =
      this.dialog.open(LinkMultiBtransactionOutcomingEntryDialogComponent, {
        width: "800px",
        data: {
          bTransactions: btransactions,
        } as LinkMultiBtransactionOutcomingEntryDialogData,
        disableClose: true,
      });

    linkMultiBtransactionOutcomingEntryDialogComponent
      .afterClosed()
      .subscribe((rs) => {
        if (rs.linkDone) {
          if (rs.outcomingEntryId) {
            this.showOutcomingEntry(rs.outcomingEntryId);
            return;
          }
        }
        this.refresh();
      });
  }
  checkCurrency(bTransactions: BTransaction[]) {
    return [...new Set(bTransactions.map((s) => s.currencyId))].length == 1;
  }
  isShowCheckboxFromChiLuong(item: BTransaction) {
    return this.isShowBtnChiLuong() && item.bTransactionStatus != 1;
  }

  checkBanNgoaiTe() {
    const bankAccountSelected = this.btransactions.filter((s) => s.check);
    const bTransactionMinus = bankAccountSelected.filter(
      (s) => s.moneyNumber < 0
    );
    const bTransactionPlus = bankAccountSelected.filter(
      (s) => s.moneyNumber > 0
    );
    if ([...new Set(bTransactionMinus.map((s) => s.currencyId))].length > 1) {
      abp.message.warn("Các biến động số dư ÂM phải cùng loại tiền", "Warning");
      return;
    }
    if ([...new Set(bTransactionPlus.map((s) => s.currencyId))].length > 1) {
      abp.message.warn("Các biến động số dư DƯƠNG phải cùng loại tiền", "Warning");
      return;
    }
    if (!this.isEnableMultiCurrency && !this.checkCurrencyBanNgoaiTe(bTransactionMinus, bTransactionPlus)) {
      return;
    }
    const currencyExchangeComponent = this.dialog.open(
      CurrencyExchangeComponent,
      {
        width: "1200px",
        data: {
          bTransactions: this.btransactions.filter((s) => s.check),
        } as CurrencyExchangeDialogData,
        disableClose: true,
      }
    );

    currencyExchangeComponent
      .afterClosed()
      .subscribe((rs) => {
        if (rs.linkDone) {
          if (rs.outcomingEntryId) {
            this.showOutcomingEntry(rs.outcomingEntryId);
            return
          }
        }
        this.refresh()
      });
  }
  checkCurrencyBanNgoaiTe(bTransactionMinus: BTransaction[], bTransactionPlus: BTransaction[]) {
    if (!bTransactionMinus.filter((s) => s.currencyName != this.defaultCurrencyCode).length) {
      abp.message.warn(
        `Các biến động số dư ÂM phải KHÁC loại tiền là ${this.defaultCurrencyCode}`,
        "Warning"
      );
      return false;
    }
    if (bTransactionPlus.filter((s) => s.currencyName != this.defaultCurrencyCode).length) {
      abp.message.warn(
        `Các biến động số dư DƯƠNG phải cùng loại tiền là ${this.defaultCurrencyCode}`,
        "Warning"
      );
      return false;
    }
    return true;
  }


  checkMuaNgoaiTe() {
    const bankAccountSelected = this.btransactions.filter((s) => s.check);
    const bTransactionMinus = bankAccountSelected.filter(
      (s) => s.moneyNumber < 0
    );
    const bTransactionPositives = bankAccountSelected.filter(
      (s) => s.moneyNumber > 0
    );

    if ([...new Set(bTransactionMinus.map((s) => s.currencyId))].length > 1) {
      abp.message.warn("Các biến động số dư âm phải cùng loại tiền", "Warning");
      return;
    }
    if (bTransactionMinus[0].currencyName != this.defaultCurrencyCode) {
      abp.message.warn(
        "Các biến động số dư âm phải có loại tiền là ",
        "Warning"
      );
      return;
    }
    if (bTransactionPositives.filter((s) => s.currencyName == this.defaultCurrencyCode).length) {
      abp.message.warn(
        `Các biến động số dư dương phải khác loại tiền là ${this.defaultCurrencyCode}`,
        "Warning"
      );
      return;
    }
    const buyForeignCurrencyComponent = this.dialog.open(
      BuyForeignCurrencyComponent,
      {
        width: "1200px",
        data: {
          bTransactions: this.btransactions.filter((s) => s.check),
        } as CurrencyExchangeDialogData,
        disableClose: true,
      }
    );

    buyForeignCurrencyComponent
      .afterClosed()
      .subscribe((id) => {
        if (id) {
          this.showOutcomingEntry(id);
          return;
        }
        this.refresh();
        //TODO: handle result
      });
  }
  CreateMultiIncomingEntry(transaction: BTransaction) {
    const createMiltiIncomingEntryComponent = this.dialog.open(
      CreateMultiIncomingEntryComponent,
      {
        width: "1200px",
        data: {
          bTransactionId: transaction.bTransactionId,
          currencyName: transaction.currencyName,
          currencyColor: this._utilities.getColorByCurrency(
            transaction.currencyName
          ),
          money:
            transaction.moneyNumber > 0
              ? "+" + transaction.money
              : transaction.money,
          moneyColor: transaction.moneyNumber > 0 ? "black" : "red",
          //title: "#" + transaction.bTransactionId.toString() + " (" + transaction.money + transaction.currencyName + ")",
          currencyId: transaction.currencyId,
          moneyNumber: transaction.moneyNumber
        } as RevenueRecognitionDialogData,
        disableClose: true,
      }
    );

    createMiltiIncomingEntryComponent
      .afterClosed()
      .subscribe((id) => {
        if (id) {
          this.showRevenue(id);
          return;
        }
        this.refresh();
      });
  }

  exportBDSDByFilter() {
    const req = new PagedRequestDto();
    req.searchText = this.searchText;
    req.filterItems = this.filterItems;
    if (this.filterItems.length > 0) {
      req.filterItems.forEach((item, index) => {
        if (item.propertyName == "") {
          req.filterItems.splice(index, 1)
        }
      })
    }
    const payload = this.getRequestParams(req);
    if (this.currentSortField) {
      payload.sort = this.currentSortField;
      payload.sortDirection = this.currentSortOption;
    }
    abp.message.confirm(
      "Bạn có chắc muốn xuất danh sách BĐSD", 
      "Xác nhận", 
      (result) => {
      if (result) {
        this._btransaction.exportBDSDByFilter(payload).subscribe((response) => {
          if(!response.success) return;
          const file = new Blob([this.convertFile(atob(response.result))], {
            type: "application/vnd.ms-excel;charset=utf-8"
          });
          FileSaver.saveAs(file,`BDSD.xlsx`);
          abp.notify.success("Export successfully");
        });
      }
    });
  }

  isShowCaiDatKhachHangThanhToanBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_CaiDatThanhToanKhachHang);
  }
  isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_Create);
  }
  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_Edit);
  }
  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_Delete);
  }
  isShowLinkToRequestChiBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_LinkToRequestChi);
  }
  isShowLinkToMultipleRequestChiBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_LinkToMultipleRequestChi);
  }
  isShowThuHoiLinkToRequestChi() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_RemoveLinkToRequestChi);
  }
  isShowChiLuongBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_ChiLuong);
  }
  isShowBanNgoaiTeBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_BanNgoaiTe);
  }
  isShowMuaNgoaiTeBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_MuaNgoaiTe);
  }
  isShowImportBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_Import);
  }
  isShowKhachHangThanhToanBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_KhachHangThanhToan);
  }
  isShowCreateIncomingEntryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_CreateIncomingEntry);
  }
  isShowCreateMultiIncomingEntryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_CreateMultiIncomingEntry);
  }
  isShowChiChuyenDoi() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BĐSD_ChiChuyenDoi);
  }

  onFilterBankAccount() {
    this.onPageFilter('bankAccountId', this.searchDetail.bankAccountId)
  }

  async onFilterId() {
    await this.onResetFilter()
    this.searchText = ""

    this.onPageFilter('id', this.id)
  }

  async onSearch() {
    this.id = null
    await this.onResetFilter()

    this.getDataPage(1)
  }

  onFilterStatus() {
    this.onPageFilter('statusId', this.searchDetail.bTransactionStatus)
  }

  applyUrlFilters() {
    var querySnapshot = this.route.snapshot.queryParams

    this.id = querySnapshot['id'] ? Utils.toNumber(querySnapshot['id']) : null;
    this.searchDetail.bankAccountId = querySnapshot['bankAccountId'] ? Utils.toNumber(querySnapshot['bankAccountId']) : this.searchDetail.bankAccountId;
    this.searchDetail.bTransactionStatus = querySnapshot['statusId'] ? Utils.toNumber(querySnapshot['statusId']) : this.searchDetail.bTransactionStatus;
    let dateFilterParam = querySnapshot['dateFilter'] ? JSON.parse(querySnapshot['dateFilter']) : {} as DateTimeSelector;

    if (dateFilterParam.dateType) {
      this.searchWithDateTime = dateFilterParam
      this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate)
      this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate)
      this.defaultDateFilterType = this.searchWithDateTime.dateType
    }
  }

  async onResetFilter() {
    this.searchDetail.bankAccountId = OPTION_ALL
    this.searchDetail.bTransactionStatus = OPTION_ALL

    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL
    } as DateTimeSelector;

    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.resetQueryParams(['id', 'bankAccountId', 'statusId', 'dateFilter'])
  }
  getMessageTooltipChiChuyenDoi() {
    return "Chi chuyển đổi thực hiện các chức năng bao gồm:"
      + "\n+ Bán ngoại tệ (Chi ngoại tệ & nhận tiền default)"
      + "\n+ Mua ngoại tệ (Chi tiền default & nhận về ngoại tệ)"
      + "\n+ Gửi tiết kiệm (Chi tiền default & nhận tiền default)"
      + "\n+ Chuyển tiền nội bộ (Chuyển tiền giữa các tài khoản ngân hàng của công ty mà khác tiền tệ)"
      + "\nChú ý: Chọn các biến động số dư âm và dương tương ứng";
  }
}

export class BTransactionPagedRequestDto extends PagedRequestDto {
  filterMoneyParam: IFilterMoneyParam;
  filterDateTimeParam: IFilterDateTimeParam;
}
export interface IFilterMoneyParam {
  type?: number;
  fromValue?: number;
  toValue?: number;
}

export interface CreateEditBTransactionDto {
  bTransactionId: number;
  bankAccountId: number;
  money: number;
  timeAt: string;
  note: string;
}
export class LinkExpenditureAndBTransDto {
  toBankAccountId: number;
  BTransactionId: number;
  outcomingEntryId: number;
  exchangeRate: number;
}

export enum ESortDirection {
  ASCENDING = 0,
  DESCENDING = 1,
}
export const DEFAULT_VALUES = {
  SORT_FIELD: "timeAt",
};
export class BTransactionAndCheck extends BTransaction {
  check: boolean;
}
