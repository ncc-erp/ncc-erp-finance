import { PeriodDto } from './../service/model/period.dto';
import { PeriodService } from './../service/api/period.service';
import { ExportReportComponent } from './export-report/export-report.component';
import { MatDialog } from '@angular/material/dialog';
import { APP_CONSTANT } from './../constant/api.constant';
import { DashBoardService } from './../service/api/dash-board.service';
import { Router } from '@angular/router';
import { Component, Injector, ChangeDetectionStrategy, ViewChild, ElementRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import * as echarts from "echarts";
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import * as moment from 'moment';
import { FormControl } from '@angular/forms';
import { PopupComponent } from '@shared/date-selector/popup/popup.component';
import { MatDatepicker } from '@angular/material/datepicker';
import { AppConsts } from '@shared/AppConsts';
import { DetailBaocaoThuComponent } from './detail-baocao-thu/detail-baocao-thu.component';
import { DetailBaocaoChiComponent } from './detail-baocao-chi/detail-baocao-chi.component';
import { DetailNhanvienNoComponent } from './detail-nhanvien-no/detail-nhanvien-no.component';
import { CircleChartDto, InputListCircleChartDto, ResultCircleChartDto } from '@app/service/model/circle-chart.dto';
import { CircleChartService } from '@app/service/api/circle-chart.service';
import { IOption } from '@shared/components/custome-select/custome-select.component';
import { DateSelectorHomeEnum } from '@shared/AppEnums';
import { DateTimeSelectorHome } from './date-selector-dashboard/date-selector-dashboard.component';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  animations: [appModuleAnimation()],
})

export class HomeComponent extends AppComponentBase {
  APP_CONSTANT = APP_CONSTANT;
  Finance_OutcomingEntry_RemindCEO = PERMISSIONS_CONSTANT.DashBoard_XemKhoiRequestChi_NhacnhoCEO;
  tableData = {} as any;
  outcomeData = []
  incomeData = []
  circleChartData: ResultCircleChartDto[]
  listChartId: number[] = []
  statusBox = {} as totalStatusDto
  cashFlowData = {
  } as CashFlowDataDto
  public isByPeriod: boolean = true
  public outcomingEntryStatistics: OverviewOutcomingEntryStatisticsDto[] = [];
  public invoiceStatistics = {} as OverviewInvoiceStatisticsDto;
  public bTransactionStatistics: OverviewBTransactionStatisticsDto[] = [];
  public isLoadingStatistic: boolean = false
  public isLoadingChart: boolean = false
  public YCTDStatus: string = "[YCTĐ] Chờ CEO duyệt"
  public selectedPeriod = {} as PeriodDto
  public baoCaoChung: BaoCaoChungDto[] = []
  public baoCaoFromDate: any
  public baoCaoToDate: any
  public baoCaoFilter = baoCaoFilterOption
  public debtStatistic = {} as HrmDebtDto
  
  distanceFromAndToDate = '';
  viewChange = new FormControl(this.APP_CONSTANT.TypeViewHomePage.Month);
  activeView: number = 0;
  endDate = new FormControl(moment())
  startDate = new FormControl(moment());
  listCircleChart: IOption[] = []
  
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.dashboard;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.dashboard;

  defaultDateFilterTypeBaoCaoThuChi: DateSelectorHomeEnum = DateSelectorHomeEnum.MONTH;
  searchWithDateTimeBaoCaoThuChi = {} as DateTimeSelectorHome;

  defaultDateFilterTypeCircleChart: DateSelectorHomeEnum = DateSelectorHomeEnum.MONTH;
  searchWithDateTimeCircleChart = {} as DateTimeSelectorHome;

  setEndDate(normalizedMonthAndYear: moment.Moment, datepicker: MatDatepicker<moment.Moment>) {
    const ctrlValue: moment.Moment = moment();
    ctrlValue?.month(normalizedMonthAndYear.month());
    ctrlValue?.year(normalizedMonthAndYear.year());

    this.endDate.setValue(this.getLastDateOfMonth(ctrlValue));
    datepicker.close();
    this.getDataForLineChart()
  }

