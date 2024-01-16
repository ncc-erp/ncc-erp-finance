import { addMultipleRequestDto } from "./../../modules/expenditure-request-detail/create-multi-transaction/create-multi-transaction.component";
import { PagedRequestDto } from "./../../../shared/paged-listing-component-base";
import { throwError, Observable } from "rxjs";
import { HttpClient, HttpParams, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import {
  DeleteOutcomingEntryDetailDto,
  DetailRequestDto,
  GetOutcomingEntryDetailDto,
} from "@app/modules/expenditure-request-detail/detail-tab/detail-tab.component";
import { ApiPagingResponse, ApiResponse } from "../model/api-response.model";
import { ResultGetOutcomingEntryDetailDto } from "@app/modules/expenditure-request-detail/detail-tab/normal-detail/normal-detail-table/normal-detail-table.component";

@Injectable({
  providedIn: "root",
})
export class RequestDetailService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return "OutcomingEntryDetail";
  }
  getDetailPaging(
    request: DetailRequestDto
  ): Observable<ApiResponse<ResultGetOutcomingEntryDetailDto>> {
    return this.http.post<any>(this.rootUrl + "/GetAllPaging", request);
  }
  getTransactionDetail(request: DetailRequestDto) {
    return this.http.post<any>(
      this.rootUrl + "/GetTransactionDetails",
      request
    );
  }
  getAllDetail(id: any) {
    return this.http.get<any>(this.rootUrl + "/GetAll?outcomingEntryId=" + id);
  }

  public deleteLinkedTransaction(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/DeleteLinkedTransaction", item);
  }
  addMultipleTransaction(id, requestBody: addMultipleRequestDto) {
    return this.http.post<any>(
      this.rootUrl + "/AddMultipleTransactions" + `?outcomingEntryId=${id}`,
      requestBody
    );
  }
  linkTransaction(requestBody: any) {
    return this.http.post<any>(
      this.rootUrl + "/LinkToExistingTransaction",
      requestBody
    );
  }
  addBankTransaction(id: any, item: any) {
    return this.http.post<any>(
      this.rootUrl + `/AddBanktransaction?outcomingEntryId=${id}`,
      item
    );
  }

  changeDone(item) {
    return this.http.post<any>(this.rootUrl + "/ChangeDone", item);
  }

  // /DeleteLinkedTransaction
  handleError(error: any) {
    let errorMessage = "";

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error: ${error.error.error.message}`;
    }

    abp.notify.error(errorMessage);
    return throwError(errorMessage);
  }
  delete(payload: DeleteOutcomingEntryDetailDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/Delete`, payload);
  }
  downloadFileTemplate(): Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + "/GetTemplateInputOutcomingEntryDetail");
  }

  importFileOutcomingEntryDetail(file, id): Observable<any> {
    const formData = new FormData();
    formData.append('FileInput', file);
    formData.append('OutcomingEntryId', id);
    const uploadReq = new HttpRequest(
      'POST', this.rootUrl + "/ImportFileOutcomingEntryDetail", formData,
      {
        reportProgress: true
      }
    );
    return this.http.request(uploadReq);
  }
  
  public updateBranch(item: any): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateBranch', item);
}
}
