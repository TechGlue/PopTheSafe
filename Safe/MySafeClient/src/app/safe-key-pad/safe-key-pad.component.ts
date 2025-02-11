import { Component, Input, input, OnInit } from '@angular/core';
import { KeypadSubmitService } from './keypad-submit.service';
import { SafestatusService } from '../safe-status/safestatus.service';
import { catchError, Observable, throwError } from 'rxjs';
import { ISafeResponse } from '../safe-response';
import { AsyncPipe } from '@angular/common';

@Component({
  imports: [AsyncPipe],
  standalone: true,
  selector: 'app-safe-key-pad',
  templateUrl: './safe-key-pad.component.html',
})
export class SafeKeyPadComponent {
  constructor(
    private submitService: KeypadSubmitService,
    private safeStatusService: SafestatusService,
  ) {}

  failingResponse: ISafeResponse = {
    isSuccessful: false,
    isDetail: 'Verify safe input',
  };

  @Input() safeStatus!: string;

  safeStatus$!: Observable<ISafeResponse>;

  digits: Number[] = [0, 1, 2, 3, 4, 5, 6, 7, 9];
  digitsInput: string = '';

  ngOnInit(): void {
    this.safeStatus$ = this.safeStatusService.getSafeStatus();
  }

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }

  clearInput(): void {
    this.digitsInput = '';
  }

  safeClose(): void {
    this.safeStatus$ = this.submitService.closeSafe();
  }

  safeOpen(): void {
    this.safeStatus$ = this.submitService.openSafe();
  }

  safeLock(): void {
    this.safeStatus$ = this.submitService.lockSafe();
  }

  submitSafePin(pin: string): void {
    if (pin === null || pin.length == 0) {
      return;
    }

    // todo: need to work on the submit pin
    this.safeStatus$ = this.submitService.submitSafePin(pin).pipe(
      catchError(() => {
        return throwError(() => this.failingResponse);
      }),
    );

    this.digitsInput = '';
  }

  resetSafePin(): void {
    this.safeStatus$ = this.submitService.resetSafePin();
  }
}
