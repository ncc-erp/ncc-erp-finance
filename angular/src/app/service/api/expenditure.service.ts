import { expenditureDto, InputFilterExpenditure } from './../../modules/expenditure/expenditure.component';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { IncomingEntryTypeOptions } from "@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component";
import { StatusEnum } from "@shared/AppEnums";
import { Observable, throwError } from "rxjs";
import { ApiResponse } from "../model/api-response.model";
import { BaseApiService } from "./base-api.service";
import { InputFilterOutcomingEntryType, OutcomingEntryTypesDto } from '@app/users/edit-user/edit-user-dialog.component';

@Injectable({
  providedIn: "root",
})
export class ExpenditureService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return "OutcomingEntryType";
  }

  public getAllByStatus(inputFilter: InputFilterExpenditure): Observable<ApiResponse<expenditureDto[]>> {
    return this.http.post<any>(this.rootUrl + '/GetAll', inputFilter);
  }

  public updateExpenditure(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/Update", item);
  }
  public GetAllByUserId(id: any): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllByUserId?id=${id}`);
  }
  public GetAllForDropdownByUser(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllForDropdownByUser`);
  }
  public GetAllForDropdown(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllForDropdown`);
  }

  public GetAllForLinechartSetting(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllForLinechartSetting`);
  }
  public addToUser(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + "/AddOutcomingTypeToUser", item);
  }
  public addAllOutcomingToUser(item: any): Observable<any> {
    return this.http.post<any>(
      this.rootUrl + "/AddAllOutcomingTypeToUser",
      item
    );
  }
  public deleteToUser(userId: any, OutcomingEntryTypeId: any): Observable<any> {
    return this.http.delete<any>(
      this.rootUrl +
        `/DeleteOutcomingTypeToUser?UserId=${userId}&OutcomingEntryTypeId=${OutcomingEntryTypeId}`,
      {}
    );
  }
  public GetAllByUser(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetAllByUser`);
  }
  public GetComboxExpenseTypes(): Observable<any> {
    return this.http.get<any>(this.rootUrl + `/GetComboxExpenseTypes`);
  }

  public GetExistOutComeInChartSetting(id: number): Observable<any> {
    return this.http.get<any>(
      this.rootUrl + `/GetExistOutComeInChartSetting?lineChartId=${id}`
    );
  }
  public getAllForDropdownByUserNew(): Observable<ApiResponse<IncomingEntryTypeOptions[]>> {
    return this.http.get<any>(this.rootUrl + `/GetAllForDropdownByUserNew`);
  }

  public getAllByGranted(inputFilter: InputFilterOutcomingEntryType): Observable<ApiResponse<OutcomingEntryTypesDto[]>> {
    return this.http.post<any>(this.rootUrl + '/GetOutcomingEntryTypeAllByUserId', inputFilter);
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

  public updateOutcomingEntryTypeByUserId(input):Observable<any>{
    return this.http.post(this.rootUrl + `/UpdateOutcomingEntryTypeByUserId`, input);
  }
}
