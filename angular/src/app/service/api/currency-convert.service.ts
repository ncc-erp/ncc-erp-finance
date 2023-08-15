import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class CurrencyConvertService extends BaseApiService{
  changeUrl() {
    return "CurrencyConvert";
  }

  constructor(http: HttpClient) {
    super(http)
  }
  getMonthOfCurrencyConvert():Observable<any>{
    return this.http.get<any>(this.rootUrl + "/GetMonthOfCurrencyConvert");
  }
  getYearOfCurrencyConvert():Promise<any>{
    return this.http.get<any>(this.rootUrl + "/GetYearOfCurrencyConvert").toPromise();
  }
}
