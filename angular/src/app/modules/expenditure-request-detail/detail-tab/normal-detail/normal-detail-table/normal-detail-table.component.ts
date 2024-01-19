import { ImportDetailComponent } from './../import-detail/import-detail.component';
import { MatDialog } from '@angular/material/dialog';
import {
  Component,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { BranchDto } from "@app/modules/branch/branch.component";
import { NewExpenditureRequestDto } from "@app/modules/expenditure-request/expenditure-request.component";
import { AccountService } from "@app/service/api/account.service";
import { BranchService } from "@app/service/api/branch.service";
import { RequestDetailService } from "@app/service/api/request-detail.service";
import { ActionTypeEnum } from "@shared/AppEnums";
import { InputFilterDto } from "@shared/filter/filter.component";
import {
  PagedListingComponentBase,
  PagedRequestDto,
  PagedResultDto,
} from "@shared/paged-listing-component-base";
import * as FileSaver from "file-saver";
import { finalize } from "rxjs/operators";
import {
  DeleteOutcomingEntryDetailDto,
  DetailRequestDto,
  GetOutcomingEntryDetailDto,
  RequestDetailDto,
} from "../../detail-tab.component";
import { UpdateBranchComponent } from '@app/modules/expenditure-request-detail/main-tab/update-branch/update-branch.component';
import { log } from 'console';

@Component({
  selector: "app-normal-detail-table",
  templateUrl: "./normal-detail-table.component.html",
  styleUrls: ["./normal-detail-table.component.css"],
})
export class NormalDetailTableComponent
  extends PagedListingComponentBase<RequestDetailDto>
  implements OnInit
{
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.detailRequest.param = request;

    this.requestDetailService
      .getDetailPaging(this.detailRequest)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((data) => {
        (this.requestListDetail = data.result),
          this.showPaging(data.result.paging, pageNumber);
      });
  }

  protected delete(entity: any): void {
    // throw new Error('Method not implemented.');
  }
  ngOnChanges(): void {}

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: "name", displayName: "Name", comparisions: [0, 6, 7, 8] },
    {
      propertyName: "accountName",
      displayName: "Account",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "unitPrice",
      displayName: "Price",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
    {
      propertyName: "total",
      displayName: "Total",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
  ];

  public isTableLoading = true;
  @Input() expenditureRequest: NewExpenditureRequestDto;
  @Output() readMode = new EventEmitter<boolean>();
  public isAllowed = true;
  public branchList: BranchDto[];
  public tempBranchList: BranchDto[];
  public detailRequest = new DetailRequestDto();
  public accountList: any;
  public tempAccountList: any;
  public action: ActionTypeEnum;
  public requestListDetail: ResultGetOutcomingEntryDetailDto;
  public defaultfilterValue = {
    branch: -1,
    isDone: -1
  }
  public searchPaid: number = -1;
  public searchBranch = "";
  public isSaving =false;
  public isRequestChiIncludedInCost:boolean = false;
  constructor(
    injector: Injector,
    private branchService: BranchService,
    private requestDetailService: RequestDetailService,
    private accountService: AccountService,
    private dialog:MatDialog
  ) {
    super(injector);
  }
  getAllAccount() {
    this.accountService
      .getAll()
      .subscribe((data) => (this.accountList = data.result));
  }
  ngOnInit(): void {
    this.detailRequest.outcomingEntryId = this.expenditureRequest.id;
    this.setDefaultFilter();
    this.getAllAccount();
    this.getAllBranch();
    this.refresh();
  }
  setDefaultFilter(){
    this.detailRequest.branchId = "";
    this.detailRequest.isNotDone = "";
  }
  editDetail(item: GetOutcomingEntryDetailDto) {
    item.createMode = true;
    this.isAllowed = false;
    this.action = ActionTypeEnum.UPDATE;
    this.readMode.emit(false);
  }

  changeStatus(item: any) {
    item.isNotDone = !item.isNotDone;
    this.requestDetailService.changeDone(item).subscribe(() => {
      abp.notify.success(
        `${item.name} ${item.isNotDone ? "chưa trả" : "đã trả"}`
      );
      if (this.isAllowed) {
        this.refresh();
      }
    });
  }

  deleteDetail(item: RequestDetailDto): void {
    const payload = {
      id: item.id,
      historyNote: item.historyNote,
    } as DeleteOutcomingEntryDetailDto;
    abp.message.confirm(
      "Delete  '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.requestDetailService.delete(payload).subscribe(
            () => {
              abp.notify.success("Deleted " + item.name);
              this.refresh();
            },
            (error) => {
              abp.message.error(error);
            }
          );
        }
      }
    );
  }
  branchSelectOpenedChange(isOpen: boolean){
    if(isOpen){
      this.onFilterBranch();
    }else{
      this.branchList = this.tempBranchList;
    }
  }

  addMore() {
    this.requestListDetail?.paging.items.unshift({
      createMode: true,
      outcomingEntryId: this.expenditureRequest.id,
      quantity: 1
    } as GetOutcomingEntryDetailDto);
    this.isAllowed = false;
    this.action = ActionTypeEnum.NEW;
  }
  saveRequestDetail(item: GetOutcomingEntryDetailDto) {
    this.isSaving = true
    if (this.action == ActionTypeEnum.NEW) {
      this.requestDetailService.create(item).subscribe((res) => {
        abp.notify.success("Created new detail ");
        this.isAllowed = true;
        this.readMode.emit(true);
        this.refresh();
        this.isSaving = false
      },
      () => {this.isSaving = false});
    } else {
      this.requestDetailService.update(item).subscribe((res) => {
        abp.notify.success("Edited detail");
        this.isAllowed = true;
        this.readMode.emit(true);
        this.refresh();
        this.isSaving = false
      },
      () => this.isSaving = false);
    }
  }
  calculateTempTotal(price, quantity) {
    return Number(price) * Number(quantity);
  }
  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe((data) => {
      this.tempBranchList = this.branchList = data.result;
      // const itemAll = { id: -1, name: "All" } as BranchDto;
      // this.branchList.unshift(itemAll);
    });
  }
  isShowLinkAccount() {
    return this.expenditureRequest?.outcomingEntryTypeCode == "SALARY";
  }
  clickBtnCancel() {
    this.refresh();
    this.isAllowed = true;
    this.readMode.emit(true);
  }

  onFilterBranch(){
    this.branchList = this.tempBranchList.filter(x => x.name.toLowerCase().trim()
    .includes(this.searchBranch.toLowerCase().trim()))
  }

  downloadFileTemplate(){
    this.requestDetailService.downloadFileTemplate()
    .subscribe(response => {
      if(!response.success) return;
      const file = new Blob([this.convertFile(atob(response.result))], {
        type: "application/vnd.ms-excel;charset=utf-8",
      });
      FileSaver.saveAs(file, `Template_mẫu_request_chi_chi_tiết.xlsx`);
    })
  }

  importDetail(){
      let ref = this.dialog.open(ImportDetailComponent, {
        width: "500px",
        data: {
          id: this.expenditureRequest.id
        }
      })
      ref.afterClosed().subscribe(rs => {
        if(rs){
          this.refresh()
        }
      })
  }

  onUpdateBranch(request){
    var dia = this.dialog.open(UpdateBranchComponent, {
      data: {
        isRequestDetail: true,
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

  isStatusIsNotExecuted(){
    return this.expenditureRequest?.workflowStatusCode != "END";
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create);
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete);
  }
  isShowChangeStatusBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive);
  }
  isShowUpdateBranchBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_UpdateBranch);
  }

}
export class ResultGetOutcomingEntryDetailDto {
  paging: PagedResultDto;
  totalMoney: number;
}
