import { Component, computed, input, output } from '@angular/core';
import { PopupService } from './popup.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ISafeResponse } from '../../safe-response';

@Component({
  selector: 'app-factory-reset-popup',
  templateUrl: './factory-reset-popup.component.html',
  styleUrl: './factory-reset-popup.component.scss',
})
export class FactoryResetPopupComponent {
  message: string | null = null;
  private resolver?: (value: boolean) => void;
  private baseUrl: string = environment.safestatusurl + '/safe';

  constructor(
    private popUpService: PopupService,
    private http: HttpClient,
  ) {
    this.popUpService.alert$.subscribe(({ message, resolve }) => {
      this.message = message;
      this.resolver = resolve;
    });
  }

  onRespond(value: boolean) {
    if (value) {
      console.log('You are nuking the safe, but you know that right?');
      this.http
        .get<ISafeResponse>(
          `${this.baseUrl}/factoryreset/${this.popUpService.id}`,
        )
        .subscribe({
          next: (res) => {
            window.location.reload();
          },
        });
    } else {
      console.log('reloading the view');
      window.location.reload();
    }
  }
}
