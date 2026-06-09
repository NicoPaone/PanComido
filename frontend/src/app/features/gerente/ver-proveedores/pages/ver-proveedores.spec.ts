import { ComponentFixture, TestBed } from '@angular/core/testing';
import { INSUMOS_MOCK as PRODUCTOS_STOCK_MOCK } from '../../../../infra/mocks/insumo.mock-data';
import { VerProveedoresComponent } from './ver-proveedores';

describe('VerProveedoresComponent', () => {
  let component: VerProveedoresComponent;
  let fixture: ComponentFixture<VerProveedoresComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VerProveedoresComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(VerProveedoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should add an item to the pedido list', () => {
    component.productos.set([...PRODUCTOS_STOCK_MOCK]);
    component.onProductoTextoChange('Ajo');
    component.cantidadProducto.set(2);
    component.precioProductoManual.set(500);

    component.agregarItemPedido();

    expect(component.pedidoItems()).toHaveLength(1);
    expect(component.pedidoItems()[0]).toEqual(
      expect.objectContaining({
        id: 1,
        nombre: 'Ajo',
        cantidad: 2,
        unidadMedida: { id: 1, nombre: 'Kg' }
      })
    );
  });

  it('should remove an item from the pedido list', () => {
    component.pedidoItems.set([
      { id: '1', nombre: 'Ajo', cantidad: 2, unidadMedida: { id: 1, nombre: 'Kg' } }
    ]);

    component.eliminarItemPedido('1');

    expect(component.pedidoItems()).toHaveLength(0);
  });
});
