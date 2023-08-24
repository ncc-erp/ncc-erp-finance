import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import * as moment from 'moment';
import { FormControl } from '@angular/forms';
import { DashBoardService } from '@app/service/api/dash-board.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { catchError, finalize, map, startWith } from 'rxjs/operators';
import { ReviewExplainComponent } from '../finance-review/review-explain/review-explain.component';
import { ViewReviewExplainComponent } from '../finance-review/view-review-explain/view-review-explain.component';
import { BankAccountDto } from '@app/service/model/bank-account.dto';
import { BankAccountService } from '@app/service/api/bank-account.service';
import { PagedRequestBankAccount } from '../bank-account/bank-account.component';
import { AccountTypeEnum } from '@shared/AppEnums';
import { PagedResultResultDto } from '@shared/paged-listing-component-base';
import { Router } from '@angular/router';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { PeriodService } from '@app/service/api/period.service';
import { HttpErrorResponse } from '@angular/common/http';
import { of } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-finance-review-old',
  templateUrl: './finance-review-old.component.html',
  styleUrls: ['./finance-review-old.component.css']
})
export class FinanceReviewOldComponent extends AppComponentBase implements OnInit {

  Account_Directory_BankAccount_ViewDetail = PERMISSIONS_CONSTANT.Account_Directory_BankAccount_ViewBankAccountDetail;

  public statistics = {} as ComparativeStatisticsDto
  explainProcess = false
  isTableLoading = false
  typeDate: any
  fromDate: any;
  toDate: any = moment(new Date()).format("YYYY-MM-DD");
  viewChange = new FormControl(this.APP_CONSTANT.TypeViewHomePage.Month);
  activeView: number = 0;
  total: number = 0;
  totalTransaction: number = 0;
  currencyRate: number = 0;
  totalVNDIn: number = 0;
  totalVNDOut: number = 0;
  totalVNDInTransaction: number = 0;
  totalVNDOutTransaction: number = 0;
  totalUSDIn: number = 0;
  totalUSDInTransaction: number = 0;
  totalUSDOut: number = 0;
  totalUSDOutTransaction: number = 0;
  startBalanceVND: number = 0;
  startBalanceUSD: number = 0;
  exchangeVND: number = 0;
  exchangeUSD: number = 0;
  comparativeData: any;
  ComparativeStatistic = {} as ComparativeServiceDto
  reviewExplains: any[] = [];
  isOver3MDiff: boolean = false;
  bankAccounts: any[];
  showBankAccounts: any[];
  isShowAllBankAccount: false;
  totalDiffIncomeVND: number = 0;
  totalDiffOutcomeVND: number = 0;
  totalDiffIncomeUSD: number = 0;
  totalDiffOutcomeUSD: number = 0;
  bankAccountDefault = ["19132608283018", "19132608283026", "19034753904029", "490069"];
  constructor(
    injector: Injector,
    private dashboardService: DashBoardService,
    private bankaccountService: BankAccountService,
    private periodService: PeriodService,
    private router: Router,
    public _utilities: UtilitiesService,
    private _modalService: BsModalService) {
    super(injector)
  }

  ngOnInit(): void {
    this.getStatistics()
    this.getBankAccount();
  }
  getStatistics() {
    this.toDate = moment(this.toDate).format("YYYY-MM-DD")
    this.isTableLoading = true
    this.dashboardService.ComparativeStatistics("", this.toDate, false)
      .pipe(catchError(this.dashboardService.handleError)).subscribe(data => {
        this.currencyRate = data.result.currencyRate;
        this.total = data.result.total;
        this.totalTransaction = data.result.totalTransaction;
        this.isOver3MDiff = this.total - this.totalTransaction > 3000000 || this.totalTransaction - this.total > 3000000;
        this.totalVNDIn = data.result.totalVNDIn;
        this.totalVNDOut = data.result.totalVNDOut;
        this.totalVNDInTransaction = data.result.totalVNDInTransaction;
        this.totalVNDOutTransaction = data.result.totalVNDOutTransaction;
        this.totalUSDIn = data.result.totalUSDIn;
        this.totalUSDInTransaction = data.result.totalUSDInTransaction;
        this.totalUSDOut = data.result.totalUSDOut;
        this.totalUSDOutTransaction = data.result.totalUSDOutTransaction;
        this.startBalanceUSD = data.result.startBalanceUSD;
        this.startBalanceVND = data.result.startBalanceVND;
        this.exchangeVND = data.result.exchangeVND;
        this.exchangeUSD = data.result.exchangeUSD;
        this.comparativeData = data.result;
      },
        () => { this.isTableLoading = false })
    this.dashboardService.GetReviewExplain()
      .pipe(catchError(this.dashboardService.handleError)).subscribe(res => {
        this.reviewExplains = Object.values(res.result);
        this.resetTotalValue()
        this.reviewExplains.forEach(x => {
          this.totalDiffIncomeUSD += x.incomingDiffUSD
          this.totalDiffIncomeVND += x.incomingDiffVND
          this.totalDiffOutcomeUSD += x.outcomingDiffUSD
          this.totalDiffOutcomeVND += x.outcomingDiffVND
        })
      })
  }

