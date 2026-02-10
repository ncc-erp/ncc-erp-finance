import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { GetAllPagingOutComingEntryDto } from '@app/modules/report/report.component';
import { Observable, throwError } from "rxjs";
import { ApiResponse } from "../model/api-response.model";
import { expenditureDto } from './../../modules/report/report.component';
import { ResultGetOutcomingEntryDto } from '@app/modules/report/report.component';

@Injectable({
  providedIn: 'root'
})
export class ReportService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'Report';
  }

  getAllByFilter(request: GetAllPagingOutComingEntryDto): Observable<ApiResponse<expenditureDto[]>> {
    return this.http.post<any>(this.rootUrl + `/GetTreeByOutcomingFilter`, request);
  }

  getAllPaging(request: GetAllPagingOutComingEntryDto): Observable<ApiResponse<ResultGetOutcomingEntryDto>> {
      return this.http.post<any>(this.rootUrl + `/GetAllPaging`, request);
    }
}
