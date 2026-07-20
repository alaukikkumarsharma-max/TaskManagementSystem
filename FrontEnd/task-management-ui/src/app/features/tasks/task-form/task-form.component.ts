import { Component, computed, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TaskService } from '../../../core/services/task.service';
import { AppUser } from '../../../core/models/user.model';
import { TaskItemStatus, TaskPriority } from '../../../core/models/task.model';

// Mirrors TaskManagement.Application.Validation.FutureDateAttribute on the
// backend - same rule, checked client-side first so the user finds out
// before a round trip, and re-checked server-side regardless.
function futureDateValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return new Date(control.value) < today ? { pastDate: true } : null;
}

@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.scss'
})
export class TaskFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly taskId = this.route.snapshot.paramMap.get('id');
  readonly isEditMode = computed(() => this.taskId !== null);
  readonly minDate = new Date().toISOString().substring(0, 10);

  readonly users = signal<AppUser[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly priorities: TaskPriority[] = ['Low', 'Medium', 'High'];
  readonly statuses: TaskItemStatus[] = ['ToDo', 'InProgress', 'Completed', 'Cancelled'];

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    priority: ['Medium' as TaskPriority, [Validators.required]],
    status: ['ToDo' as TaskItemStatus, [Validators.required]],
    dueDate: ['', [Validators.required, futureDateValidator]],
    assignedToUserId: this.fb.control<number | null>(null)
  });

  constructor() {
    this.taskService.getAssignableUsers().subscribe((users) => this.users.set(users));

    if (this.taskId) {
      this.loadTask(+this.taskId);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const basePayload = {
      title: raw.title,
      description: raw.description || null,
      priority: raw.priority,
      dueDate: raw.dueDate,
      assignedToUserId: raw.assignedToUserId
    };

    const request$ = this.isEditMode()
      ? this.taskService.updateTask(+this.taskId!, { ...basePayload, status: raw.status })
      : this.taskService.createTask(basePayload);

    request$.subscribe({
      next: () => this.router.navigate(['/tasks']),
      error: (err: HttpErrorResponse) => {
        this.isSaving.set(false);
        this.errorMessage.set(
          err.error?.message || 'Could not save this task. Please check the form and try again.'
        );
      }
    });
  }

  private loadTask(id: number): void {
    this.isLoading.set(true);
    this.taskService.getTask(id).subscribe({
      next: (task) => {
        this.form.patchValue({
          title: task.title,
          description: task.description ?? '',
          priority: task.priority,
          status: task.status,
          dueDate: task.dueDate.substring(0, 10),
          assignedToUserId: task.assignedToUserId
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load this task.');
        this.isLoading.set(false);
      }
    });
  }
}
