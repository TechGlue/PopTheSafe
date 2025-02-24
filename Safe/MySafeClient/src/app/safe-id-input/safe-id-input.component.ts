import {Component} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {forbiddenNameValidator} from './validators/forbidden-safeid.directive';
import { SafeKeyPadComponent } from '../safe-key-pad/safe-key-pad.component';

@Component({
  selector: 'app-safe-id-input',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    SafeKeyPadComponent
  ],
  templateUrl: './safe-id-input.component.html',
  styleUrl: './safe-id-input.component.css'
})
export class SafeIdInputComponent {
  safeForm = new FormGroup({
    safeIdControl: new FormControl("", [
      Validators.required,
      forbiddenNameValidator(/^[1-9]\d*$/),
    ]),
  });

  safeIdToSend: string = '';

  onSubmit() {
    let value = this.safeForm.value.safeIdControl?.toString();

    if (typeof value === 'string') {
      this.safeIdToSend = value;
    }
  }
}
