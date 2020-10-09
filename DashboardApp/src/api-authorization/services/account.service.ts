import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private _isUserAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isUserAuthenticated$: Observable<boolean> = this._isUserAuthenticatedSubject.asObservable();

  constructor(
    @Inject(DOCUMENT) 
    private document: Document,
    private httpClient: HttpClient) { }

  login() {
    const returnUrl = "http://localhost:4200";
    this.document.location.href = `http://localhost:5000/account/login?provider=Twitter&returnUrl=${returnUrl}`;
  }

  updateUserAuthenticationStatus() {
    return this.httpClient.get<boolean>(`${environment.apiUrl}/account/isAuthenticated`, { withCredentials: true }).pipe(tap(isAuthenticated => {
      this._isUserAuthenticatedSubject.next(isAuthenticated);
    }));
  }

  setUserAsNotAuthenticated() {
    this._isUserAuthenticatedSubject.next(false);
  }

  getName() {
    console.log('name')
    return this.httpClient.get<string>(`${environment.apiUrl}/account/name`, { withCredentials: true });
  }

  logout() {
    this.document.location.href = "http://localhost:5000/account/logout";
  }

}
