import { BaseApiService } from '@app/service/api/base-api.service';
import { AppConsts } from './../../../shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GoogleLoginService extends BaseApiService{
  changeUrl() {
    return 'TokenAuth';
  }

  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  name() {
    return 'TokenAuth';
  }
  googleAuthenticate(googleToken: string): Observable<any> {
    return this.http.post(AppConsts.remoteServiceBaseUrl +
      '/api/TokenAuth/GoogleAuthenticate', {googleToken: googleToken});
  }

}
