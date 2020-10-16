import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from 'src/app/models/user.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private prefix = 'users'

  constructor(
    private httpClient: HttpClient) { }

  getUsers() {
    return this.httpClient.get<User[]>(
      `${environment.apiUrl}/${this.prefix}`,
      { withCredentials: true });
  }

}
