import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  private _loading$ = new BehaviorSubject<boolean>(false);
  loading$: Observable<boolean> = this._loading$.asObservable();

  enable() {
    this._loading$.next(true);
  }

  disable() {
    this._loading$.next(false);
  }
}
