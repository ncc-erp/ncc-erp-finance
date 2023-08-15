import { Observable, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class CommentService extends BaseApiService {

  constructor(http: HttpClient) { super(http); }
  changeUrl() {
    return 'Comment';
  }
  GetAllCommentByPost(id): Observable<any> {
    return this.http.get(this.rootUrl + `/GetAllCommentByPost?requestId=${id}`)
  }
  createComment(item: any): Observable<any> {
    return this.http.post<any>(this.rootUrl + '/CreateCommentByPost', item);
  }
  public editComment(item: any): Observable<any> {
    return this.http.put<any>(this.rootUrl + '/UpdateCommentByPost', item);
  }
  handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error: ${error.error.error.message}`;
    }
    return throwError(errorMessage);
  }
}
