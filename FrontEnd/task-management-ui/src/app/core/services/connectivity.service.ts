import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ConnectivityService {
  readonly isBackendUnreachable = signal(false);

  markUnreachable(): void {
    this.isBackendUnreachable.set(true);
  }

  markReachable(): void {
    this.isBackendUnreachable.set(false);
  }
}
