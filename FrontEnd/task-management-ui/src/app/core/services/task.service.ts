import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateTaskPayload, PagedResult, Task, TaskItemStatus, TaskPriority, TaskStats, UpdateTaskPayload
} from '../models/task.model';
import { AppUser } from '../models/user.model';

export interface TaskQuery {
  search?: string;
  status?: TaskItemStatus;
  priority?: TaskPriority;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

const API_URL = '/api/tasks';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);

  // Returns a single page. Filtering/sorting/paging all happen in SQL so the
  // browser never holds more than pageSize rows.
  getTasks(query: TaskQuery = {}): Observable<PagedResult<Task>> {
    let params = new HttpParams()
      .set('sortDescending', query.sortDescending ?? false)
      .set('page', query.page ?? 1)
      .set('pageSize', query.pageSize ?? 20);

    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.status) {
      params = params.set('status', query.status);
    }
    if (query.priority) {
      params = params.set('priority', query.priority);
    }
    if (query.sortBy) {
      params = params.set('sortBy', query.sortBy);
    }

    return this.http.get<PagedResult<Task>>(API_URL, { params });
  }

  // Counts come pre-aggregated from SQL - the dashboard never downloads rows.
  getStats(): Observable<TaskStats> {
    return this.http.get<TaskStats>(`${API_URL}/stats`);
  }

  getTask(id: number): Observable<Task> {
    return this.http.get<Task>(`${API_URL}/${id}`);
  }

  // Backs the task-form "assign to" dropdown - lives on TasksController
  // rather than a standalone Users feature since tasks are the only consumer.
  getAssignableUsers(): Observable<AppUser[]> {
    return this.http.get<AppUser[]>(`${API_URL}/assignable-users`);
  }

  createTask(payload: CreateTaskPayload): Observable<Task> {
    return this.http.post<Task>(API_URL, payload);
  }

  updateTask(id: number, payload: UpdateTaskPayload): Observable<Task> {
    return this.http.put<Task>(`${API_URL}/${id}`, payload);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}`);
  }
}
