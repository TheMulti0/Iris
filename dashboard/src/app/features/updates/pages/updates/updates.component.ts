import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ViewChild,
} from '@angular/core';
import {
  CdkVirtualScrollViewport,
  ScrollDispatcher,
} from '@angular/cdk/scrolling';
import { Observable, BehaviorSubject, Subject, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { Update } from 'src/app/models/updates';
import { UpdatesService } from '../../services/updates.service';
import {
  DataSource,
  CollectionViewer,
  ListRange,
} from '@angular/cdk/collections';

enum Direction {
  Up = 'Up',
  Down = 'Down',
}

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss'],
})
export class UpdatesComponent {
  @ViewChild(CdkVirtualScrollViewport)
  virtualScroll!: CdkVirtualScrollViewport;
  updates$: Subject<Update[]> = new Subject<Update[]>();

  pageSize = 12;
  pageIndex = 0;

  lastRangeEnd = 0;

  dataSource: PaginatedDataSource<Update>;

  constructor(private service: UpdatesService) {
    this.dataSource = new PaginatedDataSource((index, size) =>
      this.service.getUpdates(index, size).pipe(map((page) => page.content))
    );
  }
  trackByUrl(index: number, update: Update) {
    return update;
  }
}

export class PaginatedDataSource<T> extends DataSource<T> {
  private cachedItems: T[] = [];
  private dataStream: Subject<T[]> = new BehaviorSubject<T[]>(this.cachedItems);
  private subscription = new Subscription();

  private pageSize = 10;
  private currentPageIndex = 0;

  constructor(
    private getBatch: (pageIndex: number, pageSize: number) => Observable<T[]>
  ) {
    super();

    this.fetchPage();
  }

  connect(
    collectionViewer: CollectionViewer
  ): Observable<T[] | ReadonlyArray<T>> {
    const scrollSubscription = collectionViewer.viewChange.subscribe((range) =>
      this.onNewRange(range)
    );

    this.subscription.add(scrollSubscription);

    return this.dataStream;
  }

  onNewRange(range: ListRange) {
    const pageIndex = this.getPageIndex(range.end, this.pageSize);

    if (pageIndex === this.currentPageIndex) {
      return;
    }

    this.currentPageIndex = pageIndex;
    this.fetchPage();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.subscription.unsubscribe();
  }

  private async fetchPage() {
    const batch: T[] = await this.getBatch(
      this.currentPageIndex,
      this.pageSize
    ).toPromise();

    this.cachedItems = this.cachedItems.concat(batch);

    this.dataStream.next(this.cachedItems);
  }

  private getPageIndex(offset: number, pageSize: number): number {
    return Math.max(0, Math.floor((offset + pageSize - 1) / pageSize));
  }
}
