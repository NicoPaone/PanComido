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
    expect(component.platoForm.get('nombre')?.value).toBe('');
    expect(component.platoForm.get('costo')?.value).toBe(0);
    expect(component.platoForm.get('precioVenta')?.value).toBe(0);
    expect(component.platoForm.get('tiempoPreparacion')?.value).toBe(15);
    expect(component.platoForm.get('tipoPlato')?.value).toBe('');
    expect(component.platoForm.get('descripcion')?.value).toBe('');
    expect(component.visible()).toBe(true);
    expect(component.imagenSelected()).toContain('photo');
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

  it('debería guardar el plato y mostrar éxito al completar datos válidos', async () => {
    component.platoForm.patchValue({
      nombre: 'Pasta',
      costo: 50,
      precioVenta: 120,
      tiempoPreparacion: 15,
      tipoPlato: 'Plato Principal',
      descripcion: 'Pasta casera artesanal'
    });
    component.visible.set(true);
    component.imagenSelected.set('url-imagen');
    component.vegano.set(true);
    component.vegetariano.set(true);
    component.celiaco.set(false);
    const ingredientes: RecetaIngrediente[] = [
      { id: '2', nombre: 'Harina', cantidad: 200, unidadMedida: 'GR' }
    ];
    component.receta.set(ingredientes);

    component.guardar();
    
    // Espera para resolver la llamada diferida del mock en PlatoService
    await new Promise(resolve => setTimeout(resolve, 250));

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
