import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public updates: Update[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Update[]>(baseUrl + 'updates').subscribe(result => {
      this.updates = result;
    }, error => console.error(error));
  }
}

export interface Update {
  content: string
  author_id: string
  creation_date: string
  url: string
  media: Media[]
  repost: boolean
}

export type Media = Photo | Video | Audio;

export interface IMedia {
  url: string
  thumbnail_url: string
}

export interface Photo extends IMedia { }

export interface Video extends IMedia {
  duration_seconds: number;
  width: number;
  height: number;
}

export interface Audio {
  duration_seconds: number;
  title: string
  artist: string
}
