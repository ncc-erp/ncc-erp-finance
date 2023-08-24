import { MatDialog } from '@angular/material/dialog';
import { ImportRequestFileComponent } from './import-request-file/import-request-file.component';
import { AccountDto } from '@app/modules/accountant-account/accountant-account.component';
import { BranchDto } from './../../branch/branch.component';
import { SessionServiceProxy, UserDto } from './../../../../shared/service-proxies/service-proxies';
import { AppSessionService } from '@shared/session/app-session.service';
import { CommentService } from './../../../service/api/comment.service';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { CurrencyService } from './../../../service/api/currency.service';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { AccountService } from './../../../service/api/account.service';
import { BranchService } from './../../../service/api/branch.service';
import { ExpenditureService } from './../../../service/api/expenditure.service'
import { CheckChangeStatusDto, NewExpenditureRequestDto, DataRequestChangeDialog, HistoryRequestChangeInfomation, ExpenditureRequestDto } from './../../expenditure-request/expenditure-request.component';
import { ExpenditureRequestService } from './../../../service/api/expenditure-request.service';
import { Component, OnInit, Injector, HostListener, ElementRef } from '@angular/core';
import { RequestDetailService } from '@app/service/api/request-detail.service';
import { TransactionService } from '@app/service/api/transaction.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { userInfo } from 'os';
import { expenditureDto } from '@app/modules/expenditure/expenditure.component';
import { saveAs } from 'file-saver';
import * as FileSaver from 'file-saver';
import { DatePipe, Time } from '@angular/common';
import { GetTempOutcomingEntryDetailDto } from '../detail-tab/detail-tab.component';
import { RequestChangeDialogComponent } from '../request-change-dialog/request-change-dialog.component';
import { EditReportDateComponent } from '@app/modules/expenditure-request/edit-report-date/edit-report-date.component';
import { EditOutcomingTypeComponent } from '@app/modules/expenditure-request/edit-outcoming-type/edit-outcoming-type.component';
import { IncomingEntryTypeOptions } from '@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { CloneRequestComponent } from '@app/modules/expenditure-request/clone-request/clone-request.component';
import { AppConsts } from '@shared/AppConsts';
import { UpdateBranchComponent } from './update-branch/update-branch.component';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
//import { DetailMoneyHistoryDialogComponent } from './detail-money-history-dialog/detail-money-history-dialog.component';

@Component({
  selector: 'app-main-tab',
  templateUrl: './main-tab.component.html',
  styleUrls: ['./main-tab.component.css']
})
export class MainTabComponent extends PagedListingComponentBase<any> implements OnInit {

  public state = false
  public isEditing = false
  @HostListener('document:click', ['$event'])
  clickout(event) {
    if (this.eRef.nativeElement.contains(event.target)) {
    } else {
      this.state = true
    }
  }

