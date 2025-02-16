import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import { Observable} from 'rxjs';
import {ISafeResponse} from '../safe-response';

@Injectable({
  providedIn: 'root',
})

export class SafeStatusService {
  constructor(private http: HttpClient) {
  }

  private apiUrl: string = environment.safestatusurl + '/safe/status/';

  getSafeStatus(id: number): Observable<ISafeResponse> {
    return this.http.get<ISafeResponse>(`${this.apiUrl}${id}`);
  }
}
