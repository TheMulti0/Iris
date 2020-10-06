import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay } from 'rxjs/operators';
import { AuthorizeService } from 'src/api-authorization/services/authorize.service';

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
  userName$: Observable<string>;

  pages: { path: string, name: string }[] = [
    {
      path: '',
      name: 'Home'
    },
    {
      path: 'counter',
      name: 'Counter'
    },
    { 
      path: 'fetch-data',
      name: 'Fetch data'
    }
  ];

  constructor(
    private breakpointObserver: BreakpointObserver,
    private authorizeService: AuthorizeService
  ) { }

  ngOnInit() {
    this.isAuthenticated$ = this.authorizeService.isAuthenticated();
    this.userName$ = this.authorizeService.getUser().pipe(map(u => u && u.name));
  }
}
