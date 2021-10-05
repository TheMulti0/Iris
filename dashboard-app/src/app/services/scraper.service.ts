import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Author, Post } from '../models/scraper.model';

@Injectable({
  providedIn: 'root'
})
export class ScraperService {

  constructor(
    private httpClient: HttpClient
  ) { }

  getAuthor(id: string, platform: string): Observable<Author> {
    return this.httpClient.get<Author>(
      `${environment.baseUrl}/Scraper/${platform}/${id}/author`);
  }

  getPosts(id: string, platform: string): Observable<Post[]> {
    return this.httpClient.get<Post[]>(
      `${environment.baseUrl}/Scraper/${platform}/${id}/posts`);
  }
}
