import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { RequestDetailService } from './../../../service/api/request-detail.service';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-create-multi-transaction',
  templateUrl: './create-multi-transaction.component.html',
  styleUrls: ['./create-multi-transaction.component.css']
})
export class CreateMultiTransactionComponent implements OnInit {
  isDisable = false
  bankAccountList: any
  requestBody = {} as addMultipleRequestDto
  accountSearch: string = ""
  constructor(private bankAccountService: BankAccountService, private detailService: RequestDetailService,
    public dialogRef: MatDialogRef<CreateMultiTransactionComponent>, private router: Router, @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    this.getBankAccount()
  }
  getBankAccount() {
    this.bankAccountService.getAll().subscribe(data => {
      this.bankAccountList = data.result
    })
  }
  saveAndClose() {
    this.isDisable = true
    this.detailService.addMultipleTransaction(this.data, this.requestBody).pipe(catchError(this.detailService.handleError)).subscribe((res) => {
      abp.notify.success("add successful ");
      this.reloadComponent()
      this.dialogRef.close();
    }, () => this.isDisable = false);

  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/requestDetail/relevant-transaction'], {
        queryParams: {
          id: this.data,
        }
      });
    });
  }
}
export class addMultipleRequestDto {
  name: string;
  fromBankAccountId: number;
  transactionDate: any;
  note: string;
}