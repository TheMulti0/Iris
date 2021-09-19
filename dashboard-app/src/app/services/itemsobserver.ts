import { BehaviorSubject, Observable } from 'rxjs';
import { shareReplay, switchMap } from 'rxjs/operators';

export class ItemsObserver<T> {
  items$: Observable<T>;

  private subject = new BehaviorSubject<void>(void 0);

  constructor(next: () => Observable<T>) {
    this.items$ = this.subject.pipe(
      switchMap(() => next()),
      shareReplay(1)
    );
  }

  next() {
    this.subject.next();
  }
}
