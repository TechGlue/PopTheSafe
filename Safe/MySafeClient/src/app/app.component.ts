import { Component } from '@angular/core';
import { SafeIdInputComponent } from './safe-id-input/safe-id-input.component';
import { FactoryResetPopupComponent } from './safe-key-pad/factory-reset-popup/factory-reset-popup.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [SafeIdInputComponent, FactoryResetPopupComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'PopTheSafe';
  protected readonly String = String;
}
