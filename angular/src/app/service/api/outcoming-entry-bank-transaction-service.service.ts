import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { Observable } from 'rxjs-compat';
import { BaseApiService } from '../api/base-api.service';
@Injectable({
  providedIn: 'root'
})
export class OutcomingEntryBankTransactionServiceService extends BaseApiService {
  changeUrl() {
    return 'OutcomingEntryBankTransaction';
  }

  constructor(http: HttpClient) {
    super(http);
  }
  public getAllOutcomingEntryByTransaction(bankTransactionId: number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllOutcomingEntryByTransaction?bankTransactionId=${bankTransactionId}`);
  }
  public saveMultipleRequest(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/SaveMultipleRequest`,item);
  }
  public getAllOutcomingEntryByStatusEND(id: any): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/getAllOutcomingEntryByStatusEND?bankTransactionId=${id}`);
  }
  public deleteLinkedTransaction(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/DeleteLinkedTransaction', item)
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
