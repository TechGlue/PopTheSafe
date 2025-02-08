import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { SafeResponse } from '../safe-response';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class KeypadSubmitService {
  constructor(private http: HttpClient) {}
  private baseUrl: string = environment.safestatusurl + '/safe/';

  openSafe(): Observable<SafeResponse> {
    return this.http.get<SafeResponse>(`${this.baseUrl}open/`);
  }

  closeSafe(): Observable<SafeResponse> {
    console.log('making open safe rest call');
    return this.http.get<SafeResponse>(`${this.baseUrl}close/`);
  }

  submitSafePin(pin: string): Observable<SafeResponse> {
    const body = { title: 'something' };
    return this.http.put<SafeResponse>(`${this.baseUrl}${pin}`, body);
  }

  resetSafePin(): Observable<SafeResponse> {
    console.log('making open safe rest call');
    return this.http.get<SafeResponse>(`${this.baseUrl}reset/`);
  }
}
