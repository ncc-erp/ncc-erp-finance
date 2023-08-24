import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AccountDto } from "@app/modules/accountant-account/accountant-account.component";
import { Observable } from "rxjs";
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: "root",
})
export class AccountantAccountService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return "Account";
  }

  changeStatus(item: AccountDto) {
    return this.http.post<any>(this.rootUrl + "/ChangeStatus", item);
  }
}
