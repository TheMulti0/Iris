import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  NavigationEnd,
  NavigationStart,
  RouteConfigLoadEnd,
  Router,
  RouterEvent,
} from '@angular/router';
import { Subscription } from 'rxjs';
import { filter, first, map, tap, timeout } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
  currentPath!: string;
  links = [
    {
      path: '/posts-listener',
      name: 'Posts Listener',
    },
    {
      path: '/telegram',
      name: 'Telegram',
    },
  ];

  constructor(private router: Router) {}

  async ngOnInit() {
    this.currentPath = await this.router.events.pipe(
      first((e) => e instanceof NavigationEnd),
      map((e) => (e as NavigationEnd).url),
      timeout(500)
    ).toPromise();
  }

  click(path: string) {
    this.currentPath = path;
  }
}