  setStartDate(normalizedMonthAndYear: moment.Moment, datepicker: MatDatepicker<moment.Moment>) {
    const ctrlValue: moment.Moment = moment();
    ctrlValue?.month(normalizedMonthAndYear.month());
    ctrlValue?.year(normalizedMonthAndYear.year());

    this.startDate.setValue(this.getFirstDateOfMonth(ctrlValue));
    datepicker.close();
    this.getDataForLineChart()
  }
  keyUpEnterStartDate(event: any) {
    this.startDate.setValue(this.convertStringToTime(event.value, true));
    this.getDataForLineChart();
  }
  keyUpEnterEndDate(event: any) {
    this.endDate.setValue(this.convertStringToTime(event.value, false));
    this.getDataForLineChart();
  }
  convertStringToTime(value: string, isFirstDate: boolean) {
    const time = value.split("/");
    const ctrlValue = moment();
    ctrlValue?.month(Number(time[0]) - 1);
    ctrlValue?.year(Number(time[1]));
    
    return isFirstDate? this.getFirstDateOfMonth(ctrlValue) : this.getLastDateOfMonth(ctrlValue);
  }

  ngAfterViewInit(): void {
    this.changeView()
  }
  ngOnInit(): void {
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe(periodId => {
        this.periodService.getById(periodId).subscribe(rs => {
          let period: PeriodDto = rs.result

          this.startDate.setValue(period.startDate)
         
          if (period.endDate) {
            this.endDate.setValue(period.endDate)
          }
          else {
            this.endDate.setValue(this.baoCaoToDate)
          }
          this.overviewBTransactionStatistics()
          this.overviewOutcomingEntryStatistics()
          this.getDataForLineChart()
        })
      }),
    )

