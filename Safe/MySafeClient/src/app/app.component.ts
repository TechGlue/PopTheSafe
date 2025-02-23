import { Component } from '@angular/core';
import { SafeKeyPadComponent } from './safe-key-pad/safe-key-pad.component';
import { SafeIdInputComponent } from './safe-id-input/safe-id-input.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [SafeKeyPadComponent, SafeIdInputComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'PopTheSafe';
  protected readonly String = String;
}



