import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { revenueRecordDto } from '@app/modules/finance-detail/create-edit-record/create-edit-record.component';
import { throwError, Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { UpdateIncomeTypeDto } from '../model/n-revenue.model';

@Injectable({
  providedIn: 'root'
})
export class RevenueRecordService extends BaseApiService {

  constructor(http: HttpClient) {
    super(http)
  }

  changeUrl() {
    return "IncomingEntry";
  }

  getAllByTransactionId(id) {
    return this.http.get<any>(this.rootUrl + '/GetAllByBankTransaction?bankTransactionId=' + id);
  }

  GetTotalByCurrency(request: PagedRequestDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/GetTotalByCurrency', request);
  }

  getAllClient() {
    return this.http.get<any>(this.rootUrl + '/GetAllClient')
  }

  updateIcomeType(input: UpdateIncomeTypeDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/UpdateIcomeType', input);
  }

  handleError(error: any) {
    let errorMessage = '';

    if (error.error instanceof ErrorEvent) {

      errorMessage = `Error: ${error.error.message}`;
    } else {

      errorMessage = `Error: ${error.error.error.message}`;
    }

    abp.notify.error(errorMessage);
    return throwError(errorMessage);
  }

  exportExcel(request: PagedRequestDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/ExportExcel', request);
  }
}
