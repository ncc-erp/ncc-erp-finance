import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { BranchDto } from '@app/modules/branch/branch.component';
import { NewExpenditureRequestDto } from '@app/modules/expenditure-request/expenditure-request.component';
import { AccountService } from '@app/service/api/account.service';
import { BranchService } from '@app/service/api/branch.service';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ActionTypeEnum } from '@shared/AppEnums';
import { GetOutcomingEntryDetailDto, GetRequestChangeOutcomingEntryDetailDto, GetTempOutcomingEntryDetailDto, RequestChangeOutcomingEntryDetailInfoDto, SendTempOutcomingEntryDetailDto } from '../../detail-tab.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { catchError } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { of } from 'rxjs';

@Component({
  selector: 'app-temp-mode-table',
  templateUrl: './temp-mode-table.component.html',
  styleUrls: ['./temp-mode-table.component.css']
})
export class TempModeTableComponent extends AppComponentBase implements OnInit {
  public isTableLoading = true;
  public getRequestChangeOutcomingEntryDetailDto: GetRequestChangeOutcomingEntryDetailDto;
  public isAllowed = true;
  public branchList: BranchDto[];
  public tempBranchList: BranchDto[];
  public searchBranch: string;
  public accountList: any;
  public tempAccountList: any;
  public action: ActionTypeEnum;
  //public init: boolean = true;
  public ActionType = ActionTypeEnum;

  @Input() expenditureRequest: NewExpenditureRequestDto;
  @Input() isShowTitle: boolean = true;
  @Input() canCRUD: boolean = true;
  @Input() isViewHistory: boolean = false;
  @Input() tempId?: number;

  @Output() readMode = new EventEmitter<boolean>();
  @Output() changeDetail = new EventEmitter<boolean>();
  @Output() isLoading = new EventEmitter<boolean>();
  @Output() hasData = new EventEmitter<boolean>();
  @Output() isClose = new EventEmitter<boolean>();

