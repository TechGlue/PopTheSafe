import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SafeKeyPadComponent } from './safe-key-pad.component';

describe('SafeKeyPadComponent', () => {
  let component: SafeKeyPadComponent;
  let fixture: ComponentFixture<SafeKeyPadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SafeKeyPadComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SafeKeyPadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
