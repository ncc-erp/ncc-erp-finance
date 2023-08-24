import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends BaseApiService {

  changeUrl() {
    return 'Role';
  }

  constructor(http: HttpClient) {
    super(http);
  }
  public getRole(): Observable<any> {
    return this.http.get<any>(this.rootUrl + '/GetRoles');
  }

  public getAllUsersInRole(roleId: number):Observable<any>{
    return this.http.get<any>(this.rootUrl + "/GetAllUsersInRole?roleId=" + roleId);
  }
  public GetUsersInRole(input):Observable<any>{
    return this.http.post<any>(this.rootUrl + "/GetUsersInRole", input);
  }
  public RemoveUserFromRole(id: number):Observable<any>{
    return this.http.delete(this.rootUrl + "/RemoveUserFromRole?Id=" + id);
  }
  public GetAllUsersNotInRole(roleId: number):Observable<any>{
    return this.http.get<any>(this.rootUrl + "/GetAllUsersNotInRole?roleId=" + roleId);
  }
  public AddUserIntoRole(input):Observable<any>{
    return this.http.post<any>(this.rootUrl + "/AddUserIntoRole" , input);
  }
}
