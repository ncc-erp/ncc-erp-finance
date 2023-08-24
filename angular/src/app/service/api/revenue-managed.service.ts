import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { throwError, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RevenueManagedService extends BaseApiService {

  constructor(http: HttpClient) {
    super(http);
  }
  changeUrl() {
    return 'RevenueManaged';
  }
  getAllPagingByStatus(request: any, status?: string): Observable<any> {
    return this.http.post<any>(this.rootUrl + `/GetAllPaging?status=${status}`, request);
  }
  revenueUploadFiles(request: any, id): Observable<any> {
    let formData = new FormData();
    formData.append('Id', id)
    request.forEach(element => {
      //console.log(element)
      formData.append('Files', element)
      formData.append('FileNames', element.name)
    });
    return this.http.post(this.rootUrl + '/UploadFiles', formData);
  }
  revenueDownloadFile(name) {
    return this.http.get(this.rootUrl + '/DownloadFile?name=' + name);
  }
  revenueGetFiles(id) {
    return this.http.get(this.rootUrl + '/GetFiles?Id=' + id);
  }
}
