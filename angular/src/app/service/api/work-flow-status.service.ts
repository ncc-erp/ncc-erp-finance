import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { GetWorkflowStatusDto } from '@app/modules/expenditure-request/expenditure-request.component';
import { Observable } from 'rxjs';
import { ApiResponse } from '../model/api-response.model';
import { BaseApiService } from './base-api.service';
@Injectable({
  providedIn: 'root'
})
export class WorkFlowStatusService extends BaseApiService {
  changeUrl() {
    return 'WorkflowStatus';
  }

  constructor(http: HttpClient) {
    super(http);
  }
  GetAllForDropdown(): Observable<any> {
    return this.http.get(this.rootUrl + '/GetAllForDropDown')
  }
  public getById(id: any): Observable<any> {
    return this.http.get<any>(this.rootUrl + '/GetByWorkflow?workflowId=' + id);
  }
  GetAllForDropDownAndNotEqualsEnd(): Observable<any> {
    return this.http.get(this.rootUrl + '/GetAllForDropDownAndNotEqualsEnd')
  }
  getTempOutComingEntryStatusOptions(): Observable<ApiResponse<GetWorkflowStatusDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetTempOutComingEntryStatusOptions');
  }

  GetStatusForOutcomeFilter(): Observable<ApiResponse<GetWorkflowStatusDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetStatusForOutcomeFilter');
  }
}
