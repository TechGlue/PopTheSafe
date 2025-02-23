import {Component, Input} from '@angular/core';
import {KeypadSubmitService} from './keypad-submit.service';
import {SafeStatusService} from '../safe-status/safe-status.service';
import {Observable} from 'rxjs';
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

  digits: Number[] = [0, 1, 2, 3, 4, 5, 6, 7, 9];
  digitsInput: string = '';

  // the ! is the non-null assertion operator.
  // it let's TS know that the value will not be null or undefined.
  @Input() safeId!: string;
  safe$!: Observable<ISafeResponse>;

  // On-reload of page. Given an id is present will fetch the safe-id
  ngOnInit(): void {
    this.safe$ = this.safeStatusService.getSafeStatus(1);
    console.log('safeId: ' + this.safeId);
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

    this.submitService.submitSafePin(pin);

    this.digitsInput = '';
  }

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }
}
