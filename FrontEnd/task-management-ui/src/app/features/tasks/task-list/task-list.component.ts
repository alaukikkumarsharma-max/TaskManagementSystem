import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { Task, TaskItemStatus, TaskPriority } from '../../../core/models/task.model';

// Status and Priority are now filter dimensions, so sorting is limited to these two.
type SortField = 'dueDate' | 'title';

// How many numbered page buttons to show at once - the window slides with the
// current page so the pager stays the same width whether there are 3 pages or 50,000.
const PAGE_WINDOW = 5;

@Component({
  selector: 'app-task-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent {
  private readonly taskService = inject(TaskService);
  private readonly authService = inject(AuthService);
  private readonly searchInput$ = new Subject<string>();

  readonly canManageTasks = this.authService.canManageTasks;
  readonly tasks = signal<Task[]>([]);
  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly sortBy = signal<SortField>('dueDate');
  readonly sortDescending = signal(false);
  // Empty string means "all" - no filter applied.
  readonly statusFilter = signal<TaskItemStatus | ''>('');
  readonly priorityFilter = signal<TaskPriority | ''>('');
  readonly taskPendingDelete = signal<Task | null>(null);
  readonly isDeleting = signal(false);

  // Paging state - the server owns the slicing, these just describe what to ask for.
  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly totalCount = signal(0);
  readonly pageSizeOptions = [10, 20, 50];

  readonly totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()) || 1);
  readonly rangeStart = computed(() => (this.totalCount() === 0 ? 0 : (this.page() - 1) * this.pageSize() + 1));
  readonly rangeEnd = computed(() => Math.min(this.page() * this.pageSize(), this.totalCount()));

  readonly pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.page();
    let start = Math.max(1, current - Math.floor(PAGE_WINDOW / 2));
    const end = Math.min(total, start + PAGE_WINDOW - 1);
    start = Math.max(1, end - PAGE_WINDOW + 1);

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  });

  private currentSearch = '';

  constructor() {
    this.searchInput$.pipe(debounceTime(350), distinctUntilChanged()).subscribe((term) => {
      this.currentSearch = term;
      this.reload();
    });

    this.loadTasks();
  }

  onSearchInput(value: string): void {
    this.searchInput$.next(value);
  }

  onManageTaskClick(event: Event): void {
    if (!this.canManageTasks()) {
      event.preventDefault();
    }
  }

  setSort(field: SortField): void {
    if (this.sortBy() === field) {
      this.sortDescending.update((value) => !value);
    } else {
      this.sortBy.set(field);
      this.sortDescending.set(false);
    }
    this.reload();
  }

  // Called by the "Sort by" dropdown - sets the field directly (no toggle).
  onSortFieldChange(field: string): void {
    this.sortBy.set(field as SortField);
    this.reload();
  }

  // Called by the direction dropdown ("false" = ascending, "true" = descending).
  onSortDirectionChange(descending: string): void {
    this.sortDescending.set(descending === 'true');
    this.reload();
  }

  // Called by the Status filter dropdown ("" = all statuses).
  onStatusFilterChange(status: string): void {
    this.statusFilter.set(status as TaskItemStatus | '');
    this.reload();
  }

  // Called by the Priority filter dropdown ("" = all priorities).
  onPriorityFilterChange(priority: string): void {
    this.priorityFilter.set(priority as TaskPriority | '');
    this.reload();
  }

  onPageSizeChange(size: string): void {
    this.pageSize.set(+size);
    this.reload();
  }

  goToPage(target: number): void {
    if (target < 1 || target > this.totalPages() || target === this.page()) {
      return;
    }
    this.page.set(target);
    this.loadTasks();
  }

  sortIcon(field: SortField): string {
    if (this.sortBy() !== field) {
      return 'bi-arrow-down-up';
    }
    return this.sortDescending() ? 'bi-sort-down' : 'bi-sort-up';
  }

  confirmDelete(task: Task): void {
    if (!this.canManageTasks()) {
      return;
    }
    this.taskPendingDelete.set(task);
  }

  cancelDelete(): void {
    this.taskPendingDelete.set(null);
  }

  deleteConfirmed(): void {
    const task = this.taskPendingDelete();
    if (!task) {
      return;
    }

    this.isDeleting.set(true);
    this.taskService.deleteTask(task.taskId).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.taskPendingDelete.set(null);

        // Refetch rather than splice locally: the row count changed, so the
        // total, the page window, and which rows belong on this page all shift.
        // If that emptied the last page, step back one.
        if (this.tasks().length === 1 && this.page() > 1) {
          this.page.update((p) => p - 1);
        }
        this.loadTasks();
      },
      error: () => {
        this.isDeleting.set(false);
        this.loadError.set('Could not delete this task. Please try again.');
      }
    });
  }

  // Any change to search/filter/sort/page-size invalidates the current offset -
  // page 7 of the old result set is meaningless in the new one.
  private reload(): void {
    this.page.set(1);
    this.loadTasks();
  }

  private loadTasks(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.taskService.getTasks({
      search: this.currentSearch || undefined,
      status: this.statusFilter() || undefined,
      priority: this.priorityFilter() || undefined,
      sortBy: this.sortBy(),
      sortDescending: this.sortDescending(),
      page: this.page(),
      pageSize: this.pageSize()
    }).subscribe({
      next: (result) => {
        this.tasks.set(result.items);
        this.totalCount.set(result.totalCount);
        this.page.set(result.page);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set('Could not load tasks. Try refreshing the page.');
        this.isLoading.set(false);
      }
    });
  }
}
