import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { manageTasksGuard } from './core/guards/manage-tasks.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: '',
    loadComponent: () => import('./layout/main-layout/main-layout.component').then((m) => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'tasks',
        loadComponent: () => import('./features/tasks/task-list/task-list.component').then((m) => m.TaskListComponent)
      },
      {
        path: 'tasks/new',
        loadComponent: () => import('./features/tasks/task-form/task-form.component').then((m) => m.TaskFormComponent),
        canActivate: [manageTasksGuard]
      },
      {
        path: 'tasks/:id/edit',
        loadComponent: () => import('./features/tasks/task-form/task-form.component').then((m) => m.TaskFormComponent),
        canActivate: [manageTasksGuard]
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('./features/audit-logs/audit-logs.component').then((m) => m.AuditLogsComponent),
        canActivate: [adminGuard]
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
