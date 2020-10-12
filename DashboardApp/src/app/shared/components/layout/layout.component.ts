import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay, tap } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { User } from 'src/app/models/user.model';
import { AppActions } from 'src/app/app.constants';
import { MeService } from 'src/app/core/services/me.service';

@Component({
  selector: 'mt-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  isAuthenticated$: Observable<boolean>;
  user$: Observable<User>;

  pages: { path: string, name: string; }[];

  constructor(
    private breakpointObserver: BreakpointObserver,
    private accountService: AuthenticationService,
    private meService: MeService
  ) {
    this.pages = Object.keys(AppActions).map(name => {
      return {
        path: AppActions[name],
        name
      }
    })
  }

  ngOnInit() {
    this.isAuthenticated$ = this.accountService.isAuthenticated$;
    this.user$ = this.meService.getMe();
  }

  loginWithTwitter() {
    this.accountService.login('Twitter');
  }

  logout() {
    this.accountService.logout();
  }
}
