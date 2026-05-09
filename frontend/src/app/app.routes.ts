import { Routes } from '@angular/router';

export const routes: Routes = [

   {
      path: '',redirectTo: 'inicio', pathMatch: 'full'

   },
   {
      path: 'inicio', loadComponent: () =>
          import('./shared/components/inicio/inicio').then(m => m.Inicio)
   }

];
