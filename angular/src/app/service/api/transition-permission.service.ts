import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
@Injectable({
  providedIn: 'root'
})
export class TransitionPermissionService extends BaseApiService {
  changeUrl() {
    return 'TransitionPermission';
  }

  constructor(http: HttpClient) {
    super(http);
  }
}
