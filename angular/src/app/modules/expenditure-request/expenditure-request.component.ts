import { MatMenuTrigger } from "@angular/material/menu";
import { ExpenditureService } from "./../../service/api/expenditure.service";
import { DropDownDataDto } from "./../../../shared/filter/filter.component";
import { ActivatedRoute } from "@angular/router";
import { StatusDto } from "./../status/status.component";
import { WorkFlowStatusService } from "./../../service/api/work-flow-status.service";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { MatDialog } from "@angular/material/dialog";
import { CreateEditRequestComponent } from "./create-edit-request/create-edit-request.component";
import { CheckWarningCreateRequestComponent } from "./check-warning-create-request/check-warning-create-request.component";
import { ExpenditureRequestService } from "./../../service/api/expenditure-request.service";
import { finalize, catchError } from "rxjs/operators";
import {
  FilterDto,
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { Component, OnInit, Injector, ViewChildren, ViewChild, ElementRef } from "@angular/core";
import { InputFilterDto } from "@shared/filter/filter.component";
import { CreateEditTransactionComponent } from "../banking-transaction/create-edit-transaction/create-edit-transaction.component";
import { DatePipe, DecimalPipe, Time } from "@angular/common";
import * as FileSaver from "file-saver";
import { StatusHistory } from "../expenditure-request-detail/main-tab/main-tab.component";
import { RequestChangeDialogComponent } from "../expenditure-request-detail/request-change-dialog/request-change-dialog.component";
import { ValueAndNameModel } from "@app/service/model/common-DTO";
import { BranchService } from "@app/service/api/branch.service";
import { BranchDto } from "../branch/branch.component";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { AppConsts } from "@shared/AppConsts";
import { EditReportDateComponent } from "./edit-report-date/edit-report-date.component";
import { DateSelectorEnum, TypeFilterTypeOptions } from "@shared/AppEnums";
import * as moment from "moment";
import { DateFormat, DateTimeSelector } from "@shared/date-selector/date-selector.component";
import { EditOutcomingTypeComponent } from "./edit-outcoming-type/edit-outcoming-type.component";
import { ApiPagingResponse } from "@app/service/model/api-response.model";
import { UtilitiesService } from "@app/service/api/new-versions/utilities.service";
import { forEach } from "lodash-es";
import { CloneRequestComponent } from "./clone-request/clone-request.component";
import { Utils } from "@app/service/helpers/utils";
import { UpdateBranchComponent } from "../expenditure-request-detail/main-tab/update-branch/update-branch.component";
import { TreeInOutTypeOption } from "@shared/components/tree-in-out-type/tree-in-out-type.component";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";

@Component({
  selector: "app-expenditure-request",
  templateUrl: "./expenditure-request.component.html",
  styleUrls: ["./expenditure-request.component.css"],
  providers: [DecimalPipe],
})
export class ExpenditureRequestComponent
  extends PagedListingComponentBase<ExpenditureRequestDto>
  implements OnInit {
  @ViewChildren(MatMenuTrigger) trigger: any;
  @ViewChild("inputSearchOutcoming") inputSearchOutcoming: ElementRef;

  Finance_OutcomingEntry_Create =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_Create;
  Finance_OutcomingEntry_Delete =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_Delete;
  Finance_OutcomingEntry_ViewAll =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_View;
  Finance_OutcomingEntry_ViewDetail =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail;
  Finance_OutcomingEntry_ChangeStatus =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ChangeStatus;
  Finance_OutcomingEntry_ExportExcel =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ExportExcel;
  Finance_OutcomingEntry_ExportPdf =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ExportPdf;
  Finance_OutcomingEntry_UpdateReportDate =
    PERMISSIONS_CONSTANT.Finance_OutcomingEntry_UpdateReportDate;
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.financeManagement;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.financeManagement;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.expenditureRequest;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.expenditureRequest;
    fileParam: DropDownDataDto[] = [
    { displayName: "No File Yet", value: 0 },
    { displayName: "Not Yet Confirmed", value: 1 },
    { displayName: "Confirmed", value: 2 },
  ];

  requestList: OutcomingEntryDto[];
  showHeader = false;
  tableHeader = [
    { name: "Check All", value: true, fieldName: "checkAll" },
    { name: "STT", value: true, fieldName: "stt" },
    { name: "Name", value: true, fieldName: "name" },
    { name: "Ngày báo cáo", value: true, fieldName: "reportDate" },
    { name: "Chi nhánh", value: true, fieldName: "branchName" },
    { name: "Tổng tiền", value: true, fieldName: "value" },
    { name: "Loại yêu cầu", value: true, fieldName: "outcomingEntryTypeCode" },
    { name: "Lịch sử", value: true, fieldName: "history" },
    { name: "Payment Code/File/GDNH", value: true, fieldName: "paymentCode" },
    { name: "Trạng thái", value: true, fieldName: "workflowStatusCode" },
    { name: "Created At", value: true, fieldName: "createAt" },
    { name: "Updated At", value: true, fieldName: "updatedAt" },
  ];
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
      propertyName: "outcomingEntryTypeCode",
      displayName: "filterExpenditureRequest.outcomingType",
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
      comparisions: [0, 1, 2, 3, 4],
    },
    {
      propertyName: "isAcceptFile",
      displayName: "File",
      filterType: 3,
      comparisions: [0],
      dropdownData: this.fileParam,
    },
    {
      propertyName: "createdAt",
      displayName: "filterExpenditureRequest.createdAt",
      filterType: 1,
      comparisions: [0, 1, 2, 3, 4],
    },
    {
      propertyName: "sendTime",
      displayName: "filterExpenditureRequest.sendTime",
      filterType: 1,
      comparisions: [0, 1, 2, 3, 4],
    },
    {
      propertyName: "approveTime",
      displayName: "filterExpenditureRequest.approveTime",
      filterType: 1,
      comparisions: [0, 1, 2, 3, 4],
    },
    {
      propertyName: "executeTime",
      displayName: "filterExpenditureRequest.executeTime",
      filterType: 1,
      comparisions: [0, 1, 2, 3, 4],
    },
  ];

  searchWithDateTime = {} as DateTimeSelector;
  defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
  totalOutcomingValue: any;
  statusList: StatusDto[];
  selectedStatus: string = "";
  accreditation: boolean = false;
  sortDrirect: number = -1;
  createDate: string = "";
  totalValue: number = 0;
  requestParam: GetAllPagingOutComingEntryDto =
    new GetAllPagingOutComingEntryDto();
  requesterOptions: ValueAndNameModel[];
  tempRequesterOptions: ValueAndNameModel[];
  getTotalCurrencyOutcomingEntryDto: GetTotalCurrencyOutcomingEntryDto[];
  selectedRequester: number[];
  searchRequester: string = "";
  listCurrency: ValueAndNameModel[] = [];
  branchOptions: BranchDto[];
  tempBranchOptions: BranchDto[];
  selectedBranch: number[];
  searchBranch: string = "";
  searchMoney: number;
  totalMoney: number;
  treeInOutTypeOption = { isShowAll: false, type: TypeFilterTypeOptions.OUTCOMING_ENTRY_TYPE } as TreeInOutTypeOption;

  searchId: number;
  selectedStatusYCTD: string;
  YCTDStatusOptions: GetWorkflowStatusDto[];
  historyExtend: boolean = false;
  TABLE_NAME: "outcomeFilter";
  treeOutcomingEntries: TreeOutcomingEntries[] = [];
  tmpTreeOutcomingEntries: TreeOutcomingEntries[] = [];
  outcomingEntryTypeId: number = OPTION_ALL
  searchOutcoming: string = ""
  expenseType: number = OPTION_ALL
  selectedCurrencyId?: number = OPTION_ALL;
  // outcomingTypeList=[]
  protected list(
    request: GetAllPagingOutComingEntryDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.requestParam = request;
    let filterItems: FilterDto[] = [];
    if (this.selectedCurrencyId != OPTION_ALL) {
      filterItems.push({
        comparision: 0,
        propertyName: "currencyId",
        value: this.selectedCurrencyId
      })
    }

    request.sort = this.createDate;
    request.sortDirection = this.sortDrirect;
    request.tempStatusCode = this.selectedStatusYCTD;
    request.outComingStatusCode = this.selectedStatus;
    request.accreditation = this.accreditation;
    request.money = this.searchMoney ? this.searchMoney : undefined;
    request.branchs = this.selectedBranch;
    request.requesters = this.selectedRequester;
    request.id = this.searchId;
    if (this.outcomingEntryTypeId != OPTION_ALL) {
      request.outComingEntryType = this.outcomingEntryTypeId
    }

    if (this.expenseType != OPTION_ALL) {
      request.expenseType = this.expenseType
    }

    if (this.searchWithDateTime.dateType !== DateSelectorEnum.ALL) {
      request.filterDateTimeParam = {
        dateTimeType: 1,
        fromDate: moment(this.searchWithDateTime.fromDate).format(
          DateFormat.YYYY_MM_DD
        ),
        toDate: moment(this.searchWithDateTime.toDate).format(
          DateFormat.YYYY_MM_DD
        ),
      };
    }

    request.filterItems = filterItems;
    this.service
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((response) => {
        this.getTotalCurrencyOutcomingEntryDto = response.result.totalCurrencies;
        this.requestList = response.result.resultPaging.items;
        this.showPaging(response.result.resultPaging, pageNumber);
        this.isTableLoading = false;
      });
    // this.GetTotalValueOutcomingEntry(request)

    // this.service
    //   .getTotalValueOutcomingEntry(request)
    //   .pipe(
    //     finalize(() => {
    //       finishedCallback();
    //     })
    //   )
    //   .subscribe((data) => {
    //     this.totalOutcomingValue = data.result;
    //     this.totalValue = this.totalOutcomingValue.reduce((sum, item) => {
    //       return (sum += item.totalValueToCurrencyDefault);
    //     }, 0);
    //   });
    this.updateBreadCrumb()
  }
  
  onRefreshCurrentPage(){
    this.OnResetSearch()
    this.onResetFilter()
    this.refresh();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }

  protected delete(entity: ExpenditureRequestDto): void {
    // throw new Error('Method not implemented.');
  }

  constructor(
    injector: Injector,
    private service: ExpenditureRequestService,
    private dialog: MatDialog,
    private statusService: WorkFlowStatusService,
    private route: ActivatedRoute,
    private _decimalPipe: DecimalPipe,
    public branchService: BranchService,
    public _utilities: UtilitiesService,
    private commonService: CommonService,
    private outcomingEntryService: ExpenditureService
  ) {
    super(injector);
    const status = this.route.snapshot.queryParamMap.get("status");
    const id = this.route.snapshot.queryParamMap.get("id");
    this.searchWithDateTime.dateType = DateSelectorEnum.ALL
    this.applyUrlFilters()
    const statusRequestChange = this.route.snapshot.queryParamMap.get(
      "statusRequestChange"
    );
    this.hasPram = status || id || statusRequestChange;
    if (this.hasPram) {
      this.selectedStatus = status ? status : undefined;
      this.selectedStatusYCTD = statusRequestChange
        ? statusRequestChange
        : undefined;
      this.searchId = id ? Number(id) : undefined;
    }
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.refresh();
      })
    );
  }
  turnOffPermission = false;
  iconSort: string = "";
  iconCondition: string = "";
  sortDate(data) {
    if (this.iconCondition !== data) {
      this.sortDrirect = -1;
    }
    this.iconCondition = data;
    this.createDate = data;
    this.sortDrirect++;
    if (this.sortDrirect > 1) {
      this.createDate = "";
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
  turnOff(): void {
    if (this.turnOffPermission) {
      this.showHeader = false;
    }
    this.turnOffPermission = true;
  }
  getAllCurrency() {
    this.commonService.getAllCurrency().subscribe((data) => {
      this.listCurrency = data.result;
      const itemAll = { value: OPTION_ALL, name: "All" };
      this.listCurrency.unshift(itemAll);
    });
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

    localStorage.setItem(this.TABLE_NAME, JSON.stringify(this.tableHeader));
  }
  setParamToUrl() {
    this.router.navigate([], {
      queryParams: {
        status: this.selectedStatus,
        id: this.searchId,
        statusRequestChange: this.selectedStatusYCTD,
      },
      queryParamsHandling: "merge",
    });
  }
  hasPram: string = "";
  ngOnInit(): void {
    this.sortDrirect = -1;
    this.refresh();
    this.getStatusForSearch();
    this.getAllRequester();
    this.getAllBranch();
    this.getTempOutComingEntryStatusOptions();
    this.tableHeader = this.setTableHeader(this.tableHeader, this.TABLE_NAME);
    this.getTreeOutcomingEntries()
    this.getAllCurrency();
  }
  approveMoney() {
    abp.message.confirm(
      `You want to change status for ${this.totalItems
      } requests with ${this._decimalPipe.transform(
        this.totalValue || 0,
        "1.0"
      )}?`,
      "",
      (result: boolean) => {
        if (result) {
          this.service.CfoTransfer().subscribe((res) => {
            this.refresh();
            abp.notify.success(res.result);
          });
          this.router
            .navigateByUrl("", { skipLocationChange: true })
            .then(() => {
              this.router.navigate(["/app/expenditure-request"], {
                queryParams: {
                  status: "TRANSFERED",
                },
              });
            });
        }
      }
    );
  }
  exportPdf(id) {
    this.service.exportPdf(id).subscribe((data) => {
      const wnd = window.open("about:blank", "", "_blank");
      wnd.document.write(data.result.html);
    });
  }
  // GetTotalValueOutcomingEntry(request: PagedRequestDto) {

  //   this.service.GetTotalValueOutcomingEntry(request, this.selectedStatus).subscribe(data => {
  //     this.totalOutcomingValue = data.result
  //   })

  // }
  FinishRequest() {
    abp.message.confirm(
      `You want to change status for ${this.totalItems
      } requests with ${this._decimalPipe.transform(
        this.totalValue || 0,
        "1.0"
      )} ${this.defaultCurrencyCode} and create a new bank transition?`,
      "",
      (result: boolean) => {
        if (result) {
          this.dialog.open(CreateEditTransactionComponent, {
            data: {
              command: "create",
              target: "transferMoney",
            },
            width: "700px",
            disableClose: true,
          });
        }
      }
    );
  }
  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/outcomingType"]);
    });
  }
  createRequest() {
    this.showDialog("create", {});
  }
  cloneRequest(outcomeEntry: OutcomingEntryDto) {
    this.dialog.open(CloneRequestComponent, {
      width: "600px",
      data: outcomeEntry
    })
  }
  editRequest(request: ExpenditureRequestDto) {
    this.showDialog("edit", request);
    this.trigger._results.forEach((item) => item.closeMenu());
  }
  getStatusForSearch() {
    this.statusService
      .GetAllForDropDownAndNotEqualsEnd()
      .subscribe((data) => {
        this.statusList = data.result 
        this.statusList.push({name: 'Chờ CEO duyệt cả YCTĐ', code: 'PENDINGCEO_OR_YCTDPENDINGCEO', id: 2})
      });
  }
  getTempOutComingEntryStatusOptions() {
    this.statusService
      .GetStatusForOutcomeFilter()
      .subscribe((data) => (this.YCTDStatusOptions = data.result));
  }

  getAllRequester() {
    this.service.getAllRequester().subscribe((data) => {
      this.tempRequesterOptions = this.requesterOptions = data.result;
    });
  }
  getAllBranch() {
    this.branchService
      .GetAllForDropdown()
      .subscribe(
        (data) => (this.tempBranchOptions = this.branchOptions = data.result)
      );
  }
  filterAccreditation() {
    this.accreditation = !this.accreditation;
    this.onPageFilter('accreditation', this.accreditation)
  }
  changeStatus(requestId: number, statusId: number) {
    this.isTableLoading = true;
    const statusBody: CheckChangeStatusDto = {
      outcomingEntryId: requestId,
      statusTransitionId: statusId,
    };
    this.service.changeStatus(statusBody).subscribe(
      () => {
        abp.notify.success("Change status successful ");
        this.refresh();
      },
      (err) => {
        this.isTableLoading = false;
        abp.notify.error(err.error.error.message);
      }
    );
  }

  checkChangeStatus(checkChangeStatusBody: CheckChangeStatusDto): void {
    this.service
      .checkChangeStatus(checkChangeStatusBody)
      .subscribe((result) => {
        if (!(result.result.isAllowed && result.result.isChangeGeneral)) {
          this.changeStatusAfterCheck(checkChangeStatusBody);
        } else {
          abp.notify.error(result.result.message);
        }
      });
  }

  showDialog(command: String, item: any): void {
    let request = {} as ExpenditureRequestDto;
    if (command == "edit") {
      request = {
        outcomingEntryTypeId: item.outcomingEntryTypeId,
        name: item.name,
        requester: item.requester,
        branchId: item.branchId,
        branchName: item.branchName,
        accountId: item.accountId,
        accountName: item.accountName,
        value: item.value,
        workflowStatusId: item.workflowStatusId,
        workflowStatusName: item.workflowStatusName,
        workflowStatusCode: item.workflowStatusCode,
        outcomingEntryTypeCode: item.outcomingEntryTypeCode,
        currencyId: item.currencyId,
        currencyName: item.currencyName,
        supplierId: item.supplierId,
        id: item.id,
        action: item.action,
        requestInBankTransaction: item.requestInBankTransaction,
        paymentCode: item.paymentCode,
        isAcceptFile: item.isAcceptFile,
        accreditation: item.accreditation,
      };
    }

    const createEditRequestComponent = this.dialog.open(CreateEditRequestComponent, {
      data: {
        item: request,
        command: command,
      },
      width: "700px",
      disableClose: true,
    });

    createEditRequestComponent.afterClosed().subscribe(()=> {
      this.refresh();
    })

  }
  deleteRequest(item: ExpenditureRequestDto): void {
    this.trigger._results.forEach((item) => item.closeMenu());
    abp.message.confirm(
      "Delete this request '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.service
            .delete(item.id)
            .pipe(catchError(this.service.handleError))
            .subscribe(() => {
              abp.notify.success("Deleted request: " + item.name);
              this.refresh();
            });
        }
      }
    );
  }

  handleApproveRequestChange(item: OutcomingEntryDto) {
    abp.message.confirm(
      `Approve request change ${item.name}?`,
      "",
      (result: boolean) => {
        if (result) {
          this.service.approveTemp(item.tempOutcomingEntryId).subscribe(() => {
            abp.notify.success(
              `Approve request change ${item.name} successfully`
            );
            this.refresh();
          });
        }
      }
    );
  }

  handleRejectRequestChange(item: OutcomingEntryDto) {
    abp.message.confirm(
      `Reject request change ${item.name}?`,
      "",
      (result: boolean) => {
        if (result) {
          this.service.rejectTemp(item.tempOutcomingEntryId).subscribe(() => {
            abp.notify.success(
              `Reject request change ${item.name} successfully`
            );
            this.refresh();
          });
        }
      }
    );
  }

  handleUpdateReportDate(request: OutcomingEntryDto) {
    let item = { ...request }
    const dialogRef = this.dialog.open(EditReportDateComponent, {
      data: item,
      width: "400px",
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(rs => {
      if (rs) {
        this.refresh()
      }
    })
  }

  showDetail(id: any) {
    if (this.permission.isGranted(this.Finance_OutcomingEntry_ViewDetail)) {
      this.router.navigate(["app/requestDetail/main"], {
        queryParams: {
          id: id,
        },
      });
    }
  }
  changeStatusAfterCheck(statusBody: CheckChangeStatusDto) {
    this.service.changeStatus(statusBody).subscribe(
      () => {
        abp.notify.success("Change status successful ");
        this.refresh();
      },
      (err) => {
        abp.notify.error(err);
      }
    );
  }
  downloadFile() {
    this.service
      .exportExcel(this.requestParam, this.selectedStatus, this.accreditation)
      .subscribe((data) => {
        const file = new Blob([this.convertFile(atob(data.result))], {
          type: "application/vnd.ms-excel;charset=utf-8",
        });
        FileSaver.saveAs(file, `Request chi.xlsx`);
      });
  }
  convertFile(fileData) {
    var buf = new ArrayBuffer(fileData.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != fileData.length; ++i)
      view[i] = fileData.charCodeAt(i) & 0xff;
    return buf;
  }
  getUrlLinkTransactionMain(id) {
    return "app/requestDetail/main?id=" + id;
  }
  viewDetailHistory(
    index: number,
    statusHistorys: StatusHistory[],
    tempId: number
  ): void {
    const currentRequestChange = {
      value: statusHistorys[index].value,
      dateTime: statusHistorys[index].creationTime,
      createdBy: statusHistorys[index].creationUser,
      statusName: statusHistorys[index].workflowStatusName,
      statusCode: statusHistorys[index].workflowStatusCode,
      isOriginal: statusHistorys[index].isRoot,
      currencyName:statusHistorys[index].currencyName,
    } as HistoryRequestChangeInfomation;

    const previousRequestChange = {
      value: statusHistorys[index + 1].value,
      dateTime: statusHistorys[index + 1].creationTime,
      createdBy: statusHistorys[index + 1].creationUser,
      statusName: statusHistorys[index + 1].workflowStatusName,
      statusCode: statusHistorys[index + 1].workflowStatusCode,
      isOriginal: statusHistorys[index + 1].isRoot,
      currencyName: statusHistorys[index + 1].currencyName,
    } as HistoryRequestChangeInfomation;
    this.openHistory(tempId, currentRequestChange, previousRequestChange);
  }
  openHistory(
    tempId: number,
    currentRequestChange: HistoryRequestChangeInfomation,
    previousRequestChange: HistoryRequestChangeInfomation,
    tab = 0
  ) {
    const requestChangeDialog = this.dialog.open(RequestChangeDialogComponent, {
      data: {
        isViewHistory: true,
        tempId,
        currentRequestChange,
        previousRequestChange,
        selectedIndex: tab,
      } as DataRequestChangeDialog,
      width: "1900px",
      disableClose: true,
    });

    requestChangeDialog.afterClosed().subscribe((res) => {
      this.reloadComponent();
    });
  }
  //   const requestChangeDialog = this.dialog.open(RequestChangeDialogComponent, {
  //     data: {isViewHistory: true, tempId, currentRequestChange, previousRequestChange} as DataRequestChangeDialog,
  //     width: "1900px",
  //     disableClose: true,
  //   });

  //   requestChangeDialog.afterClosed().subscribe(res => {
  //     //TODO::
  //   });
  // }
  searchRequesterChange() {
    this.requesterOptions = this.tempRequesterOptions.filter((s) =>
      s.name
        .toLocaleLowerCase()
        .includes(this.searchRequester.toLocaleLowerCase())
    );
  }
  searchBranchChange() {
    this.branchOptions = this.tempBranchOptions.filter((s) =>
      s.name.toLocaleLowerCase().includes(this.searchBranch.toLocaleLowerCase())
    );
  }
  selectionRequesterOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.searchRequesterChange();
      return;
    }
    this.requesterOptions = this.tempRequesterOptions;
  }
  selectionBranchOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.searchBranchChange();
      return;
    }
    this.branchOptions = this.tempBranchOptions;
  }

  onDateChange(event) {
    this.searchWithDateTime = event;
    this.defaultDateFilterType = event.dateType;
    this.searchWithDateTime.dateType = event.dateType
    let cloneDate = { ...event }
    cloneDate.fromDate = moment(cloneDate.fromDate).format("YYYY-MM-DD")
    cloneDate.toDate = moment(cloneDate.toDate).format("YYYY-MM-DD")

    this.onPageFilter('dateFilter', cloneDate)
  }

  getTreeOutcomingEntries() {
    this.commonService.getOptionTreeOutcomingEntriesByUser().subscribe((data) => {
      data.result.forEach((item) =>
        this.filterData(item as TreeOutcomingEntries, 1)
      );



      this.tmpTreeOutcomingEntries = [...this.treeOutcomingEntries];
    },);
  }

  filterData(data: TreeOutcomingEntries, level: number) {
    data.paddingLevel = "";
    for (let i = 1; i < level; i++) {
      data.paddingLevel += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    }
    this.treeOutcomingEntries.push(data);
    if (data.children.length > 0) {
      data.children.forEach((item) => {
        this.filterData(item, level + 1);
      });
    }
  }


  selectionOutcomingOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.inputSearchOutcoming.nativeElement.focus();
      return;
    }
    this.treeOutcomingEntries = this.tmpTreeOutcomingEntries;
  }

  onCancelFilterOutcomeType() {
    this.outcomingEntryTypeId = OPTION_ALL
    this.refresh()
    this.setFilterToUrl('outcomingEntryTypeId', this.outcomingEntryTypeId)
  }

  onEditOutcomingType(data: OutcomingEntryDto) {
    let item = { ...data }
    let ref = this.dialog.open(EditOutcomingTypeComponent, {
      width: "600px",
      data: item
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.refresh()
      }
    })
  }
  openRequestChange(request: OutcomingEntryDto) {
    //TODO: show dialog
    const requestChangeDialog = this.dialog.open(RequestChangeDialogComponent, {
      width: "1900px",
      data: { expenditureRequest: { id: request.id }, tempId: request.tempOutcomingEntryId, isViewHistory: false, selectedIndex: 0 } as DataRequestChangeDialog,
      disableClose: true
    });
    requestChangeDialog.afterClosed().subscribe((result) => {
      this.refresh();
    })

  }

  onFilterCurrency() {
    this.onPageFilter('currencyId', this.selectedCurrencyId)
  }

  onFillterOutcomeType() {
    this.onPageFilter('outcomingEntryTypeId', this.outcomingEntryTypeId)
  }

  onFilterExpenseType() {
    this.onPageFilter('expenseType', this.expenseType)
  }

  async onFilterId() {
    await this.onResetFilter()
    this.searchMoney = null
    this.searchText = ""

    this.onPageFilter('searchId', this.searchId)
  }

  onFilterRequester() {
    if (this.selectedRequester.length === 0) {
      this.onPageFilter('requesters', OPTION_ALL)
    }
    else {
      this.onPageFilter('requesters', this.selectedRequester)
    }
  }

  onFilterBranch() {
    if (this.selectedBranch.length === 0) {
      this.onPageFilter('branchs', OPTION_ALL)
    }
    else {
      this.onPageFilter('branchs', this.selectedBranch)
    }
  }

  async onFilterMoney() {
    await this.onResetFilter()
    this.searchId = null
    this.searchText = ""

    this.onPageFilter('money', this.searchMoney)
  }

  async onSearch(){
    this.searchId = null
    this.searchMoney = null
    await this.onResetFilter()

    this.getDataPage(1)
  }

  onFilterStatus() {
    this.onPageFilter('status', this.selectedStatus)
  }

  onFilterYCTDStatus() {
    this.onPageFilter('statusRequestChange', this.selectedStatusYCTD)
  }

  applyUrlFilters() {
    var querySnapshot = this.route.snapshot.queryParams

    this.selectedCurrencyId = querySnapshot['currencyId'] ? Utils.toNumber(querySnapshot['currencyId']) : OPTION_ALL;
    this.outcomingEntryTypeId = querySnapshot['outcomingEntryTypeId'] ? Utils.toNumber(querySnapshot['outcomingEntryTypeId']) : OPTION_ALL;
    this.expenseType = querySnapshot['expenseType'] ? Utils.toNumber(querySnapshot['expenseType']) : OPTION_ALL;
    this.searchId = querySnapshot['searchId'] ? Utils.toNumber(querySnapshot['searchId']) : null;
    this.searchMoney = querySnapshot['money'] ? Utils.toNumber(querySnapshot['money']) : null;
    this.selectedStatus = querySnapshot['status'] ? querySnapshot['status'] : "";
    this.selectedStatusYCTD = querySnapshot['statusRequestChange'] ? querySnapshot['statusRequestChange'] : "";
    this.accreditation = querySnapshot['accreditation'] ? querySnapshot['accreditation'] === 'true' : false;
    this.selectedBranch = querySnapshot['branchs'] ? JSON.parse(querySnapshot['branchs']) : [];
    this.selectedRequester = querySnapshot['requesters'] ? JSON.parse(querySnapshot['requesters']) : [];
    let dateFilterParam = querySnapshot['dateFilter'] ? JSON.parse(querySnapshot['dateFilter']) : {} as DateTimeSelector;

    if (dateFilterParam.dateType) {
      this.searchWithDateTime = dateFilterParam
      this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate)
      this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate)
      this.defaultDateFilterType = this.searchWithDateTime.dateType
    }
  }

  async OnResetSearch() {
    this.searchId = null;
    this.searchMoney = null
    this.searchText = ""
  }

  async onResetFilter() {
    this.selectedCurrencyId = OPTION_ALL
    this.outcomingEntryTypeId = OPTION_ALL
    this.expenseType = OPTION_ALL
    this.selectedStatus = ""
    this.selectedStatusYCTD = ""
    this.accreditation = false
    this.selectedBranch = []
    this.selectedRequester = []

    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL
    } as DateTimeSelector;

    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.resetQueryParams(['currencyId', 'outcomingEntryTypeId', 'expenseType', 'searchId', 'money', 'status', 'statusRequestChange', 'accreditation', 'branchs', 'requesters', 'dateFilter'])
  }
  onUpdateBranch(request){
    var dia = this.dialog.open(UpdateBranchComponent, {
      data: {
        requestId: request.id,
        oldBranchId: request.branchId
      },
      width: "700px"
    })
    dia.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }
  isAllowToRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail);
  }
  isShowCloneBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_Clone);
  }
  isShowUpdateRequestTypeBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_UpdateRequestType);
  }
  isShowViewYCTD(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ViewYCTD)
  }
  isShowMenuActions(){
    return this.isShowCloneBtn()
    || this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_Delete)
    || this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ChangeStatus)
    || this.isShowViewYCTD()
    || this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_AcceptYCTD)
    || this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_RejectYCTD)

  }
  isShowUpdateBranchBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcmingEntry_UpdateBranch);
  }
}
export class ExpenditureRequestDto {
  outcomingEntryTypeId: number;
  name: string;
  requester: string;
  branchId: number;
  branchName: string;
  action: ActionDto[];
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
  requestInBankTransaction: number;
  paymentCode: string;
  isAcceptFile: number;
  accreditation: boolean;
}
export class NewExpenditureRequestDto extends ExpenditureRequestDto {
  isShowButtonApproveRequestChange: boolean;
  isShowButtonRejectRequestChange: boolean;
  isShowButtonRequestChange: boolean;
  isShowButtonSendRequestChange: boolean;
  isShowButtonViewRequestChange: boolean;
  tempOutcomingEntryId: number;
  reportDate: string
}
export class TempOutcomingEntryDto extends NewExpenditureRequestDto {
  reason: string;
}

