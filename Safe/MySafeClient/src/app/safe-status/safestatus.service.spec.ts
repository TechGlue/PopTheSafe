import { TestBed } from '@angular/core/testing';

import { SafeStatusService } from './safe-status.service';

describe('SafestatusService', () => {
  let service: SafeStatusService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SafeStatusService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
