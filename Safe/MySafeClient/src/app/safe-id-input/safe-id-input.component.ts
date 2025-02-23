import {Component} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {forbiddenNameValidator} from './validators/forbidden-safeid.directive';


@Component({
  selector: 'app-safe-id-input',
  imports: [
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './safe-id-input.component.html',
  styleUrl: './safe-id-input.component.css'
})
export class SafeIdInputComponent {
  safeForm = new FormGroup({
    safeIdControl: new FormControl("", [
      Validators.required,
      forbiddenNameValidator(/^\d+$/gm),
    ]),
  });

  onSubmit() {
    console.log('Button has been clicked!');
    console.log('This is the safe id: ' + this.safeForm.value.safeIdControl);
  }

}
