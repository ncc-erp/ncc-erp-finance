import { Observable, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';


@Injectable({
  providedIn: 'root'
})
export class SupplierService extends BaseApiService {
  changeUrl() {
    return "Supplier"
  }

  constructor(http: HttpClient) {
    super(http);
  }
  public getAllByOutcomingEntry(id: any): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllByOutcomingEntry?OutcomingEntryId=${id}`);
  }
  public GetAllForDropdown(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllForDropdown`);
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
