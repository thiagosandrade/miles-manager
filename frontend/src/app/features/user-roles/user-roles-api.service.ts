import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { UserDetails, UserSummary } from '../user/models/user.models';
import { Role, UserRole, UserRoleRequest } from './models/user-roles.models';


@Injectable({
  providedIn: 'root'
})
export class UserRolesApiService {

  private readonly http = inject(HttpClient);

  private readonly apiBaseUrl = `${environment.apiBaseUrl}`;

  private readonly userRolesAPI = `${this.apiBaseUrl}/user-roles`;
  private readonly usersAPI = `${this.apiBaseUrl}/users`;

  listRoles(): Observable<Role[]>{
    return this.http.get<Role[]>(this.userRolesAPI);
  }

  getUserRoles(userId: string): Observable<UserRole[]> {

  return this.http
    .get<UserDetails>(`${this.usersAPI}/${userId}`)
    .pipe(
      map(user => user.roles ?? [])
    );

}

  assignRole(request: UserRoleRequest): Observable<void> {

    return this.http.post<void>(
      this.userRolesAPI,
      request);

  }

  removeRole(request: UserRoleRequest): Observable<void> {

    return this.http.request<void>(
      'DELETE',
      this.userRolesAPI,
      {
        body: request
      });

  }

}