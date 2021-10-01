import { BehaviorSubject, combineLatest, Observable, of, PartialObserver, Subscription, timer } from 'rxjs';
import { concatAll, mergeAll, share, switchMap, shareReplay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';


export class ItemsObserver<T> extends Observable<T> {
  items$: Observable<T>;

  private trigger = new BehaviorSubject<void>(void 0);

  constructor(next: () => Observable<T>) {
    super();

    const onTimer = timer(undefined, environment.pollingIntervalMs);

    const onTrigger = this.trigger;

    this.items$ = combineLatest([onTimer, onTrigger]).pipe(
      switchMap(() => next()),
      share()
    );
  }

  refresh() {
    this.trigger.next();
  }
}
