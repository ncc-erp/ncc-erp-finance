import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class WorkFlowTransitionService extends BaseApiService {
  changeUrl() {
    return 'WorkflowStatusTransition';
  }

  constructor(http: HttpClient) {
    super(http);
  }
  public getRolesByTransition(id: any): Observable<any> {
    return this.http.get<any>(this.rootUrl + '/GetRoleByTransition?id=' + id);
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
}
