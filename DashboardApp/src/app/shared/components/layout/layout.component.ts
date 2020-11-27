import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { User } from 'src/app/models/user.model';
import { AppActions } from 'src/app/app.constants';
import { MeService } from 'src/app/core/services/me.service';

interface Page {
  path: string;
  name: string;
  canActivate?: Observable<boolean>;
}

@Component({
  selector: 'mt-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, OnDestroy {

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  isAuthenticated$: Observable<boolean>;
  user: User;

  pages: Page[];
  authPagePaths = [
    AppActions.Updates
  ]
  suPagePaths = [
    AppActions.Users
  ];

  private subscriptions: Subscription[] = [];

  constructor(
    private breakpointObserver: BreakpointObserver,
    private accountService: AuthenticationService,
    private meService: MeService
  ) { 
    this.pages = Object.keys(AppActions).map(name => {
      return {
        path: AppActions[name],
        name
      };
    });
  }

  ngOnInit() {
    this.isAuthenticated$ = this.accountService.isAuthenticated$;

    this.subscriptions.push(
      this.isAuthenticated$.subscribe(isAuthenticated => this.isAuthenticated(isAuthenticated)),
      this.meService.isSuperUser$.subscribe(isSuperUser => this.isSuperUser(isSuperUser))
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  private isAuthenticated(isAuthenticated: boolean) {
    if (isAuthenticated) {
      this.subscriptions.push(
        this.meService.getMe().subscribe(user => {
          this.user = user;
        })
      );
    }

    this.removePages(isAuthenticated, this.authPagePaths);
  }

  private isSuperUser(isSuperUser: boolean) {
    this.removePages(isSuperUser, this.suPagePaths);
  }

  private removePages(condition: boolean, notViewablePaths: string[]) {
    if (condition) {
      return;
    }
    
    const suPages: Page[] = this.pages.filter(page => notViewablePaths.includes(page.path));

    if (suPages.length > 0) {
      for (const page of suPages) {
        const index = this.pages.indexOf(page, 0);
        this.pages.splice(index, 1);
      }
    }
  }

  loginWithTwitter() {
    this.accountService.login('Twitter');
  }

  logout() {
    this.accountService.logout();
  }
}