  getBankAccount() {

    let requestBankAccount = { maxResultCount: 1000, accountTypeEnum: AccountTypeEnum.COMPANY } as PagedRequestBankAccount;
    this.bankaccountService
      .getAllPaging(requestBankAccount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PagedResultResultDto) => {
        this.bankAccounts = result.result.items;
        this.showBankAccounts = result.result.items.filter(s => this.bankAccountDefault.includes(s.bankNumber));
      });
  }
  showAllChange() {
    if (this.isShowAllBankAccount) {
      this.showBankAccounts = this.bankAccounts;
    } else {
      this.showBankAccounts = this.bankAccounts.filter(s => this.bankAccountDefault.includes(s.bankNumber));
    }
  }
  showDetail(id: any) {
    if (
      this.permission.isGranted(this.Account_Directory_BankAccount_ViewDetail)
    ) {
      AppConsts.periodId.next(AppConsts.GetFirstPeriod);
      this.router.navigate(["app/bankAccountDetail"], {
        queryParams: {
          id: id,
        },
      });
    }
  }
  addExplain() {
    let dialogRef: BsModalRef;
    dialogRef = this._modalService.show(
      ReviewExplainComponent,
      {
        initialState: { comparativeData: this.comparativeData },
        class: 'modal-lg',
      }
    );

    dialogRef.content.onSave.subscribe(() => {
      this.getStatistics();
    });
  }

  showExplain(data: any) {
    let dialogRef: BsModalRef;
    dialogRef = this._modalService.show(
      ViewReviewExplainComponent,
      {
        initialState: { reviewExplain: data },
        class: 'modal-lg',
      }
    );
  }

  resetTotalValue() {
    this.totalDiffIncomeUSD = 0
    this.totalDiffIncomeVND = 0
    this.totalDiffOutcomeUSD = 0
    this.totalDiffOutcomeVND = 0
  }

  isShowCreateExplanationBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_ComparativeStatistic_CreateExplanation);
  }

  isShowExplanationBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_ComparativeStatistic_ViewExplanation);
  }

  isShowAllCompanyBankAccounts() {
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_ComparativeStatistic_ViewAllCompanyBankAccount);
  }


}
export class ComparativeStatisticsDto {
  totalIncoming: number;
  totalOutcoming: number;
  startBalance: calculateBalanceDto[];
  convertOutcomingByTransactions: BalanceDto[];
  realOutcomingByTransactions: BalanceDto[];
  balanceConvert: calculateBalanceDto[] = []
  comparativeStatisticId: number;
}
export class BalanceDto {
  bankNumber: number;
  holderName: string;
  currentBalance: number;
  currencyName: string;
  currentBalanceToVND: number;
  bankAccountId: any;
  increaseBalance: number;
  increaseBalanceToVND: number;
  reducedBalanceToVND: number;
  reducedBalance: number;
  bankAccountExplanation: string;
  explanationId: number;
}
export class calculateBalanceDto {
  bankNumber: number;
  holderName: string;
  currentBalance: number;
  currencyName: string;
  currentBalanceToVND: number;
  bankAccountId: any;
  increase: number;
  increaseBalanceToVND: number;
  decrease: number;
  reducedBalanceToVND: number;
  endBalance: number;
  endBalanceToVND: number;
  difference: number;
  differenceBalanceToVND: number;
  explaination?: string;
  explanationId: number;
  bankAccountExplanation: string;
}
export class explanationDto {
  bankAccountExplanation: string;
  type: number
  bankAccountId: number
  comparativeStatisticId: number
  id: number
}
export class ComparativeServiceDto {
  startDate: string;
  endDate: string;
  differentExplanation: string;
  id: number;
}
function finishedCallback() {
  throw new Error('Function not implemented.');
}

