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

  private baseUrl: string = environment.safestatusurl + '/safe';

  openSafe(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}/open/${id}`);
  }

  closeSafe(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}/close/${id}`);
  }

  lockSafe(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}/lock/${id}`);
  }

  submitSafePin(id: string, pin: string): Observable<ISafeResponse> {
    return this.http.put<ISafeResponse>(`${this.baseUrl}/${id}/${pin}`, {});
  }

  resetSafePin(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}/reset/${id}`);
  }

  factoryReset(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.baseUrl}/factoryreset/${id}`);
  }
}
