import { ApiResponse } from './../model/api-response.model';
import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { BaseApiService } from "./base-api.service";
import { HttpClient } from "@angular/common/http";
import { StatusEnum } from "@shared/AppEnums";
import { InputFilterRevenue, RevenuesDto } from "@app/modules/revenue/revenue.component";

@Injectable({
  providedIn: "root",
})
export class RevenueService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return "IncomingEntryType";
  }

  public getAllByStatus(input: InputFilterRevenue): Observable<ApiResponse<RevenuesDto[]>> {
      return this.http.post<any>(this.rootUrl + `/GetAll`, input);
  }

  public updateRevenue(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/Update", item);
  }

  public GetExistInComeInChartSetting(id: number): Observable<any> {
    return this.http.get<any>(
      this.rootUrl + `/GetExistInComeInChartSetting?lineChartId=${id}`
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
}
