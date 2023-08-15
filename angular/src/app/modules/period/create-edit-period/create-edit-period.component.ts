import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { PeriodService } from '@app/service/api/period.service';
import { BankAccountDto } from '@app/service/model/bank-account.dto';
import { BankAccoutForCompanyDto } from '@app/service/model/common-DTO';
import { CreateEditPeriodDto, PeriodBankAccountForFirstTimeDto, PeriodDto } from '@app/service/model/period.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import * as moment from 'moment';

@Component({
  selector: 'app-create-edit-period',
  templateUrl: './create-edit-period.component.html',
  styleUrls: ['./create-edit-period.component.css']
})
export class CreateEditPeriodComponent extends AppComponentBase implements OnInit {

  constructor(injector: Injector,
    @Inject(MAT_DIALOG_DATA)
    public data: any,
    public dialogRef: MatDialogRef<CreateEditPeriodComponent>,
    private commonService : CommonService,
    private periodService : PeriodService) {
    super(injector);
  }
  public title = "";
  public period = {} as PeriodDto;
  public bankAccount = {} as BankAccountDto;
  public listBankAccounts : BankAccoutForCompanyDto[] = [];
  public currentBalance:number = 0;
  public isLoading:boolean = false;

  ngOnInit(): void {
    this.title = this.data.title;
    if(this.data.isEditting){
      this.period = this.data.period;
    }
    this.period.startDate = moment(new Date()).format("YYYY-MM-DD")
    this.getAllBankAccount();
  }

  public getAllBankAccount(){
    this.commonService.getBankAccoutForCompany().subscribe((rs)=>{
      this.listBankAccounts = rs.result;
      this.listBankAccounts.forEach(x=> x.currentBalance = 0)
    })
  }

  public createTheFirstTime(){
    this.isLoading = true;
    let periodBankAccounts = this.listBankAccounts.map(x=>{
      return {
        bankAccountId : x.value,
        currentBalance: x.currentBalance
      } as PeriodBankAccountForFirstTimeDto;
    })
    var input = {
      name: this.period.name,
      startDate: moment(this.period.startDate).format("YYYY-MM-DD"),
      periodBankAccounts: periodBankAccounts
    } as CreateEditPeriodDto;
    this.periodService.createTheFirstTime(input).subscribe((rs)=>{
      this.isLoading = false;
      abp.notify.success("Create period success!");
      this.dialogRef.close(true);
    }, ()=> this.isLoading = false);
    
  }

  public onUpdate(){
    let input = {
      name : this.period.name
    }
    this.isLoading = true;
    this.periodService.update(input).subscribe((rs)=>{
      this.isLoading = false;
      abp.notify.success("Update name of period success!");
      this.dialogRef.close(true);
    },()=> this.isLoading = false)
  }

  public saveAndClose(){
    if(this.data.isEditting){
      this.onUpdate();
    }else{
      this.createTheFirstTime();
    }
  }

}
