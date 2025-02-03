import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { catchError, Observable, throwError } from 'rxjs';
import { SafeResponse } from './safe-response';

@Injectable({
  providedIn: 'root',
})
export class SafestatusService {
  constructor(private http: HttpClient) {}

  private apiUrl: string = environment.safestatusurl + '/safe/';

  getSafeStatus(): Observable<SafeResponse> {
    const errorResponse = new HttpErrorResponse({
      status: 404,
      statusText: 'Not Found',
      error: {
        message: 'Unable to connect to openweathermap, check connection string',
      },
    });

    // handle error
    return this.http.get<SafeResponse>(this.apiUrl).pipe(
      catchError((error) => {
        if (this.apiUrl == '') {
          return throwError(() => errorResponse);
        }
        return throwError(() => error);
      }),
    );
  }
}
