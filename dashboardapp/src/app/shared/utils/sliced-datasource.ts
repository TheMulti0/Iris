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
  private subscription = new Subscription();

  private currentRange: ListRange = { start: 0, end: 0 };

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

    this.subscription.add(scrollSubscription);

    return this.dataStream;
  }

  onNewRange(range: ListRange) {
    const { start, end } = range;

    if (start >= this.currentRange.start && end <= this.currentRange.end) {
      return;
    }

    this.currentRange = {
      start: Math.floor(start),
      end: Math.floor(end),
    }; // Fetch a range that is larger than requested

    this.fetchPage();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.subscription.unsubscribe();
  }

  private async fetchPage() {
    const { start, end } = this.currentRange;

    const batch: Slice<T> = await this.getBatch(start, end).toPromise();

    this.cachedItems.push(...batch.content);

    this.dataStream.next(this.cachedItems);
  }
}
