import { Component, Inject, Injector, OnInit } from '@angular/core';
import { InputFilterDto } from './../../../../shared/filter/filter.component';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { RevenuesTabService } from './../../../service/api/revenues-tab.service';
import { ActivatedRoute } from '@angular/router';
import { LinkRevenuesComponent } from '../link-revenues/link-revenues.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';


@Component({
  selector: 'app-revenues-tab',
  templateUrl: './revenues-tab.component.html',
  styleUrls: ['./revenues-tab.component.css']
})
export class RevenuesTabComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }

  requestId: any;
  inbyOut = [];
  chill: RevenuesDto[] = [];

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'name', displayName: 'Name', comparisions: [0, 6, 7, 8] },
    { propertyName: 'accountName', displayName: 'Account', comparisions: [0, 6, 7, 8] },
    { propertyName: 'unitPrice', displayName: 'Price', comparisions: [0, 1, 3, 6, 7, 8] },
    { propertyName: 'total', displayName: 'Total', comparisions: [0, 1, 3, 6, 7, 8] },
  ];

  constructor(
    private route: ActivatedRoute,
    public dialog: MatDialog,
    injector: Injector,

    private inbyoutService: RevenuesTabService
  ) {
    super(injector)
    this.requestId = this.route.snapshot.queryParamMap.get("id")
    this.getInbyOut()

  }

  ngOnInit(): void {
  }

  linkToTransaction() {
    let dialogRef = this.dialog.open(LinkRevenuesComponent, {
      data: this.requestId,
      width: "900px",
      disableClose: true,
    })
    dialogRef.afterClosed().subscribe(rs => {
      this.getInbyOut()
    })
  }

  getInbyOut() {
    this.inbyoutService.getInbyOut(this.requestId).subscribe((data) => {
      this.chill = data.result;
    });
  }

  deleteIncoming(chill: RevenuesDto): void {
    abp.message.confirm(
      "Delete name '" + chill.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this.inbyoutService.deleteIncoming(chill.id, this.requestId).subscribe(() => {
            abp.notify.info('Deleted revenues ' + chill.name + ' successfully');
            this.getInbyOut()
          });
        }
      }
    );
  }

  async setIsRefund(id:number, isRefund:boolean){
    abp.message.confirm(`${isRefund ? 'Update liên kết thành ghi nhận thu <strong>hoàn tiền</strong>' : 'update liên kết thành ghi nhận thu <strong>không hoàn tiền</strong>'}`, "", async (rs) =>{
      if(rs){
        await this.inbyoutService.SetIsRefund(id, isRefund).toPromise().then(rs =>{
          abp.notify.success("Update successful")
        })
      }
      this.getInbyOut()
    }, true)
  }
  isShowRevenuesTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource);
  }
  isShowLinkToRevenuesBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_LinkToIncomingEntry)
  }
  isShowDeleteLinkToRevenuesBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource_DeleteLinkToIncomingEntry)
  }

}
export class RevenuesDto {
  id: number;
  name: string;
  status: boolean;
  value: number
}
