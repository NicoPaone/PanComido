import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StaffLayout } from './staff-layout';
import { provideRouter } from '@angular/router';

describe('StaffLayout', () => {
  let component: StaffLayout;
  let fixture: ComponentFixture<StaffLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffLayout],
      providers: [
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StaffLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
