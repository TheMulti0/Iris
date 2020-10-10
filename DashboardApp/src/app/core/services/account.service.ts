import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { User } from 'src/app/models/user.model';

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
      '/account/login',
      { provider: provider, returnUrl: this.document.location.href });
  }

  updateUserAuthenticationStatus() {
    return this.httpClient.get<boolean>(`/account/isAuthenticated`, { withCredentials: true }).pipe(tap(isAuthenticated => {
      this._isAuthenticated$.next(isAuthenticated);
    }));
  }

  setUserAsNotAuthenticated() {
    this._isAuthenticated$.next(false);
  }

  me() {
    return this.httpClient.get<User>(`/account/me`, { withCredentials: true });
  }

  logout() {
    this.document.location.href = this.buildUrl(
      '/account/logout',
      { returnUrl: this.document.location.href });
  }

  private buildUrl(baseUrl: string, params: any): string {
    const queryString = Object.keys(params)
    .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
    .join('&');

    return `${baseUrl}?${queryString}`;
  }

}
