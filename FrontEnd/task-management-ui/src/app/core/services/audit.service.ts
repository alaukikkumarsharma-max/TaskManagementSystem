import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuditLogEntry } from '../models/audit.model';

@Injectable({ providedIn: 'root' })
export class AuditService {
  private readonly http = inject(HttpClient);

  getRecent(): Observable<AuditLogEntry[]> {
    return this.http.get<AuditLogEntry[]>('/api/audit');
  }
}
