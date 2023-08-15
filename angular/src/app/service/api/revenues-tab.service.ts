import { Injectable } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { RevenueRecordService } from "./revenue-record.service";

@Injectable({
  providedIn: 'root'
})
export class RevenuesTabService extends BaseApiService {
  changeUrl() {
    return "RelationInOutEntry";
  }

  constructor(http: HttpClient) {
    super(http);
  }

  getInbyOut(id): Observable<any> {
    return this.http.get(this.rootUrl + "/GetInByOutId?Id=" + id)
  }

  deleteIncoming(incomeId, outcomeId): Observable<any> {
    return this.http.delete(this.rootUrl + `/Delete?IntId=${incomeId}&OutId=${outcomeId}`)
  }

  SetIsRefund(id: number, isRefund: boolean): Observable<any> {
    return this.http.post(this.rootUrl + "/SetIsRefund", { id: id, isRefund: isRefund })
  }
}
