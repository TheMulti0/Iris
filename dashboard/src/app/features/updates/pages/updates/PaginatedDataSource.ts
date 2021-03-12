import { Observable, BehaviorSubject, Subject, Subscription } from 'rxjs';
import {
  DataSource,
  CollectionViewer,
  ListRange
} from '@angular/cdk/collections';


export class PaginatedDataSource<T> extends DataSource<T> {
  private cachedItems: T[] = [];
  private dataStream: Subject<T[]> = new BehaviorSubject<T[]>(this.cachedItems);
  private subscription = new Subscription();

  private pageSize = 20;
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
    const scrollSubscription = collectionViewer.viewChange.subscribe((range) => this.onNewRange(range)
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
