import { Component, computed, input, output } from '@angular/core';
import { PopupService } from './popup.service';

@Component({
  selector: 'app-factory-reset-popup',
  templateUrl: './factory-reset-popup.component.html',
  styleUrl: './factory-reset-popup.component.scss',
})
export class FactoryResetPopupComponent {
  message: string | null = null;
  private resolver?: (value: boolean) => void;

  constructor(private popUpService: PopupService) {
    this.popUpService.alert$.subscribe(({ message, resolve }) => {
      this.message = message;
      this.resolver = resolve;
    });
  }

  onRespond(value: boolean) {
    if (value) {
      console.log('You are nuking the safe, but you know that right?');
    } else {
      console.log('The safe lives to safe another day');
    }
  }
}