    this.overviewInvoiceStatistics()
    this.getHRMDebtStatistic()
    this.getCircleChartActive()
    this.updateBreadCrumb()
  }

  onRefreshCurrentPage(){
    this.ngOnInit();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel }
    ];
  }

  constructor(injector: Injector, private router: Router, private dialog: MatDialog,
    private dashBoardService: DashBoardService, private outcomeService: ExpenditureRequestService,
    private periodService: PeriodService, private circleChartService: CircleChartService) {
    super(injector);

  }


  getChartStartDate() {
    if (this.isByPeriod) {
      let periodStartDate: moment.Moment = moment(AppConsts.periodStartDate)
      let firstDateOfYear = moment().startOf('year')

      if (periodStartDate && periodStartDate > firstDateOfYear) {
        this.startDate.setValue(periodStartDate)
      }
      else {
        this.startDate.setValue(firstDateOfYear)
      }
    }
    else {
      this.startDate.setValue(moment().subtract(11, 'months').set("date", 1))
    }
  }

  private getFirstDateOfMonth(date: moment.Moment): moment.Moment {    
    return date.set('date', 1);
  }

  private getLastDateOfMonth(date: moment.Moment): moment.Moment {    
    return this.getFirstDateOfMonth(date).add(1,'months').subtract(1,'days');
    
  }

  onChangePeriod(event) {
    this.isByPeriod = event.checked
    if (!this.isByPeriod) {
      this.endDate.setValue(this.getLastDateOfMonth(moment()))
    }
    this.getChartStartDate()
    this.getDataForLineChart()
    this.changeView()
  }
  getTotalStatus() {
    this.dashBoardService.GetStatusDashBoard().subscribe(data => {
      this.statusBox = data.result
    })
  }
  viewPendingRequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        status: 'PENDINGCEO'
      }
    })
  }
  viewRequestChangePendingRequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        statusRequestChange: 'PENDINGCEO'
      }
    })
  }
  viewPendingCFORequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        status: 'PENDINGCFO'
      }
    })
  }
  viewApprovedRequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        status: 'APPROVED'
      }
    })
  }
  viewTransferedRequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        status: 'TRANSFERED'
      }
    })
  }
  viewEndRequest() {
    this.router.navigate(['/app/expenditure-request'], {
      queryParams: {
        status: 'END'
      }
    })

  }
  sendKomuCEO() {
    abp.message.confirm(
      " You want to sent this to CEO?",
      "",
      (result: boolean) => {
        if (result) {
          this.outcomeService.SendKomuCEO().subscribe(() => {
            abp.notify.success("Send successful");
          })
        }
      }
    );
  }
  sendKomuCFO() {
    abp.message.confirm(
      " You want to sent this to CFO?",
      "",
      (result: boolean) => {
        if (result) {
          this.outcomeService.SendKomuCFO().subscribe(() => {
            abp.notify.success("Send successful");
          })
        }
      }
    );
  }
  sendKomuCEORequestChange() {
    abp.message.confirm(
      " You want to sent this to CEO?",
      "",
      (result: boolean) => {
        if (result) {
          this.outcomeService.SendKomuCEORequestChange().subscribe(() => {
            abp.notify.success("Send successful");
          })
        }
      }
    );
  }

  buildLineChart() {
    var chartDom = document.getElementById('main');
    var myChart = echarts.init(chartDom);
    var option;
    option = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'cross',
          crossStyle: {
            color: '#999'
          }
        }
      },
      toolbox: {
        feature: {
          magicType: { show: true, type: ['line', 'bar', 'stack'] },
          restore: { show: true },
          saveAsImage: { show: true }
        }
      },
      legend: {
        data: this.cashFlowData.charts.map(x => x.name),
        bottom: 0,
        orient: 'horizontal',
      },
      xAxis: [
        {
          type: 'category',
          data: this.cashFlowData.labels,
          axisPointer: {
            type: 'shadow'
          }
        }
      ],
      yAxis: [
        {
          type: 'value',
          name: 'Số tiền (VND)',
          min: 0,
          axisLabel: {
            formatter: '{value} '
          }
        }
      ],
      series: this.cashFlowData.charts
    };

    option && myChart.setOption(option);
  }

  public getDataForLineChart() {
    let start = moment(this.startDate.value).format("YYYY-MM-DD")
    let end = moment(this.endDate.value).format("YYYY-MM-DD")
    this.isLoadingChart = true
    this.dashBoardService.GetNewChart(start, end, this.isByPeriod).subscribe(rs => {
      this.cashFlowData = rs.result
      this.tableData = {
        labels: this.cashFlowData.labels,
        data: this.cashFlowData.charts.map(x => {
          return {
            name: x.name,
            data: x.data,
            total: x.total
          }
        })
      }

      this.buildLineChart()
      this.isLoadingChart = false
    },
      () => this.isLoadingChart = false)
  }


  // buildIncomeChart() {
  //   var chartDom = document.getElementById('incomeChart');
  //   var myChart = echarts.init(chartDom);
  //   var option;
  //   option = {
  //     title: {
  //       text: this.incomeData.length > 0 ? 'Thu' : `No data `,

  //       left: 'center'
  //     },
  //     tooltip: {
  //       trigger: 'item',
  //     },
  //     legend: {
  //       orient: 'horizontal',
  //       bottom: 0,
  //       data: this.incomeData?.map(item => item.name)
  //     },
  //     toolbox: {
  //       feature: {
  //         saveAsImage: {
  //           title: ""
  //         }
  //       }
  //     },
  //     series: [
  //       {
  //         type: 'pie',
  //         radius: '65%',
  //         center: ['50%', '50%'],
  //         selectedMode: 'single',
  //         data: this.incomeData,
  //         emphasis: {
  //           itemStyle: {
  //             shadowBlur: 10,
  //             shadowOffsetX: 0,
  //             shadowColor: 'rgba(0, 0, 0, 0.5)'
  //           }
  //         }
  //       }
  //     ]
  //   };
  //   option && myChart.setOption(option);
  // }

  // buildOutcomeChart() {
  //   var chartDom = document.getElementById('outcomeChart');
  //   var myChart = echarts.init(chartDom);
  //   var option;
  //   option = {
  //     title: {
  //       text: this.outcomeData.length > 0 ? 'Chi' : `No data `,
  //       left: 'center'
  //     },
  //     tooltip: {
  //       trigger: 'item',
  //     },
  //     legend: {
  //       orient: 'horizontal',
  //       bottom: 0,
  //       data: this.outcomeData?.map(item => item.name)
  //     },
  //     toolbox: {
  //       feature: {
  //         saveAsImage: {
  //           title: ""
  //         }
  //       }
  //     },
  //     series: [
  //       {
  //         type: 'pie',
  //         radius: '65%',
  //         center: ['50%', '50%'],
  //         selectedMode: 'single',
  //         data: this.outcomeData
  //         ,
  //         emphasis: {
  //           itemStyle: {
  //             shadowBlur: 10,
  //             shadowOffsetX: 0,
  //             shadowColor: 'rgba(0, 0, 0, 0.5)'
  //           }
  //         }
  //       }
  //     ]
  //   };
  //   option && myChart.setOption(option);
  // }

  getDataForPieChart(fromDate, toDate) {
    // this.isLoadingChart = true
    // this.dashBoardService.GetPieChartIncoming(fromDate, toDate, this.isByPeriod).subscribe(rs => {
    //   this.incomeData = rs.result
    //   this.isLoadingChart = false
    //   this.buildIncomeChart()
    // },
    //   () => this.isLoadingChart = false)

    // this.dashBoardService.GetPieChartOutcoming(fromDate, toDate, this.isByPeriod).subscribe(rs => {
    //   this.outcomeData = rs.result
    //   this.isLoadingChart = false
    //   this.buildOutcomeChart()
    // },
    //   () => this.isLoadingChart = false)
      this.isLoadingChart = true
      let input = {
        circleChartIds: this.listChartId,
        startDate: fromDate,
        endDate: toDate
      } as InputListCircleChartDto;
      this.dashBoardService.GetCircleChart(input).subscribe(rs => {
        this.circleChartData = rs.result
        this.isLoadingChart = false
      },
        () => this.isLoadingChart = false)
  }


  nextOrPre(title: any) {

    if (this.viewChange.value == this.APP_CONSTANT.TypeViewHomePage.CustomTime) {
      return
    }
    if (title == 'pre') {
      this.activeView--;
    }
    if (title == 'next') {
      this.activeView++;

    }
    this.changeView();
  }

  getCircleChartActive(){
    this.circleChartService.getAllActive().subscribe((data) => {
      this.listCircleChart = data.result.map(item => {
        item.name = item.name;
        item.value = item.id;
        return item;
      })
    })
  }

  onCircleChartSelect(ids: number[]) {
    this.listChartId = ids;
    this.getDataForPieChart(this.fromDate, this.toDate)
  }

  onRefreshCircleChart(){
    this.getDataForPieChart(this.fromDate, this.toDate)
  }

  onDateChangeCircleChart(event: DateTimeSelectorHome){
    let data = event;
    this.searchWithDateTimeCircleChart = data;
    this.defaultDateFilterTypeCircleChart = data.dateType;
    this.searchWithDateTimeCircleChart.dateType = data.dateType
    this.fromDate = moment(this.searchWithDateTimeCircleChart.fromDate).format("YYYY-MM-DD");
    this.toDate = moment(this.searchWithDateTimeCircleChart.toDate).format("YYYY-MM-DD")
    this.getDataForPieChart(this.fromDate, this.toDate)
  }


  changeView(reset?: boolean, fDate?: any, tDate?: any) {
    if (reset) {
      this.activeView = 0;
    }
    let fromDate, toDate;
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Week) {
      fromDate = moment().startOf('isoWeek').add(this.activeView, 'w');
      toDate = moment(fromDate).endOf('isoWeek');
      this.typeDate = 'Week';
    }
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Month) {
      fromDate = moment().startOf('M').add(this.activeView, 'M');
      toDate = moment(fromDate).endOf('M');
      this.typeDate = 'Month';
    }
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Quater) {
      fromDate = moment().startOf('Q').add(this.activeView, 'Q');
      toDate = moment(fromDate).endOf('Q');
      this.typeDate = 'Quater';
    }
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Half_Year) {
      const currentDate = moment();
      const currentMonth = currentDate.month();
      const isFirstHalf = currentMonth < 6;
    
      if (isFirstHalf) {
        fromDate = moment().startOf('year').add(this.activeView * 6, 'months');
        toDate = moment(fromDate).add(5, 'months').endOf('month');
      } else {
        fromDate = moment().startOf('year').add((this.activeView * 6) + 6, 'months');
        toDate = moment(fromDate).add(5, 'months').endOf('month');
      }
    
      this.typeDate = 'Half-Year';
    }
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Year) {
      fromDate = moment().startOf('y').add(this.activeView, 'y');
      toDate = moment(fromDate).endOf('y');
      this.typeDate = 'Years';
    }
    if (this.viewChange.value == this.APP_CONSTANT.TypeViewHomePage.AllTime) {
      fromDate = '';
      toDate = '';
      this.distanceFromAndToDate = 'All Time';
    }
    if (this.viewChange.value == this.APP_CONSTANT.TypeViewHomePage.CustomTime) {
      fromDate = '';
      toDate = '';
      if (!reset && fDate && tDate) {
        if (fDate && tDate) {
          fromDate = fDate.format('DD MMM YYYY');
          toDate = tDate.format('DD MMM YYYY');
        }
        this.setFromAndToDate(fromDate, toDate);
        this.distanceFromAndToDate = fromDate + '  -  ' + toDate
      } else {
        this.distanceFromAndToDate = 'Custom Time';
      }
    }

    if (fromDate != '' && toDate != '') {
      let fDate = '', tDate = '';
      let list = [];
      list[0] = { value: fromDate.isSame(toDate, 'year'), type: 'YYYY' };
      list[1] = { value: fromDate.isSame(toDate, 'month'), type: 'MM' };
      list[2] = { value: fromDate.isSame(toDate, 'day'), type: 'DD' };
      list.map(value => {
        if (value.value) {
          tDate = toDate.format(value.type) + ' ' + tDate;
        } else {
          fDate = fromDate.format(value.type) + ' ' + fDate;
          tDate = toDate.format(value.type) + ' ' + tDate;
        }
      })
      this.distanceFromAndToDate = fDate + ' - ' + tDate;

    }
    if (this.viewChange.value != this.APP_CONSTANT.TypeViewHomePage.CustomTime) {
      fromDate = fromDate == '' ? '' : fromDate.format('YYYY-MM-DD');
      toDate = toDate == '' ? '' : toDate.format('YYYY-MM-DD');
      this.setFromAndToDate(fromDate, toDate);
    }
  }


  setFromAndToDate(fromDate, toDate) {
    this.fromDate = fromDate;
    this.toDate = toDate;
  }

  showPopup(): void {
    let popup = this.dialog.open(PopupComponent);
    popup.afterClosed().subscribe(result => {
      if (result != undefined) {
        if (result.result) {
          this.changeView(false, result.data.fromDateCustomTime, result.data.toDateCustomTime);
        }
      }
    });
  }

  typeDate: any
  fromDate: any;
  toDate: any;
  exportReport() {
    this.dialog.open(ExportReportComponent, {
      width: "600px"
    })
  }

  overviewOutcomingEntryStatistics() {
    this.isLoadingStatistic = true
    this.dashBoardService.OverviewOutcomingEntryStatistics().subscribe((rs) => {
      if (rs) {
        this.outcomingEntryStatistics = rs.result.map((item) => {
          if(item.statusCode === 'PENDINGCEO' ){ 
            return {...item, statusCode: 'PENDINGCEO_OR_YCTDPENDINGCEO'}
          }
          return item
        });
        this.isLoadingStatistic = false
      }
    },
      () => this.isLoadingStatistic = false)
  }
  overviewInvoiceStatistics() {
    this.isLoadingStatistic = true

    this.dashBoardService.OverviewInvoiceStatistics().subscribe((rs) => {
      if (rs) {
        this.invoiceStatistics = rs.result;
        this.isLoadingStatistic = false

      }
    },
      () => this.isLoadingStatistic = false)
  }
  overviewBTransactionStatistics() {
    this.isLoadingStatistic = true
    this.dashBoardService.OverviewBTransactionStatistics().subscribe((rs) => {
      if (rs) {
        this.bTransactionStatistics = rs.result
        this.isLoadingStatistic = false
      }
    },
      () => this.isLoadingStatistic = false)
  }

  getDataBaoCaoChung() {
    let from = moment(this.baoCaoFromDate).format("YYYY-MM-DD")
    let to = moment(this.baoCaoToDate).format("YYYY-MM-DD")
    this.dashBoardService.GetDataBaoCaoChung(from, to).subscribe(rs => {
      this.baoCaoChung = rs.result
    })
  }

  onBaoCaoFilter() {
    this.getDataBaoCaoChung()
  }

  onDateChangeBaoCaoThuChi(event: DateTimeSelectorHome){
    let data = event;
    this.searchWithDateTimeBaoCaoThuChi = data;
    this.defaultDateFilterTypeBaoCaoThuChi = data.dateType;
    this.searchWithDateTimeBaoCaoThuChi.dateType = data.dateType;
    this.baoCaoFromDate = this.searchWithDateTimeBaoCaoThuChi.fromDate
    this.baoCaoToDate = this.searchWithDateTimeBaoCaoThuChi.toDate
    this.getDataBaoCaoChung();
  }

  getExpenditureRequestUrl(param) {
    return '/app/expenditure-request?status=' + param.statusCode;
  }
  getBtransactionUrl(param) {
    return '/app/btransaction?status=' + param;
  }
  getInvoiceUrl() {
    return '/app/nrevenue?status=CHUA_TRA&TRA_1_PHAN';
  }

  checkSendPendingCEO(item: OverviewOutcomingEntryStatisticsDto) {
    return this.isShowRemindCEOBtn()
      && item.statusCode == 'PENDINGCEO' && !item.statusName.trim().toLowerCase().includes('[yctđ] chờ ceo duyệt') && item.count > 0
  }
  checkSendPendingCEORequestChange(item: OverviewOutcomingEntryStatisticsDto) {
    return this.isShowRemindCEOBtn()
      && item.statusCode == 'PENDINGCEO' && item.statusName.trim().toLowerCase().includes('[yctđ] chờ ceo duyệt') && item.count > 0
  }

  getIndexTotalIncome(list) {
    return list.length - 2
  }

  getIndexTotalOutcome(list) {
    return list.length - 1

  }

  isShowExportBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.DashBoard_ExportReport);
  }

  isShowRemindCEOBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.DashBoard_XemKhoiRequestChi_NhacnhoCEO);
  }
  isDisplayHomePage() {
    return this.isGranted(PERMISSIONS_CONSTANT.Dashboard)
  }

  isTotal(index: number) {
    if (index == (this.baoCaoChung.length - 1)) {
      return true
    }
    return false
  }

  chiKhongThucColor(index:number,checknumber:string):boolean{
    return !this.isTotal(index) && checknumber !== '0'
  }

  viewBaoCaoThuDetail(tinhVaoDoanhThu: boolean, isDoanhThu : any) {
    this.dialog.open(DetailBaocaoThuComponent, {
      width: "90vw",
      maxWidth: "90vw",
      data: {
        startDate: moment(this.baoCaoFromDate).format("YYYY-MM-DD"),
        endDate: moment(this.baoCaoToDate).format("YYYY-MM-DD"),
        tinhVaoDoanhThu: tinhVaoDoanhThu,
        isDoanhThu : isDoanhThu
      },
      disableClose: true
    })
  }

  viewBaoCaoChiDetail(branchName:string,branchId:number, expenseType:number ) {
    this.dialog.open(DetailBaocaoChiComponent, {
      width: "90vw",
      maxWidth: "90vw",
      data: {
        startDate: moment(this.baoCaoFromDate).format("YYYY-MM-DD"),
        endDate: moment(this.baoCaoToDate).format("YYYY-MM-DD"),        
        branchName: branchName,
        branchId: branchId,
        expenseType: expenseType

      },
      disableClose: true
    })
  }

  getHRMDebtStatistic(){
    this.dashBoardService.GetHRMDebtStatistic().subscribe(rs => {
      this.debtStatistic = rs.result
    })
  }

  viewDetailNhanVienNo(){
    this.dialog.open(DetailNhanvienNoComponent, {
      width: "1200px",
      data: this.debtStatistic
    })
  }
}
export class totalStatusDto {
  totalPending: number
  totalApprove: number
  totalTransfered: number
  totalEnd: number
  totalRequestChangePending: number
}
export class CashFlowDashBoardByYear {
  month: string;
  totalIncomingByMonth: number;
  totalOutcomingByMonth: number;
}
export class OverviewOutcomingEntryStatisticsDto {
  statusId: number;
  statusCode: string;
  statusName: string;
  count: number;
  index: number;
}
export class OverviewInvoiceStatisticsDto {
  quantityInvoiceDebt: number;
  invoiceCurrencies: InvoiceCurrenciesDto[]
}
export class InvoiceCurrenciesDto {
  currencyId: number;
  currencyCode: string;
  value: number;
  valueFormat: string;
}

