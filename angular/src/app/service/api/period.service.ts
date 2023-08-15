import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../model/api-response.model';
import {BaseApiService} from './base-api.service'
@Injectable({
  providedIn: 'root'
})
export class PeriodService extends BaseApiService{
  changeUrl() {
    return "Period"
  }

  constructor(http: HttpClient) {
    super(http);
  }


  public closeAndCreatePeriod(input): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/CloseAndCreatePeriod', input);
  }

  public createTheFirstTime(input): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/CreateTheFirstTime', input);
  }

  public isTheFirstRecord():Observable<any>{
    return this.http.get<any>(this.rootUrl + '/IsTheFirstRecord');
  }

  public update(input):Observable<any>{
    return this.http.put<any>(this.rootUrl + '/Update', input);
  }

  public previewBeforeWhenClosePeriod():Observable<any>{
    return this.http.get<any>(this.rootUrl + '/PreviewBeforeWhenClosePeriod');
  }

  public checkDiffRealBalanceAndBTransaction(input):Observable<any>{
    return this.http.post<any>(this.rootUrl + '/CheckDiffRealBalanceAndBTransaction', input);
  }
  public getFirstPeriod():Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + '/GetFirstPeriod');
  }


}
