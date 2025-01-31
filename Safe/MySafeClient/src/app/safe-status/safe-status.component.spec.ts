import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SafeStatusComponent } from './safe-status.component';

describe('SafeStatusComponent', () => {
  let component: SafeStatusComponent;
  let fixture: ComponentFixture<SafeStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SafeStatusComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SafeStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
