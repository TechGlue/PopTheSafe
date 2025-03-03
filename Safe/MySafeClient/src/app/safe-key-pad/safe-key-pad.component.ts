import {Component, Input} from '@angular/core';
import {KeypadSubmitService} from './keypad-submit.service';
import {SafeStatusService} from '../safe-status/safe-status.service';
import {catchError, EMPTY, Observable} from 'rxjs';
import {ISafeResponse} from '../safe-response';
import {ReactiveFormsModule} from '@angular/forms';

@Component({
  imports: [ReactiveFormsModule],
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
  @Input() safeId!: string;
  safeResponse!: ISafeResponse;

  ngOnInit(): void {
    this.safeStatusService.getSafeStatus(this.safeId)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );
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
