import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type Role = string;

export interface RoleAssignment {
  userId: string;
  roleName: string;
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/api/role`;
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) {}

  getAllRoles(): Observable<{ message: string; roles: Role[] }> {
    return this.http.get<{ message: string; roles: Role[] }>(this.apiUrl);
  }

  createRole(roleName: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.apiUrl, JSON.stringify(roleName), this.httpOptions);
  }

  deleteRole(roleName: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${encodeURIComponent(roleName)}`);
  }

  assignRoleToUser(assignment: RoleAssignment): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/assign`, assignment, this.httpOptions);
  }

  removeRoleFromUser(assignment: RoleAssignment): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/remove`, assignment, this.httpOptions);
  }

  getUserRoles(userId: string): Observable<{ message: string; roles: Role[] }> {
    return this.http.get<{ message: string; roles: Role[] }>(`${this.apiUrl}/by-user/${userId}`);
  }

  getUsersInRole(roleName: string): Observable<{ message: string; users: any[] }> {
    return this.http.get<{ message: string; users: any[] }>(`${this.apiUrl}/by-role/${encodeURIComponent(roleName)}/users`);
  }
}
