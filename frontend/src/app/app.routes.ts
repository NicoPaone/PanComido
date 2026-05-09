import { Routes } from '@angular/router';

export const routes: Routes = [

   {
      path: '',redirectTo: 'prueba', pathMatch: 'full'

   },
   {
      path: 'prueba', loadComponent: () =>
          import('./shared/components/prueba/prueba').then(m => m.Prueba)
   }

];
