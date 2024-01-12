import { Component, Inject, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountantAccountService } from '@app/service/api/accountant-account.service';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { BankService } from '@app/service/api/bank.service';
import { CurrencyService } from '@app/service/api/currency.service';
import { BankAccountDto, BankAcountTransactionDto } from '@app/service/model/bank-account.dto';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import * as FileSaver from 'file-saver';
import { AppConsts } from '@shared/AppConsts';
import { MatDialog } from '@angular/material/dialog';
import { UpdateBaseBalanaceComponent, UpdateBaseBalanaceData } from './update-base-balanace/update-base-balanace.component';
import { number } from 'echarts';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';



@Component({
  selector: 'app-bank-account-detail',
  templateUrl: './bank-account-detail.component.html',
  styleUrls: ['./bank-account-detail.component.css']
})
export class BankAccountDetailComponent extends AppComponentBase implements OnInit {
  Finance_BankTransaction_ViewDetail = PERMISSIONS_CONSTANT.Finance_BankTransaction_BankTransactionDetail;

  Account_Directory_BankAccount_EditBaseBalanace =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_EditBaseBalanace;

  bankAccount: BankAccountDto;
  bankAccountId: any;
  bankDetail: any;
  bankDetailTransaction = {} as BankAcountTransactionDto;
  balance = 0
  firstBalance=0
  afterBalance = 0;
  isTableLoading:boolean =false
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.account;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.account;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.bankAccount;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.bankAccount;
  routeUrlThirdLevel = this.APP_CONSTANT.UrlBreadcrumbThirdLevel.bankAccountDetail;
  ; constructor(
    private bankAccountService: BankAccountService,
    injector: Injector,
    private route: ActivatedRoute,
    private bankService: BankService,
    private currencyService: CurrencyService,
    private accountService: AccountantAccountService,
    private dialog: MatDialog,
    private router: Router
  ) {
    super(injector);
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        if(this.bankAccountId){
          this.getByPeriod();
          this.getBankAcountTransactionDetail();
        }

      })
    );
  }

  banks: [];
  accounts: [];
  currencies: [];
  searchValue: '';
  searchValue2: '';
  searchValue3: '';
  startDate
  endDate

  ngOnInit(): void {
    this.startDate = moment().add(-1, "years");
    this.endDate =  moment();
    this.bankAccountId = this.route.snapshot.queryParamMap.get('id');
    this.getByPeriod();
    this.getAllAccount();
    this.getAllBank();
    this.getAllCurrency();
    this.getBankAcountTransactionDetail();
  }

  onRefreshCurrentPage(){
    this.ngOnInit();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel, url: this.routeUrlSecondLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.bankDetail?.holderName , url: this.routeUrlThirdLevel, queryParams: {id: this.bankDetail?.id } },
    ];
  }

  cal(increase, decrease) {
    let result = 0
    result = this.balance + increase - decrease
    this.balance = result
    return result
  }


  getByPeriod() {
    this.bankAccountService.getByPeriod(this.bankAccountId).subscribe(data => {
      this.bankDetail = data.result
      this.updateBreadCrumb();
    })
  }

  getAllBank() {
    this.bankService.getAll().subscribe((data) => {
      this.banks = data.result
    })
  }
  getAllCurrency() {
    this.currencyService.getAll().subscribe((data) => {
      this.currencies = data.result
    })
  }
  getAllAccount() {
    this.accountService.getAll().subscribe((data) => {
      this.accounts = data.result
    })
  }
  getBankAcountTransactionDetail() {
    this.isTableLoading =true
    this.bankDetailTransaction.startDate = this.startDate
    this.bankDetailTransaction.endDate = this.endDate
    if(this.startDate > this.endDate){
      abp.notify.error("Start date must less than End date")
      this.isTableLoading = false
    }
    else{
      this.bankAccountService.BankAccountStatementByPeriod(this.bankAccountId, moment(this.bankDetailTransaction.startDate).format("YYYY/MM/DD"), moment(this.bankDetailTransaction.endDate).format("YYYY/MM/DD")).subscribe((data) => {
        this.isTableLoading = false
        this.bankDetailTransaction = data.result; this.balance = data.result.beginningBalance;
        this.firstBalance=data.result.beginningBalance;
        for (let i = this.bankDetailTransaction.bankTransaction.length - 1; i >= 0; i--) {
          this.bankDetailTransaction.bankTransaction[i].afterBalance = this.cal(this.bankDetailTransaction.bankTransaction[i].increase, this.bankDetailTransaction.bankTransaction[i].reduce)
        }
        this.afterBalance = this.bankDetailTransaction.bankTransaction[0].afterBalance;
      },
      ()=> { this.isTableLoading =false})
    }

  }

  showDetail(id) {
    if (this.permission.isGranted(this.Finance_BankTransaction_ViewDetail)) {
      this.router.navigate(["/app/detail"], {
        queryParams: {
          id: id,
          index: 0,
        },
      });
    }
  }
  ExportExcelDetail(){
    this.startDate =moment(this.startDate).format("YYYY-MM-DD")
    this.endDate =moment(this.endDate).format("YYYY-MM-DD")
    this.bankAccountService.ExportExcelDetail(this.bankAccountId, this.startDate,this.endDate,this.afterBalance,this.firstBalance,this.afterBalance).subscribe(data => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8"
      });
      FileSaver.saveAs(file,`Bank account.xlsx`);
    })
  }
  totalPlus(){
    return this.bankDetailTransaction?.bankTransaction?.reduce((sum, item) => {
      return (sum += item.increase);
    }, 0);
  }
  totalMinus(){
    return this.bankDetailTransaction?.bankTransaction?.reduce((sum, item) => {
      return (sum += item.reduce);
    }, 0);
  }
   updateBaseBalanace(){
    const bankAccountId = Number(this.bankAccountId);
    const updateBaseBalanaceComponent = this.dialog.open(
      UpdateBaseBalanaceComponent,
      {
        width: "550px",
        data: {name: this.bankDetail.bankAccountName, id: bankAccountId, value: this.bankDetailTransaction.beginningBalance} as UpdateBaseBalanaceData,
        disableClose: true,
      }
    );

    updateBaseBalanaceComponent
      .afterClosed()
      .subscribe(() => {
        this.getBankAcountTransactionDetail();
        this.getByPeriod();
      });
  }
}

