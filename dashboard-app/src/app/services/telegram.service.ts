import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TelegramSubscription } from '../models/telegram.model';
import { RefreshableObservable } from './RefreshableObservable';

@Injectable({
  providedIn: 'root',
})
export class TelegramService {
  constructor(private httpClient: HttpClient) {}

  private readonly subscriptions$ = new RefreshableObservable(
    this.getSubscriptions()
  );

  getRefreshableSubscriptions(): RefreshableObservable<TelegramSubscription[]> {
    return this.subscriptions$;
  }

  getSubscriptions(): Observable<TelegramSubscription[]> {
    return this.httpClient.get<TelegramSubscription[]>(
      `${environment.baseUrl}/TelegramSubscriptions`
    );
  }
}
