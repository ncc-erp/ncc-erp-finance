import { Injectable } from '@angular/core';
import { Observable, throwError } from "rxjs";
import { ApiResponse } from "../model/api-response.model";
import { BaseApiService } from './base-api.service';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { GetAllPagingInComingEntry } from '@app/modules/revenue-report/revenue-report.component';
import { RevenuesDto } from '@app/modules/revenue-report/revenue-report.component';
import { ResultGetIncomingEntryDto } from '@app/modules/revenue-report/revenue-report.component';

@Injectable({
  providedIn: 'root'
})
export class RevenueReportService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  changeUrl() {
    return 'IncomingEntryReport';
  }

  getAllByFilter(request: GetAllPagingInComingEntry): Observable<ApiResponse<RevenuesDto[]>> {
    return this.http.post<any>(this.rootUrl + `/GetTreeByIncomingFilter`, request);
  }

  getAllPaging(request: GetAllPagingInComingEntry): Observable<ApiResponse<ResultGetIncomingEntryDto>> {
      return this.http.post<any>(this.rootUrl + `/GetAllPaging`, request);
    }

}
