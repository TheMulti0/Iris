import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { MeService } from '../services/me.service';

@Injectable({
  providedIn: 'root'
})
export class SuperUserGuard implements CanActivate {

  constructor(
    private meService: MeService) { }

  canActivate(
    _next: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.meService.isSuperUser$;
  }

}
