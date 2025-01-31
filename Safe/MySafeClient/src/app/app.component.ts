import { Component } from '@angular/core';
import { SafeStatusComponent } from './safe-status/safe-status.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [SafeStatusComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'PopTheSafe';
}
