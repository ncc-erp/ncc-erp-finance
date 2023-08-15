import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { catchError } from 'rxjs/operators';
import { RequestDetailService } from './../../../service/api/request-detail.service';
import { TransactionService } from './../../../service/api/transaction.service';
import { Component, OnInit, Inject } from '@angular/core';
import * as moment from 'moment';
import { CommonService } from '@app/service/api/new-versions/common.service';

@Component({
  selector: 'app-link-transaction',
  templateUrl: './link-transaction.component.html',
  styleUrls: ['./link-transaction.component.css']
})
export class LinkTransactionComponent implements OnInit {
  searchText: string = ""
  transactionList: any
  isDisable = false
  requestBody = {} as LinkTransactionRequestDto
  constructor(private router: Router, private transactionService: TransactionService, private detailService: RequestDetailService,
    private _common: CommonService,
    public dialogRef: MatDialogRef<LinkTransactionComponent>, @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    this.getAllTransaction()
  }
  getAllTransaction() {
    this._common.getBankTransactionFromCompanyByCurrency(this.data.currencyId).subscribe(data => {
      this.transactionList = data.result;
    })
  }
  saveAndClose() {
    this.isDisable = true
    this.requestBody.outcomingEntryId = this.data.id
    this.detailService.linkTransaction(this.requestBody).pipe(catchError(this.detailService.handleError)).subscribe((res) => {
      abp.notify.success("linked transaction ");
      this.reloadComponent()
      this.dialogRef.close();
    }, () => this.isDisable = false);

  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/requestDetail/relevant-transaction'], {
        queryParams: {
          id: this.data.id,
        }
      });
    });
  }
}
export class LinkTransactionRequestDto {
  bankTransactionId: number;
  outcomingEntryId: number;
}
