import { BaseApiService } from "./base-api.service";
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { AddLinechartSettingDto } from "../model/linechartSetting.dto";

@Injectable({
  providedIn: 'root'
})
export class LinechartSettingService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'LineChartSetting';
  }

  public addReferenceToLineChart(input: AddLinechartSettingDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/AddReferenceToLineChart',input);
  }

  public getLineChartInCome(id:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetLineChartInCome?chartId=${id}`);
  }

  public getLineChartOutCome(id:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetLineChartOutCome?chartId=${id}`);
  }

  public RemoveLineChartReference(input): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/RemoveLineChartReference`,input);
  }

  public DeActive(id:number): Observable<any> {
    return this.http.put<any>(this.rootUrl + `/DeActive?id=${id}`,{});
  }

  public Active(id:number): Observable<any> {
    return this.http.put<any>(this.rootUrl + `/Active?id=${id}`,{});
  }
}
