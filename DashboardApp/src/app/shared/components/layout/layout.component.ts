import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay, tap } from 'rxjs/operators';
import { AccountService } from 'src/app/core/services/account.service';
import { User } from 'src/app/models/user.model';
import { AppActions } from 'src/app/app.constants';

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
    private accountService: AccountService
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
    this.user$ = this.accountService.me();
  }

  loginWithTwitter() {
    this.accountService.login('Twitter');
  }

  logout() {
    this.accountService.logout();
  }
}
