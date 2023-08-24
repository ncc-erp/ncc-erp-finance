import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { BaseApiService } from "./base-api.service";
import { AccountDto } from "@app/modules/accountant-account/accountant-account.component";
import { ApiResponse } from "../model/api-response.model";

@Injectable({
  providedIn: "root",
})
export class AccountService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  changeUrl() {
    return "Account";
  }

  getAllAccount(): Observable<any> {
    return this.http.get(this.rootUrl + "/GetAll/");
  }
  public create(payload: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/CreateNew", payload);
  }

  getAccountDefault(): Observable<ApiResponse<AccountDto>> {
    return this.http.get<any>(this.rootUrl + "/GetAccountDefault");
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
}