  constructor(
    injector: Injector,
    private branchService: BranchService,
    private requestService: ExpenditureRequestService,
    private accountService: AccountService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setDataComponent();
  }
  private setDataComponent(): void {
    if (this.isViewHistory) {
      this.getViewHistoryRequestChangeOutcomingEntryDetail();
    }
    else {
      this.getRequestChangeOutcomingEntryDetail();
    }
    this.getAllAccount();
    this.getAllBranch();
  }
  getAllAccount() {
    this.accountService.getAll().subscribe(data => (this.accountList = data.result))
  }
  branchSelectOpenedChange(isOpen: boolean){
    if(isOpen){
      this.filterBranch();
    }else{
      this.branchList = this.tempBranchList;
    }
  }
  getRequestChangeOutcomingEntryDetail() {
    this.isAllowed = true;
    this.requestService.getRequestChangeOutcomingEntryDetail(this.tempId)
      .subscribe(response => {
        this.setRequestChangeOutcomingEntryDetail(response.result);
      }, () => {this.isClose.emit(true)})
  }
  // emitDataInit(data: GetRequestChangeOutcomingEntryDetailDto){
  //   this.hasData.emit(data.requestChangeDetails.length != 0);
  // }
  getViewHistoryRequestChangeOutcomingEntryDetail() {
    this.isAllowed = true;
    this.requestService.getViewHistoryRequestChangeOutcomingEntryDetail(this.tempId)
      .subscribe(response => {
        this.setRequestChangeOutcomingEntryDetail(response.result);
      }, error => abp.message.error(error))
  }
  private setRequestChangeOutcomingEntryDetail(data: GetRequestChangeOutcomingEntryDetailDto): void {
    this.getRequestChangeOutcomingEntryDetailDto = data;
    this.isTableLoading = false;
    this.isLoading.emit(false);
    this.changeDetail.emit(true);
    // if(this.init){
    //   this.emitDataInit(data);
    //   this.init = false;
    // }
  }
  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe(data => (this.branchList = data.result, this.tempBranchList = data.result))
  }
  filterBranch() {
    this.branchList = this.tempBranchList.filter(item => item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase()))
  }
  getClassAction(actionTypeEnum: ActionTypeEnum) {
    switch (actionTypeEnum) {
      case ActionTypeEnum.NEW:
        return 'temp-new temp';
      case ActionTypeEnum.DELETE:
        return 'temp-delete temp';
      case ActionTypeEnum.UPDATE:
        return 'temp-update temp';
      case ActionTypeEnum.NO_ACTION:
        return 'temp';
      default:
        return 'align-middle';
    }
  }
  getCompareName(requestChangeOutcomingEntryDetailInfoDto: RequestChangeOutcomingEntryDetailInfoDto) {
    let name: string[] = [];
    if (requestChangeOutcomingEntryDetailInfoDto.outcomingEntryDetail)
      name.push(requestChangeOutcomingEntryDetailInfoDto.outcomingEntryDetail.name);

    if (requestChangeOutcomingEntryDetailInfoDto.tempOutcomingEntryDetailDto && requestChangeOutcomingEntryDetailInfoDto.outcomingEntryDetail.name != requestChangeOutcomingEntryDetailInfoDto.tempOutcomingEntryDetailDto.name)
      name.push(requestChangeOutcomingEntryDetailInfoDto.tempOutcomingEntryDetailDto.name);

    return name.join(" => ");
  }
  addMore() {
    this.getRequestChangeOutcomingEntryDetailDto.requestChangeDetails.unshift({
      actionType: ActionTypeEnum.NEW,
      actionTypeName: ActionTypeEnum[ActionTypeEnum.NEW],
      outcomingEntryDetail: null,
      tempOutcomingEntryDetailDto: {
        outcomingEntryId: this.expenditureRequest.id,
        createMode: true
      } as GetTempOutcomingEntryDetailDto
    } as RequestChangeOutcomingEntryDetailInfoDto)
    this.isAllowed = false
    this.action = ActionTypeEnum.NEW
    this.readMode.emit(false);
  }
  saveTempRequestDetail(item: SendTempOutcomingEntryDetailDto) {
    if (this.action == ActionTypeEnum.NEW) {
      this.requestService.createTempOutcomingEntryDetail(item)
        .subscribe((res) => {
          abp.notify.success("Request change created new detail");
          this.isAllowed = true;
          this.readMode.emit(true);
          this.isTableLoading = true;
          this.isLoading.emit(true);
          this.getRequestChangeOutcomingEntryDetail();
        }, () => {
          this.isLoading.emit(false);
        })
    }
    else if (this.action == ActionTypeEnum.UPDATE) {
      this.requestService.updateTempOutcomingEntryDetail(item)
        .subscribe((res) => {
          abp.notify.success("Request change updated detail");
          this.isAllowed = true;
          this.readMode.emit(true);
          this.isTableLoading = true;
          this.isLoading.emit(true);
          this.getRequestChangeOutcomingEntryDetail();
        }, (error) => {
          this.isLoading.emit(false);
        });
    }
  }
  clickBtnUpdate(item: GetTempOutcomingEntryDetailDto) {
    this.isAllowed = false;
    item.createMode = true;
    this.readMode.emit(false);
    this.action = ActionTypeEnum.UPDATE
  }
  clickBtnCancel(item: GetTempOutcomingEntryDetailDto) {
    this.isAllowed = true;
    item.createMode = false;
    this.readMode.emit(true);
    this.getRequestChangeOutcomingEntryDetail();
  }
  deleteTempOutcomingEntryDetail(tempOutcomingEntryDetailId: number) {
    this.requestService.deleteTempOutcomingEntryDetail(tempOutcomingEntryDetailId)
      .pipe(catchError((err: HttpErrorResponse) => {
        return of({ error: err });
      }))
      .subscribe(response => {
        this.getRequestChangeOutcomingEntryDetail();
        this.readMode.emit(true);
      }, (error) => {
        this.isLoading.emit(false);
      })
  }
  revertTempOutcomingDetailByRootId(item: GetOutcomingEntryDetailDto) {
    this.requestService.revertTempOutcomingDetailByRootId(item.id, item.outcomingEntryId)
      .pipe(catchError((err: HttpErrorResponse) => {
        return of({ error: err });
      }))
      .subscribe(response => {
        this.getRequestChangeOutcomingEntryDetail();
      }, (error) => {
        this.isLoading.emit(false);
      })
  }
  isShowLinkAccount() {
    return this.expenditureRequest?.outcomingEntryTypeCode == 'SALARY';
  }
  calculateTempTotal(price, quantity) {
    return Number(price) * Number(quantity);
  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_AddInfoToYCTD)
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_EditInfoOfYCTD);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_DeleteInfoOfYCTD);
  }
  
  isShowRevertBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_RevertInfoOfYCTD);
  }
}
