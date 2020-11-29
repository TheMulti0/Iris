import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { interval, Observable, Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ServerSideDataSource } from '../core/services/server-side-data-source';
import { PageSearchParams } from '../models/page.model';
import { Media, Update, Photo, Video, Audio } from '../models/update.model';
import { UpdatesService } from '../services/updates.service';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', paddingLeft: '10pt', minHeight: '0', visibility: 'hidden' })),
      state('expanded', style({ height: '*', paddingLeft: '10pt', visibility: 'visible' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class UpdatesComponent implements OnInit {
  dataSource: ServerSideDataSource<Update> = new ServerSideDataSource();
  updatesLength = 0;

  displayedColumns = [
    'content',
    'authorId',
    'creationDate',
    'url',
    'repost',
    'actions'
  ];

  @ViewChild(MatPaginator, { static: true })
  private paginator: MatPaginator;

  pageSizeOptions: number[] = [5, 20, 30, 50, 100];

  subscriptions: Subscription[] = [];

  constructor(
    private updatesService: UpdatesService
  ) { }

  ngOnInit() {
    this.requestCurrentAsync();

    this.subscriptions.push(
      interval(1000).subscribe(_ => this.onInterval()),
      this.paginator.page.subscribe(
        () => this.requestCurrentAsync()),
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach(
      subscription => subscription.unsubscribe());
  }

  toggleRowState(row) {
    row.isExpanded = !row.isExpanded;
  }

  getIcon(media: Media) {
    switch (media.type) {
      case 'Video':
        return 'movie';

      case 'Audio':
        return 'audiotrack';

      default:
        return 'image';
    }
  }

  getName(media: Media) {
    console.log(media);
    return media.type;
  }

  private onInterval() {
    return this.updatesService.getUpdatesCount().subscribe(count => this.onCount(count));
  }

  private onCount(count: number) {
    this.paginator.length = count;

    const pageSize = this.paginator.pageSize;
    const totalPages = Math.floor((count + pageSize - 1) / pageSize); // start from 1

    if (this.paginator.pageIndex == totalPages - 1) { // this is last page
      const itemsInLastPage = count % pageSize;
      if (this.updatesLength < itemsInLastPage) {
        this.requestCurrentAsync();
      }
    }
  }

  private requestCurrentAsync() {
    this.requestAsync(
      this.createSearchParams(this.paginator.pageIndex));
  }

  private requestAsync(searchParams: PageSearchParams) {
    this.dataSource.pushAsync(() => this.getUpdates(searchParams));
  }

  private getUpdates(searchParams: PageSearchParams): Observable<Update[]> {
    return this.updatesService.getUpdates(searchParams)
      .pipe(
        tap(updates => this.updatesLength = updates.length)
      );
  }

  private createSearchParams(pageIndex: number): PageSearchParams {
    return {
      pageIndex: pageIndex,
      pageSize: this.paginator.pageSize ?? this.pageSizeOptions[0]
    };
  }

  async remove(update) {
    await this.updatesService.removeUpdate(update.id).toPromise();

    this.requestCurrentAsync();
  }
}