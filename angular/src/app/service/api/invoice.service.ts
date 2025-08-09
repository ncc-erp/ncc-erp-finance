import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseApiService } from './base-api.service';
import { ApiResponse } from '@app/service/model/api-response.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http); // <-- quan trọng: truyền http cho BaseApiService để có this.http
  }

  // => /api/services/app/Invoice
  changeUrl() {
    return 'Invoice';
  }

  // GET /api/services/app/Invoice/GetListInvoiceByAccountId?accountId=...
  getListInvoiceByAccountId(accountId: number): Observable<ApiResponse<InvoiceItemDto[]>> {
    return this.http.get<any>(
      this.rootUrl + `/GetListInvoiceByAccountId?accountId=${accountId}`
    );
  }
}

export interface InvoiceItemDto {
  invoiceId: number;
  remainValue: number;
  currencyName: string;
}

  //public 
  // public deleteInvoice(id: any): Observable<any> {
  //   return this.http.delete<any>(this.rootUrl + '/Delete', {
  //     params: new HttpParams().set('invoiceId', id)
  //   })
  // }
  // public getInvoiceById(id: any): Observable<any> {
  //   return this.http.get<any>(this.rootUrl + '/Get?invoiceId=' + id);
  // }
  // public getProjectTimeSheet(invoiceId: any): Observable<any> {
  //   return this.http.get<any>(this.rootUrl + '/InvoiceProjectTimesheet?invoiceId=' + invoiceId);
  // }
  // public DownloadFileTimesheetProject(invoiceId: any): Observable<any> {
  //   return this.http.get<any>(this.rootUrl + '/DownloadFileTimesheetProject?invoiceDetailId=' + invoiceId);
  // }
  // public getBankTransactionsByInvoice(invoiceId: any){
  //   return this.http.get<any>(this.rootUrl + '/GetBankTransactionsByInvoice?id=' + invoiceId);
  // }
  // handleError(error: any) {
  //   let errorMessage = '';

  //   if (error.error instanceof ErrorEvent) {

  //     errorMessage = `Error: ${error.error.message}`;
  //   } else {

  //     errorMessage = `Error: ${error.error.error.message}`;
  //   }

  //   abp.notify.error(errorMessage);
  //   return throwError(errorMessage);
  // }
  // exportExcel(request: PagedRequestDto): Observable<any> {
  //   return this.http.post<any>(this.rootUrl + '/ExportExcel', request);
  // }
}
