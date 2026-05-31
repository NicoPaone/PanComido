import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { mockInterceptor } from './core/interceptors/mock.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([mockInterceptor]))
  ]
};


//MEJORAR PARA DESPUES CAMBIAR ROL SEGUN AUTENTICACION 
export const ROLE_ROUTES: Record<string, string> = {
  'Gerente': 'staff/gerente',
  'Cocina':  'staff/cocina',
  'Mozo':    'staff/mozo',
};

export const DEFAULT_ROUTE = 'staff/cocina';