  showEditor = false
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    throw new Error('Method not implemented.');
  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }

  requestListDetail: any
  expenditureRequest = {} as NewExpenditureRequestDto
  requestId: any
  readMode: boolean = true
  branchList: BranchDto[]
  requestTypeList: expenditureDto[]
  accountList: AccountDto[]
  tempBranchList: any
  tempRequestTypeList: any
  tempAccountList: any
  searchAccount: string = ""
  searchBranch: string = ""
  searchRequestType: string = ""
  transactionDetailList: any
  tabIndex: any = 1
  searchCurrency: string = ""
  outcomingEntryTypeOptions: IncomingEntryTypeOptions;
  currencyList: ValueAndNameModel[];
  commentList: CommentDto
  commentContent: string = ""
  currentUser: any = {}
  statusHistorys: StatusHistory[];
  isLoading = true;
  public outcomingFileList: outcomeFileDto[] = [];
  public isUpdateBranchMode: boolean = false;
  public currentLogInUserId:number = 0;
  constructor(private requestService: ExpenditureRequestService, private route: ActivatedRoute,
    private branchService: BranchService, private accountService: AccountService,
    private currencyService: CurrencyService, injector: Injector, private eRef: ElementRef, private commentService: CommentService,
    private sessionService: SessionServiceProxy,
    private service: ExpenditureRequestService, private dialog: MatDialog,
    private expenditureService: ExpenditureService,
    private commonService: CommonService,
    private datepipe: DatePipe
  ) { super(injector);
    this.requestId = this.route.snapshot.queryParamMap.get("id")
    const tempId = Number(this.route.snapshot.queryParamMap.get("tempId"))
    if(tempId) this.showRequestChangeDialogNow(tempId);
    this.currentLogInUserId = this.appSession.userId;

   }

  async ngOnInit() {
    await this.getStatusHistorys();
    // this.getCurrentLogin()
    this.getAllBranch()
    this.getAllRequestType()
    this.getAllAccount()
    this.getCurrency()
    this.getAllComment()
    this.getOutcomeFiles()

    this.subscriptions.push(
    AppConsts.periodId.asObservable().subscribe((rs) => {
      this.getRequestById();
    }));
  }
  filterAccreditation(e) {
    if (e.checked == true) {
      this.expenditureRequest.accreditation = true;
    } else {
      this.expenditureRequest.accreditation = false;
    }
  }
  getAllComment() {
    this.commentService.GetAllCommentByPost(this.requestId).subscribe(data => {
      this.commentList = data.result.items

    })
  }

  getCurrentLogin() {
    this.sessionService.getCurrentLoginInformations().subscribe(data => {
      this.currentUser = data.user
    })
  }
  async getStatusHistorys() {
    await this.service.asyncgetOutcomingEntryStatusHistoryByOutcomingEntryId(this.requestId).then(result => {
      this.statusHistorys = result.result;
    });
  }

  deleteComment(item): void {
    abp.message.confirm(
      "delete this comment" + "?",
      "",
      (result: boolean) => {
        if (result) {
          this.commentService.delete(item.id).pipe(catchError(this.commentService.handleError)).subscribe(() => {
            abp.notify.success("Deleted comment ");
            this.getAllComment()
          });
        }
      }
    );
  }
  changeStatus(statusId: any) {
    this.isLoading = true;
    const statusBody: CheckChangeStatusDto = {
      outcomingEntryId: this.requestId,
      statusTransitionId: statusId
    }
    this.service.changeStatus(statusBody).subscribe(() => {
      abp.notify.success("Change status successful ");
      this.getRequestById();
      this.getStatusHistorys();
    },
      () => {
        this.isLoading = false;
      })

  }

  saveComment(event) {
    if (this.isEditing == false) {
      if (this.commentContent != "") {
        let commentBody = {
          commentTypeId: 0,
          postId: this.requestId,
          title: "",
          content: this.commentContent
        }
        this.commentService.createComment(commentBody).subscribe(res => {
          this.getAllComment()
          this.commentContent = ""
        })
      }
    }
    else {
      this.commentBody.content = this.commentContent
      this.commentService.editComment(this.commentBody).subscribe(rs => {
        abp.notify.success("edit successful ")
        this.getAllComment()
        this.commentContent = ""
      })
      this.isEditing = false
    }
  }
  cancelEditComment() {
    this.isEditing = false,
      this.commentContent = ""

  }
  commentBody = {} as commentBodyDto
  editComment(content, id) {
    this.commentBody.commentTypeId = 0
    this.commentBody.postId = this.requestId
    this.commentBody.id = id
    this.commentContent = content
    this.isEditing = true
  }
  getRequestById() {
    this.requestService.getById(this.requestId).subscribe(data => {
      this.expenditureRequest = data.result;
      this.isLoading = false;
    })
  }
  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe(data => (this.branchList = data.result, this.tempBranchList = data.result))
  }
  getAllRequestType() {
    // this.expenditureService.GetAllForDropdown().subscribe(data => (this.requestTypeList = data.result, this.tempRequestTypeList = data.result))
    this.expenditureService.getAllForDropdownByUserNew().subscribe(response => {
      if (!response.success) return;
      this.outcomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: [...response.result]};
    })
  }
  getCurrency() {
    //this.currencyService.GetAllForDropdown().subscribe(data => this.currencyList = data.result)
    this.commonService.getAllCurrency().subscribe(data => this.currencyList = data.result)
  }
  getAllAccount() {
    this.accountService.getAll().subscribe(data => (this.accountList = data.result, this.tempAccountList = data.result))
  }
  filterBranch() {
    this.branchList = this.tempBranchList.filter(item => item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase()))
  }
  filterRequestType() {
    this.requestTypeList = this.tempRequestTypeList.filter(item => item.name.trim().toLowerCase().includes(this.searchRequestType.trim().toLowerCase()))
  }
  filterAccount() {
    this.accountList = this.tempAccountList.filter(item => item.name.trim().toLowerCase().includes(this.searchAccount.trim().toLowerCase()))
  }
  editRequest() {
    this.readMode = false;
  }
  saveRequest() {
    this.requestService.update(this.expenditureRequest).pipe(catchError(this.requestService.handleError)).subscribe((res) => {
      abp.notify.success("edited request ");
      this.getRequestById()
      this.readMode = true
    });
  }
  cancelEdit() {
    this.reloadComponent();
  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/requestDetail/main'], {
        queryParams: {
          id: this.requestId
        }
      });
    });
  }

  getCommentDate(commentDate) {
    let date = new Date(commentDate)
    return `${date.getHours()}:${date.getMinutes().toString().replace(/^(\d)$/, '0$1')}`
  }
  isShowMainTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral);
  }

  configEditor: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    minHeight: '5rem',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'textarea',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      [
        'strikeThrough',
        'subscript',
        'superscript',
        'justifyFull',
        'indent',
        'outdent',
        'insertUnorderedList',
        'insertOrderedList',
      ],
      [
        'customClasses',
        'link',
        'unlink',
        'insertVideo',
        'insertHorizontalRule',
        'removeFormat',
        'toggleEditorMode']


    ],
    customClasses: [

    ]
  };
  importExcel() {
    const dialog = this.dialog.open(ImportRequestFileComponent, {
      data: { id: this.requestId, width: '500px' }
    });
    dialog.afterClosed().subscribe(result => {
      if (result) {
        this.getOutcomeFiles();
        this.getRequestById()
      }

    });
  }
  getOutcomeFiles() {
    this.requestService.GetFiles(this.requestId).pipe(catchError(this.requestService.handleError)).subscribe(data => {
      this.outcomingFileList = data.result
    })
  }
  refreshFile() {
    this.getOutcomeFiles()
  }
  deleteFile(file: any) {
    abp.message.confirm(
      `delete file: ${file.fileName}?`,
      "",
      (result: boolean) => {
        if (result) {
          this.requestService.DeleteFile(file.id).pipe(catchError(this.requestService.handleError)).subscribe(() => {
            abp.notify.success(`Deleted file ${file.fileName}`);
            this.getOutcomeFiles();
            this.getRequestById();

          });
        }
      }
    );

  }
  AcceptFile(isAccept) {
    this.requestService.AcceptFile(this.requestId, !isAccept.checked).pipe(catchError(this.requestService.handleError)).subscribe(rs => {
      if (isAccept.checked == true) {
        abp.notify.success("Accepted")

      }
      else if (isAccept.checked == false) {
        abp.notify.success("Unaccepted")

      }
      this.getRequestById();
    })
  }
  downloadFile(content: any) {
    const file = new Blob([this.s2ab(atob(content.data))], {
      type: "application/vnd.ms-excel;charset=utf-8"
    });
    FileSaver.saveAs(file, content.fileName);
  }
  exportPdf(id) {
    this.service.exportPdf(id).subscribe((data) => {
      const wnd = window.open("about:blank", "", "_blank");
      wnd.document.write(data.result.html)
    })
  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }


  branchSelectOpenedChange() {
    if (this.branchList.length) return;
    this.searchBranch = "";
    this.filterBranch();
  }

  clickRequestChangeBtn() {
    this.requestService.checkExistTempOutCommingEntry(this.requestId)
      .subscribe(data => {
        if (!data.result) {
          abp.message.confirm("",
            "Bạn có chắc chắn muốn tạo yêu cầu thay đổi?",
            (result: boolean) => {
              if (result) {
                this.createTempOutCommingEntry();
              }
            });
        }
      })
  }

  createTempOutCommingEntry() {
    this.requestService.createTempOutCommingEntry(this.requestId)
      .subscribe(data => {
        if (data) {
          abp.notify.success("Tạo yêu cầu thay đổi thành công");
          this.expenditureRequest.tempOutcomingEntryId = data.result.id
          //this.requestChangeDialog();
          this.checkTempOutCommingEntryHasDetail(data.result.id);
        }
      })
  }


  checkTempOutCommingEntryHasDetail(tempOutcomingEntryId?: number){
    this.requestChangeDialog();
  }
  requestChangeDialog(indexTab = 0): void {
    //TODO: show dialog
    const requestChangeDialog = this.dialog.open(RequestChangeDialogComponent, {
      width: "1900px",
      data: { expenditureRequest: this.expenditureRequest, tempId: this.expenditureRequest.tempOutcomingEntryId, isViewHistory: false,selectedIndex: indexTab } as DataRequestChangeDialog,
      disableClose: true
    });

    requestChangeDialog.afterClosed().subscribe((result) => {
      this.getStatusHistorys();
      this.getRequestById();
      //TODO: handle result
    })
  }
  showRequestChangeDialogNow(tempId: number){
    this.expenditureRequest = {name : this.expenditureRequest.name, id :this.requestId, tempOutcomingEntryId : tempId} as NewExpenditureRequestDto
    this.service.checkTempOutcomingEntryApproved(tempId).subscribe(response => {
      if(response.result){
        const temp = this.statusHistorys.find(s => s.tempId == tempId);
        if(!temp) {
          abp.message.error("Yêu cầu thay đổi không tồn tại trong lịch sử");
          this.getRequestById();
          return;
        }
        this.viewDetailHistory(this.statusHistorys.indexOf(temp), tempId);
        return;
      }
      this.checkTempOutCommingEntryHasDetail();
    })
  }

  viewDetailHistory(index: number, tempId?: number): void {
    const currentRequestChange = {
      value: this.statusHistorys[index].value,
      dateTime: this.statusHistorys[index].creationTime,
      createdBy: this.statusHistorys[index].creationUser,
      statusName: this.statusHistorys[index].workflowStatusName,
      statusCode: this.statusHistorys[index].workflowStatusCode,
      isOriginal: this.statusHistorys[index].isRoot,
      currencyName: this.statusHistorys[index].currencyName,
    } as HistoryRequestChangeInfomation;

    const previousRequestChange = {
      value: this.statusHistorys[index + 1].value,
      dateTime: this.statusHistorys[index + 1].creationTime,
      createdBy: this.statusHistorys[index + 1].creationUser,
      statusName: this.statusHistorys[index + 1].workflowStatusName,
      statusCode: this.statusHistorys[index + 1].workflowStatusCode,
      isOriginal: this.statusHistorys[index + 1].isRoot,
      currencyName: this.statusHistorys[index + 1].currencyName,
    } as HistoryRequestChangeInfomation;

    this.openHistory(tempId, currentRequestChange, previousRequestChange);

  }
  openHistory(tempId: number, currentRequestChange: HistoryRequestChangeInfomation, previousRequestChange: HistoryRequestChangeInfomation, tab = 0){
    const requestChangeDialog = this.dialog.open(RequestChangeDialogComponent, {
      data: { isViewHistory: true, tempId, currentRequestChange, previousRequestChange, selectedIndex: tab } as DataRequestChangeDialog,
      width: "1900px",
      disableClose: true,
    });

    requestChangeDialog.afterClosed().subscribe(res => {
      //TODO::
    });
  }

  onUpdateReportDate(){
    let item = {...this.expenditureRequest}
    let ref = this.dialog.open(EditReportDateComponent, {
      width: "500px",
      data: item
    })
    ref.afterClosed().subscribe(rs =>{
      if(rs){
        this.getRequestById()
      }
    })
  }

  onUpdateOutcomingType() {
    let item = { ...this.expenditureRequest }
    let ref = this.dialog.open(EditOutcomingTypeComponent, {
      width: "600px",
      data: item
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.getRequestById()
      }
    })
  }
  cloneRequest(){
    this.dialog.open(CloneRequestComponent, {
      width: "600px",
      data: this.expenditureRequest
    })
  }
  onUpdateBranch(){
    this.isUpdateBranchMode = true;
    var dia = this.dialog.open(UpdateBranchComponent, {
      data: {
        requestId: this.requestId,
        oldBranchId: this.expenditureRequest.branchId
      },
      width: "700px"
    })
    dia.afterClosed().subscribe((rs)=>{
      if(rs){
        this.getRequestById();
      }
      this.isUpdateBranchMode = false;
    })
  }



  isShowActionChangeStatus(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_ChangeStatus);
  }
  isShowEditBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Edit)
  }
  isShowEditReportDateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditReportDate)
  }
  isShowEditType(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditRequestType)
  }
  isShowAddAndEditDiscussion(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus)
  }
  isShowDeleteDiscussion(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus)
  }
  isAcceptFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AcceptFile)
  }
  isAttachFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_AttachFile)
  }
  isDeleteFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteFile)
  }
  isShowExportPDFBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_ExportPdf)
  }
  isShowCloneBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_Clone);
  }
  isShowUpdateBranchBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcmingEntry_UpdateBranch);
  }
  isShowAddAndEditMyDiscussion(userId){
    return  this.isAllowAddMyDisuss() && userId == this.currentLogInUserId;
    
  }
  isShowDeleteMyDiscussion(userId){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus)&&
    userId == this.currentLogInUserId;
  }

  isAllowAddMyDisuss(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus);
  }

}
export class CommentDto {
  commentTypeId: number;
  postId: number
  userId: number
  userName: string
  title: string
  content: string
  id: number
}
export class commentBodyDto {
  commentTypeId: number
  postId: number;
  content: string
  id: number
}
export class outcomeFileDto {
  fileName: string;
  data: string;
}
export class StatusHistory {
  outcomingEntryId: number;
  outcomingEntryName: string;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  creationTime: Date;
  creationUserId: number;
  creationUser: string;
  tempId?: number;
  valueNumber: number;
  value: string;
  isRoot: boolean;
  currencyName: string;
}
export class UpdateTempOutcomingEntryDto {
  id: number;
  name: string;
  outcomingEntryTypeId: number;
  value: number
  workflowStatusId: number;
  accountId: number;
  branchId: number;
  currencyId: number;
  supplierId: number;
  accreditation: boolean
  isAcceptFile: number;
  paymentCode: string;
  reason: string;
}