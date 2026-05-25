import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { CrearPlatoComponent } from './crear-plato';
import { RecetaIngrediente } from '../../../../core/models/plato';
import { vi } from 'vitest';

describe('CrearPlatoComponent', () => {
  let component: CrearPlatoComponent;
  let fixture: ComponentFixture<CrearPlatoComponent>;
  let routerMock: any;

  beforeEach(async () => {
    routerMock = {
      navigate: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [CrearPlatoComponent],
      providers: [
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CrearPlatoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería inicializar con los valores por defecto', () => {
    expect(component.nombre()).toBe('');
    expect(component.costo()).toBeNull();
    expect(component.precioVenta()).toBeNull();
    expect(component.tiempoPreparacion()).toBeNull();
    expect(component.tipoPlato()).toBe('');
    expect(component.descripcion()).toBe('');
    expect(component.visible()).toBe(true);
    expect(component.imagen()).toBe('');
    expect(component.vegano()).toBe(false);
    expect(component.vegetariano()).toBe(false);
    expect(component.celiaco()).toBe(false);
    expect(component.receta()).toEqual([]);
    expect(component.mostrarExito()).toBe(false);
  });

  it('debería alternar el tag vegano al llamar a toggleTag', () => {
    component.toggleTag('vegano');
    expect(component.vegano()).toBe(true);
    component.toggleTag('vegano');
    expect(component.vegano()).toBe(false);
  });

  it('debería alternar el tag vegetariano al llamar a toggleTag', () => {
    component.toggleTag('vegetariano');
    expect(component.vegetariano()).toBe(true);
    component.toggleTag('vegetariano');
    expect(component.vegetariano()).toBe(false);
  });

  it('debería alternar el tag celiaco al llamar a toggleTag', () => {
    component.toggleTag('celiaco');
    expect(component.celiaco()).toBe(true);
    component.toggleTag('celiaco');
    expect(component.celiaco()).toBe(false);
  });

  it('debería alternar la visibilidad al llamar a onToggleVisible', () => {
    component.onToggleVisible();
    expect(component.visible()).toBe(false);
    component.onToggleVisible();
    expect(component.visible()).toBe(true);
  });

  it('debería actualizar la receta al llamar a onRecetaCambiada', () => {
    const ingredientes: RecetaIngrediente[] = [
      { id: '1', nombre: 'Sal', cantidad: 5, unidadMedida: 'GR' }
    ];
    component.onRecetaCambiada(ingredientes);
    expect(component.receta()).toEqual(ingredientes);
  });

  it('debería imprimir el plato y mostrar éxito al guardar', () => {
    const consoleSpy = vi.spyOn(console, 'log');
    component.nombre.set('Pasta');
    component.costo.set(50);
    component.precioVenta.set(120);
    component.tiempoPreparacion.set(15);
    component.tipoPlato.set('Plato Principal');
    component.descripcion.set('Pasta casera');
    component.visible.set(true);
    component.imagen.set('url-imagen');
    component.vegano.set(true);
    component.vegetariano.set(true);
    component.celiaco.set(false);
    const ingredientes: RecetaIngrediente[] = [
      { id: '2', nombre: 'Harina', cantidad: 200, unidadMedida: 'GR' }
    ];
    component.receta.set(ingredientes);

    component.guardar();

    expect(consoleSpy).toHaveBeenCalledWith('Guardando plato:', {
      nombre: 'Pasta',
      costo: 50,
      precioVenta: 120,
      tiempoPreparacion: 15,
      tipoPlato: 'Plato Principal',
      descripcion: 'Pasta casera',
      visible: true,
      imagen: 'url-imagen',
      tags: {
        vegano: true,
        vegetariano: true,
        celiaco: false
      },
      receta: ingredientes
    });
    expect(component.mostrarExito()).toBe(true);
  });

  it('debería ocultar éxito y navegar al cerrar con éxito', () => {
    component.mostrarExito.set(true);
    component.cerrarExito();
    expect(component.mostrarExito()).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff/gerente/modificar-carta']);
  });

  it('debería navegar a la carta al cancelar', () => {
    component.cancelar();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff/gerente/modificar-carta']);
  });
});
