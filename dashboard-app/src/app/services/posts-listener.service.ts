import { HttpClient, HttpResponseBase } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { NewPostSubscription } from '../models/posts-listener.model';
import { RefreshableObservable } from './RefreshableObservable';

@Injectable({
  providedIn: 'root',
})
export class PostsListenerService {
  constructor(private httpClient: HttpClient) {}

  private readonly subscriptions$ = new RefreshableObservable(
    this.getSubscriptions()
  );

  getRefreshableSubscriptions(): RefreshableObservable<NewPostSubscription[]> {
    return this.subscriptions$;
  }

  getSubscriptions(): Observable<NewPostSubscription[]> {
    return this.httpClient.get<NewPostSubscription[]>(
      `${environment.baseUrl}/PostsListenerSubscriptions`
    );
  }

  addOrUpdateSubscription(
    id: string,
    platform: string,
    pollInterval: string,
    earliestPostDate: string | undefined
  ): Observable<HttpResponseBase> {
    let params: any = {
      pollInterval: pollInterval,
    };

    if (earliestPostDate != undefined) {
      params.earliestPostDate = earliestPostDate;
    }

    return this.httpClient.post(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      undefined,
      {
        observe: 'response',
        params: params,
      }
    );
  }

  triggerPoll(id: string, platform: string): Observable<HttpResponseBase> {
    return this.httpClient.post(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}/poll`,
      undefined,
      {
        observe: 'response',
      }
    );
  }

  removeSubscription(
    id: string,
    platform: string
  ): Observable<HttpResponseBase> {
    return this.httpClient.delete(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      {
        observe: 'response',
      }
    );
  }
}
