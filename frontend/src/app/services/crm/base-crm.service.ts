import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseCRMService<T, CreateDTO, UpdateDTO> {
  protected apiUrl: string;

  constructor(
    protected http: HttpClient,
    endpoint: string,
    protected authService: AuthService
  ) {
    this.apiUrl = `${environment.apiUrl}/api/${endpoint}`;
  }

  protected getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    const tenantId = this.authService.getTenantId();

    if (!token) {
      throw new Error('Authentication token is required');
    }

    if (!tenantId) {
      throw new Error('Tenant identifier is required. Please contact your administrator.');
    }

    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
      'X-Tenant-Identifier': tenantId
    });
  }

  create(dto: CreateDTO): Observable<{ message: string; data: T }> {
    return this.http.post<{ message: string; data: T }>(this.apiUrl, dto, { headers: this.getHeaders() }).pipe(
      catchError(error => {
        console.error('Error creating item:', error);
        return throwError(() => new Error(error.error?.message || 'An unexpected error occurred'));
      })
    );
  }

  update(id: string, dto: UpdateDTO): Observable<{ message: string; data: T }> {
    return this.http.put<{ message: string; data: T }>(`${this.apiUrl}/${id}`, dto, { headers: this.getHeaders() }).pipe(
      catchError(error => {
        console.error('Error updating item:', error);
        return throwError(() => new Error(error.error?.message || 'An unexpected error occurred'));
      })
    );
  }

  delete(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      catchError(error => {
        console.error('Error deleting item:', error);
        return throwError(() => new Error(error.error?.message || 'An unexpected error occurred'));
      })
    );
  }

  getById(id: string): Observable<{ message: string; data: T }> {
    return this.http.get<{ message: string; data: T }>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      catchError(error => {
        console.error('Error fetching item:', error);
        return throwError(() => new Error(error.error?.message || 'An unexpected error occurred'));
      })
    );
  }

  getAll(): Observable<{ message: string; data: T[] }> {
    return this.http.get<{ message: string; data: T[] }>(this.apiUrl, { headers: this.getHeaders() }).pipe(
      catchError(error => {
        console.error('Error fetching data:', error);
        return throwError(() => new Error(error.error?.message || 'An unexpected error occurred'));
      })
    );
  }
}
