import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-factory-reset-popup',
  imports: [],
  templateUrl: './factory-reset-popup.component.html',
  styleUrl: './factory-reset-popup.component.scss',
})
export class FactoryResetPopupComponent {
  message = input('');
  closed = output<void>();

  state = computed(() => (this.message() ? 'opened' : 'closed'));
}
