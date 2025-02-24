import { Component } from '@angular/core';
import { SafeIdInputComponent } from './safe-id-input/safe-id-input.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [ SafeIdInputComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'PopTheSafe';
  protected readonly String = String;
}



