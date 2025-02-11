import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { catchError, Observable, throwError } from 'rxjs';
import { ISafeResponse } from '../safe-response';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class KeypadSubmitService {
  constructor(private http: HttpClient) {}
  private baseUrl: string = environment.safestatusurl + '/safe/';

  openSafe(): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}open/`);
  }

  closeSafe(): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}close/`);
  }

  lockSafe(): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}lock/`);
  }

  submitSafePin(pin: string): Observable<ISafeResponse> {
    const body = { title: 'something' };
    return this.http.put<ISafeResponse>(`${this.baseUrl}${pin}`, body);
  }

  resetSafePin(): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}reset/`);
  }
}
