import { Injectable } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountForDropdownDto, BankAccountTransDto, IncomingEntryTypeDto, ValueAndNameModel } from '@app/service/model/common-DTO';
import { ApiResponse } from '../../model/api-response.model';
import { BTransactionDto } from '@app/modules/banking-transaction/link-BTransaction-dialog/link-b-transaction-dialog.component';
import { IncomingEntryTypeOptions } from '@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { SelectionOutcomingEntry } from '@app/modules/new-versions/b-transaction/link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';
import { GetAccountCompanyForDropdownDto } from '@app/modules/expenditure-request/expenditure-request.component';
import { ClientInfoDto } from '@app/service/model/circle-chart.dto';

@Injectable({
  providedIn: 'root'
})
export class CommonService extends BaseApiService {
  changeUrl() {
    return 'common';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  getBTransactionStatus(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetBTransactionStatus');
  }
  getBankAccountInBTransaction(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetBankAccountsInBTransaction');
  }
  getAccountInRevenues(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAccountInRevenues');
  }
  getIncomingEntryTypes(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetIncomingEntryTypes');
  }
  getAllAccounts(isInActive?: boolean): Observable<ApiResponse<AccountForDropdownDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllAccounts' + (isInActive?'?isInActive=true':''));
  }
  getAllRevenueStatuses(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetRevenueStatuses');
  }
  getAllInvoiceStatuses(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetInvoiceStatuses');
  }
  gettAllBankAccount(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GettAllBankAccount');
  }
  getAllAccounTypeEnum(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllAccounTypeEnum');
  }
  getAllClient(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllClient');
  }
  getAllClientInfo(): Observable<ApiResponse<ClientInfoDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllClientInfo');
  }
  getBTransactionOptions(): Observable<ApiResponse<BTransactionDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetBTransactionOptions');
  }
  getOutcomingEntry(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetOutcomingEntry');
  }
  getApprovedOutcomingEntry(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetApprovedOutcomingEntry');
  }
  getApprovedAndEndOutcomingEntry(onlyApproved: boolean, currencyId = 0): Observable<ApiResponse<SelectionOutcomingEntry[]>> {
    return this.http.get<any>(this.rootUrl + '/GetApprovedAndEndOutcomingEntry?onlyApproved='+ onlyApproved + (currencyId == 0?'':(`&currencyId=${currencyId}`)));
  }
  getCommonBankAccountTransactionOpstion(): Observable<ApiResponse<BankAccountTransDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetCommonBankAccountTransactionOpstion');
  }
  getAccountTypeIdOptions(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAccountTypeIdOptions');
  }
  getAccountIdOptions(accountTypeId: number): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAccountIdOptions?accountTypeId='+accountTypeId);
  }
  gettAllBankAccountByAccoutId(accountId: number): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GettAllBankAccountByAccoutId?accountId='+accountId);
  }
  getIncomingEntryByIncomingEntryId(incomingEntryTypeId: number): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetIncomingEntryByIncomingEntryId?incomingEntryTypeId='+incomingEntryTypeId);
  }
  getAllIncomingEntryType(): Observable<ApiResponse<IncomingEntryTypeDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetAllIncomingEntryType');
  }
  getAllBankAccount(): Observable<ApiResponse<ValueAndNameModel[]>>{
    return this.http.get<ApiResponse<ValueAndNameModel[]>>(this.rootUrl + '/GetAllBankAccount');
  }
  getAllBank(): Observable<ApiResponse<ValueAndNameModel[]>>{
    return this.http.get<ApiResponse<ValueAndNameModel[]>>(this.rootUrl + '/GetAllBank');
  }
  getAllCurrency(): Observable<ApiResponse<ValueAndNameModel[]>>{
    return this.http.get<ApiResponse<ValueAndNameModel[]>>(this.rootUrl + '/GetAllCurrency');
  }
  getBankAccountByCurrency(currencyId: number): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + '/GetBankAccountByCurrency?currencyId='+currencyId);
  }
  getDefaultBankAccountByCurrencyId(currencyId: number): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + '/GetDefaultBankAccountByCurrencyId?currencyId='+currencyId);
  }
  getBankAccountVNDOpstion(): Observable<ApiResponse<BankAccountTransDto[]>> {
    return this.http.get<any>(this.rootUrl + '/GetBankAccountVNDOpstion');
  }
  getTreeIncomingEntries(isActiveOnly : boolean = true): Observable<ApiResponse<IncomingEntryTypeOptions[]>> {
    return this.http.get<any>(this.rootUrl + `/GetTreeIncomingEntries?isActiveOnly=${isActiveOnly}`);
  }
  getTreeOutcomingEntries(isActiveOnly : boolean = true): Observable<ApiResponse<IncomingEntryTypeOptions[]>> {
    return this.http.get<any>(this.rootUrl + `/GetTreeOutcomingEntries?isActiveOnly=${isActiveOnly}`);
  }
  getBankAccoutForCompany(): Observable<any>{
    return this.http.get<any>(this.rootUrl + '/getBankAccoutForCompany');
  }
  getConversionTransactionDefaultBankAccountByCurrencyId(currencyId: number): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + '/GetConversionTransactionDefaultBankAccountByCurrencyId?currencyId=' + currencyId);
  }
  getBankAccountByCurrencyCode(code: string):Observable<any>{
    return this.http.get(this.rootUrl + '/GetBankAccountByCurrencyCode?code='+ code);
  }
  getDefaultFromBankAccountByCurrencyIdWhenBuy(currencyId: number): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + '/GetDefaultFromBankAccountByCurrencyIdWhenBuy?currencyId=' + currencyId);
  }
  getDefaultToBankAccountByCurrencyIdWhenBuy(currencyId: number): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + '/GetDefaultToBankAccountByCurrencyIdWhenBuy?currencyId=' + currencyId);
  }
  getBankAccountOptions(filterBankAccount: FilterBankAccount): Observable<ApiResponse<BankAccountTransDto[]>> {
    return this.http.post<any>(this.rootUrl + `/GetBankAccountOptions`, filterBankAccount);
  }

  getBankTransactionFromCompanyByCurrency(currencyId?: number): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + `/GetBankTransactionFromCompanyByCurrency${currencyId?'?currencyId='+currencyId:''}`);

  }
  getOptionTreeOutcomingEntriesByUser(): Observable<ApiResponse<IncomingEntryTypeOptions[]>> {
    return this.http.get<any>(this.rootUrl + `/GetOptionTreeOutcomingEntriesByUser`);

  }
  getDefaultToBankAccountByCurrencyId(currencyId: number): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + '/GetDefaultToBankAccountByCurrencyId?currencyId=' + currencyId);
  }
  getAllAccountCompany(isShowAll:boolean = false): Observable<ApiResponse<GetAccountCompanyForDropdownDto[]>> {
    return this.http.get<any>(this.rootUrl + `/GetAllAccountCompany?isShowAll=${isShowAll}`);
  }
  GetTypeOptions(FilterTypeOptions: any):Observable<ApiResponse<IncomingEntryTypeOptions[]>>{
    return this.http.post<any>(this.rootUrl + `/GetTypeOptions`, FilterTypeOptions);
  }
}
