import { BaseApiService } from "./base-api.service";
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CircleChartInfoDto } from "../model/circle-chart.dto";

@Injectable({
  providedIn: 'root'
})
export class CircleChartDetailService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  
  changeUrl() {
    return 'CircleChartDetail';
  }

  public GetCircleChartDetailsByChartId(circleChartId:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetCircleChartDetailsByChartId?circleChartId=${circleChartId}`,{});
  }
  public GetCircleChartDetailInfoById(id:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetCircleChartDetailInfoById?id=${id}`,{});
  }
  public UpdateInOutcomeTypeIds(item: any): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateInOutcomeTypeIds', item);
}
}
