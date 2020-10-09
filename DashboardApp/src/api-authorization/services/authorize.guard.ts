import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { tap } from 'rxjs/operators';
import { ApplicationPaths, QueryParameterNames } from '../api-authorization.constants';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {

  constructor(
    private accountService: AccountService) { }

  canActivate(
    _next: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
      return this.accountService.isUserAuthenticated$
        .pipe(tap(isAuthenticated => this.handleAuthorization(isAuthenticated)));
  }

  private handleAuthorization(isAuthenticated: boolean) {
    if (!isAuthenticated) {
      this.accountService.login();
    }
  }
}
