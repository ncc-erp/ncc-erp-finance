import { AppConsts } from './../../../shared/AppConsts';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { CheckChangeOutcomingEntryStatusDto, CheckChangeStatusDto, GetAllPagingOutComingEntryDto, cloneOutcomingEntryDto, OutcomingEntryDto, RequestChangeOutcomingEntryInfoDto, ResultGetOutcomingEntryDto, ExpenditureRequestDto, CheckWarningCreateRequestDto } from '@app/modules/expenditure-request/expenditure-request.component';
import { ApiPagingResponse, ApiResponse } from '../model/api-response.model';
import { StatusHistory, UpdateTempOutcomingEntryDto } from '@app/modules/expenditure-request-detail/main-tab/main-tab.component';
import { GetRequestChangeOutcomingEntryDetailDto, GetTempOutcomingEntryDetailDto, RequestChangeOutcomingEntryDetailInfoDto, ResultCheckUpdateOutcomingEntryDetailDto, SendTempOutcomingEntryDetailDto } from '@app/modules/expenditure-request-detail/detail-tab/detail-tab.component';
import { ValueAndNameModel } from '../model/common-DTO';


@Injectable({
  providedIn: 'root'
})
export class ExpenditureRequestService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'OutcomingEntry';
  }
  changeStatus(item: any) {
    return this.http.post<any>(this.rootUrl + '/ChangeStatus', item);
  }
  newChangeStatus(item: CheckChangeStatusDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/NewChangeStatus', item);
  }
  getAllPagingByStatus(request: PagedRequestDto, status?: string, accreditation?: boolean): Observable<ApiPagingResponse<OutcomingEntryDto[]>> {
    return this.http.post<any>(this.rootUrl + `/GetAllPaging?status=${status}&accreditation=${accreditation}`, request);
  }
  getAllPaging(request: GetAllPagingOutComingEntryDto): Observable<ApiResponse<ResultGetOutcomingEntryDto>> {
    return this.http.post<any>(this.rootUrl + `/GetAllPaging`, request);
  }

  updateReportDate(id: number, reportDate: string) {
    return this.http.post<any>(this.rootUrl + '/UpdateReportDate', {outcomingEntryId: id, reportDate})
  }
  UpdateOutcomingEntryType(id: number, outcomeId: number) {
    return this.http.post<any>(this.rootUrl + '/UpdateOutcomingEntryType', {OutcomingEntryId: id, OutcomingEntryTypeId: outcomeId })
  }

  IsOutcomingEntryHasDetail(outcomeId: number) {
    return this.http.get<any>(this.rootUrl + `/IsOutcomingEntryHasDetail?outcomingEntryId=${outcomeId}`)
  }

  SendKomuCEO(): Observable<any> {
    return this.http.get(this.rootUrl + `/RemindCEO`)
  }
  SendKomuCEORequestChange(){
    return this.http.get(this.rootUrl + `/RemindCEORequestChange`)
  }
  SendKomuCFO(): Observable<any> {
    return this.http.get(this.rootUrl + `/RemindCFO`)
  }
  CfoTransfer(): Observable<any> {
    return this.http.get(this.rootUrl + `/CFOTransfer`)
  }
  // TransferToEnd
  TransferToEnd(item: any) {
    return this.http.post<any>(this.rootUrl + '/TransferToEnd', item);
  }
  // GetTotalValueOutcomingEntry(request: PagedRequestDto, status: string): Observable<any> {
  //   return this.http.post<any>(this.rootUrl + `/GetTotalValueOutcomingEntry?status=${status}`, request)
  // }
  UploadFile(file, id): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('OutcomingEntryId', id);
    const uploadReq = new HttpRequest(
      'POST', AppConsts.remoteServiceBaseUrl + '/api/services/app/OutcomingEntry/UploadFile', formData,
      {
        reportProgress: true
      }
    );
    return this.http.request(uploadReq);
  }
  GetFiles(id: number): Observable<any> {
    return this.http.get(this.rootUrl + `/GetFiles?outcomingEntryId=${id}`)
  }
  DeleteFile(fileId: number): Observable<any> {
    return this.http.delete(this.rootUrl + `/DeleteFile?&id=${fileId}`)
  }
  // GetTotalValueOutcomingEntry(request: PagedRequestDto, status: string, accreditation: boolean): Observable<any> {
  //   return this.http.post<any>(this.rootUrl + `/GetTotalValueOutcomingEntry?status=${status}&accreditation=${accreditation}`, request);
  // }
  getTotalValueOutcomingEntry(request: GetAllPagingOutComingEntryDto): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/GetTotalValueOutcomingEntry`, request);
  }
  handleError(error: any) {
    let errorMessage = '';

    if (error.error instanceof ErrorEvent) {

      errorMessage = `Error: ${error.error.message}`;
    } else {

      errorMessage = `Error: ${error.error.error.message}`;
    }

    // abp.notify.error(errorMessage);
    return throwError(errorMessage);
  }
  AcceptFile(id: number, isConfirmFile: boolean): Observable<any> {
    return this.http.get(this.rootUrl + `/AcceptFile?id=${id}&isConfirmFile=${isConfirmFile}`)
  }
  exportExcel(request: PagedRequestDto, status: any, accreditation: boolean): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/ExportExcel?status=${status}&accreditation=${accreditation}`, request);
  }
  exportPdf(id: number): Observable<any> {

    return this.http.get<any>(this.rootUrl + `/ExportPdfById?Id=${id}`);
  }
  checkChangeStatus(payload: CheckChangeStatusDto): Observable<ApiResponse<CheckChangeOutcomingEntryStatusDto>> {
    return this.http.post<any>(this.rootUrl + `/CheckUpdateOutcomingEntryDetail`, payload);
  }
  asyncgetOutcomingEntryStatusHistoryByOutcomingEntryId(outcomingEntryId: number): Promise<ApiResponse<StatusHistory[]>> {
    return this.http.get<any>(this.rootUrl + `/GetOutcomingEntryStatusHistoryByOutcomingEntryId?outcomingEntryId=${outcomingEntryId}`).toPromise();
  }
  getOutcomingEntryStatusHistoryByOutcomingEntryId(outcomingEntryId: number): Observable<ApiResponse<StatusHistory[]>> {
    return this.http.get<any>(this.rootUrl + `/GetOutcomingEntryStatusHistoryByOutcomingEntryId?outcomingEntryId=${outcomingEntryId}`);
  }
  checkUpdateOutcomingEntryDetail(outcomingEntryId: number): Observable<ApiResponse<ResultCheckUpdateOutcomingEntryDetailDto>> {
    //return this.http.post<any>(this.rootUrl + `/CheckUpdateOutcomingEntryDetail?outcomingEntryId=${outcomingEntryId}`);
    return this.http.post<any>(this.rootUrl + `/CheckUpdateOutcomingEntryDetail?outcomingEntryId=${outcomingEntryId}`, null);
  }
  checkExistTempOutCommingEntry(outcomingEntryId: number): Observable<ApiResponse<boolean>> {
    return this.http.get<any>(this.rootUrl + `/CheckExistTempOutCommingEntry?outcomingEntryId=${outcomingEntryId}`);
  }
  createTempOutCommingEntry(outcomingEntryId: number): Observable<ApiResponse<GetTempOutcomingEntryDetailDto>> {
    return this.http.get<any>(this.rootUrl + `/CreateTempOutCommingEntry?outcomingEntryId=${outcomingEntryId}`);
  }
  getRequestChangeOutcomingEntry(tempOutcomingEntryId: number): Observable<ApiResponse<RequestChangeOutcomingEntryInfoDto>> {
    return this.http.get<any>(this.rootUrl + `/GetRequestChangeOutcomingEntry?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }
  getViewHistoryRequestChangeOutcomingEntry(tempId: number): Observable<ApiResponse<RequestChangeOutcomingEntryInfoDto>> {
    return this.http.get<any>(this.rootUrl + `/ViewHistoryChangeOutcomingEntry?tempId=${tempId}`);
  }
  getViewHistoryRequestChangeOutcomingEntryDetail(tempId: number): Observable<ApiResponse<GetRequestChangeOutcomingEntryDetailDto>> {
    return this.http.get<any>(this.rootUrl + `/ViewHistoryChangeOutcomingEntryDetail?tempId=${tempId}`);
  }
  sendTemp(tempOutcomingEntryId: number): Observable<ApiResponse<null>> {
    return this.http.get<any>(this.rootUrl + `/SendTemp?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }
  rejectTemp(tempOutcomingEntryId: number): Observable<ApiResponse<null>> {
    return this.http.get<any>(this.rootUrl + `/RejectTemp?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }
  approveTemp(tempOutcomingEntryId: number): Observable<ApiResponse<null>> {
    return this.http.get<any>(this.rootUrl + `/ApproveTemp?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }
  saveTempOutCommingEntry(payload: UpdateTempOutcomingEntryDto): Observable<ApiResponse<RequestChangeOutcomingEntryInfoDto>> {
    return this.http.post<any>(this.rootUrl + `/SaveTempOutCommingEntry`, payload);
  }
  getRequestChangeOutcomingEntryDetail(tempOutcomingEntryId: number): Observable<ApiResponse<GetRequestChangeOutcomingEntryDetailDto>> {
    return this.http.get<any>(this.rootUrl + `/GetRequestChangeOutcomingEntryDetail?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }
  createTempOutcomingEntryDetail(payload: SendTempOutcomingEntryDetailDto): Observable<ApiResponse<null>> {
    return this.http.post<any>(this.rootUrl + `/CreateTempOutcomingEntryDetail`, payload);
  }
  updateTempOutcomingEntryDetail(payload: SendTempOutcomingEntryDetailDto): Observable<ApiResponse<null>> {
    return this.http.post<any>(this.rootUrl + `/UpdateTempOutcomingEntryDetail`, payload);
  }
  deleteTempOutcomingEntryDetail(tempOutcomingEntryDetailId: number): Observable<ApiResponse<null>> {
    return this.http.delete<any>(this.rootUrl + `/DeleteTempOutcomingEntryDetail?tempOutcomingEntryDetailId=${tempOutcomingEntryDetailId}`);
  }
  revertTempOutcomingDetailByRootId(rootOutcomingEntryDetailId: number, rootOutcomingEntryId: number): Observable<ApiResponse<null>> {
    return this.http.get<any>(this.rootUrl + `/RevertTempOutcomingDetailByRootId?rootOutcomingEntryDetailId=${rootOutcomingEntryDetailId}&rootOutcomingEntryId=${rootOutcomingEntryId}`);
  }
  getAllRequester(): Observable<ApiResponse<ValueAndNameModel[]>> {
    return this.http.get<any>(this.rootUrl + `/GetAllRequester`);
  }
  checkTempOutcomingEntryApproved(tempId: number): Observable<ApiResponse<boolean>> {
    return this.http.get<any>(this.rootUrl + `/CheckTempOutcomingEntryApproved?tempId=${tempId}`);
  }
  checkTempOutCommingEntryHasDetail(tempOutcomingEntryId: number): Observable<ApiResponse<boolean>> {
    return this.http.get<any>(this.rootUrl + `/CheckTempOutCommingEntryHasDetail?tempOutcomingEntryId=${tempOutcomingEntryId}`);
  }

  cloneOutcomingEntry(input:cloneOutcomingEntryDto): Observable<ApiResponse<boolean>> {
    return this.http.post<any>(this.rootUrl + `/CloneOutcomingEntry`,input);
  }
  forceUpdateBranch(input):Observable<ApiResponse<any>>{
    return this.http.put<any>(this.rootUrl + `/ForceUpdateBranch`, input);
  }
  getDoneWorkFlowStatusTransaction():Observable<ApiResponse<any>>{
    return this.http.get<any>(this.rootUrl + `/GetDoneWorkFlowStatusTransaction`);
  }
  checkWarningCreateRequest(input: ExpenditureRequestDto):Observable<ApiResponse<any>>{
    return this.http.post<any>(this.rootUrl + `/CheckWarningCreateRequest`,input);
  }
}
