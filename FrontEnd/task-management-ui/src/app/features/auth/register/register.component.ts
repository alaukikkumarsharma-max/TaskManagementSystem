import { Component, OnInit, inject, signal } from '@angular/core';
import { ReactiveFormsModule, Validators, FormBuilder } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { Role } from '../../../core/models/role.model';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly roles = signal<Role[]>([]);
  readonly rolesLoadFailed = signal(false);

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    // 0 is a sentinel for "nothing picked yet" - real RoleIds start at 1.
    roleId: [0, [Validators.min(1)]]
  });

  constructor() {
    // Already signed in - no reason to show a registration form.
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngOnInit(): void {
    this.authService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: () => this.rolesLoadFailed.set(true)
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(this.messageFor(err));
      }
    });
  }

  private messageFor(err: HttpErrorResponse): string {
    if (err.status === 0) {
      return 'Cannot reach the server. Make sure the API is running.';
    }
    if (err.status === 400) {
      return err.error?.message ?? 'That email is already registered.';
    }
    return 'Something went wrong. Please try again.';
  }
}
