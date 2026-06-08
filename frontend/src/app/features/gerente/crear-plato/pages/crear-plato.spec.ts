import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { CrearPlatoPage } from './crear-plato';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { vi } from 'vitest';
import { of } from 'rxjs';
import { CrearPlatoApiService } from '../services/crear-plato.api';

describe('CrearPlatoPage', () => {
  let component: CrearPlatoPage;
  let fixture: ComponentFixture<CrearPlatoPage>;
  let routerMock: any;

  beforeEach(async () => {
    routerMock = {
      navigate: vi.fn(),
      getCurrentNavigation: vi.fn().mockReturnValue(null),
    };

    const apiMock = {
      crearPlato: vi.fn().mockReturnValue(of({ id: 1 })),
    };

    await TestBed.configureTestingModule({
      imports: [CrearPlatoPage],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: CrearPlatoApiService, useValue: apiMock },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CrearPlatoPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar con los valores por defecto del state', () => {
    expect(component.visible()).toBe(true);
    expect(component.vegano()).toBe(false);
    expect(component.vegetariano()).toBe(false);
    expect(component.celiaco()).toBe(false);
    expect(component.receta()).toEqual([]);
    expect(component.mostrarExito()).toBe(false);
  });

  it('debería delegar toggleTag al state', () => {
    component.onToggleTag('vegano');
    expect(component.vegano()).toBe(true);
    component.onToggleTag('vegano');
    expect(component.vegano()).toBe(false);
  });

  it('debería delegar toggleVisible al state', () => {
    component.onToggleVisible();
    expect(component.visible()).toBe(false);
    component.onToggleVisible();
    expect(component.visible()).toBe(true);
  });

  it('debería delegar updateReceta al state', () => {
    const ingredientes: RecetaIngrediente[] = [
      { id: '1', nombre: 'Sal', cantidad: 5, unidadMedida: 'GR' }
    ];
    component.onRecetaCambiada(ingredientes);
    expect(component.receta()).toEqual(ingredientes);
  });

  it('debería ocultar éxito y navegar al cerrar con éxito', () => {
    component.mostrarExito.set(true);
    component.onCerrarExito();
    expect(component.mostrarExito()).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff/gerente/modificar-carta']);
  });

  it('debería navegar a la carta al cancelar', () => {
    component.onCancelar();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff/gerente/modificar-carta']);
  });

  it('debería seleccionar imagen y cerrar el selector', () => {
    const url = 'https://example.com/image.jpg';
    component.onSeleccionarImagen(url);
    expect(component.imagenSelected()).toBe(url);
    expect(component.mostrarSelectorImagen()).toBe(false);
  });

  it('debería abrir y cerrar el selector de imagen', () => {
    component.onAbrirSelectorImagen();
    expect(component.mostrarSelectorImagen()).toBe(true);
    component.onCerrarSelectorImagen();
    expect(component.mostrarSelectorImagen()).toBe(false);
  });
});
