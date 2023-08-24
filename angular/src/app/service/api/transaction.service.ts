import { PagedRequestDto } from "./../../../shared/paged-listing-component-base";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BankDto, BankTransactionDto } from "@app/modules/banking-transaction/banking-transaction.component";
import { Observable, throwError } from "rxjs";
import { BaseApiService } from "./base-api.service";
import { ApiResponse } from "../model/api-response.model";

@Injectable({
  providedIn: "root",
})
export class TransactionService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return "BankTransaction";
  }
  updateBankTransaction(item: BankTransactionDto) {
    return this.http.post<any>(this.rootUrl + "/edit", item);
  }

  deleteTransition(id) {
    return this.http.delete<any>(
      this.rootUrl + "/DeleteFromOutcomingEntry" + `?id=${id}`
    );
  }
  public lockBankTransaction(id: any): Observable<any> {
    return this.http.post<any>(
      this.rootUrl + `/LockBankTransaction?banktransactionId=${id}`,
      {}
    );
  }
  public unlockBankTransaction(id: any): Observable<any> {
    return this.http.post<any>(
      this.rootUrl + `/UnlockBankTransaction?banktransactionId=${id}`,
      {}
    );
  }

  getAllFromBankAccount(): Observable<ApiResponse<BankDto[]>> {
    return this.http.get<any>(
      this.rootUrl + "/GetAllFromBankAccountInTransaction"
    );
  }

  getAllToBankAccount() {
    return this.http.get<any>(
      this.rootUrl + "/GetAllToBankAccountInTransaction"
    );
  }

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
  exportExcel(request: PagedRequestDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/ExportExcel", request);
  }
  exportNewExcel(request: PagedRequestDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/NewExportExcel", request);
  }
}
