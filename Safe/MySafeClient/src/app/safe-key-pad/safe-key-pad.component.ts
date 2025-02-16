import {Component, Input, input, OnInit} from '@angular/core';
import {KeypadSubmitService} from './keypad-submit.service';
import {SafeStatusService} from '../safe-status/safe-status.service';
import {catchError, Observable, throwError} from 'rxjs';
import {ISafeResponse} from '../safe-response';
import {AsyncPipe} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';

@Component({
  imports: [AsyncPipe, ReactiveFormsModule],
  standalone: true,
  selector: 'app-safe-key-pad',
  templateUrl: './safe-key-pad.component.html',
})

export class SafeKeyPadComponent {
  constructor(
    private submitService: KeypadSubmitService,
    private safeStatusService: SafeStatusService,
  ) {
  }

  failingResponse: ISafeResponse = {
    isSuccessful: false,
    isDetail: 'Verify safe input',
  };

  digits: Number[] = [0, 1, 2, 3, 4, 5, 6, 7, 9];
  digitsInput: string = '';

  safe$!: Observable<ISafeResponse>;

  // On-reload of page. Given an id is present will fetch the safe-id
  ngOnInit(): void {
    this.safe$ = this.safeStatusService.getSafeStatus(1);
  }

  clearInput(): void {
    this.digitsInput = '';
  }

  safeClose(): void {
  }

  safeOpen(): void {
  }

  safeLock(): void {
  }

  resetSafePin(): void {
  }

  submitSafePin(pin: string): void {
    if (pin === null || pin.length == 0) {
      return;
    }

    this.submitService.submitSafePin(pin).pipe(
      catchError(() => {
        return throwError(() => this.failingResponse);
      }),
    );

    this.digitsInput = '';
  }

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }
}
