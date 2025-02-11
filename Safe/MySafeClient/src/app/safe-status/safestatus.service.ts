import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { catchError, Observable, throwError } from 'rxjs';
import { ISafeResponse } from '../safe-response';

@Injectable({
  providedIn: 'root',
})
export class SafestatusService {
  constructor(private http: HttpClient) {}

  private apiUrl: string = environment.safestatusurl + '/safe/status/';
  public _SafeStatus: string = '';

  errorStatus: ISafeResponse = {
    isSuccessful: true,
    isDetail: 'check safe input',
  };

  private errorResponse = new HttpErrorResponse({
    status: 404,
    statusText: 'Not Found',
    error: {
      message: 'Unable to connect to openweathermap, check connection string',
    },
  });

  getSafeStatus(): Observable<ISafeResponse> {
    // handle error
    return this.http.get<ISafeResponse>(this.apiUrl).pipe(
      catchError((error) => {
        if (this.apiUrl == '') {
          return throwError(() => this.errorStatus);
        }
        return throwError(() => this.errorStatus);
      }),
    );
  }

  setValue(val: string): void {
    this._SafeStatus = val;
  }

  getValue(val: string): string {
    return this._SafeStatus;
  }

  clearData(val: string) {
    this._SafeStatus = '';
  }
}
