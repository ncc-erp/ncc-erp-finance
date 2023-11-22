import { AppConsts } from './../../../shared/AppConsts';
import { PagedRequestDto } from './../../../shared/paged-listing-component-base';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { InputListCircleChartDto } from '../model/circle-chart.dto';
@Injectable({
  providedIn: 'root'
})
export class DashBoardService extends BaseApiService {
/**
 *
 */
constructor( http: HttpClient) {
  super(http);

}
  changeUrl() {
    return 'DashBoard';
  }
  PercentEntryType(startDate:string, endDate:string): Observable<any> {

    return this.http.get(this.rootUrl + `/PercentEntryType?startDate=${startDate}&endDate=${endDate}`)
  }
  GetCashFlowDashBoard(year:string):Observable<any>{
    return this.http.get(this.rootUrl + `/GetCashFlowDashBoard?year=${year}`)
  }

  GetNewChart(startDate:string, endDate:string, isByPeriod:boolean):Observable<any>{
    return this.http.get(this.rootUrl + `/GetNewChart?startDate=${startDate}&endDate=${endDate}&isByPeriod=${isByPeriod}`)
  }

  GetCircleChart(input: InputListCircleChartDto):Observable<any>{
    return this.http.post<any>(this.rootUrl + `/GetCircleChart`, input)
  }

  GetPieChartIncoming(startDate:string, endDate:string, isByPeriod:boolean):Observable<any>{
    return this.http.get(this.rootUrl + `/GetPieChartIncoming?startDate=${startDate}&endDate=${endDate}&isByPeriod=${isByPeriod}`)
  }
  GetPieChartOutcoming(startDate:string, endDate:string, isByPeriod:boolean):Observable<any>{
    return this.http.get(this.rootUrl + `/GetPieChartOutcoming?startDate=${startDate}&endDate=${endDate}&isByPeriod=${isByPeriod}`)
  }


  GetStatusDashBoard():Observable<any>{
    return this.http.get(this.rootUrl + `/GetStatusDashBoard`)

  }
  public ExportExcel(fromDate:string, toDate:string, branch:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/ExportExcel?fromDate=${fromDate}&toDate=${toDate}&branchId=${branch}`);
  }
  public ComparativeStatistics(startDate:any, endDate:any,isReal:boolean): Observable<any>{
    return this.http.get(this.rootUrl + `/ComparativeStatistics?startDate=${startDate}&endDate=${endDate}&isReal=${isReal}`)

  }
  public SendReviewExplain(data: any) {
    return this.http.post(this.rootUrl + `/AddReviewExplain`, data);
  }
  public GetReviewExplain(): Observable<any> {
    return this.http.get(this.rootUrl + `/GetReviewExplain`);
  }
  exportExcelStatistics(fromDate, toDate): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/ExportComparativeStatistics?startDate=${fromDate}&endDate=${toDate}`);
  }
  exportExcelBBC(startDate, endDate, branchId, expenseType){
    if(expenseType === -1){
      return this.http.get<any>(this.rootUrl + `/ExportBCC?startDate=${startDate}&endDate=${endDate}&branchId=${branchId}`);
      }
      return this.http.get<any>(this.rootUrl + `/ExportBCC?startDate=${startDate}&endDate=${endDate}&branchId=${branchId}&isExpense=${expenseType}`);
  }
  public getChart(year):Observable<any>{
    return this.http.get(this.rootUrl + `/GetChart?year=${year}`)

  }

  GetComparativeStatisticsOutBankTransaction(): Observable<any> {
    return this.http.get(this.rootUrl + `/GetComparativeStatisticsOutBankTransaction`)
  }

  GetBankAccountStatistics(isIncludeBTransPending: boolean): Observable<any> {
    return this.http.get(this.rootUrl + `/GetBankAccountStatistics?isIncludeBTransPending=${isIncludeBTransPending }`)
  }

  GetComparativeStatisticsIncomingEntry(): Observable<any> {
    return this.http.get(this.rootUrl + `/GetComparativeStatisticsIncomingEntry`)
  }

  GetComparativeStatisticsOutcomingEntry(): Observable<any> {
    return this.http.get(this.rootUrl + `/GetComparativeStatisticsOutcomingEntry`)
  }

  GetComparativeStatisticByCurrency(): Observable<any> {
    return this.http.get(this.rootUrl + `/GetComparativeStatisticByCurrency`)
  }

  handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error: ${error.error.error.message}`;
    }
    return throwError(errorMessage);
  }

  OverviewOutcomingEntryStatistics(): Observable<any> {
    return this.http.get(this.rootUrl + `/OverviewOutcomingEntryStatistics`)
  }
  OverviewInvoiceStatistics(): Observable<any> {
    return this.http.get(this.rootUrl + `/OverviewInvoiceStatistics`)
  }
  OverviewBTransactionStatistics(): Observable<any> {
    return this.http.get(this.rootUrl + `/OverviewBTransactionStatistics`)
  }
  exportStatisticDashboard(periodId: number, fromDate, toDate): Observable<any>{
    return this.http.get(this.rootUrl + `/ExportStatisticDashboard?periodId=${periodId}&startDate=${fromDate}&endDate=${toDate}`);
  }

  GetDataBaoCaoChung(startDate, endDate): Observable<any>{
    return this.http.get(this.rootUrl + `/GetDataBaoCaoChung?startDate=${startDate}&endDate=${endDate}`);
  }

  GetDataBaoCaoThu(startDate, endDate, isDoanhThu): Observable<any>{
    if(isDoanhThu === null){
      return this.http.get(this.rootUrl + `/GetDataBaoCaoThu?startDate=${startDate}&endDate=${endDate}`);
    }
    return this.http.get(this.rootUrl + `/GetDataBaoCaoThu?startDate=${startDate}&endDate=${endDate}&isDoanhThu=${isDoanhThu}`);
  }

  GetDataBaoCaoThuForCircleChart(startDate, endDate, circleChartDetail): Observable<any>{
    return this.http.post(this.rootUrl + `/GetDataBaoCaoThuForCircleChart?startDate=${startDate}&endDate=${endDate}`, circleChartDetail);
  }

  GetDataBaoCaoChi(startDate, endDate, branchId, expenseType): Observable<any>{
    if(expenseType === -1){
    return this.http.get(this.rootUrl + `/GetDataBaoCaoChi?startDate=${startDate}&endDate=${endDate}&branchId=${branchId}`);
    }
    return this.http.get(this.rootUrl + `/GetDataBaoCaoChi?startDate=${startDate}&endDate=${endDate}&branchId=${branchId}&isExpense=${expenseType}`);
  }

  getDetailBaoCaoChiForCircleChart(startDate, endDate, circleChartDetail): Observable<any>{
    return this.http.post(this.rootUrl + `/GetDataBaoCaoChiForCircleChart?startDate=${startDate}&endDate=${endDate}`, circleChartDetail);
  }
  GetHRMDebtStatistic(): Observable<any>{
    return this.http.get(this.rootUrl + `/GetHRMDebtStatistic`);
  }

}