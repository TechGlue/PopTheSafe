import { Component, Input, input, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { KeypadSubmitService } from './keypad-submit.service';
import { Observable } from 'rxjs';
import { SafeResponse } from '../safe-response';

@Component({
  imports: [AsyncPipe],
  standalone: true,
  selector: 'app-safe-key-pad',
  templateUrl: './safe-key-pad.component.html',
})
export class SafeKeyPadComponent {
  constructor(private submitService: KeypadSubmitService) {}

  @Input() safeStatus!: string;

  safeStatus$!: Observable<SafeResponse>;

  digits: Number[] = [0, 1, 2, 3, 4, 5, 6, 7, 9];
  digitsInput: string = '';

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }

  clearInput(): void {
    this.digitsInput = '';
  }

  safeClose(): void {
    this.submitService.closeSafe().subscribe((result) => {
      console.log(result);
    });
  }

  safeOpen(): void {
    this.submitService.openSafe().subscribe((result) => {
      console.log(result);
    });
  }

  submitSafePin(pin: string): void {
    this.submitService.submitSafePin(pin).subscribe((result) => {
      console.log(result);
    });
  }

  resetSafePin(): void {
    this.submitService.resetSafePin().subscribe((result) => {
      console.log(result);
    });
  }
}
