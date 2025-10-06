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

  id: string | null = null;

  alert$ = this.alertSubject.asObservable();

  show(message: string, id: string): Promise<boolean> {
    this.id = id;
    return new Promise((resolve) => {
      this.alertSubject.next({ message, resolve });
    });
  }
}
