import { IOption } from './../../../shared/components/custome-select/custome-select.component';
import { ApiResponse, ApiPagingResponse } from './../model/api-response.model';
import { PagedRequestDto } from './../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { AuditlogDto, EmailAddressInAuditLog } from './../model/auditlog.model';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuditLogPagedRequestDto } from '@app/modules/auditlog/auditlog.component';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService extends BaseApiService {
  changeUrl() {
    return 'AuditLog';
  }

  constructor(httpClient: HttpClient) {
    super(httpClient);
  }
  getAllPaging(payload: AuditLogPagedRequestDto): Observable<ApiPagingResponse<AuditlogDto[]>> {
    return this.http.post<any>(this.rootUrl + '/GetAllPagging', payload);
  }
  getEmailAddressInAuditLog(): Observable<ApiResponse<IOption[]>>{
    return this.http.get<ApiResponse<IOption[]>>(this.rootUrl + '/GetAllEmailAddressInAuditLog');
  }
  getAllMethodName(): Observable<ApiResponse<IOption[]>>{
    return this.http.get<ApiResponse<IOption[]>>(this.rootUrl + '/GetAllMethodName');
  }
  getAllServiceName(): Observable<ApiResponse<IOption[]>>{
    return this.http.get<ApiResponse<IOption[]>>(this.rootUrl + '/GetAllServiceName');
  }
}
