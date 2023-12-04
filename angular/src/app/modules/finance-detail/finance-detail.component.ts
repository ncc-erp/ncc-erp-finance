import { UpdateImcomeTypeComponent } from './update-imcome-type/update-imcome-type.component';
import { InputFilterDto } from "@shared/filter/filter.component";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { BankAccountService } from "./../../service/api/bank-account.service";
import { MatDialog } from "@angular/material/dialog";
import {
  CreateEditRecordComponent,
  revenueRecordDto,
} from "./create-edit-record/create-edit-record.component";
import {
  IncomingEntryDto,
  RevenueRecordDto,
} from "./../revenue-recording/revenue-recording.component";
import { ActivatedRoute, Router } from "@angular/router";
import { Component, OnInit, Injector } from "@angular/core";
import { RevenueRecordService } from "@app/service/api/revenue-record.service";
import { BankTransactionDto } from "../banking-transaction/banking-transaction.component";
import { TransactionService } from "@app/service/api/transaction.service";
import { finalize, catchError } from "rxjs/operators";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import * as moment from "moment";
import { MAT_DATE_LOCALE } from "@angular/material/core";
import { LinkToExpenditureRequestComponent } from "./link-to-expenditure-request/link-to-expenditure-request.component";
import { OutcomingEntryBankTransactionServiceService } from "../../service/api/outcoming-entry-bank-transaction-service.service";
import { RequestDetailService } from "../../service/api/request-detail.service";
import { Time } from "@angular/common";
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
@Component({
  selector: "app-finance-detail",
  templateUrl: "./finance-detail.component.html",
  styleUrls: ["./finance-detail.component.css"],
})
export class FinanceDetailComponent
  extends PagedListingComponentBase<any>
  implements OnInit
{
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    {
      propertyName: "name",
      displayName: "filterExpenditureRequest.Name",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "requester",
      displayName: "filterExpenditureRequest.Requester",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "branchName",
      displayName: "filterExpenditureRequest.Branch",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "accountName",
      displayName: "filterExpenditureRequest.Account",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "value",
      displayName: "filterExpenditureRequest.Value",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
  ];
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    // throw new Error('Method not implemented.');
  }
  protected delete(entity: any): void {
    // throw new Error('Method not implemented.');
  }
  Finance_BankTransaction_Edit =
    PERMISSIONS_CONSTANT.Finance_BankTransaction_BankTransactionDetail_TabBankTransaction_Edit;
  tabIndex = "0";
  transaction: BankTransactionDto;
  revenueRecordList: IncomingEntryDto[];
  paramId: any;
  readMode = true;
  searchRecord: string = "";
  tempRecordList: IncomingEntryDto[];
  bankAccountList: DetailBankAccountDto[];
  formBankList: DetailBankAccountDto[];
  requestId: number;
  toBankList: DetailBankAccountDto[];
  searchFromBank: string = "";
  searchToBank: string = "";
  fromBankAccountName: string;
  toBankAccountName: string;
  searchList: DetailBankAccountDto[];
  formBankCurrency: string;
  toBankCurrency: string;
  tong: number;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.financeManagement;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.bankTransaction;
  routeUrlThirdLevel = this.APP_CONSTANT.UrlBreadcrumbThirdLevel.revenueRecordDetail;
  queryParams;
  outcomingEntrysByTransaction: expenditureRequestDto[] = [];
  constructor(
    private transactionService: TransactionService,
    private revenueService: RevenueRecordService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private bankAccountService: BankAccountService,
    injector: Injector,
    private outcomingEntryBankTransactionServiceService: OutcomingEntryBankTransactionServiceService,
    private requestDetail: RequestDetailService,
    private translate: TranslateService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.paramId = this.route.snapshot.queryParamMap.get("id");
    this.getOutComingEntryByTransaction(this.paramId);
    if (this.route.snapshot.queryParamMap.get("index") != null) {
      this.tabIndex = this.route.snapshot.queryParamMap.get("index").toString();
    }
    this.getTransition();
    this.getRevenueById();
    this.getBankAccount();
    this.refresh();
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu5").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu5.m5_child3").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title, url: this.routeUrlSecondLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.transaction.name , url: this.routeUrlThirdLevel + (queryParamsString ? '?' + queryParamsString : '') },
    ];
  }
  getTransition() {
    this.transactionService.getById(this.paramId).subscribe((data) => {
      this.transaction = data.result;
      this.route.queryParams.subscribe(params => {
        this.queryParams = new HttpParams({ fromObject: params });
        this.onLangChange();
      });
    });
  }
  getOutComingEntryByTransaction(id: number): void {
    this.outcomingEntryBankTransactionServiceService
      .getAllOutcomingEntryByTransaction(id)
      .subscribe((item) => {
        this.outcomingEntrysByTransaction = item.result;
        this.tong = this.outcomingEntrysByTransaction.reduce((acc, item) => {
          return (acc += item.value);
        }, 0);
      });
  }
  getBTRansactionInfo(transaction){
    return "#"+ transaction.bTransactionId +
    " " + (transaction.bTransactionMoneyNumber > 0 ? "+":"") + transaction.bTransactionMoney + transaction.bTransactionCurrencyName +
    " TK " + transaction.bTransactionBankNumber;
  }
  showBTransaction(bTRansactionid: number) {
    // if (this.permission.isGranted(this.Finance_OutcomingEntry_ViewDetail)) {
    //   this.router.navigate(['app/btransaction'], {
    //     queryParams: {
    //       id: bTRansactionid,
    //     }
    //   })
    // }
    this.router.navigate(['app/btransaction'], {
      queryParams: {
        id: bTRansactionid,
      }
    })
  }

  getUrlLinkBTransaction(id) {
    return "app/btransaction?id=" + id;
  }
  getRevenueById() {
    this.revenueService
      .getAllByTransactionId(this.paramId)
      .subscribe((data) => {
        (this.revenueRecordList = data.result),
          (this.tempRecordList = data.result);
      });
  }
  filterRecordList() {
    this.revenueRecordList = this.tempRecordList.filter(
      (item) =>
        item.name?.toLowerCase().includes(this.searchRecord.toLowerCase()) ||
        item?.accountName
          .toLowerCase()
          .includes(this.searchRecord.toLowerCase()) ||
        item.branchName
          ?.toLowerCase()
          .includes(this.searchRecord.toLowerCase()) ||
        item?.value.toString().includes(this.searchRecord)
    );
  }
  createRecord() {
    this.showDialog("create", {});
  }
  editRecord(revenueRecord: revenueRecordDto) {
    this.showDialog("edit", revenueRecord);
  }
  editTransaction() {
    this.readMode = false;
  }
  setFromBankCurrency(value: string) {
    this.transaction.fromBankAccountCurrency = value;
  }
  setToBankCurrency(value: string) {
    this.transaction.toBankAccountCurrency = value;
  }
  saveTransaction() {
    this.transaction.transactionDate = moment(
      this.transaction.transactionDate
    ).format("MM/DD/YYYY, h:mm:ss");
    this.transactionService
      .updateBankTransaction(this.transaction)
      .pipe(catchError(this.transactionService.handleError))
      .subscribe((res) => {
        abp.notify.success("edited transaction ");
        this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
          this.router.navigate(["/app/detail"], {
            queryParams: {
              id: this.paramId,
              index: 0,
            },
          });
        });
      });
  }
  cancelEdit() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/detail"], {
        queryParams: {
          id: this.paramId,
          index: 0,
        },
      });
    });
  }
  showDialog(command: String, revenue: any): void {
    let record = {} as RevenueRecordDto;
    if (command == "edit") {
      record = {
        incomingEntryTypeId: revenue.incomingEntryTypeId,
        bankTransactionId: revenue.bankTransactionId,
        name: revenue.name,
        status: revenue.status,
        accountId: revenue.accountId,
        accountName: revenue.accountName,
        branchId: revenue.branchId,
        branchName: revenue.branchName,
        value: revenue.value,
        currencyId: revenue.currencyId,
        currencyName: revenue.currencyName,
        id: revenue.id,
      };
    }
    this.dialog.open(CreateEditRecordComponent, {
      data: {
        item: record,
        command: command,
        transactionId: this.paramId,
        transaction: this.transaction,
        fromBankAccountCurrency: this.transaction.fromBankAccountCurrency,
      },
      width: "700px",
    });
  }
  linkToExpenditureRequest() {
    const dialogRef = this.dialog.open(LinkToExpenditureRequestComponent, {
      data: this.requestId,
      width: "80vw",
      height: "90vh",
      disableClose: false,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.getOutComingEntryByTransaction(this.paramId);
      this.getRevenueById();
    });
  }
  deleteLinkToExpenditureRequest(item: expenditureRequestDto): void {
    let linkedTransaction = {
      bankTransactionId: this.paramId,
      outcomingEntryId: item.id,
    };
    abp.message.confirm(
      "Delete link to expenditure request '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.requestDetail
            .deleteLinkedTransaction(linkedTransaction)
            .pipe(catchError(this.revenueService.handleError))
            .subscribe(() => {
              abp.notify.success(
                "Delete link to expenditure request: " + item.name
              );
              this.getOutComingEntryByTransaction(this.paramId);
              this.getRevenueById();
            });
        }
      }
    );
  }
  deleteRecord(item: RevenueRecordDto): void {
    abp.message.confirm(
      "delete revenue record '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.revenueService
            .delete(item.id)
            .pipe(catchError(this.revenueService.handleError))
            .subscribe(() => {
              abp.notify.success("Deleted revenue record: " + item.name);
              this.reloadComponent();
            });
        }
      }
    );
  }

  getBankAccount() {
    this.bankAccountService
      .getAll()
      .subscribe(
        (apiData) => {
          apiData.result = apiData.result.map(item => {
            item.holderName = `${item.holderName}(${item.currencyName}) ${item.bankNumber} [${item.accountTypeCode}]`;

            return item;
          })
          this.bankAccountList = apiData.result;
          this.searchList = apiData.result
          this.formBankList = apiData.result
          this.toBankList = apiData.result
        }
      );
  }

  getBankName() {
    this.bankAccountService.getAll().subscribe((data) => {
      data.result.forEach((item) => {
        if (item.id == this.transaction.fromBankAccountId) {
          this.fromBankAccountName = item.holderName;
        }
        if (item.id == this.transaction.toBankAccountId) {
          this.toBankAccountName = item.holderName;
        }
      });
    });
    this.transaction.fromBankAccountName = this.fromBankAccountName;
    this.transaction.toBankAccountName = this.toBankAccountName;
  }
  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/detail"], {
        queryParams: {
          id: this.paramId,
          index: 1,
        },
      });
    });
  }

  selectionToBankAccountOpenChange(isOpen: boolean){
    if(isOpen){
      this.filterToBank();
    }
    else{
      this.toBankList = this.searchList;
    }
  }
  selectionFromBankAccountOpenChange(isOpen: boolean){
    if(isOpen){
      this.filterFromBank();
    }
    else{
      this.formBankList = this.searchList;
    }
  }
  filterFromBank() {
    this.formBankList = this.searchList.filter((item) =>
    item.holderName.trim().toLowerCase().includes(this.searchFromBank.trim().toLowerCase())
    );
  }
  filterToBank() {
    this.toBankList = this.searchList.filter((item) =>
    item.holderName.trim().toLowerCase().includes(this.searchToBank.trim().toLowerCase())
    );
  }

  onUpdateIncomingEntryType(income: IncomingEntryDto){
    let item = {...income}
    let ref = this.dialog.open(UpdateImcomeTypeComponent, {
      width: "600px",
      data: item
    })
    ref.afterClosed().subscribe(rs =>{
      if(rs){
        this.getRevenueById()
      }
    })
  }

}
export class expenditureRequestDto {
  outcomingEntryTypeId: number;
  name: string;
  requester: string;
  branchId: number;
  branchName: string;
  accountId: number;
  accountName: string;
  value: number;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  outcomingEntryTypeCode: string;
  currencyId: number;
  currencyName: string;
  supplierId: number;
  id: number;
  toBankAccountCurrency: string;
}
export class DetailBankAccountDto {
  holderName: string;
  bankNumber: string;
  bankId: number;
  bankName: string;
  currencyId: number;
  currencyName: string;
  accountId: number;
  accountName: string;
  accountTypeCode: string;
  amount: number;
  baseBalance: number;
  lockedStatus: boolean;
  invoiceBankTransactions: InvoiceBankTransactionDto[];
}
export class InvoiceBankTransactionDto {
  id: number;
  invoiceId: number;
  paymentAmount: number;
}

