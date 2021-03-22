import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from 'src/app/models/user.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MeService {
  private prefix = 'me';

  constructor(private httpClient: HttpClient) {}

  getMe() {
    return this.httpClient.get<User>(`${environment.apiUrl}/${this.prefix}`, {
      withCredentials: true,
    });
  }
}
