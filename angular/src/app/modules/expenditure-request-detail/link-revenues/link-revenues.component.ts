import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { catchError } from 'rxjs/operators';
import { RequestDetailService } from './../../../service/api/request-detail.service';
import { TransactionService } from './../../../service/api/transaction.service';
import { RevenueRecordService } from '@app/service/api/revenue-record.service';
import { RevenuesTabService } from '@app/service/api/revenues-tab.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';


@Component({
  selector: 'app-link-revenues',
  templateUrl: './link-revenues.component.html',
  styleUrls: ['./link-revenues.component.css']
})
export class LinkRevenuesComponent implements OnInit {

  searchText: string = ""
  transactionList: any
  isDisable = false
  incoming = []
  revenues = []
  requestId: any
  requestBody = {
    isRefund: false
  } as LinkRevenuesRequestDto
  constructor(
    private incomingService: RevenueRecordService,
    private route: ActivatedRoute,
    private revenuestabService: RevenuesTabService,
    public dialogRef: MatDialogRef<LinkRevenuesComponent>,
    public _utilities: UtilitiesService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
    this.requestId = this.route.snapshot.queryParamMap.get("id")
    this.getAllIncoming()

  }

  getAllIncoming() {
    this.incomingService.getAll().subscribe((data) => {
      this.incoming = data.result.map(item => {
        item.searchText = `#${item.id} ${item.name} ${this._utilities.formatMoneyCustom(item.value)} ${item.currencyName}`
        return item
      })
    })
  }

  createRevenues() {
    this.requestBody.outcomingEntryId = this.requestId
    this.revenuestabService.create(this.requestBody).subscribe((data) => {
      abp.notify.success("linked revenue ");
      this.dialogRef.close();
    })
  }

  setIsRefund(checked:boolean){
      this.requestBody.isRefund = checked
  }
}
export class LinkRevenuesRequestDto {
  outcomingEntryId: number
  incomingEntryId: number
  isRefund:boolean
}
