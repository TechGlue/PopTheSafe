import {Component, Input} from '@angular/core';
import {KeypadSubmitService} from './keypad-submit.service';
import {SafeStatusService} from '../safe-status/safe-status.service';
import {catchError, EMPTY} from 'rxjs';
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

  openValidStatusId: Set<number> = new Set<number>([0,3,5]);
  closeValidStatusId: Set<number> = new Set<number>([2,1]);
  lockValidStatusId: Set<number> = new Set<number>([6]);
  submitValidStatusId: Set<number> = new Set<number>([3,4]);
  resetValidStatusId: Set<number> = new Set<number>([1]);

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

  safeClose(): void {
    this.submitService.closeSafe(this.safeId)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );
  }

  safeOpen(): void {
    this.submitService.openSafe(this.safeId)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );
  }

  safeLock(): void {
    this.submitService.lockSafe(this.safeId)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );
  }

  resetSafePin(): void {
    this.submitService.resetSafePin(this.safeId)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );
  }

  submitSafePin(pin: string): void {
    if (pin === null || pin.length == 0) {
      return;
    }

    this.submitService.submitSafePin(this.safeId, pin)
      .pipe(
        catchError(error => EMPTY)
      )
      .subscribe(
        (data: ISafeResponse) => {
          this.safeResponse = data;
        }
      );

    this.digitsInput = '';
  }

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }

  clearInput(): void {
    this.digitsInput = '';
  }

  protected readonly open = open;
}
