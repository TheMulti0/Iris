import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Slice } from 'src/app/models/slice.model';
import { Update } from 'src/app/models/updates.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UpdatesService {
  constructor(private client: HttpClient) {}

  getUpdates(startIndex: number, limit: number): Observable<Slice<Update>> {
    return this.client.get<Slice<Update>>(
      `${environment.apiUrl}/updates?startIndex=${startIndex}&limit=${limit}`
    );
  }
}
