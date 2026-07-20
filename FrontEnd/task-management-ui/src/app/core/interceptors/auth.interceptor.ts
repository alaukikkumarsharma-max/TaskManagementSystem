import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ConnectivityService } from '../services/connectivity.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const connectivityService = inject(ConnectivityService);
  const token = authService.getToken();

  const authorizedReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authorizedReq).pipe(
    tap(() => connectivityService.markReachable()),
    catchError((error: HttpErrorResponse) => {
      // status 0 means no HTTP response came back at all - the API process
      // isn't running, is on the wrong port, or a CORS preflight failed.
      // That's a different problem than a normal 4xx/5xx and worth surfacing
      // globally rather than leaving each screen to fail silently.
      if (error.status === 0) {
        connectivityService.markUnreachable();
      }

      // A 401 here means the token is missing, expired, or invalid -
      // in every case the right move is to drop the session and send
      // the user back to login rather than let the app sit in a broken state.
      if (error.status === 401) {
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};
