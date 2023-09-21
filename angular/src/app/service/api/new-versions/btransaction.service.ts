import { ApiResponse } from './../../model/api-response.model';
import { Observable, throwError } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { CurrencyNeedConvert } from '@app/modules/new-versions/b-transaction/payment-dialog/payment-dialog.component';
import { ImportBTransactionResult, InComingAndBTransactionDto, PaymentInvoiceForAccount } from '@app/service/model/b-transaction.model';
import { CreateEditBTransactionDto, LinkExpenditureAndBTransDto } from '@app/modules/new-versions/b-transaction/list-btransaction/list-btransaction.component';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { DifferentBetweenBankTransAndBTrans, LinkBankTransactionToBTransactionDto } from '@app/modules/banking-transaction/link-BTransaction-dialog/link-b-transaction-dialog.component';
import { CreateResult } from '@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { LinkMultipleOutcomingEntryWithBTransactionDto } from '@app/modules/new-versions/b-transaction/link-b-transaction-multi-out-coming-dialog/link-b-transaction-multi-out-coming-dialog.component';
import { LinkOutcomingSalaryWithBTransactionsDto } from '@app/modules/new-versions/b-transaction/link-multi-btransaction-outcoming-entry-dialog/link-multi-btransaction-outcoming-entry-dialog.component';
import { GetInfoRollbackOutcomingEntry } from '@app/modules/new-versions/b-transaction/rollback-link-outcoming-entry/rollback-link-outcoming-entry.component';
import { BankAccountTransDto } from '@app/service/model/common-DTO';
import { ConversionTransactionDto } from '@app/modules/new-versions/b-transaction/currency-exchange/currency-exchange.component';
import { CheckCurrencyLinkOutcomToBTransaction } from '@app/modules/new-versions/b-transaction/link-expenditure-dialog/link-expenditure-dialog.component';
import { CreateMultiIncomingEntryDto, ResultCreateMultiIncoming } from '@app/modules/new-versions/b-transaction/create-multi-incoming-entry/create-multi-incoming-entry.component';
import { ChiChuyenDoiDto } from '@app/modules/new-versions/b-transaction/chi-chuyen-doi/chi-chuyen-doi.component';

