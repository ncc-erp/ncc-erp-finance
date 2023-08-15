import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BranchDto } from '@app/modules/branch/branch.component';
import { Observable, throwError } from 'rxjs';
import { ApiResponse } from '../model/api-response.model';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class BranchService extends BaseApiService{

    constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'Branch';
  }
  GetAllForDropdown(): Observable<ApiResponse<BranchDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllForDropdown')
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
