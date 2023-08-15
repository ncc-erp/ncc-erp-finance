import { InvoiceCreateEditDto } from './../../model/n-revenue.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseApiService } from '../base-api.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AutoPaidDto, UpdateNoteDto, UpdateStatusInvoiceDto } from '@app/service/model/n-revenue.model';
import { ApiResponse } from '@app/service/model/api-response.model';
import { IncomingClientPayDeviant } from '@app/modules/new-versions/n-revenue/client-pay-deviant-dialog/client-pay-deviant-dialog.component';
import { InputToGetAllPaging } from '@app/modules/new-versions/n-revenue/list-n-revenue/list-n-revenue.component';

@Injectable({
  providedIn: 'root'
})
export class NRevenueService extends BaseApiService {
  changeUrl() {
    return 'invoice';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  createInvoice(item: InvoiceCreateEditDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/CreateInvoice', item);
  }
  updateInvoice(item: InvoiceCreateEditDto): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateInvoice', item);
  }

  updateNote(item: UpdateNoteDto): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateNote', item);
  }

  updateStatus(item: UpdateStatusInvoiceDto): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateStatus', item);
  }
  checkAutoPaid(accountId:number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/CheckAutoPaid?accountId=${accountId}`);
  }
  autoPaidForAccount(item: AutoPaidDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/AutoPaidForAccount', item);
  }

  deleteInvoice(id: any): Observable<any> {
    return this.http.delete<any>(this.rootUrl + '/Delete', {
        params: new HttpParams().set('Id', id)
    })
  }
  setDoneInvoice(invoiceId: number): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/SetDoneInvoice?invoiceId=${invoiceId}`, {})
  }
  checkSetDoneInvoice(invoiceId: number): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/CheckSetDoneInvoice?invoiceId=${invoiceId}`, {})
  }
  setClientPayDeviant(data: IncomingClientPayDeviant): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + '/SetClientPayDeviant', data);
  }
  getAllPagingRevenue(request: InputToGetAllPaging): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/GetAllPaging', request);
  }
  ExportReport(): Observable<any> {
    return this.http.get<any>(this.rootUrl + "/ExportReport");
  }
}
