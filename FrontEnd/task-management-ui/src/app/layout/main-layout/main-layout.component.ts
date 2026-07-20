import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  private readonly authService = inject(AuthService);
  private readonly elementRef = inject(ElementRef);

  readonly currentUser = this.authService.currentUser;
  readonly canManageTasks = this.authService.canManageTasks;
  readonly isAdmin = this.authService.isAdmin;
  readonly isUserMenuOpen = signal(false);

  get initials(): string {
    const name = this.currentUser()?.fullName ?? '';
    return name
      .split(' ')
      .filter(Boolean)
      .map((part) => part.charAt(0))
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  toggleUserMenu(): void {
    this.isUserMenuOpen.update((open) => !open);
  }

  logout(): void {
    this.authService.logout();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isUserMenuOpen() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isUserMenuOpen.set(false);
    }
  }
}
