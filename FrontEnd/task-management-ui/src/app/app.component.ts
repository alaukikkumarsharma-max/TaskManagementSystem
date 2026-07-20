import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ConnectivityService } from './core/services/connectivity.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: `
    @if (connectivityService.isBackendUnreachable()) {
      <div class="connectivity-banner">
        <i class="bi bi-wifi-off"></i>
        Can't reach the server. Make sure the API is running, then try again.
      </div>
    }
    <router-outlet />
  `,
  styles: [
    `
      .connectivity-banner {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        z-index: 2000;
        background: #dc2626;
        color: #fff;
        text-align: center;
        padding: 0.5rem 1rem;
        font-size: 0.9rem;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
      }
    `
  ]
})
export class AppComponent {
  protected readonly connectivityService = inject(ConnectivityService);
}
