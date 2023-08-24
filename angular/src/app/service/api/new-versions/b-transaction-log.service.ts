import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {BaseApiService} from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class BTransactionLogService extends BaseApiService {
  changeUrl() {
    return 'bTransactionLog';
  }

  constructor(httpClient: HttpClient) {
    super(httpClient);
  }
}
