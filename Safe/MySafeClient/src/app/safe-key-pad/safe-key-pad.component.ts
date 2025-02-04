import { Component } from '@angular/core';

@Component({
  selector: 'app-safe-key-pad',
  standalone: true,
  templateUrl: './safe-key-pad.component.html',
})
export class SafeKeyPadComponent {
  digits: Number[] = [0, 1, 2, 3, 4, 5, 6, 7, 9];
  digitsInput: String = '';

  concatNumberToInput(num: string): void {
    if (this.digitsInput.length <= 3) {
      this.digitsInput = this.digitsInput.concat(num);
    }
  }

  resetInput(): void {
    this.digitsInput = '';
  }
}
