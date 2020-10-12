import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { User } from 'src/app/models/user.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private _isAuthenticated$ = new BehaviorSubject<boolean>(false);
  isAuthenticated$: Observable<boolean> = this._isAuthenticated$.asObservable();

  constructor(
    @Inject(DOCUMENT) 
    private document: Document,
    private httpClient: HttpClient) { }

  login(provider: string) {
    this.document.location.href = this.buildUrl(
      `${environment.apiUrl}/account/login`,
      { provider: provider, returnUrl: this.document.location.href });
  }

  updateUserAuthenticationStatus() {
    return this.httpClient.get<boolean>(`${environment.apiUrl}/account/isAuthenticated`, { withCredentials: true }).pipe(tap(isAuthenticated => {
      this._isAuthenticated$.next(isAuthenticated);
    }));
  }

  setUserAsNotAuthenticated() {
    this._isAuthenticated$.next(false);
  }

  me() {
    return this.httpClient.get<User>(`${environment.apiUrl}/account/me`, { withCredentials: true });
  }

  getUsers() {
    return this.httpClient.get<User[]>(`${environment.apiUrl}/account/users`, { withCredentials: true });
  }

  logout() {
    this.document.location.href = this.buildUrl(
      `${environment.apiUrl}/account/logout`,
      { returnUrl: this.document.location.href });
  }

  private buildUrl(baseUrl: string, params: any): string {
    const queryString = Object.keys(params)
    .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
    .join('&');

    return `${baseUrl}?${queryString}`;
  }

}
