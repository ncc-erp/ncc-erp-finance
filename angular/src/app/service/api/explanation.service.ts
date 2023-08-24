import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: 'root'
})
export class ExplanationService extends BaseApiService {

  constructor(http: HttpClient) {
    super(http)
  }
  changeUrl() {
    return "Explanation";
  }
  GetAllByType(startDate, endDate, type): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllByType?startDate=${startDate}&endDate=${endDate}&type=${type}`);
  }

  public updateExplanation(item: any): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/Update', item);
  }
}
