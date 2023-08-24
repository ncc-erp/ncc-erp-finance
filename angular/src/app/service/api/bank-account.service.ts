import { HttpClient } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { Injectable } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import { BankAccountDto } from "../model/bank-account.dto";
import { ActiveBankAccountDto } from "@app/modules/bank-account/bank-account.component";
import { ApiResponse } from "../model/api-response.model";

@Injectable({
  providedIn: "root",
})
export class BankAccountService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  changeUrl() {
    return "BankAccount";
  }
  getAll(): Observable<any> {
    return this.http.get(this.rootUrl + "/GetAll");
  }
  getBankAccountStatement(
    id: any,
    startDate: any,
    endDate: any
  ): Observable<any> {
    return this.http.get(
      this.rootUrl +
        "/BankAccountStatement?bankAccountId=" +
        id +
        "&startDate=" +
        startDate +
        "&endDate=" +
        endDate
    );
  }
  BankAccountStatementByPeriod(
    id: any,
    startDate: any,
    endDate: any
  ): Observable<any> {
    return this.http.get(
      this.rootUrl +
        "/BankAccountStatementByPeriod?bankAccountId=" +
        id +
        "&startDate=" +
        startDate +
        "&endDate=" +
        endDate
    );
  }
  public lockBankAccount(id: any): Observable<any> {
    return this.http.post<any>(
      this.rootUrl + `/LockBankAccount?bankAccountId=${id}`,
      {}
    );
  }
  public unlockBankAccount(id: any): Observable<any> {
    return this.http.post<any>(
      this.rootUrl + `/unlockBankAccount?bankAccountId=${id}`,
      {}
    );
  }

  changeStatus(item: BankAccountDto) {
    return this.http.post<any>(this.rootUrl + "/ChangeStatus", item);
  }

  handleError(error: any) {
    let errorMessage = "";

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.error.message}`;
    } else {
      errorMessage = `Error: ${error.error.error.message}`;
    }
    // abp.notify.error(errorMessage);
    return throwError(errorMessage);
  }
  public exportBankAccount(request: PagedRequestDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/ExportExcel`, request);
  }
  public ExportExcelDetail(
    bankAccountId: any,
    startDate: string,
    endDate: string,
    surPlus: number,
    firstBalance: number,
    lastBalance: number
  ): Observable<any> {
    return this.http.post<any>(
      this.rootUrl +
        `/ExportExcelDetail?bankAccountId=${bankAccountId}&startDate=${startDate}&endDate=${endDate}&Surplus=${surPlus}&firstBalance=${firstBalance}&lastBalance=${lastBalance}`,
      {}
    );
  }
  public active(activeBankAccountDto: ActiveBankAccountDto):Observable<ApiResponse<string>>{
    return this.http.post<any>(this.rootUrl + "/Active", activeBankAccountDto);
  }
  public deActive(bankAccountId: number):Observable<ApiResponse<string>>{
    return this.http.get<any>(this.rootUrl + `/DeActive?bankAccountId=${bankAccountId}`);
  }
  public getByPeriod(id: number):Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + '/GetByPeriod?id=' + id);
  }
  public EditBalanace(editBalanace: ActiveBankAccountDto):Observable<ApiResponse<string>>{
    return this.http.post<any>(this.rootUrl + "/EditBalanace", editBalanace);
  }
}
