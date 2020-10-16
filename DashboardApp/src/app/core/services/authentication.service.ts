import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private prefix = 'authentication'

  private _isAuthenticated$ = new BehaviorSubject<boolean>(false);
  isAuthenticated$: Observable<boolean> = this._isAuthenticated$.asObservable();

  constructor(
    @Inject(DOCUMENT) 
    private document: Document,
    private httpClient: HttpClient) { }

  login(provider: string) {
    this.document.location.href = this.buildUrl(
      `${environment.apiUrl}/${this.prefix}/login`,
      { provider: provider, returnUrl: this.document.location.href });
  }

  updateAuthenticationStatus(): Observable<void> {
    return this.isAuthenticated().pipe(map(isAuthenticated => this._isAuthenticated$.next(isAuthenticated)));
  }

  isAuthenticated(): Observable<boolean> {
    return this.httpClient.get<boolean>(
      `${environment.apiUrl}/${this.prefix}/isAuthenticated`,
      { withCredentials: true });
  }

  notAuthenticated() {
    this._isAuthenticated$.next(false);
  }

  logout() {
    this.document.location.href = this.buildUrl(
      `${environment.apiUrl}/${this.prefix}/logout`,
      { returnUrl: this.document.location.href });
  }

  private buildUrl(baseUrl: string, params: any): string {
    const queryString = Object.keys(params)
    .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
    .join('&');

    return `${baseUrl}?${queryString}`;
  }

}
