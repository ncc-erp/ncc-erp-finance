import {
    HttpEvent,
    HttpInterceptor,
    HttpHandler,
    HttpRequest,
    HttpErrorResponse
  } from '@angular/common/http';
  import { Observable, throwError } from 'rxjs';
  import { catchError } from 'rxjs/operators';
  export class HttpErrorInterceptor implements HttpInterceptor {
    findId(list: any[], value: string) {
      const foundId = list.find(el => el.value === value);
      return foundId ? foundId.message : '';
    }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      return next.handle(request)
        .pipe(
          catchError((response: HttpErrorResponse) => {
            if (response.status === 401) {
              abp.message.error('Your session is expired,');
              setTimeout(() => { window.location.href = '/account/login'}, 3000);
              return;
            }
            if (response.status === 403 ) {
              window.location.href = '/app/no-permission' ;
              return;
            }

            const error = response.error;
            let errMsg = '';
            // let listErrMsg = errorMsg.DanhSach
            // let language : TranslateService
            if (error instanceof ErrorEvent) {
              errMsg = `Error: ${error.message}`;
            } else {
              errMsg = error ? `${error.error.message || response.message}` : response.message;
              abp.notify.error(errMsg);
            }
            return throwError(errMsg);
          })
        );
    }
  }
