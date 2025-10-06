import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PopupService {
  private alertSubject = new Subject<{
    message: string;
    resolve: (value: boolean) => void;
  }>();

  alert$ = this.alertSubject.asObservable();

  show(message: string): Promise<boolean> {
    return new Promise((resolve) => {
      this.alertSubject.next({ message, resolve });
    });
  }
}
