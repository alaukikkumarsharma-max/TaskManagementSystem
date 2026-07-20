import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';
import { Role } from '../models/role.model';

const STORAGE_KEY = 'taskflow.auth';
const API_URL = '/api/auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly authState = signal<AuthResponse | null>(this.readStoredAuth());

  readonly currentUser = computed(() => this.authState());
  readonly isAuthenticated = computed(() => this.authState() !== null);

  // Mirrors the [Authorize(Roles = "Admin,Manager")] gate on TasksController's
  // create/update/delete actions - Employees are view-only for tasks.
  readonly canManageTasks = computed(() => {
    const role = this.authState()?.role;
    return role === 'Admin' || role === 'Manager';
  });

  // Mirrors [Authorize(Roles = "Admin")] on AuditController - only Admins see the audit trail.
  readonly isAdmin = computed(() => this.authState()?.role === 'Admin');

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/login`, request)
      .pipe(tap((response) => this.setAuth(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/register`, request)
      .pipe(tap((response) => this.setAuth(response)));
  }

  // Backed by the bare GET on AuthController (no "/roles" segment) - roles
  // now live alongside login/register rather than their own controller.
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(API_URL);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.authState.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.authState()?.token ?? null;
  }

  private setAuth(response: AuthResponse): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
    this.authState.set(response);
  }

  private readStoredAuth(): AuthResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }

    try {
      const parsed = JSON.parse(raw) as AuthResponse;
      if (new Date(parsed.expiresAtUtc).getTime() <= Date.now()) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  }
}
