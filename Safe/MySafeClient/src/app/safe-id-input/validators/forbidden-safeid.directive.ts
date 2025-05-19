import {Directive, Input} from '@angular/core';
import {AbstractControl, NG_VALIDATORS, ValidationErrors, Validator, ValidatorFn} from '@angular/forms';

// factory to pass regex to ensure validation of a form control value
export function forbiddenNameValidator(safeIdRe: RegExp): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const forbiddenId = safeIdRe.test(control.value);
    return forbiddenId ? null : {forbiddenId: {value: control.value}};
  };
}

@Directive({
  selector: '[appForbiddenSafeId]',
  providers: [{provide: NG_VALIDATORS, useExisting: ForbiddenSafeidDirective, multi: true}]
})

export class ForbiddenSafeidDirective implements Validator {
  @Input('appForbiddenSafeId') forbiddenSafeId = '';

  validate(control: AbstractControl): ValidationErrors | null {
    return this.forbiddenSafeId ? forbiddenNameValidator(new RegExp(this.forbiddenSafeId))(control)
      : null;
  }
}
