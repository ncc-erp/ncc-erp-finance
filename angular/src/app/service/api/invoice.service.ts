import { PagedRequestDto } from './../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs-compat';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'Invoice';
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
