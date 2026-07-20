import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AuditService } from '../../core/services/audit.service';
import { AuditLogEntry } from '../../core/models/audit.model';

@Component({
  selector: 'app-audit-logs',
  imports: [DatePipe],
  templateUrl: './audit-logs.component.html',
  styleUrl: './audit-logs.component.scss'
})
export class AuditLogsComponent {
  private readonly auditService = inject(AuditService);

  readonly logs = signal<AuditLogEntry[]>([]);
  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);

  constructor() {
    this.auditService.getRecent().subscribe({
      next: (logs) => {
        this.logs.set(logs);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set('Could not load audit logs. Try refreshing the page.');
        this.isLoading.set(false);
      }
    });
  }

  // Maps an action to a badge colour class (see scss).
  actionClass(action: string): string {
    switch (action) {
      case 'Login':
        return 'action-login';
      case 'TaskCreated':
        return 'action-created';
      case 'TaskUpdated':
        return 'action-updated';
      case 'TaskDeleted':
        return 'action-deleted';
      default:
        return 'action-default';
    }
  }
}
