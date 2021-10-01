import {
  BehaviorSubject,
  combineLatest,
  Observable,
  timer,
} from 'rxjs';
import { switchMap, shareReplay, } from 'rxjs/operators';

// This is a hot observable which is generated from a cold observable.
// Every manual refresh call or timer interval the observable will connect
// to the newly created cold observable.

export class RefreshableObservable<T> extends Observable<T> {

  onRefresh: BehaviorSubject<void>;

  constructor(
    next: () => Observable<T>,
    intervalMs: number,
  ) {
    const onRefresh = new BehaviorSubject<void>(void 0);

    const onTimer = timer(undefined, intervalMs);

    const items$ = combineLatest([onRefresh, onTimer]).pipe(
      switchMap(() => next()),
      shareReplay({ refCount: true, bufferSize: 1 })
    );

    super(o => items$.subscribe(o));

    // this classes fields must be set after the base class constructor call
    this.onRefresh = onRefresh;
  }

  refresh() {
    this.onRefresh.next();
  }
}
