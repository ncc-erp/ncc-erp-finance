import { Observable, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';


@Injectable({
  providedIn: 'root'
})
export class OutcomingEntrySupplierService extends BaseApiService {

  changeUrl() {
    return 'OutcomingEntrySupplier';
  }

  constructor(http: HttpClient) {
    super(http);
  }
  public RemoveSupplierFromOutcomingEntry(id:any): Observable<any> {
    return this.http.delete<any>(this.rootUrl + `/RemoveSupplierFromOutcomingEntry?Id=${id}`);
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
