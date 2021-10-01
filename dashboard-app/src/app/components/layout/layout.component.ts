import { Component, OnInit, OnDestroy } from '@angular/core';
import { NavigationEnd, NavigationStart, RouteConfigLoadEnd, Router, RouterEvent } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit, OnDestroy {
  currentRoutePath: string = '/';
  routes = [
    {
      path: '/posts-listener',
      name: 'Posts Listener'
    },
    {
      path: '/telegram',
      name: 'Telegram'
    }
  ]

  private routeSubscription!: Subscription;

  constructor(private router: Router) {}

  ngOnInit() {
    this.routeSubscription = this.router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        map((e) => e as NavigationEnd )
      )
      .subscribe((e) => (this.currentRoutePath = e.url));
  }

  ngOnDestroy() {
    this.routeSubscription?.unsubscribe();
  }

  click(path: string) {
    this.currentRoutePath = path;
  }
}
