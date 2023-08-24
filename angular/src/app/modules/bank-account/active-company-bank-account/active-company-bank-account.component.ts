import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { BankAccountDto } from '@app/service/model/bank-account.dto';

@Component({
  selector: 'app-active-company-bank-account',
  templateUrl: './active-company-bank-account.component.html',
  styleUrls: ['./active-company-bank-account.component.css']
})
export class ActiveCompanyBankAccountComponent implements OnInit {

  public baseBalance = 0;
  constructor(
  @Inject(MAT_DIALOG_DATA)
  public data: BankAccountDto,
  public dialogRef: MatDialogRef<ActiveCompanyBankAccountComponent>,
  private bankaccountService: BankAccountService,) { }

  ngOnInit(): void {
  }
  update(){
    this.bankaccountService.active({bankAccountId : this.data.id, baseBalance: this.baseBalance}).subscribe(response => {
      if(response.success){
        abp.notify.success(response.result);
        this.dialogRef.close();
      }
    });
  }

}
