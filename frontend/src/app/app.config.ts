import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { mockInterceptor } from './core/interceptors/mock.interceptor';
import { jwtInterceptor } from './core/interceptors/jwt/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([jwtInterceptor,mockInterceptor])),
    { provide: LOCALE_ID, useValue: 'es-AR' }
  ]
};