export class DataRequestChangeDialog {
  tempId?: number;
  isViewHistory: boolean;
  currentRequestChange?: HistoryRequestChangeInfomation;
  previousRequestChange?: HistoryRequestChangeInfomation;
  expenditureRequest: NewExpenditureRequestDto;
  selectedIndex: number;
}

export class HistoryRequestChangeInfomation {
  statusName: string;
  statusCode: string;
  value: string;
  dateTime: Date;
  createdBy: string;
  isOriginal: boolean;
  currencyName: string;
}

export class RequestChangeOutcomingEntryInfoDto {
  rootOutcomingEntry: NewExpenditureRequestDto;
  tempOutcomingEntry: TempOutcomingEntryDto;
}

export class ActionDto {
  fromStatusId: number;
  name: string;
  statusTransitionId: string;
  toStatusId: number;
  workflowId: number;
}
export class CheckChangeOutcomingEntryStatusDto {
  isAllowed: boolean;
  isChangeGeneral: boolean;
  message: string;
}
export class CheckChangeStatusDto {
  statusTransitionId: number;
  outcomingEntryId: number;
}
export class OutcomingEntryDto {
  id: number;
  outcomingEntryTypeId: number;
  name: string;
  requester: string;
  branchId: number;
  branchName: string;
  accountId: number;
  accountName: string;
  currencyId: number;
  currencyName: string;
  value: number;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  action: ActionDto[];
  outcomingEntryTypeCode: string;
  supplierId: number;
  createdAt: Time;
  sendTime: Time;
  approveTime: Time;
  executeTime: Time;
  isAcceptFile: number;
  paymentCode: string;
  creatorUserId: number;
  requestInBankTransaction: number;
  accreditation: true;
  updatedBy: string;
  updatedTime: Time;
  creationTime: Time;
  creationUserId: number;
  creationUser: string;
  lastModifiedTime: Time;
  lastModifiedUserId: number;
  lastModifiedUser: string;
  statusHistories: StatusHistory[];
  tempOutcomingEntryId: number;
  reportDate?: number;
}
export class GetAllPagingOutComingEntryDto extends PagedRequestDto {
  money: number;
  requesters: number[];
  branchs: number[];
  outComingEntryType: number;
  tempStatusCode: string;
  outComingStatusCode: string;
  accreditation: boolean;
  id: number;
  expenseType: number;
  filterDateTimeParam: {
    dateTimeType: number;
    fromDate: string;
    toDate: string;
  };
}
export class GetWorkflowStatusDto {
  name: string;
  code: string;
}
export interface TreeOutcomingEntries {
  item: OptionOutcomingEntriesDto;
  children: TreeOutcomingEntries[];
  paddingLevel: string;
}
export interface OptionOutcomingEntriesDto {
  name: string;
  id: number;
  parentId: number;
  level: number;
}
export const OPTION_ALL: number = -1
export class GetTotalCurrencyOutcomingEntryDto {
  currencyId: number;
  currencyName: string;
  value: number;
  valueFormat: string;
}
export class ResultGetOutcomingEntryDto {
  resultPaging: {
    totalCount: number;
    items: OutcomingEntryDto[];
  };
  totalCurrencies: GetTotalCurrencyOutcomingEntryDto[];
}

export class cloneOutcomingEntryDto {
  outcomeEntryId: number;
  name: string;
  currencyId: number;
  value: number;
}

export class GetAccountCompanyForDropdownDto extends ValueAndNameModel{
  isDefault: boolean;
}

export class CheckWarningCreateRequestDto{
  items: OutcomingEntryDto[];
}