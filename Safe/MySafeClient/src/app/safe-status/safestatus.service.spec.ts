import { TestBed } from '@angular/core/testing';

import { SafestatusService } from './safestatus.service';

describe('SafestatusService', () => {
  let service: SafestatusService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SafestatusService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
