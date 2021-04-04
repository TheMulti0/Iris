import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { User } from 'src/app/models/user.model';
import { AuthenticationService } from '../../services/authentication.service';
import { MeService } from '../../services/me.service';
import { LoadingService } from '../../services/loading.service';

interface ListItem {
  matIcon: string;
  name: string;
}

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  animations: [
    trigger('menu', [
      state(
        'false',
        style({
          transform: 'rotate(0deg)',
        })
      ),
      state(
        'true',
        style({
          transform: 'rotate(-90deg)',
        })
      ),
      transition('true <=> false', animate('0.15s ease-out')),
    ]),
    trigger('slide', [
      transition(':enter', [
        style({ marginRight: '10pt' }),
        animate('0.15s ease-out', style({ marginRight: '0pt', opacity: 1 })),
      ]),
      transition(':leave', [
        style({ marginRight: '0pt', opacity: 1 }),
        animate('0.15s ease-out', style({ marginRight: '10pt', opacity: 0 })),
      ]),
    ]),
  ],
})
export class LayoutComponent implements OnInit {
  isHandset$: Observable<boolean> = this.breakpointObserver
    .observe(Breakpoints.Handset)
    .pipe(
      map((result) => result.matches),
      shareReplay()
    );
  isExpanded = false;
  listItems: ListItem[] = [];

  isAuthenticated$: Observable<boolean>;
  user!: User;

  constructor(
    private breakpointObserver: BreakpointObserver,
    private authenticationService: AuthenticationService,
    private meService: MeService,
    public progressBarService: LoadingService
  ) {
    this.isAuthenticated$ = this.authenticationService.isAuthenticated$;
  }

  async ngOnInit() {
    await this.authenticationService.updateAuthenticationStatus().toPromise();

    this.isAuthenticated$.subscribe((isAuthenticated) =>
      this.onAuthenticateChange(isAuthenticated)
    );
  }

  async onAuthenticateChange(isAuthenticated: boolean) {
    if (!isAuthenticated) {
      return;
    }

    this.user = await this.meService.getMe().toPromise();

    this.listItems.push(
      {
        matIcon: 'feed',
        name: 'Feed #1',
      },
      {
        matIcon: 'add',
        name: 'New feed',
      }
    );
  }

  loginWithTwitter() {
    this.authenticationService.login('Twitter');
  }

  logout() {
    this.authenticationService.logout();
  }
}