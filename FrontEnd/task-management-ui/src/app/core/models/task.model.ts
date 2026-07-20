// These string unions match the enum member names in
// TaskManagement.Domain.Enums exactly (Priority/Status are serialized as
// strings via JsonStringEnumConverter, configured in Program.cs).
export type TaskPriority = 'Low' | 'Medium' | 'High';
export type TaskItemStatus = 'ToDo' | 'InProgress' | 'Completed' | 'Cancelled';

export interface Task {
  taskId: number;
  title: string;
  description: string | null;
  priority: TaskPriority;
  status: TaskItemStatus;
  dueDate: string;
  assignedToUserId: number | null;
  assignedToName: string | null;
  createdByUserId: number;
  createdByName: string;
  createdDate: string;
  updatedDate: string;
}

// Mirrors CreateTaskDto - deliberately has no `status`: new tasks always
// start as ToDo server-side, so the create form doesn't ask for one.
export interface CreateTaskPayload {
  title: string;
  description: string | null;
  priority: TaskPriority;
  dueDate: string;
  assignedToUserId: number | null;
}

// Mirrors UpdateTaskDto.
export interface UpdateTaskPayload extends CreateTaskPayload {
  status: TaskItemStatus;
}

// Mirrors PagedResult<T> - one page of rows plus what the pager needs.
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Mirrors TaskStatsDto - dashboard counters aggregated server-side.
export interface TaskStats {
  total: number;
  toDo: number;
  inProgress: number;
  completed: number;
  cancelled: number;
  overdue: number;
}
