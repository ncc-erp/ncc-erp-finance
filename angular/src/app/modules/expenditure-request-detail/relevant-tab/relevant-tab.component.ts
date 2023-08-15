import { ExpenditureRequestDto } from "./../../expenditure-request/expenditure-request.component";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { TransactionService } from "@app/service/api/transaction.service";
import { InputFilterDto } from "./../../../../shared/filter/filter.component";
import { RequestDetailService } from "@app/service/api/request-detail.service";
import { ActivatedRoute } from "@angular/router";
import { Component, OnInit, Injector } from "@angular/core";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { LinkTransactionComponent } from "../link-transaction/link-transaction.component";
import {
  DetailRequestDto,
  LinkedTransactionDto,
} from "../detail-tab/detail-tab.component";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { CreateMultiTransactionComponent } from "../create-multi-transaction/create-multi-transaction.component";
import { catchError, finalize } from "rxjs/operators";
import { CreateEditTransactionComponent } from "@app/modules/banking-transaction/create-edit-transaction/create-edit-transaction.component";
import { CreateEditTransitionComponent } from "@app/modules/work-flow/create-edit-transition/create-edit-transition.component";
import { ExpenditureRequestService } from "@app/service/api/expenditure-request.service";
import { AppConsts } from "@shared/AppConsts";
@Component({
  selector: "app-relevant-tab",
  templateUrl: "./relevant-tab.component.html",
  styleUrls: ["./relevant-tab.component.css"],
})
export class RelevantTabComponent
  extends PagedListingComponentBase<any>
  implements OnInit
{
  public readonly BANK_FILTER_CONFIG: InputFilterDto[] = [
    {
      propertyName: "fromBank",
      displayName: "From Bank Account",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "toBank",
      displayName: "To Bank Account",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "fromValue",
      displayName: "From Value",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
    {
      propertyName: "toValue",
      displayName: "To Value",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
  ];
  transactionDetailList: any;
  public totalMoney: string;
  requestId: any;
  expenditureRequest: any = {};
  detailList: any = {};

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    let detailRequest = new DetailRequestDto();
    detailRequest.outcomingEntryId = this.requestId;
    detailRequest.param = request;
    this.requestDetailService
      .getTransactionDetail(detailRequest)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((data) => {
        this.transactionDetailList = data.result.items.map((item) => {
          item.name = `#${item.transactionId} ${item.name}`;
          return item;
        });
        this.totalMoney = data.result.totalMoneyString;
        this.showPaging(data["result"], pageNumber);
      });
  }
  protected delete(entity: any): void {}

  constructor(
    public dialog: MatDialog,
    private route: ActivatedRoute,
    private requestService: ExpenditureRequestService,
    private requestDetailService: RequestDetailService,
    injector: Injector,
    private transactionService: TransactionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.requestId = this.route.snapshot.queryParamMap.get("id");
    this.getRequestById();
    this.getAllDetail();
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.refresh();
      }));
  }

  linkToTransaction() {
    this.dialog.open(LinkTransactionComponent, {
      data: this.expenditureRequest,
      width: "900px",
      disableClose: true,
    });
  }
  getAllDetail() {
    this.requestDetailService
      .getAllDetail(this.requestId)
      .subscribe((data) => (this.detailList = data.result));
  }
  getRequestById() {
    this.requestService
      .getById(this.requestId)
      .subscribe((data) => (this.expenditureRequest = data.result));
  }
  deleteLinkedTransaction(item) {
    let linkedTransaction = {
      bankTransactionId: item.transactionId,
      outcomingEntryId: item.outcomingEntryId,
    } as LinkedTransactionDto;
    abp.message.confirm("Delete this link" + "?", "", (result: boolean) => {
      if (result) {
        this.requestDetailService
          .deleteLinkedTransaction(linkedTransaction)
          .pipe(catchError(this.requestDetailService.handleError))
          .subscribe(() => {
            abp.notify.success("Deleted");
            this.refresh();
          });
      }
    });
  }

  // addMultipleTransaction() {
  //   this.dialog.open(CreateMultiTransactionComponent, {
  //     data: this.requestId,
  //     width: "700px",
  //     disableClose: true,
  //   });
  // }
  saveRequest() {
    this.requestService
      .update(this.expenditureRequest)
      .pipe(catchError(this.requestService.handleError))
      .subscribe((res) => {
        abp.notify.success("edited request ");
        this.reloadComponent();
      });
  }
  cancelEdit() {
    this.reloadComponent();
  }
  createTransaction() {
    this.showTransactionDialog("create");
  }
  showTransactionDialog(command) {
    this.dialog.open(CreateEditTransactionComponent, {
      data: {
        item: {},
        command: command,
        target: "detailRequest",
        outcomingEntryId: this.requestId,
      },
      width: "700px",
      disableClose: true,
    });
  }
  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["app/requestDetail"], {
        queryParams: {
          id: this.requestId,
          index: 3,
        },
      });
    });
  }
  getUrlLinkTransaction(id) {
    return "app/btransaction?id=" + id;
  }

  getUrlBankingTransaction(id: number) {
    return `app/bank-transaction`;
  }

  isShowRelevantBankTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction);
  }
  isShowLinkToTransactionBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans);
  }
  isShowDeleteLinkBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans);
  }
}
