import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: 'root'
})
export class ComparativeStatisticService extends BaseApiService {


  constructor(http: HttpClient) {
    super(http)
  }
  changeUrl() {
    return "ComparativeStatistic";
  }
  getComparative(startDate, endDate): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/Get?startDate=${startDate}&endDate=${endDate}`);
  }


}
