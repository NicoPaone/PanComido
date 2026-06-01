import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InsumoList } from './insumo-list';

describe('InsumoList', () => {
  let component: InsumoList;
  let fixture: ComponentFixture<InsumoList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsumoList],
    }).compileComponents();

    fixture = TestBed.createComponent(InsumoList);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('productos', []);
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
