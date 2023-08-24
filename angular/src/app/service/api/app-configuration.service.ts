import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { EspecialIncomingEntryTypeDto } from '@app/modules/new-versions/b-transaction/setting-payment-dialog/setting-payment-dialog.component';
import { ApiResponse } from '../model/api-response.model';
import { OutcomingSalaryDto } from '@app/modules/new-versions/b-transaction/link-multi-btransaction-outcoming-entry-dialog/link-multi-btransaction-outcoming-entry-dialog.component';
import { DefaultIncomingEntryType } from '@app/modules/new-versions/b-transaction/currency-exchange/currency-exchange.component';
import { RequestChiSettingDto } from '@app/modules/admin-setting/admin-setting.component';

@Injectable({
  providedIn: 'root'
})
export class AppConfigurationService extends BaseApiService{
  changeUrl() {
    return 'Configuration';
  }

  constructor( http: HttpClient) {
    super(http)
  }
  getConfiguration():Observable<any>{
    return this.http.get(this.rootUrl + '/Get')
  }
  GetGoogleClientAppId():Observable<any>{
    return this.http.get(this.rootUrl + '/GetGoogleClientAppId')
  }
  getEspecialIncomingEntryType(): Observable<ApiResponse<EspecialIncomingEntryTypeDto>> {
    return this.http.get<any>(this.rootUrl + `/GetEspecialIncomingEntryType`);
  }
  setEspecialIncomingEntryType(payload: EspecialIncomingEntryTypeDto): Observable<ApiResponse<EspecialIncomingEntryTypeDto>> {
    return this.http.post<any>(this.rootUrl + '/SetEspecialIncomingEntryType', payload);
  }
  getDeviantCode(): Observable<ApiResponse<string>>{
    return this.http.get<ApiResponse<string>>(this.rootUrl + '/GetDeviantCode');
  }
  getOutcomingSalary(): Observable<ApiResponse<OutcomingSalaryDto>>{
    return this.http.get<ApiResponse<OutcomingSalaryDto>>(this.rootUrl + '/GetOutcomingSalary');
  }
  setOutcomingSalary(payload: OutcomingSalaryDto): Observable<ApiResponse<OutcomingSalaryDto>>{
    return this.http.post<ApiResponse<OutcomingSalaryDto>>(this.rootUrl + '/SetOutcomingSalary', payload);
  }
  clearDefaultToBankAccount(): Observable<ApiResponse<OutcomingSalaryDto>>{
    return this.http.post<ApiResponse<OutcomingSalaryDto>>(this.rootUrl + '/ClearDefaultToBankAccount', null);
  }
  getCanLinkWithOutComingEnd(): Observable<ApiResponse<boolean>>{
    return this.http.get<ApiResponse<boolean>>(this.rootUrl + '/GetCanLinkWithOutComingEnd');
  }
  setCanLinkWithOutComingEnd(canLinkWithOutComingEnd: string): Observable<ApiResponse<boolean>>{
    return this.http.post<any>(this.rootUrl + `/SetCanLinkWithOutComingEnd?canLinkWithOutComingEnd=${canLinkWithOutComingEnd}`, {})
  }
  getDefaultMaLoaiThuBanNgoaiTe(): Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.rootUrl + '/GetDefaultMaLoaiThuBanNgoaiTe');
  }
  setDefaultMaLoaiThuBanNgoaiTe(payload: DefaultIncomingEntryType): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/SetDefaultMaLoaiThuBanNgoaiTe`, payload)
  }
  clearDefaultMaLoaiThuBanNgoaiTe(): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/ClearDefaultMaLoaiThuBanNgoaiTe', undefined);
  }
  getDefaultMaLoaiMuaNgoaiTe(): Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.rootUrl + '/GetDefaultMaLoaiMuaNgoaiTe');
  }
  setDefaultMaLoaiMuaNgoaiTe(payload: DefaultIncomingEntryType): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/SetDefaultMaLoaiMuaNgoaiTe`, payload)
  }
  clearDefaultMaLoaiMuaNgoaiTe(): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/ClearDefaultMaLoaiMuaNgoaiTe', undefined);
  }

  getDefaultLoaiThuIdKhiChiChuyenDoi(): Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.rootUrl + '/GetDefaultLoaiThuIdKhiChiChuyenDoi');
  }
  setDefaultLoaiThuIdKhiChiChuyenDoi(payload: DefaultIncomingEntryType): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/SetDefaultLoaiThuIdKhiChiChuyenDoi`, payload)
  }
  clearDefaultLoaiThuIdKhiChiChuyenDoi(): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/ClearDefaultLoaiThuIdKhiChiChuyenDoi', undefined);
  }

  public ChangeNotifyKomuChannel(input):Observable<any>{
    return this.http.post(this.rootUrl + '/ChangeNotifyKomuChannel', input);
  }
  public ChangeFinanceSecretKey(input):Observable<any>{
    return this.http.post(this.rootUrl + '/ChangeFinanceSecretKey', input);
  }
  public ChangeClientAppId(input):Observable<any>{
    return this.http.post(this.rootUrl + '/ChangeClientAppId', input);
  }

  GetRequestChiSetting(): Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(this.rootUrl + '/GetRequestChiSetting');
  }

  CheckStatusOfMaLoaiChi(input:string): Observable<ApiResponse<any>>{
    return this.http.post<ApiResponse<any>>(this.rootUrl + `/CheckStatusOfMaLoaiChi?input=${input}`,{});
  }

  UpdateRequestChiSetting(input:RequestChiSettingDto): Observable<ApiResponse<any>>{
    return this.http.put<any>(this.rootUrl + '/UpdateRequestChiSetting', input)
  }

  GetApplyToMultiCurrencyOutcome(): Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<number>>(this.rootUrl + '/GetApplyToMultiCurrencyOutcome');
  }

  SetApplyToMultiCurrencyOutcome(input:string): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/SetApplyToMultiCurrencyOutcome?canApplyToMultiCurrencyOutcome=${input}`,{})
  }
  getDefaultMaLoaiThuKhachHangBonus(): Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.rootUrl + '/GetDefaultMaLoaiThuKhachHangBonus');
  }
  setDefaultMaLoaiThuKhachHangBonus(payload: DefaultIncomingEntryType): Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/SetDefaultMaLoaiThuKhachHangBonus`, payload)
  }
  clearDefaultMaLoaiThuKhachHangBonus(): Observable<any>{
    return this.http.post<any>(this.rootUrl + '/ClearDefaultMaLoaiThuKhachHangBonus', undefined);
  }
  getAllowChangeEntityInPeriodClosed(): Observable<ApiResponse<boolean>>{
    return this.http.get<any>(this.rootUrl + '/GetAllowChangeEntityInPeriodClosed');
  }
  setAllowChangeEntityInPeriodClosed(config: boolean): Observable<any>{
    return this.http.post<any>(this.rootUrl + `/SetAllowChangeEntityInPeriodClosed?config=${config}`, undefined);
  }
  GetEnableCrawlBTransactionNoti(): Observable<any>{
    return this.http.get<any>(this.rootUrl + `/GetEnableCrawlBTransactionNoti`);
  }
  SetEnableCrawlBTransactionNoti(isEnable: boolean): Observable<any>{
    return this.http.post<any>(this.rootUrl + `/SetEnableCrawlBTransactionNoti?isEnable=${isEnable}`, undefined);
  }
  getHRMConfig(): Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<string>>(this.rootUrl + '/GetHrmConfig');
  }
}
