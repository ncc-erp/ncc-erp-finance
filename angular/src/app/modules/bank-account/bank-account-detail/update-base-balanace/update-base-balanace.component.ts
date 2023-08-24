import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { BankAccountStatisticDto } from '@app/service/model/banktransStatistic.model';

@Component({
  selector: 'app-update-base-balanace',
  templateUrl: './update-base-balanace.component.html',
  styleUrls: ['./update-base-balanace.component.css']
})
export class UpdateBaseBalanaceComponent implements OnInit {

  constructor(
  @Inject(MAT_DIALOG_DATA)
  public data: UpdateBaseBalanaceData,
  private bankAccountService: BankAccountService,
  public dialogRef: MatDialogRef<UpdateBaseBalanaceComponent>,
  ) { }

  ngOnInit(): void {
  }
  update(){
    this.bankAccountService.EditBalanace({bankAccountId : this.data.id, baseBalance: this.data.value}).subscribe(response => {
      if(response.success){
        abp.notify.success(response.result);
        this.dialogRef.close();
      }
    });
  }

}
export class UpdateBaseBalanaceData{
  name: string;
  id: number;
  value: number;
}
