import { Observable, BehaviorSubject, Subject, Subscription } from 'rxjs';
import {
  DataSource,
  CollectionViewer,
  ListRange,
} from '@angular/cdk/collections';
import { Slice } from 'src/app/models/slice.model';

export class SlicedDataSource<T> extends DataSource<T> {
  private cachedItems: T[] = [];
  private dataStream: Subject<T[]> = new BehaviorSubject<T[]>(this.cachedItems);
  private scrollSubscription = new Subscription();
  private batchSubscription = new Subscription();

  private backlog = 2;

  private currentRange: ListRange = { start: 0, end: 50 };
  private totalElementount = 0;

  private _hasReachedEnd$ = new BehaviorSubject<boolean>(false);
  public get hasReachedEnd$(): Observable<boolean> {
    return this._hasReachedEnd$;
  }

  constructor(
    private getBatch: (
      startIndex: number,
      limit: number
    ) => Observable<Slice<T>>
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

    this.scrollSubscription.add(scrollSubscription);

    return this.dataStream;
  }

  private onNewRange(range: ListRange) {
    const { start, end } = range;

    if (start >= this.currentRange.start && end <= this.currentRange.end) {
      this.dataStream.next(this.cachedItems);
      return;
    }

    this.currentRange = {
      start: Math.max(0, start - this.backlog),
      end: end + this.backlog,
    }; // Fetch a range that is larger than requested

    this.fetchPage();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.scrollSubscription.unsubscribe();
  }

  private fetchPage() {
    const { start, end } = this.currentRange;

    this.batchSubscription.unsubscribe();

    this.batchSubscription = this.getBatch(start, end).subscribe((batch) =>
      this.onBatch(batch)
    );
  }

  private onBatch(batch: Slice<T>) {
    this.cachedItems = this.cachedItems.concat(batch.content);

    this.dataStream.next(this.cachedItems);
  }
}
