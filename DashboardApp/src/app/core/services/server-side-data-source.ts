import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { Observable, Subject } from 'rxjs';

export class ServerSideDataSource<T> extends DataSource<T> {
  private dataSubject: Subject<T[]> = new Subject<T[]>();

  public connect(collectionViewer: CollectionViewer): Observable<T[] | ReadonlyArray<T>> {
    return this.dataSubject;
  }

  public disconnect(collectionViewer: CollectionViewer): void {
    this.dataSubject.complete();
  }

  public pushAsync(dataSupplier: () => Observable<T[]>): void {
    dataSupplier().subscribe(
      (data: T[]) => this.dataSubject.next(data));
  }
}
