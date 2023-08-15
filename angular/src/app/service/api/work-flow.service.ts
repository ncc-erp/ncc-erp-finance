import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { BaseApiService } from './base-api.service';
@Injectable({
  providedIn: 'root'
})
export class WorkFlowService extends BaseApiService {
  changeUrl() {
    return 'Workflow';
  }

  GetAllForDropdown(): Observable<any> {
    return this.http.get(this.rootUrl + '/GetAllDropDown')
  }
  constructor(http: HttpClient) {
    super(http);
  }
  handleError(error: any) {
    let errorMessage = '';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.error.message}`;
    } else {

      errorMessage = `Error: ${error.error.error.message}`;
    }
    // abp.notify.error(errorMessage);
    return throwError(errorMessage);
  }
}
