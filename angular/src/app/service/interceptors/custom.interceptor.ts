import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class CustomInterceptor implements HttpInterceptor {

  constructor(
    private router: Router
  ) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if(request.headers && !request.headers.has("Ncc-PeriodId") && AppConsts.periodId?.value){
        var periodId =  AppConsts.periodId.getValue().toString()
        let clonedRequest: HttpRequest<any> = request.clone({ headers: request.headers.append('Ncc-PeriodId', periodId)});
        return next.handle(clonedRequest).pipe(catchError(error => this.handleAuthError(error)));
      }
    return next.handle(request).pipe(catchError(error => this.handleAuthError(error)));
  }

  private handleAuthError(err: HttpErrorResponse): Observable<any> {
    if (err.status === 401) {
      this.router.navigateByUrl(`/account/login`);
      abp.message.error('Your session is expired, please login again', 'Authentication Failed');
      return;
    }
    if (err.status === 400 && err.error.error.validationErrors) {
      let validationErrors = err.error.error.validationErrors.map(s => s.message);
      abp.message.error('<div class="text-left">'+validationErrors.join("<br>")+'</div>', "Bad request", true);
      return;
    }
    if (err instanceof HttpErrorResponse || err) {
      if (err.status == 0) {
        abp.message.error(err.statusText || 'Exist an error!');
      }
      else {
        const errorObj = err.error.error;
        abp.message.error(errorObj.message, "Error", true);
        return throwError(errorObj);
      }
    }

    return throwError(err);
  }
}