@Injectable({
  providedIn: 'root'
})
export class BtransactionService extends BaseApiService {
  changeUrl() {
    return 'btransaction';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  checkAccount(btransactionId: number, accountId: number): Observable<ApiResponse<CurrencyNeedConvert[]>> {
    return this.http.get<any>(this.rootUrl + `/CheckAccount?btransactionId=${btransactionId}&accountId=${accountId}`);
  }
  paymentForAccount(payload: PaymentInvoiceForAccount): Observable<ApiResponse<string>> {
    return this.http.post<any>(this.rootUrl + '/PaymentInvoiceByAccount', payload);
  }
  createTransaction(payload: CreateEditBTransactionDto): Observable<ApiResponse<CreateEditBTransactionDto>> {
    return this.http.post<any>(this.rootUrl + '/CreateTransaction', payload);
  }
  updateTransaction(payload: CreateEditBTransactionDto): Observable<ApiResponse<CreateEditBTransactionDto>> {
    return this.http.post<any>(this.rootUrl + '/UpdateTransaction', payload);
  }
  getAllTransactionPaging(request: PagedRequestDto, id?:number|string): Observable<any> {
    let para = (id == null || '') ? '' : `?id=${id}`;
    return this.http.post<any>(this.rootUrl + `/GetAllPaging${para}`, request);
  }
  linkOutcomingEntryWithBTransaction(payload: LinkExpenditureAndBTransDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/LinkOutcomingEntryWithBTransaction`, payload);
  }
  linkMultipleOutcomingEntryWithBTransaction(payload: LinkMultipleOutcomingEntryWithBTransactionDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/LinkMultipleOutcomingEntryWithBTransaction`, payload);
  }
  createIncomingEntry(payload: InComingAndBTransactionDto): Observable<ApiResponse<CreateResult>> {
    return this.http.post<any>(this.rootUrl + `/CreateIncomingEntry`, payload);
  }
  linkBankTransactionToBTransaction(payload: LinkBankTransactionToBTransactionDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/LinkBankTransactionToBTransaction`, payload);
  }
  checkDifferentBetweenBankTransAndBTrans(payload: LinkBankTransactionToBTransactionDto): Observable<ApiResponse<DifferentBetweenBankTransAndBTrans>> {
    return this.http.post<any>(this.rootUrl + `/CheckDifferentBetweenBankTransAndBTrans`, payload);
  }
  checkCurrencyLinkOutcomingEntryWithBTransaction(bTransactionId: number): Observable<ApiResponse<CheckCurrencyLinkOutcomToBTransaction>> {
    return this.http.get<ApiResponse<CheckCurrencyLinkOutcomToBTransaction>>(this.rootUrl + `/CheckCurrencyLinkOutcomingEntryWithBTransaction?bTransactionId=${bTransactionId}`);
  }
  importBTransaction(formData: FormData): Observable<ApiResponse<ImportBTransactionResult>>{
    return this.http.post<ApiResponse<ImportBTransactionResult>>(this.rootUrl + '/ImportBTransaction', formData);
  }
  checkLinkOutcomingEntrySalaryWithBTransactions(payload: LinkOutcomingSalaryWithBTransactionsDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.rootUrl + `/CheckLinkOutcomingEntrySalaryWithBTransactions`, payload);
  }
  linkOutcomingEntrySalaryWithBTransactions(payload: LinkOutcomingSalaryWithBTransactionsDto): Observable<ApiResponse<any>> {
    return this.http.post<any>(this.rootUrl + `/LinkOutcomingEntrySalaryWithBTransactions`, payload);
  }
  getCurrentOutcomingSalary(): Observable<ApiResponse<number>> {
    return this.http.get<any>(this.rootUrl + `/GetCurrentOutcomingSalary`);
  }
  rollbackLinkOutcomingEntry(id: number): Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + "/RollBackOutcomingEntryWithBTransaction?bTransactionId=" + id);
  }
  getRollbackLinkOutcomingEntry(id: number): Observable<ApiResponse<GetInfoRollbackOutcomingEntry>>{
    return this.http.get<ApiResponse<GetInfoRollbackOutcomingEntry>>(this.rootUrl + "/RollBackOutcomingEntryWithBTransaction?bTransactionId=" + id);
  }
  getInfoRollbankOutcomingEntryWithBTransaction(id: number):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(this.rootUrl +  `/GetInfoRollbankOutcomingEntryWithBTransaction?bTransactionId=${id}`);
  }
  getDefaultToBankAccount(): Observable<ApiResponse<BankAccountTransDto>> {
    return this.http.get<any>(this.rootUrl + `/GetDefaultToBankAccount`);
  }
  checkConversionTransaction(payload: ConversionTransactionDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.rootUrl + `/CheckConversionTransaction`, payload);
  }
  conversionTransaction(payload: ConversionTransactionDto): Observable<ApiResponse<any>> {
    return this.http.post<any>(this.rootUrl + `/ConversionTransaction`, payload);
  }
  checkMuaNgoaiTe(payload: ConversionTransactionDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.rootUrl + `/CheckMuaNgoaiTe`, payload);
  }
  muaNgoaiTe(payload: ConversionTransactionDto): Observable<ApiResponse<any>> {
    return this.http.post<any>(this.rootUrl + `/MuaNgoaiTe`, payload);
  }
  checkCreateMultiIncomingEntry(payload: CreateMultiIncomingEntryDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.rootUrl + `/CheckCreateMultiIncomingEntry`, payload);
  }
  createMultiIncomingEntry(payload: CreateMultiIncomingEntryDto): Observable<ApiResponse<ResultCreateMultiIncoming>> {
    return this.http.post<any>(this.rootUrl + `/CreateMultiIncomingEntry`, payload);
  }
  checkChiChuyenDoi(payload: ChiChuyenDoiDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.rootUrl + `/CheckChiChuyenDoi`, payload);
  }
  chiChuyenDoi(payload: ChiChuyenDoiDto): Observable<ApiResponse<any>> {
    return this.http.post<any>(this.rootUrl + `/ChiChuyenDoi`, payload);
  }
  getInfoRollbackBTransactionHasIncomingEntry(id: any):Observable<ApiResponse<any>>{
    return this.http.get<ApiResponse<any>>(this.rootUrl +  `/GetInfoRollbackBTransactionHasIncomingEntry?bTransactionId=${id}`);
  }
  rollbackBTransactionHasIncomingEntry(id: any): Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + `/RollbackBTransactionHasIncomingEntry?bTransactionId=${id}`);
  }
}
