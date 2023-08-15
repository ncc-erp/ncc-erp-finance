import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class AccountTypeService extends BaseApiService{

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'AccountType';
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
