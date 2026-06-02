import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerCartaComponent } from './ver-carta';

describe('VerCartaComponent', () => {

  let component: VerCartaComponent;
  let fixture: ComponentFixture<VerCartaComponent>;

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [VerCartaComponent],
    })
    .compileComponents();

    fixture = TestBed.createComponent(VerCartaComponent);

    component = fixture.componentInstance;

    await fixture.whenStable();

  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

});