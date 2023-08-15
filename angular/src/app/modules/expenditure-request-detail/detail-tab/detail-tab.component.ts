import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { MatDialog } from '@angular/material/dialog';
import { Component, OnInit, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { ActionTypeEnum } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-detail-tab',
  templateUrl: './detail-tab.component.html',
  styleUrls: ['./detail-tab.component.css']
})
export class DetailTabComponent extends AppComponentBase implements OnInit {
  requestId: number;
  tabIndex: number = 1;
  tempMode: boolean;
  readMode = true;
  expenditureRequest: any;
  searchText: string;
  advancedFiltersVisible: boolean;
  router: Router;
  constructor(
    public route: ActivatedRoute,
    injector: Injector,
    public dialog: MatDialog,
    private requestService: ExpenditureRequestService,)
  {
    super(injector)
  }


  ngOnInit(): void {
    this.requestId = Number(this.route.snapshot.queryParamMap.get("id"));
    this.getRequestById();
  }
  getRequestById() {
    this.requestService.getById(this.requestId).subscribe(response => {
      if(!response.success) return;
      this.expenditureRequest = response.result;
    })
  }

  clickRequestChangeBtn(){
    this.requestService.checkExistTempOutCommingEntry(this.requestId)
    .subscribe(data => {
      if(!data.result){
        abp.message.confirm("",
        "Bạn có chắn chắn muốn tạo temp request chi",
        (result: boolean) => {
          if (result) {
            this.createTempOutCommingEntry();
          }
        });
      }
      else{
        this.tempMode = true;
        this.getRequestById();
      }
    }, error => abp.message.error(error))
  }
  createTempOutCommingEntry(){
    this.requestService.createTempOutCommingEntry(this.requestId)
    .subscribe(data => {
      if(data){
        abp.notify.success("Create temp request chi success");
        this.tempMode = true;
        this.getRequestById();
      }
    }, error => abp.message.error(error))
  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/requestDetail/detail'], {
        queryParams: {
          id: this.requestId
        }
      });
    });
  }
  exitTemp(){
    this.tempMode = false;
  }
  readModeChange(event: boolean){
    this.readMode = event;
  }
  isShowDetailTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo);
  }
}
export class DetailRequestDto {
  outcomingEntryId: number;
  param: PagedRequestDto;
  branchId?: number|string;
  isNotDone?: boolean| string;
}
export class RequestDetailDto {
  name: string;
  accountId: number;
  accountName: string;
  quantity: number;
  unitPrice: number;
  total: number;
  outcomingEntryId: number;
  id: number;
  historyNote: string;
  branchId: number;
}
export class LinkedTransactionDto {
  bankTransactionId: number;
  outcomingEntryId: number;
}
export class ResultCheckUpdateOutcomingEntryDetailDto{
  message: string;
  isUpdate: boolean;
  historyNote: string
}
export class DeleteOutcomingEntryDetailDto{
  id: number;
  historyNote: string;
}
export class RequestChangeOutcomingEntryDetailInfoDto{
  actionType: ActionTypeEnum;
  actionTypeName: string;
  outcomingEntryDetail: GetOutcomingEntryDetailDto;
  tempOutcomingEntryDetailDto: GetTempOutcomingEntryDetailDto;
}

export class GetTempOutcomingEntryDetailDto{
  id: number;
  name: string;
  accountId: number;
  userCode: string;
  accountName: string;
  quantity: number;
  unitPrice: number;
  total: number;
  outcomingEntryId: number;
  outcomingEntryTypeCode: string;
  branchId: number;
  branchCode: string;
  branchName: string;
  rootOutcomingEntryDetailId: number;
  rootTempOutcomingEntryId: number;
  createMode: boolean
}
export class GetOutcomingEntryDetailDto{
  tenantId: number;
  name: string;
  accountId: number;
  userCode: string;
  accountName: string;
  quantity: number;
  unitPrice: number;
  total: number;
  outcomingEntryId: number;
  outcomingEntryTypeCode: string;
  branchId: number;
  branchCode: string;
  branchName: string;
  id: number;
  createMode: boolean;
}
export class SendTempOutcomingEntryDetailDto{
  id: number;
  accountId: number;
  name: string;
  quantity: number;
  unitPrice: number;
  total: number;
  outcomingEntryId: number;
  branchId: number;
  rootTempOutcomingEntryId: number;
}
export class GetRequestChangeOutcomingEntryDetailDto{
  requestChangeDetails: RequestChangeOutcomingEntryDetailInfoDto[]
  totalMoneyNumber: number;
  totalMoney: string;
  statusCode: string;
  statusName: string;
}
