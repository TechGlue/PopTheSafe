import { Component } from '@angular/core';
import { SafeStatusComponent } from './safe-status/safe-status.component';
import { SafeKeyPadComponent } from './safe-key-pad/safe-key-pad.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [SafeStatusComponent, SafeKeyPadComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'PopTheSafe';
}
