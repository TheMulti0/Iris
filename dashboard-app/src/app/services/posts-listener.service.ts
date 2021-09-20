import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { NewPostSubscription } from '../models/posts-listener.model';
import { filter, mapTo } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class PostsListenerService {
  constructor(private httpClient: HttpClient) {}

  getSubscriptions(): Observable<NewPostSubscription[]> {
    return this.httpClient.get<NewPostSubscription[]>(
      `${environment.baseUrl}/PostsListenerSubscriptions`
    );
  }

  addOrUpdateSubscription(
    id: string,
    platform: string,
    pollInterval: string,
    earliestPostDate: string
  ): Observable<void> {
    const response: Observable<HttpResponse<Object>> = this.httpClient.post(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      {
        pollInterval: pollInterval,
        earliestPostDate: earliestPostDate,
      },
      {
        observe: 'response',
      }
    );

    return response.pipe(
      filter((r) => r.ok),
      mapTo(void 0)
    );
  }

  removeSubscription(id: string, platform: string): Observable<void> {
    const response: Observable<HttpResponse<Object>> = this.httpClient.delete(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      {
        observe: 'response',
      }
    );

    return response.pipe(
      filter((r) => r.ok),
      mapTo(void 0)
    );
  }
}
