import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

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
      transition('true <=> false', animate('0.25s ease-out')),
    ]),
    trigger('slide', [
      transition(':enter', [
        style({ marginRight: '10pt' }),
        animate(
          '0.25s ease-out',
          style({ marginRight: '0pt', opacity: 1 })
        ),
      ]),
      transition(':leave', [
        style({ marginRight: '0pt', opacity: 1 }),
        animate(
          '0.25s ease-out',
          style({ marginRight: '10pt', opacity: 0 })
        ),
      ]),
    ]),
  ],
})
export class LayoutComponent {
  isHandset$: Observable<boolean> = this.breakpointObserver
    .observe(Breakpoints.Handset)
    .pipe(
      map((result) => result.matches),
      shareReplay()
    );

  isExpanded = false;

  listItems: ListItem[] = [
    {
      matIcon: 'feed',
      name: 'Feed #1'
    },
    {
      matIcon: 'add',
      name: 'New feed'
    }
  ];

  constructor(private breakpointObserver: BreakpointObserver) {}
}
