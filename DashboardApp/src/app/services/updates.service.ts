import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Update } from '../models/update.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UpdatesService {

  constructor(
    private httpClient: HttpClient) { }

  getUpdates(): Observable<Update[]> {
    return this.httpClient.get<Update[]>(`${environment.apiUrl}/updates`, { withCredentials: true });
  }

  removeUpdate(id: number): Observable<Object> {
    return this.httpClient.delete(`${environment.apiUrl}/updates`, { withCredentials: true, params: { id: id.toString() } })
  }

}
