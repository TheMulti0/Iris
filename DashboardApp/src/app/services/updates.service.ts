import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Update } from '../models/update.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PageSearchParams } from '../models/page.model';

@Injectable({
  providedIn: 'root'
})
export class UpdatesService {

  constructor(
    private httpClient: HttpClient) { }

  getUpdatesCount(): Observable<number> {
    return this.httpClient.get<number>(
      `${environment.apiUrl}/updates/count`,
      { withCredentials: true })
  }

  getUpdates(searchParams: PageSearchParams): Observable<Update[]> {
    const params = {};

    for (const key of Object.keys(searchParams)) {
      params[key] = searchParams[key].toString();
    }

    return this.httpClient.get<Update[]>(
      `${environment.apiUrl}/updates`,
      { withCredentials: true, params: params })
  }

  removeUpdate(id: string): Observable<Object> {
    return this.httpClient.delete(
      `${environment.apiUrl}/updates/${id}`,
      { withCredentials: true });
  }

}
