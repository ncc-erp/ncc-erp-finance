import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class StatusService extends BaseApiService {
  changeUrl() {
    return 'WorkflowStatus';
  }
  constructor(http: HttpClient) {
    super(http);
  }
  handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error: ${error.error.error.message}`;
    }
    return throwError(errorMessage);
  }
}
