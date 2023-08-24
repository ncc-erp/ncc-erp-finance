import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { UpdateStatusInvoiceDto } from '@app/service/model/n-revenue.model';
import { AppComponentBase } from '@shared/app-component-base';
import { InvoiceStatus } from '@shared/AppEnums';
import { ListNRevenueComponent } from '../list-n-revenue/list-n-revenue.component';
import { CommonService } from './../../../../service/api/new-versions/common.service';
@Component({
  selector: 'app-edit-status-n-revenue',
  templateUrl: './edit-status-n-revenue.component.html',
  styleUrls: ['./edit-status-n-revenue.component.css']
})
export class EditStatusNRevenueComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  revenue = {} as UpdateStatusInvoiceDto
  public title: string = 'Update status invoice';
  listRevenueStatuses: ValueAndNameModel[] = [];
  resultCheckSetDoneInvoice = {} as  CheckSetDoneInvoiceDto

  constructor(injector: Injector, public dialogRef: MatDialogRef<ListNRevenueComponent>,
    private _common: CommonService,
    @Inject(MAT_DIALOG_DATA) public data: any, private _revenue: NRevenueService) {
    super(injector);
    this.revenue = this.data.item;
  }
  ngOnInit(): void {
    this.getAllInvoiceStatuses();
  }

  saveAndClose() {
    this.isDisable = true;
    if(this.revenue.status == InvoiceStatus.HOAN_THANH){
      this.checkSetDoneInvoice()
      return;
    }
    this.updateStatus();
    return;
  }

  private updateStatus() {
    this._revenue.updateStatus(this.revenue).subscribe(rs => {
      if (rs.success) {
        this.handleAfterUuccess();
      }
    }, () => { this.isDisable = false })
  }

  private checkSetDoneInvoice() {
    this._revenue.checkSetDoneInvoice(this.revenue.id).subscribe(rs => {
      if (rs.success) {
        this.resultCheckSetDoneInvoice = rs.result;
        if(this.resultCheckSetDoneInvoice.isAllowedSetDone){
          this.setDoneInvoice();
        }
        this.isDisable = false;
      }
    }, () => { this.isDisable = false })
  }

  private setDoneInvoice() {
    this._revenue.setDoneInvoice(this.revenue.id).subscribe(rs => {
      if (rs.success) {
        this.handleAfterUuccess();
      }
    }, () => { this.isDisable = false })
  }

  private handleAfterUuccess() {
    abp.notify.success("Updated status successfully");
    let statusName = this.listRevenueStatuses.find(x => x.value == this.revenue.status).name
    this.revenue = { ...this.revenue, statusName: statusName }
    this.dialogRef.close(this.revenue);
  }

  private getAllInvoiceStatuses(): void {
    this._common.getAllInvoiceStatuses()
      .subscribe(response => {
        if (!response.success) return;
        this.listRevenueStatuses = response.result;
      })
  }
}
export interface CheckSetDoneInvoiceDto {
  totalDebt: number;
  maxITF: number;
  isAllowedSetDone: boolean;
}