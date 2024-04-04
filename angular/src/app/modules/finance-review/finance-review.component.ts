import { UtilitiesService } from './../../service/api/new-versions/utilities.service';
import { result } from 'lodash';
import { BankAccountStatisticDto, BanktransStatisticDto, StatisticsIncomingEntryDto, StatisticsOutcomingEntry, BankTransDto, StatisticByCurrencyDto, StatisticResultDto, ExchangeRates, nomney } from './../../service/model/banktransStatistic.model';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { DashBoardService } from './../../service/api/dash-board.service';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-finance-review',
  templateUrl: './finance-review.component.html',
  styleUrls: ['./finance-review.component.css'],
  providers: [DecimalPipe]
})
export class FinanceReviewComponent extends AppComponentBase implements OnInit {


  public bankTransStatistic = {} as StatisticResultDto
  public listbankAccountStatistic: BankAccountStatisticDto[] = []
  public liststatisticsIncomingEntry: StatisticsIncomingEntryDto[] = []
  public statisticsOutcomingEntry = {} as StatisticsOutcomingEntry
  public statisticByCurrency: StatisticByCurrencyDto[] = []
  public totalValueStatisticByCurrency: nomney;
  public totalValueStatisticBankAccount: nomney;
  public exchangeRates: ExchangeRates[] = [];
  public tooltip: string = "";
  public tooltipExchangeRateOfBankAccount = "";
  public isIncludeBTransPending = true;
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.financeManagement;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.financeManagement;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.financeReview;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.financeReview;
    constructor(
    injector: Injector,
    private dashboardService: DashBoardService,
    public _utilities: UtilitiesService)
  {
    super(injector)
  }

  ngOnInit(): void {
    this.subscriptions.push(
    AppConsts.periodId.asObservable().subscribe(rs => {
      this.getData();
    }))
    this.updateBreadCrumb()
  }

  onRefreshCurrentPage(){
    this.ngOnInit();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }
  private getData() {
    this.GetBankAccountStatistics()
    // this.GetComparativeStatisticsIncomingEntry()
    // this.GetComparativeStatisticsOutcomingEntry()
    this.GetComparativeStatisticsOutBankTransaction()
    this.GetStatisticByCurrency()
  }

  GetComparativeStatisticsOutBankTransaction() {
    this.dashboardService.GetComparativeStatisticsOutBankTransaction().subscribe(rs => {
      this.bankTransStatistic = rs.result
    })
  }

  GetBankAccountStatistics() {
    this.dashboardService.GetBankAccountStatistics(this.isIncludeBTransPending).subscribe(rs => {
      this.listbankAccountStatistic = rs.result.statistics
      this.tooltipExchangeRateOfBankAccount = rs.result.exchangeRates.map(s => {
        return `1 ${s.currencyName} = ${s.exchangeRateFormat} ${this.defaultCurrencyCode} \n`
      }).join("");
      this.totalValueStatisticBankAccount = {value: rs.result.totalToVND, valueFormat: rs.result.totalToVNDFormat};
    })
  }
  GetComparativeStatisticsIncomingEntry() {
    this.dashboardService.GetComparativeStatisticsIncomingEntry().subscribe(rs => {
      this.liststatisticsIncomingEntry = rs.result
    })
  }
  GetComparativeStatisticsOutcomingEntry() {
    this.dashboardService.GetComparativeStatisticsOutcomingEntry().subscribe(rs => {
      this.statisticsOutcomingEntry = rs.result
    })
  }

  GetStatisticByCurrency() {
    this.dashboardService.GetComparativeStatisticByCurrency().subscribe(rs => {
      this.statisticByCurrency = rs.result.statistics;
      this.exchangeRates = rs.result.exchangeRates;
      this.tooltip = rs.result.exchangeRates.map(s => {
        return `1 ${s.tienTe} = ${s.exchangeRateFormat} ${this.defaultCurrencyCode} \n`
      }).join("");
      this.totalValueStatisticByCurrency = {value: rs.result.totalToVND, valueFormat : rs.result.totalToVNDFormat};
    })
  }

  isEqual(bankTran: BankTransDto) {
    if (bankTran?.fromValue == bankTran?.toValue && bankTran?.fromCurrencyName == bankTran?.toCurrencyName) {
      return true
    }
    return false
  }

  getMoneyColor(money: number) {
    return money < 0 ? "#dc3545" : ""
  }

  getUrlOutcomingEntry(id: number) {
    return `/app/requestDetail/main?id=${id}`
  }

  getUrlBanktrans(id: number) {
    return `/app/detail?id=${id}`
  }

  getUrlBankAccount(id: number) {
    return `/app/bankAccountDetail?id=${id}`
  }

  getUrlBtrans(id: number) {
    return `/app/btransaction?id=${id}`
  }

  getCurrencyColor(currency: string) {
    switch (currency) {
      case "YEN": {
        return "rgb(66, 3, 44)"
      }
      case "USD": {
        return "blue"
      }
      case "VND": {
        return "black"
      }
      case "EURO": {
        return "range"
      }
    }

  }

  getColorcompareDiff(diff1: number, diff2: number) {
    let total = diff1 + diff2
    if (total > 1 || total < -1) {
      return "#dc3545"
    }
    else {
      return 'blue'
    }
  }

  roundToTwo(num:number) {
    return Number((Math.round(num * 100) / 100).toFixed(2));
  }

}
