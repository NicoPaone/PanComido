import { Routes } from '@angular/router';

export const GERENTE_ROUTES: Routes = [
  {
    path: 'modificar-carta',
    loadComponent: () => import('./modificar-carta/page/modificar-carta').then(m => m.ModificarCartaComponent)
  }

];