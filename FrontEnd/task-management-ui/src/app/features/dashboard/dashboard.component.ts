import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { TaskService } from '../../core/services/task.service';
import { Task, TaskStats } from '../../core/models/task.model';

// The dashboard only ever needs a handful of rows for the "Recent tasks" panel;
// every number on the page comes pre-aggregated from /api/tasks/stats.
const RECENT_TASK_COUNT = 5;

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  private readonly authService = inject(AuthService);
  private readonly taskService = inject(TaskService);

  readonly currentUser = this.authService.currentUser;
  readonly firstName = computed(() => this.currentUser()?.fullName?.split(' ')[0] ?? '');
  readonly canManageTasks = this.authService.canManageTasks;

  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly recentTasks = signal<Task[]>([]);

  private readonly stats = signal<TaskStats | null>(null);

  readonly totalCount = computed(() => this.stats()?.total ?? 0);
  readonly toDoCount = computed(() => this.stats()?.toDo ?? 0);
  readonly inProgressCount = computed(() => this.stats()?.inProgress ?? 0);
  readonly completedCount = computed(() => this.stats()?.completed ?? 0);
  readonly cancelledCount = computed(() => this.stats()?.cancelled ?? 0);
  readonly overdueCount = computed(() => this.stats()?.overdue ?? 0);

  // One row per status with its count, colour, and share of the total.
  readonly statusSegments = computed(() => {
    const total = this.totalCount();
    const rows = [
      { label: 'To Do', count: this.toDoCount(), color: '#3b82f6' },
      { label: 'In Progress', count: this.inProgressCount(), color: '#f59e0b' },
      { label: 'Completed', count: this.completedCount(), color: '#22c55e' },
      { label: 'Cancelled', count: this.cancelledCount(), color: '#94a3b8' }
    ];
    return rows.map((r) => ({ ...r, percent: total ? Math.round((r.count / total) * 100) : 0 }));
  });

  // Builds the conic-gradient that paints the donut; grey ring when there are no tasks.
  readonly donutGradient = computed(() => {
    const total = this.totalCount();
    if (!total) {
      return 'conic-gradient(#e5e7eb 0deg 360deg)';
    }
    let acc = 0;
    const stops = this.statusSegments()
      .filter((s) => s.count > 0)
      .map((s) => {
        const start = (acc / total) * 360;
        acc += s.count;
        const end = (acc / total) * 360;
        return `${s.color} ${start}deg ${end}deg`;
      });
    return `conic-gradient(${stops.join(', ')})`;
  });

  constructor() {
    this.loadDashboard();
  }

  onNewTaskClick(event: Event): void {
    if (!this.canManageTasks()) {
      event.preventDefault();
    }
  }

  private loadDashboard(): void {
    this.isLoading.set(true);

    // Counts and the recent list are two small, independent queries - no sortBy
    // means the API's default ordering (newest created first) applies.
    forkJoin({
      stats: this.taskService.getStats(),
      recent: this.taskService.getTasks({ page: 1, pageSize: RECENT_TASK_COUNT })
    }).subscribe({
      next: ({ stats, recent }) => {
        this.stats.set(stats);
        this.recentTasks.set(recent.items);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set('Could not load tasks. Try refreshing the page.');
        this.isLoading.set(false);
      }
    });
  }
}
