import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { catchError, EMPTY, Observable, of, throwError } from 'rxjs';
import { ISafeResponse } from '../safe-response';

@Injectable({
  providedIn: 'root',
})
export class SafeStatusService {
  constructor(private http: HttpClient) {}

  private apiUrl: string = environment.safestatusurl + '/safe/status/';

  getSafeStatus(id: string): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.apiUrl}${id}`).pipe(
      catchError((error) => {
        return of<ISafeResponse>({
          isSuccessful: false,
          isDetail: error.error,
          stateId: -1,
        });
      }),
    );
  }
}