export class OverviewBTransactionStatisticsDto {
  quantity: number;
  statusName: string;
}

export interface CashFlowDataDto {
  year: string,
  labels: string[],
  charts: ChartDataDto[]
}

export interface ChartDataDto {
  name: string,
  itemStyle: ChartStyleDto,
  type: string,
  data: number[],
  total: number
}
export interface ChartStyleDto {
  color: string
}

export interface BaoCaoChungDto {
  branchId: string,
  branchName: number,
  tongThu: number,
  tongThuFormat: string,
  tongThuThuc: number
  tongThuThucFormat: string,
  thuKhongThuc: number,
  thuKhongThucFormat: string,
  tongChi: number,
  tongChiFormat: string,
  tongChiThuc: number,
  tongChiThucFormat: string,
  chiKhongThuc: number,
  chiKhongThucFormat: string,
  du: number,
  duFormat: string,
  duThuc: number,
  duThucFormat: string
}

export interface BaoCaoThuDto {
  id: number,
  name: string
  clientName: string,
  branchId: number,
  branchName: string,
  month: number,
  year: number,
  monthYear: string,
  transactionDate: string
  value: number,
  valueFormat: string,
  currencyName: string,
  exchangeRate: number,
  exchangeRateFormat: string,
  totalVND: number,
  totalVNDFormat: string,
  incomingEntryType: number,
  isDoanhThu: boolean,
  tinhDoanhThu: string,
  bankTransactionId: number
}

export interface BaoCaoChiDto {
  id: number,
  name: string,
  detailName: string
  total: number,
  totalFormat: string,
  branchId: number,
  branchName: string,
  expenseType: number,
  reportDate: string,
  exchangeRate: number,
  exchangeRateFromat: string,
  totalVND: number,
  totalVNDFormat: string,
  currencyName: string,
  outcomingEntryType: string,
  laChiPhi: string
}

export interface HrmDebtDto{
  listDebtEmployees: EmployeeDebtDto[],
  totalLoan: number,
  employeeCount:number
}
export interface EmployeeDebtDto{
  email: string,
  fullName: string,
  money: number,
  startDate: string,
  interestRate: number,
  note: string
}

export const baoCaoFilterOption = {  
  REAL_EXPENSE: 0,
  NON_EXPENSE: 1
}
