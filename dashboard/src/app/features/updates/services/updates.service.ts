import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Paged } from 'src/app/models/paged';
import { Update } from 'src/app/models/updates';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UpdatesService {
  constructor(private client: HttpClient) {}

  getUpdates(pageIndex: number, pageSize: number): Observable<Paged<Update>> {
    return this.client.get<Paged<Update>>(
      `${environment.apiUrl}/updates?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }
}
