import { BaseApiService } from "./base-api.service";
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CircleChartService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'CircleChart';
  }

  public DeActive(id:number): Observable<any> {
    return this.http.put<any>(this.rootUrl + `/DeActive?id=${id}`,{});
  }

  public Active(id:number): Observable<any> {
    return this.http.put<any>(this.rootUrl + `/Active?id=${id}`,{});
  }
}
