import { Router } from '@angular/router';
import { Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from './../../../../../service/api/new-versions/btransaction.service';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { LinkExpenditureAndBTransDto } from '../../list-btransaction/list-btransaction.component';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';

@Component({
  selector: 'app-link-expenditure-result',
  templateUrl: './link-expenditure-result.component.html',
  styleUrls: ['./link-expenditure-result.component.css']
})
export class LinkExpenditureResultComponent implements OnInit {
  public isSaved: boolean = false
  public isSaving: boolean = false
  public message: string = ""
  private linkExpenditureAndBTrans = {} as LinkExpenditureAndBTransDto;
  public linkResult = {} as LinkResultDto
  public isLinked: boolean = false
  private doneTransactionId: number = 0
  public doneResult:string = ""

  constructor(private _btransaction: BtransactionService,
    private rqChiService: ExpenditureRequestService,
    private router: Router,
    public dialogRef: MatDialogRef<LinkExpenditureResultComponent>, @Inject(MAT_DIALOG_DATA) private data) {
    this.message = this.data.message
    this.linkExpenditureAndBTrans = this.data.item
  }

  ngOnInit(): void {
    this.getDoneTransaction()
  }

  doSave() {
    this.isSaving = true
    this._btransaction.linkOutcomingEntryWithBTransaction(this.linkExpenditureAndBTrans)
      .subscribe(rs => {
        this.isLinked = true
        this.linkResult.isSuccess = true
        this.linkResult.message = `Bạn đã link thành công biến động số dư <strong class='text-dark'>#${this.linkExpenditureAndBTrans.BTransactionId}</strong>
        <br/>Với request chi <strong class='text-dark'>#${this.linkExpenditureAndBTrans.outcomingEntryId}</strong> <br/>`
        abp.notify.success("Link successful")
        this.isSaving = false
      }, (rs) => {
        this.linkResult.isSuccess = false
        this.linkResult.message = `Link fail: ${rs.message}`
        this.isSaving = false
      });
  }

  getDoneTransaction() {
    this.rqChiService.getDoneWorkFlowStatusTransaction().subscribe(rs => {
      this.doneTransactionId = rs.result
    })
  }

  doneRqChi() {
    let dto = {
      outcomingEntryId: this.linkExpenditureAndBTrans.outcomingEntryId,
      statusTransitionId: this.doneTransactionId
    }
    this.rqChiService.changeStatus(dto).subscribe(rs => {
      this.doneResult = "<strong class='text-success'>Thực thi request chi thành công</strong>"
      this.isSaved = true
      abp.notify.success("Thực thi thành công")
    },
    (err) => {
      this.doneResult = `<strong class='text-danger'>Done request chi fail: ${err.message}</strong>`
    })
  }

  showOutcomingEntry() {
    this.dialogRef.close(true)
    this.router.navigate(["/app/requestDetail/relevant-transaction"], {
      queryParams: {
        index: 2,
        id: this.linkExpenditureAndBTrans.outcomingEntryId,
      },
    });
  }

  onClose() {
    if (this.isSaved) {
      this.dialogRef.close(true)
    }
    else {
      this.dialogRef.close()
    }
  }
}
export interface LinkResultDto {
  isSuccess: boolean,
  message: string
}
