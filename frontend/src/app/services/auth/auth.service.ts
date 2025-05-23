import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  tenantIdentifier?: string;
}

export interface AuthResponse {
  message: string;
  token: string;
  tenant?: {
    id: string;
    identifier: string;
    name: string;
  };
}

export interface EmailVerificationRequest {
  email: string;
  verificationCode: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/api/auth`;
  private tokenKey = 'auth_token';
  private tenantIdKey = 'tenant_id';
  private userKey = 'user_data';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Load user from localStorage on service initialization
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(response => {
        this.setToken(response.token);
        if (response.tenant?.identifier) {
          this.setTenantId(response.tenant.identifier);
        }
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data).pipe(
      tap(response => {
        this.setToken(response.token);
        if (response.tenant?.identifier) {
          this.setTenantId(response.tenant.identifier);
        }
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.tenantIdKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getTenantId(): string | null {
    return localStorage.getItem(this.tenantIdKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private setTenantId(tenantId: string): void {
    localStorage.setItem(this.tenantIdKey, tenantId);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  verifyEmail(data: EmailVerificationRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/verify-email`, data);
  }

  requestVerificationCode(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/request-verification-code`, { email });
  }

  resetPassword(email: string, code: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/reset-password`, {
      email,
      code,
      newPassword
    });
  }

  // Google Login SSO

  googleLogin(idToken: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl.replace('/auth', '/sso')}/google`, { idToken }).pipe(
      tap(response => {
        this.setToken(response.token);
        if (response.tenant?.identifier) {
          this.setTenantId(response.tenant.identifier);
        }
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  // Microsoft Login SSO
  microsoftLogin(idToken: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl.replace('/auth', '/sso')}/microsoft`, { idToken }).pipe(
      tap(response => {
        this.setToken(response.token);
        if (response.tenant?.identifier) {
          this.setTenantId(response.tenant.identifier);
        }
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  getCurrentUserId(): string | null {
    const currentUser = this.currentUserSubject.value;
    return currentUser?.id || null;
  }

  private getHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getProfile(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/profile`, { headers: this.getHeaders() }).pipe(
      tap(response => {
        if (response.tenant?.identifier) {
          this.setTenantId(response.tenant.identifier);
        }
        this.currentUserSubject.next(response);
        localStorage.setItem(this.userKey, JSON.stringify(response));
      })
    );
  }
}
