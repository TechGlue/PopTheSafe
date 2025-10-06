import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { forbiddenNameValidator } from './validators/forbidden-safeid.directive';
import { SafeKeyPadComponent } from '../safe-key-pad/safe-key-pad.component';
import { SafeStatusService } from '../safe-status/safe-status.service';
import { ISafeResponse } from '../safe-response';
import { catchError, EMPTY } from 'rxjs';

@Component({
  selector: 'app-safe-id-input',
  imports: [ReactiveFormsModule, FormsModule, SafeKeyPadComponent],
  templateUrl: './safe-id-input.component.html',
})
export class SafeIdInputComponent {
  constructor(private safeStatusService: SafeStatusService) {}

  safeIdToSend: string = '';
  statusMessage: string = '';

  safeForm = new FormGroup({
    safeIdControl: new FormControl('', [
      Validators.required,
      forbiddenNameValidator(/^[1-9]\d*$/),
    ]),
  });

  onSubmit() {
    let userSafeValue = this.safeForm.value.safeIdControl?.toString();

    if (typeof userSafeValue === 'string') {
      this.safeStatusService
        .getSafeStatus(userSafeValue)
        .subscribe((data: ISafeResponse) => {
          if (data.isSuccessful) {
            this.safeIdToSend = userSafeValue;
          } else {
            this.statusMessage = data.isDetail;
            this.safeForm.get('safeIdControl')?.reset();
          }
        });
    }
  }
}
