import { TestBed } from '@angular/core/testing';

import { KeypadSubmitService } from './keypad-submit.service';

describe('KeypadSubmitService', () => {
  let service: KeypadSubmitService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(KeypadSubmitService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
