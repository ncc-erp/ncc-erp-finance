import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CurrencyDto } from '@app/modules/currency/currency.component';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService extends BaseApiService {
  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  changeUrl() {
    return 'Currency';
  }


  getAllCurrency(): Observable<any> {
    return this.http.get(this.rootUrl + '/GetAll/')
  }
  GetAllForDropdown(): Observable<any> {
    return this.http.get(this.rootUrl + '/GetAllForDropdown')
  }
  opts = [];

  // getData() {
  //   return this.opts.length ?
  //     of(this.opts) :
  //     this.http.get<any>(this.rootUrl + '/GetAll/').pipe(tap(data => this.opts = data.result))
  // }
  getData() {
    return of([{ name: "ziro", cid: "524023240" },
    { name: "plus", cid: "524023240" }])
  }
  getAllCurrencyForDropdown(): Observable<any>{
    return this.http.get(this.rootUrl + '/GetAllCurrencyForDropdown')
  }
  defaultFromBankAccountIdWhenBuy(payload: CurrencyDto): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/SetDefaultFromBankAccountIdWhenBuy ', payload)
  }
  defaultToBankAccountIdWhenBuy(payload: CurrencyDto): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/SetDefaultToBankAccountIdWhenBuy ', payload)
  }
  defaultCurrency(payload: CurrencyDto): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/SetDefaultCurrency ', payload)
  }
  defaultBankAccount(payload: CurrencyDto): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/SetDefaultBankAccount ', payload)
  }
}
