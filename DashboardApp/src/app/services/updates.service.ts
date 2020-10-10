import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Update } from '../models/update.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UpdatesService {

  constructor(
    private httpClient: HttpClient) { }

  getUpdates(): Observable<Update[]> {
    return this.httpClient.get<Update[]>(`/updates`, { withCredentials: true });
  }

}
