import { HttpClient, HttpResponse, HttpResponseBase } from '@angular/common/http';
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
    earliestPostDate: string | undefined
  ): Observable<void> {

    let params: any = {
      pollInterval: pollInterval,
    };

    if (earliestPostDate != undefined) {
      params.earliestPostDate = earliestPostDate;
    }

    const response: Observable<HttpResponse<Object>> = this.httpClient.post(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      undefined,
      {
        observe: 'response',
        params: params,
      }
    );

    return response.pipe(
      filter((r) => r.ok),
      mapTo(void 0)
    );
  }

  triggerPoll(id: string, platform: string): Observable<HttpResponseBase> {
    return this.httpClient.delete(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}/poll`,
      {
        observe: 'response'
      }
    );
  }

  removeSubscription(id: string, platform: string): Observable<HttpResponseBase> {
    return this.httpClient.delete(
      `${environment.baseUrl}/PostsListenerSubscriptions/${platform}/${id}`,
      {
        observe: 'response'
      }
    );;
  }
}
