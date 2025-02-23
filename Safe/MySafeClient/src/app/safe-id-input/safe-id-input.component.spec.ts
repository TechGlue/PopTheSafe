import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SafeIdInputComponent } from './safe-id-input.component';

describe('SafeIdInputComponent', () => {
  let component: SafeIdInputComponent;
  let fixture: ComponentFixture<SafeIdInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SafeIdInputComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SafeIdInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
