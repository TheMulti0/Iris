import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Role, User } from 'src/app/models/user.model';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MeService {

  private prefix = 'me';

  private _roles$: BehaviorSubject<Role[]>;
  public isSuperUser$: Observable<boolean>;

  constructor(
    private httpClient: HttpClient
  ) {
    this._roles$ = new BehaviorSubject<Role[]>([]);
    this.isSuperUser$ = this._roles$.asObservable()
      .pipe(
        map(roles => roles.includes(Role.SuperUser)));
  }

  getMe() {
    return this.httpClient.get<User>(
      `${environment.apiUrl}/${this.prefix}`,
      { withCredentials: true });
  }

  updateRoles(): Observable<void> {
    return this.getRoles().pipe(map(roles => this._roles$.next(roles)));
  }

  getRoles() {
    return this.httpClient.get<Role[]>(
      `${environment.apiUrl}/${this.prefix}/roles`,
      { withCredentials: true });
  }

}
