import { HttpErrorResponse } from "@angular/common/http";
import {
  Component,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { AccountDto } from "@app/modules/accountant-account/accountant-account.component";
import { BranchDto } from "@app/modules/branch/branch.component";
import {
  NewExpenditureRequestDto,
  RequestChangeOutcomingEntryInfoDto,
  TempOutcomingEntryDto,
} from "@app/modules/expenditure-request/expenditure-request.component";
import { expenditureDto } from "@app/modules/expenditure/expenditure.component";
import { AccountService } from "@app/service/api/account.service";
import { BranchService } from "@app/service/api/branch.service";
import { CurrencyService } from "@app/service/api/currency.service";
import { ExpenditureRequestService } from "@app/service/api/expenditure-request.service";
import { ExpenditureService } from "@app/service/api/expenditure.service";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { ValueAndNameModel } from "@app/service/model/common-DTO";
import { AppComponentBase } from "@shared/app-component-base";
import * as FileSaver from "file-saver";
import { of } from "rxjs";
import { catchError } from "rxjs/operators";
import {
  outcomeFileDto,
  StatusHistory,
  UpdateTempOutcomingEntryDto,
} from "../../main-tab/main-tab.component";
import { StatusInfo } from "../request-change-dialog.component";

@Component({
  selector: "app-request-change-main-tab",
  templateUrl: "./request-change-main-tab.component.html",
  styleUrls: ["./request-change-main-tab.component.css"],
})
export class RequestChangeMainTabComponent
  extends AppComponentBase
  implements OnInit
{
  public state = false;
  showEditor = false;

  requestListDetail: any;
  tempExpenditureRequest: TempOutcomingEntryDto;
  readMode: boolean = true;
  branchList: BranchDto[];
  requestTypeList: expenditureDto[];
  accountList: AccountDto[];
  tempBranchList: BranchDto[];
  tempRequestTypeList: any;
  tempAccountList: any;
  searchAccount: string = "";
  searchBranch: string = "";
  searchRequestType: string = "";
  tabIndex: any = 1;
  searchCurrency: string = "";
  currencyList: ValueAndNameModel[];
  commentContent: string = "";
  statusHistorys: StatusHistory[];
  public outcomingFileList: outcomeFileDto[] = [];
  public isLoading = true;

  @Input() expenditureRequest: NewExpenditureRequestDto;
  @Input() isViewHistory: boolean;
  @Input() tempId?: number;

  @Output() isLoadingRequest = new EventEmitter<boolean>();
  @Output() isEditing = new EventEmitter<boolean>();
  @Output() status = new EventEmitter<StatusInfo>();
  @Output() isClose = new EventEmitter<boolean>();
  @Output() getDifferenceValue = new EventEmitter<number>();

  constructor(
    private requestService: ExpenditureRequestService,
    private branchService: BranchService,
    private accountService: AccountService,
    private currencyService: CurrencyService,
    private injector: Injector,
    private commonService: CommonService,
    private service: ExpenditureRequestService,
    private dialog: MatDialog,
    private expenditureService: ExpenditureService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.setDataComponent();
  }
  private setDataComponent() {
    if (this.isViewHistory) {
      this.getViewHistoryRequestChangeOutcomingEntry();
    } else {
      this.getStatusHistorys();
      this.getRequestChangeOutcomingEntry();
      this.getOutcomeFiles();
    }
    this.getAllBranch();
    this.getAllRequestType();
    this.getAllAccount();
    this.getCurrency();
  }
  getStatusHistorys() {
    this.service
      .getOutcomingEntryStatusHistoryByOutcomingEntryId(
        this.expenditureRequest.id
      )
      .subscribe((result) => {
        this.statusHistorys = result.result;
      });
  }
  getRequestById() {
    this.requestService
      .getById(this.expenditureRequest.id)
      .subscribe((data) => (this.expenditureRequest = data.result));
  }
  getAllBranch() {
    this.branchService
      .GetAllForDropdown()
      .subscribe(
        (data) => (
          (this.branchList = data.result), (this.tempBranchList = data.result)
        )
      );
  }
  getAllRequestType() {
    this.expenditureService
      .GetAllForDropdown()
      .subscribe(
        (data) => (
          (this.requestTypeList = data.result),
          (this.tempRequestTypeList = data.result)
        )
      );
  }
  getCurrency() {
    this.commonService.getAllCurrency().subscribe((data) => {
      this.currencyList = data.result;
    });
  }
  getAllAccount() {
    this.accountService
      .getAll()
      .subscribe(
        (data) => (
          (this.accountList = data.result), (this.tempAccountList = data.result)
        )
      );
  }
  filterBranch() {
    this.branchList = this.tempBranchList.filter((item) =>
      item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase())
    );
  }
  filterRequestType() {
    this.requestTypeList = this.tempRequestTypeList.filter((item) =>
      item.name.trim().toLowerCase().includes(this.searchRequestType.trim().toLowerCase())
    );
  }
  filterAccount() {
    this.accountList = this.tempAccountList.filter((item) =>
      item.name.trim().toLowerCase().includes(this.searchAccount.trim().toLowerCase())
    );
  }
  editRequest() {
    this.readMode = false;
    this.isEditing.emit(true);
  }
  saveTempOutCommingEntry() {
    const payload = {
      id: this.tempExpenditureRequest.id,
      name: this.tempExpenditureRequest.name,
      outcomingEntryTypeId: this.tempExpenditureRequest.outcomingEntryTypeId,
      value: this.tempExpenditureRequest.value,
      workflowStatusId: this.tempExpenditureRequest.workflowStatusId,
      accountId: this.tempExpenditureRequest.accountId,
      branchId: this.tempExpenditureRequest.branchId,
      currencyId: this.tempExpenditureRequest.currencyId,
      supplierId: this.tempExpenditureRequest.supplierId,
      accreditation: this.tempExpenditureRequest.accreditation,
      isAcceptFile: this.tempExpenditureRequest.isAcceptFile,
      paymentCode: this.tempExpenditureRequest.paymentCode,
      reason: this.tempExpenditureRequest.reason,
    } as UpdateTempOutcomingEntryDto;
    this.requestService.saveTempOutCommingEntry(payload).subscribe(
      (res) => {
        abp.notify.success("Edited request");
        this.isLoading = true;
        this.isLoadingRequest.emit(true);
        this.getRequestChangeOutcomingEntry();
        this.getStatusHistorys();
        this.readMode = true;
        this.isEditing.emit(false);
      },
      () => {
        this.getApiError();
      }
    );
  }

  cancelEdit() {
    this.readMode = true;
    this.isLoading = true;
    this.isLoadingRequest.emit(true);
    this.getRequestChangeOutcomingEntry();
    this.isEditing.emit(false);
  }

  getOutcomeFiles() {
    this.requestService
        .GetFiles(this.expenditureRequest.id)
        .pipe(catchError(this.requestService.handleError))
        .subscribe((data) => {
          this.outcomingFileList = data.result;
        });
  }
  deleteFile(file: any) {
    abp.message.confirm(
      `delete file: ${file.fileName}?`,
      "",
      (result: boolean) => {
        if (result) {
          this.requestService
            .DeleteFile(file.id)
            .pipe(catchError(this.requestService.handleError))
            .subscribe(() => {
              abp.notify.success(`Deleted file ${file.fileName}`);
              this.getOutcomeFiles();
              this.getRequestById();
            });
        }
      }
    );
  }
  AcceptFile(isAccept) {
    this.requestService
      .AcceptFile(this.expenditureRequest.id, !isAccept.checked)
      .pipe(catchError(this.requestService.handleError))
      .subscribe((rs) => {
        if (isAccept.checked == true) {
          abp.notify.success("Accepted");
        } else if (isAccept.checked == false) {
          abp.notify.success("Unaccepted");
        }
        this.getRequestById();
      });
  }
  downloadFile(content: any) {
    const file = new Blob([this.s2ab(atob(content.data))], {
      type: "application/vnd.ms-excel;charset=utf-8",
    });
    FileSaver.saveAs(file, content.fileName);
  }
  exportPdf(id) {
    this.service.exportPdf(id).subscribe((data) => {
      const wnd = window.open("about:blank", "", "_blank");
      wnd.document.write(data.result.html);
    });
  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xff;
    return buf;
  }

  branchSelectOpenedChange() {
    if (this.branchList.length) return;
    this.searchBranch = "";
    this.filterBranch();
  }
  getRequestChangeOutcomingEntry() {
    if (this.isViewHistory) return;
    this.requestService.getRequestChangeOutcomingEntry(this.tempId).subscribe(
      (response) => {
        if (!response.success) {
          this.getApiError();
          this.isClose.emit(true);
          return;
        }
        this.setRequestChangeOutcomingEntry(response.result);
      },
      () => {
        this.getApiError();
        this.isClose.emit(true);
      }
    );
  }
  getViewHistoryRequestChangeOutcomingEntry(): void {
    this.requestService
      .getViewHistoryRequestChangeOutcomingEntry(this.tempId)
      .subscribe(
        (response) => {
          this.setRequestChangeOutcomingEntry(response.result);
        },
        () => {
          this.getApiError();
          this.isClose.emit(true);
        }
      );
  }
  private setRequestChangeOutcomingEntry(
    data: RequestChangeOutcomingEntryInfoDto
  ) {
    this.getDifferenceValue.emit(
      data.tempOutcomingEntry.currencyId == data.rootOutcomingEntry.currencyId ?
      data.tempOutcomingEntry.value - data.rootOutcomingEntry.value :
      0
    );
    this.expenditureRequest = data.rootOutcomingEntry;
    this.tempExpenditureRequest = data.tempOutcomingEntry;
    this.isLoadingRequest.emit(false);
    this.isLoading = false;
    this.status.emit({
      Code: data.tempOutcomingEntry.workflowStatusCode,
      Name: data.tempOutcomingEntry.workflowStatusName,
    } as StatusInfo);
  }
  sendRequestChange() {
    this.requestService
      .sendTemp(this.tempId)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          return of({ error: err });
        })
      )
      .subscribe(
        (response) => {
          this.getRequestChangeOutcomingEntry();
          abp.notify.success("Send success");
        },
        () => {
          this.getApiError();
        }
      );
  }
  rejectRequestChange() {
    this.requestService
      .rejectTemp(this.tempId)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          return of({ error: err });
        })
      )
      .subscribe(
        (response) => {
          this.getRequestChangeOutcomingEntry();
          abp.notify.success("Request Rejected");
        },
        () => {
          this.getApiError();
        }
      );
  }
  approveRequestChange() {
    this.requestService
      .approveTemp(this.tempId)
      .subscribe(
        (response) => {
          this.getRequestChangeOutcomingEntry();
          abp.notify.success("Request approved");
          this.isClose.emit(true);
        },
        () => {
          this.getApiError();
        }
      );
  }
  getApiError() {
    this.isLoadingRequest.emit(false);
  }

  isDeleteFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile)
  }

}
