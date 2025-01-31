import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SafestatusService {
  constructor(private http: HttpClient) {}

  private apiUrl: string = environment.safestatusurl;

  getSafeStatus(): void {
    const errorResponse = new HttpErrorResponse({
      status: 404,
      statusText: 'Not Found',
      error: {
        message: 'unable to connect to safe service, check connection string',
      },
    });

    this.http.get(this.apiUrl, { observe: 'response' }).subscribe((res) => {
      console.log('Response status:', res.status);
      console.log('Body:', res.body);
    });
  }
}
