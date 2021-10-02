import { BehaviorSubject, combineLatest, Observable, timer } from 'rxjs';
import { switchMap, publishReplay, refCount } from 'rxjs/operators';
import { environment } from '../../environments/environment';

// This is a hot observable which is generated from a cold observable.
// Every manual refresh call or timer interval the observable will connect
// to the newly created cold observable.

export class RefreshableObservable<T> extends Observable<T> {
  onRefresh: BehaviorSubject<void>;

  constructor(cold: Observable<T>) {
    const onRefresh = new BehaviorSubject<void>(void 0);

    const onTimer = timer(undefined, environment.pollingIntervalMs);

    const items$ = combineLatest([onRefresh, onTimer]).pipe(
      switchMap(() => cold),
      publishReplay(1),
      refCount()
    );

    super((o) => items$.subscribe(o));

    // this classes fields must be set after the base class constructor call
    this.onRefresh = onRefresh;
  }

  refresh() {
    this.onRefresh.next();
  }
}
