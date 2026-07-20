// Matches AuditLogDto from GET /api/audit.
export interface AuditLogEntry {
  auditLogId: number;
  userName: string;
  action: string;
  entityName: string | null;
  entityId: number | null;
  details: string | null;
  createdDate: string;
}
