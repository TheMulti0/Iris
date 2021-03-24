import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Slice } from 'src/app/models/slice.model';
import { Update } from 'src/app/models/updates.model';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UpdatesService {
  constructor(
    private client: HttpClient,
    private loadingService: LoadingService
  ) {}

  getUpdates(startIndex: number, limit: number): Observable<Slice<Update>> {
    this.loadingService.enable();

    return this.client
      .get<Slice<Update>>(
        `${environment.apiUrl}/updates?startIndex=${startIndex}&limit=${limit}`
      )
      .pipe(tap(() => this.loadingService.disable()));
  }
}
